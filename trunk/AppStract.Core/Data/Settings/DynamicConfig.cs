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
using System.IO;
using AppStract.Utilities.Assembly;

namespace AppStract.Core.Data.Settings
{
  public class DynamicConfig : IConfigurationObject
  {

    #region Properties

    /// <summary>
    /// Gets or sets the root folder of the current process.
    /// </summary>
    public string Root
    {
      get; private set;
    }

    #endregion

    #region IConfigurationObject Members

    public void LoadDefaults()
    {
      Root = null;
      var entryAssemblyFile = AssemblyHelper.GetEntryAssembly().Location;
      if (entryAssemblyFile != null)
        Root = Path.GetDirectoryName(entryAssemblyFile);
      if (string.IsNullOrEmpty(Root))
        /// Don't use "else ...", Path.GetDirectoryName() might return an empty string.
        Root = Environment.CurrentDirectory;
    }

    #endregion

  }
}
