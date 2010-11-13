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
using System.Runtime.Serialization;

namespace AppStract.Engine.Configuration
{
  /// <summary>
  /// Represents a collection of <see cref="EngineRule"/> objects specifying rules on the registry virtualization engine.
  /// </summary>
  [Serializable]
  public sealed class RegistryRuleCollection : EngineRuleCollection
  {

    #region Constructors

    private RegistryRuleCollection()
    {

    }

    private RegistryRuleCollection(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {

    }

    #endregion

    #region Protected Methods

    protected override IEnumerable<EngineRule> GetDefaultRules()
    {
      return new[]
               {
                 new EngineRule("HKEY_USERS%", VirtualizationType.VirtualWithFallback),
                 new EngineRule("HKEY_CURRENT_USER%", VirtualizationType.VirtualWithFallback),
                 new EngineRule("HKEY_CURRENT_CONFIG%", VirtualizationType.TransparentRead),
                 new EngineRule("HKEY_LOCAL_MACHINE%", VirtualizationType.TransparentRead),
                 new EngineRule("HKEY_CLASSES_ROOT%", VirtualizationType.TransparentRead),
                 new EngineRule("HKEY_PERFORMANCE_DATA%", VirtualizationType.Transparent),
                 new EngineRule("HKEY_DYN_DATA%", VirtualizationType.Transparent)
               };
    }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Returns an empty collection of registry rules.
    /// </summary>
    /// <returns></returns>
    public static RegistryRuleCollection GetEmptyRuleCollection()
    {
      return new RegistryRuleCollection();
    }

    /// <summary>
    /// Returns the default collection of registry rules.
    /// </summary>
    /// <returns></returns>
    public static RegistryRuleCollection GetDefaultRuleCollection()
    {
      var rules = new RegistryRuleCollection();
      rules.PopulateWithDefaultRules();
      return rules;
    }

    #endregion

  }
}
