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
using EasyHook;

namespace AppStract.Engine.Engine.Hooking
{
  /// <summary>
  /// The data needed for creating a managed hook.
  /// </summary>
  public struct HookData
  {

    #region Variables

    /// <summary>
    /// Description of the hook that can be installed with the current <see cref="HookData"/>.
    /// </summary>
    private readonly string _description;
    /// <summary>
    /// A system DLL name like "kernel32.dll" or a full qualified path to any DLL.
    /// </summary>
    private readonly string _targetLibraryName;
    /// <summary>
    /// An exported symbol name like "CreateFileW".
    /// </summary>
    private readonly string _targetSymbolName;
    /// <summary>
    /// Handler with the same signature as the original entry point that will
    /// be invoked for every call that has passed the Fiber Deadlock Barrier
    /// and various integrity checks.
    /// </summary>
    private readonly Delegate _handler;
    /// <summary>
    /// Uninterpreted callback that will later be available through <see cref="HookRuntimeInfo.Callback"/>.
    /// </summary>
    private readonly object _callback;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the description of the hook that can be installed with the current <see cref="HookData"/>.
    /// </summary>
    public string Description
    {
      get { return _description; }
    }

    /// <summary>
    /// Gets the name of the library containing the to-be-hooked symbol.
    /// </summary>
    public string TargetLibrary
    {
      get { return _targetLibraryName; }
    }

    /// <summary>
    /// Gets the name of the symbol that can be hooked with the data from the current <see cref="HookData"/>.
    /// </summary>
    public string TargetSymbol
    {
      get { return _targetSymbolName; }
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
    /// Initializes a new instance of <see cref="HookData"/>.
    /// </summary>
    /// <param name="description">
    /// The description of the API hook that can be installed with this <see cref="HookData"/>.
    /// </param>
    /// <param name="targetLibrary">
    /// The name or a full qualified path of the system DLL containing <paramref name="targetSymbol"/>.
    /// </param>
    /// <param name="targetSymbol">
    /// The exported symbol name of the target function, for example "CreateFileW".
    /// </param>
    /// <param name="handler">
    /// The handler with the same signature as the function described by
    /// the combination of <paramref name="targetLibrary"/> and <paramref name="targetSymbol"/>.
    /// After the hooks are installed, this is the handler that will be invoked for every call
    /// that has passed the various integrity checks conducted by EasyHook.
    /// </param>
    /// <param name="callback">
    /// Uninterpreted callback that will later be available through <see cref="HookRuntimeInfo.Callback"/>.
    /// </param>
    public HookData(string description, string targetLibrary, string targetSymbol, Delegate handler, object callback)
    {
      _description = description;
      _targetLibraryName = targetLibrary;
      _targetSymbolName = targetSymbol;
      _handler = handler;
      _callback = callback;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the target entry point that should be hooked.
    /// </summary>
    /// <exception cref="HookingException">
    /// A <see cref="HookingException"/> is thrown if no <see cref="IntPtr"/> can be retrieved
    /// for the API hook described by the current <see cref="HookData"/>.
    /// </exception>
    public IntPtr GetTargetEntryPoint()
    {
      try
      {
        return LocalHook.GetProcAddress(_targetLibraryName, _targetSymbolName);
      }
      catch (SystemException e)
      {
        throw new HookingException("Failed to get pointer for API Hook: " + _description, this, e);
      }
    }

    /// <summary>
    /// Returns a string representation of the current <see cref="HookData"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "[" + _targetLibraryName + " | " + _targetSymbolName + "] " + _description;
    }

    #endregion

  }
}
