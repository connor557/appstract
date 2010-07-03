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

namespace AppStract.Core.Virtualization.Engine.FileSystem
{
  /// <summary>
  /// Represents a collection of <see cref="EngineRule"/> objects specifying rules on the file system virtualization engine.
  /// </summary>
  [Serializable]
  public sealed class FileSystemRuleCollection : EngineRuleCollection
  {

    #region Constructors

    private FileSystemRuleCollection()
    {

    }

    private FileSystemRuleCollection(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {

    }

    #endregion

    #region Protected Methods

    protected override IEnumerable<EngineRule> GetDefaultRules()
    {
      return new EngineRule[0];
    }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Returns an empty collection of file system rules.
    /// </summary>
    /// <returns></returns>
    public static FileSystemRuleCollection GetEmptyRuleCollection()
    {
      return new FileSystemRuleCollection();
    }

    /// <summary>
    /// Returns the default collection of file system rules.
    /// </summary>
    /// <returns></returns>
    public static FileSystemRuleCollection GetDefaultRuleCollection()
    {
      var rules = new FileSystemRuleCollection();
      rules.PopulateWithDefaultRules();
      return rules;
    }

    #endregion

  }
}
