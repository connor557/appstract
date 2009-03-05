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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Win32.Interop;

namespace AppStract.Server.Providers.Registry.Data
{
  /// <summary>
  /// Base class for all classes functioning as databases in the virtual registry.
  /// </summary>
  public abstract class RegistryDatabase : IIndexUser
  {

    #region Variables

    /// <summary>
    /// Generates the indices used by the current <see cref="RegistryDatabase"/>.
    /// </summary>
    protected readonly IndexGenerator _indexGenerator;
    /// <summary>
    /// Holds all known <see cref="VirtualRegistryKey"/>s.
    /// </summary>
    protected readonly IDictionary<uint, VirtualRegistryKey> _keys;
    /// <summary>
    /// Monitor used for synchronization on the current <see cref="RegistryDatabase"/>.
    /// </summary>
    protected readonly ReaderWriterLockSlim _keysSynchronizationLock;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes <see cref="_indexGenerator"/>, <see cref="_keys"/>, and <see cref="_keysSynchronizationLock"/>.
    /// </summary>
    /// <param name="indexGenerator">The <see cref="IndexGenerator"/> to use for the new instance.</param>
    protected RegistryDatabase(IndexGenerator indexGenerator)
    {
      _indexGenerator = indexGenerator;
      _keys = new Dictionary<uint, VirtualRegistryKey>();
      _keysSynchronizationLock = new ReaderWriterLockSlim();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether the database knows a key with the specified index.
    /// </summary>
    /// <param name="hkey">The index to search a key for.</param>
    /// <returns></returns>
    public bool IsKnownKey(uint hkey)
    {
      string tmp;
      return IsKnownKey(hkey, out tmp);
    }

    /// <summary>
    /// Returns whether the database knows a key with the specified index.
    /// The full path of the key is set to <paramref name="keyFullPath"/> if the key is known.
    /// </summary>
    /// <param name="hkey">The index to search a key for.</param>
    /// <param name="keyFullPath">The name of the key, as used in the host's registry.</param>
    /// <returns></returns>
    public bool IsKnownKey(uint hkey, out string keyFullPath)
    {
      keyFullPath = null;
      _keysSynchronizationLock.EnterReadLock();
      try
      {
        if (!_keys.Keys.Contains(hkey))
          return false;
        keyFullPath = RegistryTranslator.ToRealPath(_keys[hkey].Path);
        return true;
      }
      finally
      {
        _keysSynchronizationLock.ExitReadLock();
      }
    }

    /// <summary>
    /// Deletes the key with the specified index.
    /// </summary>
    /// <param name="hKey"></param>
    /// <returns>A <see cref="WinError"/> code.</returns>
    public virtual StateCode DeleteKey(uint hKey)
    {
      _keysSynchronizationLock.EnterWriteLock();
      try
      {
        return _keys.Remove(hKey)
                 ? StateCode.Succes
                 : StateCode.InvalidHandle;
      }
      finally
      {
        _keysSynchronizationLock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Opens the key from the specified path.
    /// </summary>
    /// <param name="keyFullPath"></param>
    /// <returns></returns>
    public virtual uint? OpenKey(string keyFullPath)
    {
      keyFullPath = RegistryTranslator.ToVirtualPath(keyFullPath);
      _keysSynchronizationLock.EnterUpgradeableReadLock();  /// We're not sure if this method is readonly.
      try
      {
        /// Try to find the key in the virtual registry.
        VirtualRegistryKey virtualRegistryKey
          = _keys.Values.First(key => key.Path.ToLowerInvariant() == keyFullPath);
        if (virtualRegistryKey == null)
        {
          /// Create a new key.
          virtualRegistryKey = ConstructRegistryKey(keyFullPath);
          WriteKey(virtualRegistryKey, false);
        }
        return virtualRegistryKey.Index;
      }
      finally
      {
        _keysSynchronizationLock.ExitUpgradeableReadLock();
      }
    }

    /// <summary>
    /// Creates a key with the specified path.
    /// </summary>
    /// <param name="keyFullPath">The path for the new key.</param>
    /// <param name="hKey">The allocated index.</param>
    /// <returns>A <see cref="WinError"/> code.</returns>
    public virtual StateCode CreateKey(string keyFullPath, out uint? hKey)
    {
      uint? index = OpenKey(keyFullPath);
      if (index != null)
      {
        hKey = index;
      }
      else
      {
        VirtualRegistryKey key = ConstructRegistryKey(keyFullPath);
        WriteKey(key, false);
        hKey = key.Index;
      }
      return StateCode.Succes;
    }

    /// <summary>
    /// Retrieves the value associated with the specified key and name.
    /// Returns null if the key or name is not found.
    /// </summary>
    /// <param name="hKey"></param>
    /// <param name="valueName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public abstract StateCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value);

    public abstract StateCode SetValue(uint hKey, string valueName, VirtualRegistryValue value);

    public abstract StateCode DeleteValue(uint hKey, string valueName);

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
    /// This method needs to be able to acquire a read and a write lock on <see cref="_keysSynchronizationLock"/>.
    /// </summary>
    /// <exception cref="ThreadStateException">
    /// A <see cref="ThreadStateException"/> is thrown if the current thread
    /// can't acquire a read or write lock on <see cref="_keysSynchronizationLock"/>.
    /// </exception>
    /// <param name="registryKey"><see cref="VirtualRegistryKey"/> to write to the database.</param>
    /// <param name="loadAllValuesFirst"> Set to true if all existing values of the key must be saved,
    /// even if they are not specified in <see cref="VirtualRegistryKey.Values"/>.</param>
    protected void WriteKey(VirtualRegistryKey registryKey, bool loadAllValuesFirst)
    {
      if (loadAllValuesFirst)
        registryKey = LoadAllValues(registryKey, true, true);
      WriteKey(registryKey);
    }

    #endregion

    #region Private Methods

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
        if (!_keys.ContainsKey(registryKey.Index))
          return registryKey;
        VirtualRegistryKey loadedKey = _keys[registryKey.Index];
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
    /// Writes the provided <see cref="VirtualRegistryKey"/> to <see cref="_keys"/>.
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
          Thread.CurrentThread, registryKey.Path));
      try
      {
        if (_keys.ContainsKey(registryKey.Index))
          _keys[registryKey.Index] = registryKey;
        else
          _keys.Add(registryKey.Index, registryKey);
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
      _keysSynchronizationLock.EnterReadLock();
      try
      {
        return _keys.Keys.Contains(index);
      }
      finally
      {
        _keysSynchronizationLock.ExitReadLock();
      }
    }

    #endregion

  }
}
