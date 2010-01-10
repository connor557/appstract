#region Copyright (C) 2009-2010 Simon Allaeys

/*
    Copyright (C) 2009-2010 Simon Allaeys
 
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
using System.Threading;

namespace AppStract.Utilities.Extensions
{
  /// <summary>
  /// Holds extension methods for <see cref="ReaderWriterLockSlim"/>.
  /// </summary>
  public static class ReaderWriterLockSlimExtensions
  {

    /// <summary>
    /// Tries to enter the lock in read mode,
    /// and returns an object that exits the acquired lock on disposal.
    /// </summary>
    /// <exception cref="LockRecursionException">
    /// The <see cref="ReaderWriterLockSlim.RecursionPolicy"/> property is <see cref="LockRecursionPolicy.NoRecursion"/> and the current thread has already entered read mode. 
    /// -or-
    /// The recursion number would exceed the capacity of the counter. This limit is so large that applications should never encounter it.
    /// </exception>
    /// <param name="lockSlim"></param>
    /// <returns>An object that exits the acquired lock on disposal.</returns>
    public static IDisposable EnterDisposableReadLock(this ReaderWriterLockSlim lockSlim)
    {
      lockSlim.EnterReadLock();
      return new LockExiter(lockSlim.ExitReadLock, lockSlim.HoldsReadLock);
    }

    /// <summary>
    /// Tries to enter the lock in upgradeable mode,
    /// and returns an object that exits the acquired lock on disposal.
    /// </summary>
    /// <exception cref="LockRecursionException">
    /// The <see cref="ReaderWriterLockSlim.RecursionPolicy"/> property is <see cref="LockRecursionPolicy.NoRecursion"/> and the current thread has already entered the lock in any mode. 
    /// -or-
    /// The current thread has entered read mode, so trying to enter upgradeable mode would create the possibility of a deadlock. 
    /// -or-
    /// The recursion number would exceed the capacity of the counter. This limit is so large that applications should never encounter it.
    /// </exception>
    /// <param name="lockSlim"></param>
    /// <returns>An object that exits the acquired lock on disposal.</returns>
    public static IDisposable EnterDisposableUpgradeableReadLock(this ReaderWriterLockSlim lockSlim)
    {
      lockSlim.EnterUpgradeableReadLock();
      return new LockExiter(lockSlim.ExitUpgradeableReadLock, lockSlim.HoldsUpgradeableReadLock);
    }

    /// <summary>
    /// Tries to enter the lock in write mode,
    /// and returns an object that exits the acquired lock on disposal.
    /// </summary>
    /// <exception cref="LockRecursionException">
    /// The <see cref="ReaderWriterLockSlim.RecursionPolicy"/> property is <see cref="LockRecursionPolicy.NoRecursion"/> and the current thread has already entered the lock in any mode. 
    /// -or-
    /// The current thread has entered read mode, so trying to enter the lock in write mode would create the possibility of a deadlock. 
    /// -or-
    /// The recursion number would exceed the capacity of the counter. This limit is so large that applications should never encounter it.
    /// </exception>
    /// <param name="lockSlim"></param>
    /// <returns>An object that exits the acquired lock on disposal.</returns>
    public static IDisposable EnterDisposableWriteLock(this ReaderWriterLockSlim lockSlim)
    {
      lockSlim.EnterWriteLock();
      return new LockExiter(lockSlim.EnterWriteLock, lockSlim.HoldsWriteLock);
    }

    /// <summary>
    /// Returns the value of <see cref="ReaderWriterLockSlim.IsReadLockHeld"/>.
    /// </summary>
    /// <param name="lockSlim"></param>
    /// <returns></returns>
    private static bool HoldsReadLock(this ReaderWriterLockSlim lockSlim)
    {
      return lockSlim.IsReadLockHeld;
    }

    /// <summary>
    /// Returns the value of <see cref="ReaderWriterLockSlim.IsUpgradeableReadLockHeld"/>.
    /// </summary>
    /// <param name="lockSlim"></param>
    /// <returns></returns>
    private static bool HoldsUpgradeableReadLock(this ReaderWriterLockSlim lockSlim)
    {
      return lockSlim.IsUpgradeableReadLockHeld;
    }

    /// <summary>
    /// Returns the value of <see cref="ReaderWriterLockSlim.IsWriteLockHeld"/>.
    /// </summary>
    /// <param name="lockSlim"></param>
    /// <returns></returns>
    private static bool HoldsWriteLock(this ReaderWriterLockSlim lockSlim)
    {
      return lockSlim.IsWriteLockHeld;
    }

    /// <summary>
    /// Helper class for exiting locks.
    /// </summary>
    private class LockExiter : IDisposable
    {

      #region Delegates

      public delegate void ExitLock();
      public delegate bool VerifyLock();

      #endregion

      #region Variables

      private readonly ExitLock _exitLock;
      private readonly VerifyLock _verifyLock;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of <see cref="LockExiter"/>.
      /// </summary>
      /// <param name="exitLockMethod">The delegate to invoke if a call to <paramref name="verifyLockMethod"/> returns true.</param>
      /// <param name="verifyLockMethod">The delegate indicating whether <paramref name="exitLockMethod"/> is allowed to be invoked.</param>
      public LockExiter(ExitLock exitLockMethod, VerifyLock verifyLockMethod)
      {
        _exitLock = exitLockMethod;
        _verifyLock = verifyLockMethod;
      }

      #endregion

      #region IDisposable Members

      /// <summary>
      /// Exits the acquired lock.
      /// </summary>
      public void Dispose()
      {
        if (_verifyLock())
          _exitLock();
      }

      #endregion
    
    }

  }
}
