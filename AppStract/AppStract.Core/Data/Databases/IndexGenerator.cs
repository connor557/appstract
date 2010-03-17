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

namespace AppStract.Core.Data.Databases
{
  /// <summary>
  /// Generates indices to be used in a database.
  /// Users must first register before being able to acquire indices.
  /// </summary>
  public sealed class IndexGenerator
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

    /// <summary>
    /// Gets a collection of the ranges of indices that are excluded by the current <see cref="IndexGenerator"/>.
    /// Indices within these ranges will thereby never be returned by <see cref="Next"/>.
    /// </summary>
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
      foreach (var range in excludedRanges)
        _excludedRanges.Add(range);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the first free index.
    /// </summary>
    /// <param name="indexRequester">The object requesting the new key, usually the object invoking this method.</param>
    /// <returns></returns>
    public uint Next(IIndexUser indexRequester)
    {
      RegisterUser(indexRequester);
      lock (_indicesLock)
      {
        if (_freeIndices.Count != 0)
        {
          var freeIndex = _freeIndices[_freeIndices.Count - 1];
          _freeIndices.RemoveAt(_freeIndices.Count - 1);
          return freeIndex;
        }
        /// ELSE: find a new index.
        do
        {
          _currentIndex++;
        } while (IsInUse(_currentIndex));
        return _currentIndex;
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
    /// This means that new indices, generated with <see cref="Next"/>, will not be verified anymore
    /// from <paramref name="indexUser"/> and may thereby be simultaneously used by the detached object.
    /// </summary>
    /// <param name="indexUser">The <see cref="IIndexUser"/> to detach.</param>
    /// <returns>
    /// True if the specified <see cref="IIndexUser"/> was successfully detached from the <see cref="IndexGenerator"/>; otherwise, false.
    /// This method also returns false if the <paramref name="indexUser"/> is not found to be registered with the current <see cref="IndexGenerator"/>.
    /// </returns>
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
    private void RegisterUser(IIndexUser user)
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
    private bool IsInUse(uint indexToValidate)
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
