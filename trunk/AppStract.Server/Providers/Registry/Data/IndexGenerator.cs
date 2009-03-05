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

namespace AppStract.Server.Providers.Registry.Data
{
  /// <summary>
  /// Generates indices to be used for registry keys.
  /// </summary>
  public class IndexGenerator
  {

    #region Variables

    /// <summary>
    /// The by the current instance highest generated unique index.
    /// </summary>
    private uint _currentIndex;
    /// <summary>
    /// All indices that have been released and are allowed to be reused.
    /// </summary>
    private readonly IList<uint> _freeIndices;
    /// <summary>
    /// All users of the current <see cref="IndexGenerator"/>.
    /// </summary>
    private readonly IList<IIndexUser> _users;
    /// <summary>
    /// Object to lock when using <see cref="_currentIndex"/> or <see cref="_freeIndices"/>.
    /// </summary>
    private readonly object _indicesLock;
    /// <summary>
    /// Locker for <see cref="_users"/>.
    /// </summary>
    private readonly ReaderWriterLockSlim _usersLock;
    /// <summary>
    /// The ranges to exclude when generating indices;
    /// </summary>
    private readonly IList<IndexRange> _excludedRanges;

    #endregion

    #region Properties

    public ICollection<IndexRange> ExcludedRanges
    {
      get { return _excludedRanges; }
    }

    #endregion

    #region Constructors

    public IndexGenerator()
    {
      _currentIndex = 0;
      _freeIndices = new List<uint>();
      _users = new List<IIndexUser>(2);
      _indicesLock = new object();
      _usersLock = new ReaderWriterLockSlim();
      _excludedRanges = new List<IndexRange>();
    }

    public IndexGenerator(IEnumerable<IndexRange> excludedRanges)
      : this()
    {
      foreach (IndexRange range in excludedRanges)
        _excludedRanges.Add(range);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the first free index for registry keys.
    /// </summary>
    /// <param name="indexRequester">The instance requesting the new key, usually "this".</param>
    /// <returns></returns>
    public uint Next(IIndexUser indexRequester)
    {
      ValidateUser(indexRequester);
      lock (_indicesLock)
      {
        if (_freeIndices.Count == 0)
        {
          do
          {
            _currentIndex++;
          } while (IsExisting(_currentIndex)); 
          return _currentIndex;
        }
        /// ELSE: get the index from the free indices.
        uint keyIndex = _freeIndices[_freeIndices.Count - 1];
        _freeIndices.RemoveAt(_freeIndices.Count - 1);
        return keyIndex;
      }
    }

    /// <summary>
    /// Frees the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">Index to release.</param>
    public void Release(uint index)
    {
      lock (_indicesLock)
        _freeIndices.Add(index);
    }

    /// <summary>
    /// Detaches the specified <see cref="IIndexUser"/> from the current <see cref="IndexGenerator"/>.
    /// This means that new indices, generated with <see cref="Next"/>, may be simultaneously used
    /// by the detached object.
    /// </summary>
    /// <param name="indexUser"></param>
    /// <returns></returns>
    public bool Detach(IIndexUser indexUser)
    {
      _usersLock.EnterWriteLock();
      try
      {
        return _users.Remove(indexUser);
      }
      finally
      {
        _usersLock.ExitWriteLock();
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validates the specified <see cref="IIndexUser"/>.
    /// The <paramref name="user"/> is added to <see cref="_users"/>
    /// if the user isn't known by the current <see cref="IndexGenerator"/>.
    /// </summary>
    /// <param name="user"></param>
    private void ValidateUser(IIndexUser user)
    {
      _usersLock.EnterUpgradeableReadLock();
      try
      {
        if (_users.Contains(user))
          return;
        _usersLock.EnterWriteLock();
        try
        {
          _users.Add(user); 
        }
        finally
        {
          _usersLock.ExitWriteLock();
        }
      }
      finally
      {
        _usersLock.ExitUpgradeableReadLock();
      }
    }

    /// <summary>
    /// Returns true if the specified index (<paramref name="indexToValidate"/>)
    /// is already used by one of the users (<see cref="_users"/>).
    /// </summary>
    /// <param name="indexToValidate"></param>
    /// <returns></returns>
    private bool IsExisting(uint indexToValidate)
    {
      /// First check the ranges with indices to exclude.
      foreach (IndexRange range in _excludedRanges)
        if (range.IsInRange(indexToValidate))
          return true;
      /// Now check if one of the users holds the index.
      _usersLock.EnterReadLock();
      try
      {
        foreach (IIndexUser user in _users)
        {
          if (user.IsUsedIndex(indexToValidate))
            return true;
        }
      }
      finally
      {
        _usersLock.ExitReadLock();
      }
      return false;
    }

    #endregion

  }
}
