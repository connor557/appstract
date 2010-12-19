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

using System.Collections.Generic;
using AppStract.Engine.Virtualization.Registry;

namespace AppStract.Engine.Virtualization.Hooking
{
  /// <summary>
  /// Provides all API hooks regarding the registry.
  /// </summary>
  public partial class RegistryHookProvider : HookProvider<RegistryHookHandler>
  {

    #region Variables

    /// <summary>
    /// The object to use as callback object for the API hooks.
    /// </summary>
    private readonly object _callback;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="RegistryHookProvider"/>.
    /// </summary>
    /// <param name="registryProvider">The <see cref="IRegistryProvider"/> to use for the processing of intercepted API calls.</param>
    public RegistryHookProvider(IRegistryProvider registryProvider)
      : base (new RegistryHookHandler(registryProvider))
    {
      _callback = registryProvider;
    }

    #endregion

    #region Protected Methods

    protected override IEnumerable<HookData> GetHooks()
    {
      var hooks = new List<HookData>(8);
      hooks.Add(new HookData("Open Registry Key [Unicode]",
                             "advapi32.dll", "RegOpenKeyExW",
                             new Delegates.OpenKeyW(Handler.OpenKey),
                             _callback));
      hooks.Add(new HookData("Open Registry Key [Ansi]",
                             "advapi32.dll", "RegOpenKeyExA",
                             new Delegates.OpenKeyA(Handler.OpenKey),
                             _callback));
      hooks.Add(new HookData("Create Registry Key [Unicode]",
                             "advapi32.dll", "RegCreateKeyExW",
                             new Delegates.CreateKeyW(Handler.CreateKeyEx),
                             _callback));
      hooks.Add(new HookData("Create Registry Key [Ansi]",
                             "advapi32.dll", "RegCreateKeyExA",
                             new Delegates.CreateKeyA(Handler.CreateKeyEx),
                             _callback));
      hooks.Add(new HookData("Close Registry Key",
                             "advapi32.dll", "RegCloseKey",
                             new Delegates.CloseKey(Handler.CloseKey),
                             _callback));
      hooks.Add(new HookData("Set Registry Value [Unicode]",
                             "advapi32.dll", "RegSetValueExW",
                             new Delegates.SetValueW(Handler.SetValueEx),
                             _callback));
      hooks.Add(new HookData("Set Registry Value [Ansi]",
                             "advapi32.dll", "RegSetValueExA",
                             new Delegates.SetValueA(Handler.SetValueEx),
                             _callback));
      hooks.Add(new HookData("Query Registry Value [Unicode]",
                             "advapi32.dll", "RegQueryValueExW",
                             new Delegates.QueryValueW(Handler.QueryValue),
                             _callback));
      hooks.Add(new HookData("Query Registry Value [Ansi]",
                             "advapi32.dll", "RegQueryValueExA",
                             new Delegates.QueryValueA(Handler.QueryValue),
                             _callback));
      return hooks;
    }

    #endregion

  }
}
