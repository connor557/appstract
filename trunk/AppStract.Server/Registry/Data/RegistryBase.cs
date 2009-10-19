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
using System.Threading;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Registry;
using Microsoft.Win32;
using Microsoft.Win32.Interop;

namespace AppStract.Server.Registry.Data
{
  /// <summary>
  /// Base class for all classes functioning as databases in the virtual registry.
  /// </summary>
  public abstract class RegistryBase : IIndexUser
  {

    #region Variables

    /// <summary>
    /// Generates the indices used by the current <see cref="RegistryBase"/>.
    /// </summary>
    protected readonly IndexGenerator _indexGenerator;
    /// <summary>
    /// Holds all known <see cref="VirtualRegistryKey"/>s.
    /// </summary>
    protected readonly IDictionary<uint, VirtualRegistryKey> _keys;
    /// <summary>
    /// Monitor used for synchronization on the current <see cref="RegistryBase"/>.
    /// </summary>
    protected readonly ReaderWriterLockSlim _keysSynchronizationLock;

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
    public virtual NativeResultCode DeleteKey(uint hKey)
    {
      _keysSynchronizationLock.EnterWriteLock();
      try
      {
        return _keys.Remove(hKey)
                 ? NativeResultCode.Succes
                 : NativeResultCode.InvalidHandle;
      }
      finally
      {
        _keysSynchronizationLock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Creates a key with the specified path.
    /// </summary>
    /// <param name="keyFullPath">The path for the new key.</param>
    /// <param name="hKey">The allocated index.</param>
    /// <param name="creationDisposition">Whether the key is opened or created.</param>
    /// <returns>A <see cref="WinError"/> code.</returns>
    public virtual NativeResultCode CreateKey(string keyFullPath, out uint hKey, out RegCreationDisposition creationDisposition)
    {
      if(OpenKey(keyFullPath, out hKey))
      {
        creationDisposition = RegCreationDisposition.REG_OPENED_EXISTING_KEY;
      }
      else
      {
        creationDisposition = RegCreationDisposition.REG_CREATED_NEW_KEY;
        VirtualRegistryKey key = ConstructRegistryKey(keyFullPath);
        WriteKey(key, false);
        hKey = key.Handle;
      }
      return NativeResultCode.Succes;
    }

    /// <summary>
    /// Opens the key from the specified path.
    /// </summary>
    /// <param name="keyFullPath"></param>
    /// <param name="hResult"></param>
    /// <returns></returns>
    public abstract bool OpenKey(string keyFullPath, out uint hResult);

    /// <summary>
    /// Retrieves the value associated with the specified key and name.
    /// Returns null if the key or name is not found.
    /// </summary>
    /// <param name="hKey"></param>
    /// <param name="valueName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public abstract NativeResultCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value);

    /// <summary>
    /// Sets a value for the key with the specified handle.
    /// </summary>
    /// <param name="hKey">Handle of the key to set a value for.</param>
    /// <param name="value">The data to set for the value.</param>
    /// <returns></returns>
    public abstract NativeResultCode SetValue(uint hKey, VirtualRegistryValue value);

    /// <summary>
    /// Deletes a value from the key with the specified handle.
    /// </summary>
    /// <param name="hKey">Key to delete a value from.</param>
    /// <param name="valueName">The name of the value to delete.</param>
    /// <returns></returns>
    public abstract NativeResultCode DeleteValue(uint hKey, string valueName);

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

    /// <summary>
    /// Returns whether the key with the specified path exists in the current host's registry.
    /// </summary>
    /// <param name="keyFullPath">The full path of the key, including the root key.</param>
    /// <returns>Whether the key exist's in the current host's registry.</returns>
    protected static bool KeyExistsInHostRegistry(string keyFullPath)
    {
      RegistryKey key = ReadKeyFromHostRegistry(keyFullPath, false);
      if (key == null)
        return false;
      key.Close();
      return true;
    }

    /// <summary>
    /// Reads the specified key from the host's registry.
    /// </summary>
    /// <param name="keyFullPath">The full path of the key, including the root key.</param>
    /// <param name="writable">Set to true if write access is required.</param>
    /// <returns>Null if key isn't read from the host's registry.</returns>
    protected static RegistryKey ReadKeyFromHostRegistry(string keyFullPath, bool writable)
    {
      string subKeyName;
      RegistryKey registryKey = RegistryHelper.GetHiveAsKey(keyFullPath, out subKeyName);
      if (registryKey == null)
        return null;
      if (subKeyName == null)
        return registryKey;
      RegistryKey subRegistryKey;
      try
      {
        subRegistryKey = registryKey.OpenSubKey(subKeyName, writable);
        registryKey.Close();
      }
      catch
      {
        return null;
      }
      return subRegistryKey;
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
