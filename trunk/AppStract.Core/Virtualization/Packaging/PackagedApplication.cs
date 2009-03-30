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

using System.Collections.Generic;

namespace AppStract.Core.Virtualization.Packaging
{
  public class PackagedApplication
  {

    #region Variables

    private readonly List<string> _executables;
    private readonly string _outputLocation;
    private readonly string _relDbFileSystem;
    private readonly string _relDbRegistry;

    #endregion

    #region Properties

    public IList<string> Executables
    {
      get { return _executables; }
    }

    public string OutputLocation
    {
      get { return _outputLocation; }
    }

    public string FileSystemDatabase
    {
      get { return _relDbFileSystem; }
    }

    public string RegistryDatabase
    {
      get { return _relDbRegistry; }
    }

    #endregion

    #region Constructors

    public PackagedApplication(string outputLocation, IEnumerable<string> executables, string dbFileSystem, string dbRegistry)
    {
      _outputLocation = outputLocation;
      _executables = new List<string>(executables);
      _relDbFileSystem = dbFileSystem;
      _relDbRegistry = dbRegistry;
    }

    #endregion

  }
}
