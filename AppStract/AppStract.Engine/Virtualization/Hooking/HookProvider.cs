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
using System.Collections.Generic;

namespace AppStract.Engine.Virtualization.Hooking
{
  /// <summary>
  /// Abstract class representing a type able to provide API hooks.
  /// </summary>
  public abstract class HookProvider<THandler> : IHookProvider
    where THandler : HookHandler
  {

    #region Properties

    /// <summary>
    /// Gets the instance handling all intercepted API calls.
    /// </summary>
    protected THandler Handler
    {
      get; private set;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes the hook provider with the given <see cref="HookHandler"/>.
    /// </summary>
    /// <param name="handler">The instance to use for handling intercepted API calls.</param>
    protected HookProvider(THandler handler)
    {
      Handler = handler;
    }

    #endregion

    #region IHookProvider Members

    /// <summary>
    /// Initializes the current provider.
    /// </summary>
    public void Initialize()
    {
      Handler.Initialize();
    }

    /// <summary>
    /// Installs all hooks known by the current <see cref="HookProvider{T}"/>.
    /// </summary>
    /// <exception cref="HookingException">
    /// A <see cref="HookingException"/> is thrown if the installation of any of the API hooks fails.
    /// </exception>
    /// <param name="installHook">A delegate to the method to use for installing the API hooks.</param>
    public void InstallHooks(HookInstaller installHook)
    {
      var hooks = GetHooks();
      foreach (var hook in hooks)
      {
        try
        {
          installHook(hook.GetTargetEntryPoint(), hook.Handler, hook.Callback);
          EngineCore.Log.Debug("Installed API hook: " + hook);
        }
        catch (Exception e)
        {
          EngineCore.Log.Error("Failed to install API hook: " + hook, e);
          throw new HookingException("Failed to install API hook.", hook, e);
        }
      }
    }

    #endregion

    #region Protected Members
    
    /// <summary>
    /// Returns all data needed for installing the API hooks provided by the current <see cref="HookProvider{T}"/>.
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<HookData> GetHooks();

    #endregion

  }
}
