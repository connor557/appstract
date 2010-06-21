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
using System.Xml.Serialization;

namespace AppStract.Core.Data.Application
{
  /// <summary>
  /// Stores files associated to an application.
  /// </summary>
  [Serializable]
  public sealed class ApplicationFiles
  {

    #region Properties

    /// <summary>
    /// Gets or sets the path of the root of the file system.
    /// </summary>
    [XmlElement]
    public ApplicationFile RootDirectory { get; set; }

    /// <summary>
    /// Gets or sets the path of the file containing the SQLite database holding the file system's file table.
    /// </summary>
    [XmlElement]
    public ApplicationFile DatabaseFileSystem { get; set; }

    /// <summary>
    /// Gets or sets the path of the file containing the SQLite database holding the registry keys and values.
    /// </summary>
    [XmlElement]
    public ApplicationFile DatabaseRegistry { get; set; }

    /// <summary>
    /// Gets or sets the path of the executable to start the process with.
    /// </summary>
    [XmlElement]
    public ApplicationFile Executable { get; set; }

    #endregion

  }
}
