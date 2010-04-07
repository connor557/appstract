#region Copyright (C) 2008-2009 Simon Allaeys

/*
    Copyright (C) 2008-2009 Simon Allaeys
 
    This file is part of AppStract

    AppStract is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    AppStract is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with AppStract.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Interop;
using AppStract.Core.Virtualization.Registry;
using AppStract.Utilities.Extensions;
using ValueType=AppStract.Core.Virtualization.Registry.ValueType;

namespace AppStract.Server.Registry.Data
{
  /// <summary>
  /// Base class for all classes functioning as data providers in the virtual registry.
  /// </summary>
  public abstract class RegistryBase : IIndexUser
  {

    #region Variables

    /// <summary>
    /// Generates the indices used by the current <see cref="RegistryBase"/>.
    /// </summary>
    private readonly IndexGenerator _indexGenerator;
    /// <summary>
    /// Holds all known <see cref="VirtualRegistryKey"/>s.
    /// </summary>
    private readonly IDictionary<uint, VirtualRegistryKey> _keys;
    /// <summary>
    /// Holds all possible aliases for known virtual keys.
    /// The Key values are the aliases, the Value values are known virtual key handles.
    /// </summary>
    private readonly IDictionary<uint, uint> _keyAliases;
    /// <summary>
    /// Lock used for synchronization on the current <see cref="RegistryBase"/>.
    /// </summary>
    private readonly ReaderWriterLockSlim _keysSynchronizationLock;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes <see cref="_indexGenerator"/>, <see cref="_keys"/>, and <see cref="_keysSynchronizationLock"/>.
    /// </summary>
    /// <param name="indexGenerator">The <see cref="IndexGenerator"/> to use for the new instance.</param>
    protected RegistryBase(IndexGenerator indexGenerator)
      : this(indexGenerator, new Dictionary<uint, VirtualRegistryKey>())
    {
    }

    /// <summary>
    /// Initializes <see cref="_indexGenerator"/>, <see cref="_keys"/>, and <see cref="_keysSynchronizationLock"/>.
    /// </summary>
    /// <param name="indexGenerator">The <see cref="IndexGenerator"/> to use for the new instance.</param>
    /// <param name="keys">The <see cref="IDictionary{TKey,TValue}"/> to use for the new instance.</param>
    protected RegistryBase(IndexGenerator indexGenerator, IDictionary<uint, VirtualRegistryKey> keys)
    {
      _indexGenerator = indexGenerator;
      _keys = keys;
      _keysSynchronizationLock = new ReaderWriterLockSlim();
      _keyAliases = new Dictionary<uint, uint>();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether the database knows a key with the specified index.
    /// The full path of the key is set to <paramref name="keyFullPath"/> if the key is known.
    /// </summary>
    /// <param name="hkey">The index to search a key for.</param>
    /// <param name="keyFullPath">The name of the key, as used in the host's registry.</param>
    /// <returns></returns>
    public bool IsKnownKey(uint hkey, out string keyFullPath)
    {
      using (_keysSynchronizationLock.EnterDisposableReadLock())
      {
        if (_keyAliases.ContainsKey(hkey))
          hkey = _keyAliases[hkey];
        if (_keys.Keys.Contains(hkey))
        {
          keyFullPath = _keys[hkey].Path;
          return true;
        }
      }
      keyFullPath = null;
      return false;
    }

    /// <summary>
    /// Opens the key from the specified path.
    /// </summary>
    /// <param name="keyFullPath"></param>
    /// <param name="hKey"></param>
    /// <returns></returns>
    public virtual bool OpenKey(string keyFullPath, out uint hKey)
    {
      VirtualRegistryKey key;
      using (_keysSynchronizationLock.EnterDisposableReadLock())
        key = _keys.Values.FirstOrDefault(k => k.Path.ToLowerInvariant() == keyFullPath);
      if (key == null)
      {
        hKey = 0;
        return false;
      }
      hKey = key.Handle;
      return true;
    }

    /// <summary>
    /// Creates a key with the specified path.
    /// </summary>
    /// <param name="keyFullPath">The path for the new key.</param>
    /// <param name="hKey">The allocated index.</param>
    /// <param name="creationDisposition">Whether the key is opened or created.</param>
    /// <returns></returns>
    public virtual NativeResultCode CreateKey(string keyFullPath, out uint hKey, out RegCreationDisposition creationDisposition)
    {
      if (OpenKey(keyFullPath, out hKey))
      {
        creationDisposition = RegCreationDisposition.OpenedExistingKey;
      }
      else
      {
        creationDisposition = RegCreationDisposition.CreatedNewKey;
        VirtualRegistryKey key = ConstructRegistryKey(keyFullPath);
        WriteKey(key);
        hKey = key.Handle;
      }
      return NativeResultCode.Success;
    }

    /// <summary>
    /// Deletes the key with the specified index.
    /// </summary>
    /// <param name="hKey"></param>
    /// <returns></returns>
    public virtual NativeResultCode DeleteKey(uint hKey)
    {
      using (_keysSynchronizationLock.EnterDisposableWriteLock())
      {
        hKey = EnsureHandleIsNoAlias(hKey);
        if (!_keys.ContainsKey(hKey))
          return NativeResultCode.InvalidHandle;
        RemoveAliasesFor(hKey);
        _keys.Remove(hKey);
      }
      _indexGenerator.Release(hKey);
      return NativeResultCode.Success;
    }

    /// <summary>
    /// Retrieves the value associated with the specified key and name.
    /// Returns null if the key or name is not found.
    /// </summary>
    /// <param name="hKey"></param>
    /// <param name="valueName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual NativeResultCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value)
    {
      value = new VirtualRegistryValue(valueName, null, ValueType.INVALID);
      using (_keysSynchronizationLock.EnterDisposableReadLock())
      {
        hKey = EnsureHandleIsNoAlias(hKey);
        if (!_keys.Keys.Contains(hKey))
          return NativeResultCode.InvalidHandle;
        var key = _keys[hKey];
        if (!key.Values.Keys.Contains(valueName))
          return NativeResultCode.FileNotFound;
        value = key.Values[valueName];
        return NativeResultCode.Success;
      }
    }

    /// <summary>
    /// Sets a value for the key with the specified handle.
    /// </summary>
    /// <param name="hKey">Handle of the key to set a value for.</param>
    /// <param name="value">The data to set for the value.</param>
    /// <returns></returns>
    public virtual NativeResultCode SetValue(uint hKey, VirtualRegistryValue value)
    {
      using (_keysSynchronizationLock.EnterDisposableReadLock())
      {
        hKey = EnsureHandleIsNoAlias(hKey);
        if (!_keys.Keys.Contains(hKey))
          return NativeResultCode.InvalidHandle;
        var key = _keys[hKey];
        if (key.Values.Keys.Contains(value.Name))
          key.Values[value.Name] = value;
        else
          key.Values.Add(value.Name, value);
      }
      return NativeResultCode.Success;
    }

    /// <summary>
    /// Deletes a value from the key with the specified handle.
    /// </summary>
    /// <param name="hKey">Key to delete a value from.</param>
    /// <param name="valueName">The name of the value to delete.</param>
    /// <returns></returns>
    public virtual NativeResultCode DeleteValue(uint hKey, string valueName)
    {
      using (_keysSynchronizationLock.EnterDisposableReadLock())
      {
        hKey = EnsureHandleIsNoAlias(hKey);
        if (!_keys.Keys.Contains(hKey))
          return NativeResultCode.InvalidHandle;
        return _keys[hKey].Values.Remove(valueName)
                 ? NativeResultCode.Success
                 : NativeResultCode.FileNotFound;
      }
    }

    /// <summary>
    /// Adds an alias for an already known handle.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown if <paramref name="hKey"/>
    /// is unknown the the current <see cref="RegistryBase"/>.
    /// </exception>
    /// <exception cref="ApplicationException">
    /// The key handle to alias already exists in the virtual registry.
    /// = OR =
    /// An uncompatible alias already exists for an other virtual key.
    /// </exception>
    /// <param name="hKey">The virtual key handle, known by the current <see cref="RegistryBase"/>.</param>
    /// <param name="aliasHKey">The handle to add as alias for <paramref name="hKey"/>.</param>
    public void AddAlias(uint hKey, uint aliasHKey)
    {
      using (_keysSynchronizationLock.EnterDisposableUpgradeableReadLock())
      {
        hKey = EnsureHandleIsNoAlias(hKey);
        if (!_keys.ContainsKey(hKey))
          throw new ArgumentException("The handle specified is unknown to the current RegistryBase.", "hKey");
        if (!_keyAliases.ContainsKey(aliasHKey))
        {
          if (_indexGenerator.IsInUse(aliasHKey))
            throw new ApplicationException("The alias key handle already exists in the virtual registry.");
          using (_keysSynchronizationLock.EnterDisposableWriteLock())
            _keyAliases.Add(aliasHKey, hKey);
        }
        else if (_keyAliases[aliasHKey] != hKey)
          throw new ApplicationException("An incompatible alias already exists for an other virtual key.");
      }
    }

    /// <summary>
    /// Removes an alias.
    /// </summary>
    /// <param name="aliasHKey">The to alias to be removed.</param>
    public bool RemoveAlias(uint aliasHKey)
    {
      using (_keysSynchronizationLock.EnterDisposableUpgradeableReadLock())
        if (_keyAliases.ContainsKey(aliasHKey))
          using (_keysSynchronizationLock.EnterDisposableWriteLock())
            return _keyAliases.Remove(aliasHKey);
      return false;
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Creates a new key for the specified <paramref name="keyFullPath"/>.
    /// </summary>
    /// <param name="keyFullPath"></param>
    /// <returns></returns>
    protected VirtualRegistryKey ConstructRegistryKey(string keyFullPath)
    {
      uint keyIndex = _indexGenerator.Next(this);
      VirtualRegistryKey registryKey = new VirtualRegistryKey(keyIndex, keyFullPath);
      return registryKey;
    }

    /// <summary>
    /// Writes the provided <see cref="VirtualRegistryKey"/> to <see cref="_keys"/>.
    /// </summary>
    /// <exception cref="ThreadStateException">
    /// A <see cref="ThreadStateException"/> is thrown if the current thread
    /// can't acquire a read or write lock on <see cref="_keysSynchronizationLock"/>.
    /// </exception>
    /// <param name="registryKey"><see cref="VirtualRegistryKey"/> to write to the database.</param>
    /// <param name="discardOldKeyValues">
    /// Set to true if all existing values of the key must be discarded and only those specified in <paramref name="registryKey"/> must be saved;
    /// Otherwise, if all existing values must be preserved or overwritten in case <paramref name="registryKey"/> contains the same value, set to false.
    /// </param>
    protected void WriteKey(VirtualRegistryKey registryKey, bool discardOldKeyValues)
    {
      if (!discardOldKeyValues)
        registryKey = LoadAllValues(registryKey, false, true);
      WriteKey(registryKey);
    }

    /// <summary>
    /// Returns whether or not <see cref="aliasHKey"/> is an alias pointing to another key.
    /// </summary>
    /// <param name="aliasHKey">The handle to check.</param>
    /// <param name="hKey">The key pointed to by <paramref name="aliasHKey"/>, if any.</param>
    /// <returns></returns>
    protected bool IsAlias(uint aliasHKey, out uint hKey)
    {
      using (_keysSynchronizationLock.EnterDisposableReadLock())
      {
        var isAlias = _keyAliases.ContainsKey(aliasHKey);
        hKey = isAlias ? _keyAliases[aliasHKey] : 0;
        return isAlias;
      }
    }

    /// <summary>
    /// Returns whether or not the given <see cref="hKey"/> has aliases pointing to it.
    /// </summary>
    /// <param name="hKey"></param>
    /// <returns></returns>
    protected bool HasAliases(uint hKey)
    {
      using (_keysSynchronizationLock.EnterDisposableReadLock())
        return _keyAliases.Values.Contains(hKey);
    }

    /// <summary>
    /// Removes al aliases for a known key handle.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown if <paramref name="hKey"/>
    /// is unknown the the current <see cref="RegistryBase"/>.
    /// </exception>
    /// <param name="hKey">The virtual key handle to remove all aliases for.</param>
    protected void RemoveAliasesFor(uint hKey)
    {
      var upgradeableLock = _keysSynchronizationLock.IsAnyLockHeld()
                              ? null
                              : _keysSynchronizationLock.EnterDisposableUpgradeableReadLock();
      if (!_keys.ContainsKey(hKey))
        throw new ArgumentException("The handle specified is unknown to the current RegistryBase.", "hKey");
      var aliases = new List<uint>();
      // Find all aliases
      foreach (var alias in _keyAliases)
        if (alias.Value == hKey)
          aliases.Add(alias.Key);
      // Remove all aliases for this key.
      if (aliases.Count != 0)
      {
        var writeLock = _keysSynchronizationLock.IsWriteLockHeld
                          ? null
                          : _keysSynchronizationLock.EnterDisposableWriteLock();
        foreach (var alias in aliases)
          _keyAliases.Remove(alias);
        if (writeLock != null)
          writeLock.Dispose();
      }
      if (upgradeableLock != null)
        upgradeableLock.Dispose();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// If <paramref name="hKey"/> is a known key alias, the associated virtual key handle is returned.
    /// Otherwise, the given <paramref name="hKey"/> is returned.
    /// </summary>
    /// <remarks>
    /// This method should be called in all methods having a parameter which represents a virtual key handle.
    /// </remarks>
    /// <param name="hKey"></param>
    /// <returns></returns>
    private uint EnsureHandleIsNoAlias(uint hKey)
    {
      var readLock = _keysSynchronizationLock.IsAnyLockHeld()
                       ? null
                       : _keysSynchronizationLock.EnterDisposableReadLock();
      hKey = _keyAliases.ContainsKey(hKey)
               ? _keyAliases[hKey]
               : hKey;
      if (readLock != null)
        readLock.Dispose();
      return hKey;
    }

    /// <summary>
    /// Loads all values to the provided <see cref="VirtualRegistryKey"/>.
    /// </summary>
    /// <param name="registryKey">The <see cref="VirtualRegistryKey"/> to load the values for.</param>
    /// <param name="overwriteIfExists">
    /// Set to true if the values in <paramref name="registryKey"/> must overwrite existing values, if any.
    /// </param>
    /// <param name="handleOwnLock">
    /// Set to true if this method should handle its own read lock on <see cref="_keysSynchronizationLock"/>.
    /// </param>
    /// <returns>The <see cref="VirtualRegistryKey"/> with all its values loaded.</returns>
    private VirtualRegistryKey LoadAllValues(VirtualRegistryKey registryKey, bool overwriteIfExists, bool handleOwnLock)
    {
      if (handleOwnLock)
        _keysSynchronizationLock.EnterReadLock();
      try
      {
        if (!_keys.ContainsKey(registryKey.Handle))
          return registryKey;
        VirtualRegistryKey loadedKey = _keys[registryKey.Handle];
        foreach (var valuePair in registryKey.Values)
        {
          if (!loadedKey.Values.ContainsKey(valuePair.Key))
            loadedKey.Values.Add(valuePair);
          else if (overwriteIfExists)
            loadedKey.Values[valuePair.Key] = valuePair.Value;
        }
        return loadedKey;
      }
      finally
      {
        if (handleOwnLock)
          _keysSynchronizationLock.ExitReadLock();
      }
    }

    /// <summary>
    /// Writes or overwrites the provided <see cref="VirtualRegistryKey"/> to <see cref="_keys"/>.
    /// This method needs to be able to acquire a write lock on <see cref="_keysSynchronizationLock"/>.
    /// </summary>
    /// <exception cref="ThreadStateException">
    /// A <see cref="ThreadStateException"/> is thrown if the current thread
    /// can't acquire a writelock on <see cref="_keysSynchronizationLock"/>.
    /// </exception>
    /// <param name="registryKey"><see cref="VirtualRegistryKey"/> to write to the database.</param>
    private void WriteKey(VirtualRegistryKey registryKey)
    {
      if (!_keysSynchronizationLock.TryEnterWriteLock(2500))
        throw new ThreadStateException(
          string.Format("Thread {0} can't get a write-lock to write the new key with path {1}.",
          Thread.CurrentThread.Name, registryKey.Path));
      try
      {
        if (_keys.ContainsKey(registryKey.Handle))
          _keys[registryKey.Handle] = registryKey;
        else
          _keys.Add(registryKey.Handle, registryKey);
      }
      finally
      {
        _keysSynchronizationLock.ExitWriteLock();
      }
    }

    #endregion

    #region IIndexUser Members

    public bool IsUsedIndex(uint index)
    {
      var readLock = _keysSynchronizationLock.IsAnyLockHeld()
                       ? null
                       : _keysSynchronizationLock.EnterDisposableReadLock();
      var result = _keys.ContainsKey(index) || _keyAliases.ContainsKey(index);
      if (readLock != null)
        readLock.Dispose();
      return result;
    }

    #endregion

  }
}
