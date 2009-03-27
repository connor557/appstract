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
using EasyHook;

namespace AppStract.Server.Inject
{

  /// <summary>
  /// The data needed for creating a managed hook.
  /// </summary>
  public struct HookData
  {

    #region Variables

    /// <summary>
    /// Target entry point that should be hooked.
    /// </summary>
    private readonly IntPtr _targetEntryPoint;
    /// <summary>
    /// Handler with the same signature as the original entry point that will
    /// be invoked for every call that has passed the Fiber Deadlock Barrier
    /// and various integrity checks.
    /// </summary>
    private readonly Delegate _handler;
    /// <summary>
    /// Uninterpreted callback that will later be available through <seealso cref="HookRuntimeInfo.Callback"/>.
    /// </summary>
    private readonly object _callback;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the target entry point that should be hooked.
    /// </summary>
    public IntPtr TargetEntryPoint
    {
      get { return _targetEntryPoint; }
    }

    /// <summary>
    /// Gets the handler, which has the same signature as the original entry point
    /// </summary>
    public Delegate Handler
    {
      get { return _handler; }
    }

    /// <summary>
    /// Gets the callback object.
    /// </summary>
    public object Callback
    {
      get { return _callback; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetEntryPoint">
    /// The target entry point that should be hooked.
    /// </param>
    /// <param name="handler">The handler with the same signature as the original entry
    /// point that will be invoked for every call that has passed the Fiber Deadlock Barrier
    /// and various integrity checks.
    /// </param>
    /// <param name="callback">
    /// Uninterpreted callback that will later be available through
    /// <seealso cref="HookRuntimeInfo.Callback"/>.
    /// </param>
    public HookData(IntPtr targetEntryPoint, Delegate handler, object callback)
    {
      _targetEntryPoint = targetEntryPoint;
      _handler = handler;
      _callback = callback;
    }

    #endregion

    #region Public Methods

    // Override ToString() for debug use
    public override string ToString()
    {
      return "[" + _targetEntryPoint + "] " + _handler.Method.Name;
    }

    #endregion

  }
}
