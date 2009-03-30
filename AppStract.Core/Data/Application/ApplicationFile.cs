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
using AppStract.Utilities.Extensions;

namespace AppStract.Core.Data.Application
{
  [Serializable]
  public class ApplicationFile
  {

    #region Variables

    private FileType _type;
    private string _file;

    #endregion

    #region Properties

    public FileType Type
    {
      get { return _type; }
    }

    public string File
    {
      get { return _file; }
      set
      {
        _type = GetFileType(value);
        _file = value;
      }
    }

    #endregion

    #region Constructors

    public ApplicationFile() { }

    public ApplicationFile(string file)
      : this()
    {
      File = file;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether the curretn <see cref="ApplicationFile"/> exists on the local media.
    /// </summary>
    /// <returns></returns>
    public bool Exists()
    {
      return System.IO.File.Exists(File);
    }

    /// <summary>
    /// Returns a string representation of the current <see cref="ApplicationFile"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "[" + Type + "] " + File; 
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the <see cref="FileType"/> for the filename specified.
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    /// <param name="filename"></param>
    /// <returns></returns>
    private static FileType GetFileType(string filename)
    {
      if (Directory.Exists(filename))
        return FileType.Directory;
      if (!System.IO.File.Exists(filename))
        throw new FileNotFoundException();
      if (filename.EndsWith(".db3"))
        return FileType.Directory;
      if (filename.EndsWithAny(new[] {".exe", ".dll"}))
      {
        return AssemblyHelper.GetAssemblyType(filename) == AssemblyType.Native
                 ? FileType.Assembly_Native
                 : FileType.Assembly_Managed;
      }
      return FileType.File;
    }

    #endregion

  }
}
