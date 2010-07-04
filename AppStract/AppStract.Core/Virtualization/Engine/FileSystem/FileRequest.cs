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

namespace AppStract.Core.Virtualization.Engine.FileSystem
{
  /// <summary>
  /// Wraps all data related to a request to the file system virtualization engine.
  /// </summary>
  public struct FileRequest
  {
    /// <summary>
    /// Gets or sets the requested path.
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// Gets or sets the type of resource expected to be pointed to by <see cref="Path"/>.
    /// </summary>
    public ResourceType ResourceType { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="FileCreationDisposition"/> to be applied on <see cref="Path"/>.
    /// </summary>
    public FileCreationDisposition CreationDisposition { get; set; }

  }
}