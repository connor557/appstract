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

using AppStract.Core.Virtualization.Engine.FileSystem;

namespace AppStract.Engine.Engine.FileSystem
{
  /// <summary>
  /// Wraps a <see cref="FileRequest"/> and the data resulting from handling that <see cref="FileRequest"/>.
  /// </summary>
  public struct FileRequestResult
  {
    /// <summary>
    /// Gets or sets the original <see cref="FileRequest"/>.
    /// </summary>
    public FileRequest Request { get; set;}
    /// <summary>
    /// Gets or sets the absolute path to which <see cref="Request"/> should be redirected.
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="VirtualFolder"/> under which <see cref="Path"/> resides.
    /// </summary>
    public VirtualFolder SystemFolder { get; set; }

  }
}
