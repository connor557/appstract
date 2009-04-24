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

using System.IO;

namespace AppStract.Core.Virtualization.FileSystem
{
  /// <summary>
  /// Request of the guest process for a file, directory, or library.
  /// </summary>
  public struct FileRequest
  {
    
    #region Variables

    private readonly string _filename;
    private readonly ResourceKind _resourceKind;
    private readonly FileCreationDisposition _creationDisposition;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the path of the requested file.
    /// </summary>
    public string FileName
    {
      get { return _filename; }
    }

    /// <summary>
    /// Gets the <see cref="ResourceKind"/> of the requested resource,
    /// which is usually set to FileOrDirectory or Library.
    /// </summary>
    public ResourceKind ResourceKind
    {
      get { return _resourceKind; }
    }

    /// <summary>
    /// Gets the <see cref="FileCreationDisposition"/> for the requested resource,
    /// this is the action to be taken if the resource doesn't exist yet.
    /// </summary>
    public FileCreationDisposition CreationDisposition
    {
      get { return _creationDisposition; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="FileRequest"/>.
    /// </summary>
    /// <param name="filename">The requested file, directory, or library.</param>
    /// <param name="resourceType">The type of the requested resource.</param>
    /// <param name="creationDisposition">The creation disposition, as specified by the guest process.</param>
    public FileRequest(string filename, ResourceKind resourceType, FileCreationDisposition creationDisposition)
    {
      if (resourceType != ResourceKind.Library  /// Libraries don't need a full path.
        && !Path.IsPathRooted(filename))        /// Avoid getting full paths for pipes.
        filename = Path.GetFullPath(filename);
      _filename = filename.ToLowerInvariant();
      _resourceKind = resourceType;
      _creationDisposition = creationDisposition;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a string representation of the current <see cref="FileRequest"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return string.Format("{0} [{1}||{2}]", _filename, _resourceKind, _creationDisposition);
    }

    #endregion

  }
}
