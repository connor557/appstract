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

using AppStract.Core.Virtualization.Engine;
using AppStract.Core.Virtualization.Engine.Registry;

namespace AppStract.Manager.Utilities.ApplicationConfiguration
{
  public class RegistryEngineSettingsPageContent : EngineSettingsPageContent
  {

    #region Overrides

    protected override string Title
    {
      get { return "Registry"; }
    }

    protected override string KeyText
    {
      get { return "Key Full Path"; }
    }

    protected override string KeyItemText
    {
      get { return "Value Name"; }
    }

    protected override string RuleSelectorText
    {
      get { return "Virtualization Type"; }
    }

    protected override EngineRuleCollection GetEmptyRuleCollection()
    {
      return RegistryRuleCollection.GetEmptyRuleCollection();
    }

    #endregion

  }
}
