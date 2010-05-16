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
using System.Runtime.Serialization;

namespace AppStract.Core.Virtualization.Engine.Registry
{
  [Serializable]
  public sealed class RegistryRuleCollection : EngineRuleCollectionBase<string, AccessMechanism>
  {

    #region Constructors

    public RegistryRuleCollection()
    {

    }

    private RegistryRuleCollection(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {

    }

    #endregion

    #region Protected Methods

    protected override bool Matches(string value, string otherValue)
    {
      if (string.IsNullOrEmpty(value))
        return string.IsNullOrEmpty(otherValue);
      if (string.IsNullOrEmpty(otherValue))
        return string.IsNullOrEmpty(value);
      const char wildcard = '%';
      value = value.ToLowerInvariant();
      otherValue = otherValue.ToLowerInvariant();
      if (value[0] == wildcard)
        return value[value.Length - 1] == wildcard
                 ? otherValue.Contains(value.Substring(1, value.Length - 2))
                 : otherValue.EndsWith(value.Substring(1));
      if (value[value.Length - 1] == wildcard)
        return otherValue.StartsWith(value.Substring(0, value.Length - 1));
      return value == otherValue;
    }

    #endregion

  }
}
