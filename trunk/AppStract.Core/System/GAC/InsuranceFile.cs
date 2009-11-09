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
using System.IO;
using System.Reflection;

namespace AppStract.Core.System.GAC
{
  /// <summary>
  /// Represents an insurance as saved in a file.
  /// </summary>
  internal sealed class InsuranceFile : InsuranceBase
  {

    #region Variables

    private readonly string _fileName;

    #endregion

    #region Properties

    public string FileName
    {
      get { return _fileName; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="InsuranceFile"/>, which represents an insurance with the specified data.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="machineId"></param>
    /// <param name="creationDate"></param>
    /// <param name="assemblies"></param>
    public InsuranceFile(string fileName, string machineId, DateTime creationDate, IEnumerable<AssemblyName> assemblies)
      : base(Path.GetFileNameWithoutExtension(fileName), machineId, creationDate, assemblies)
    {
      if (!Path.IsPathRooted(fileName))
        throw new ArgumentException("The filename specified must be a rooted path.", "fileName");
      if (!Directory.Exists(Path.GetDirectoryName(fileName)))
        throw new DirectoryNotFoundException("\"" + Path.GetDirectoryName(fileName) + "\" can not be found.");
      _fileName = fileName;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Writes the specified <see cref="InsuranceFile"/> to the file with the specified <see cref="FileName"/>.
    /// </summary>
    /// <param name="insuranceFile"></param>
    public static void Write(InsuranceFile insuranceFile)
    {
      using (var str = File.Open(insuranceFile.FileName, FileMode.Create, FileAccess.Write))
      {
        using (var writer = new StreamWriter(str))
        {
          writer.WriteLine("MachineId=" + insuranceFile.MachineId + Environment.NewLine
                           + "CreationDate=" + insuranceFile.CreationDateTime.ToString(_DateTimeFormat) + Environment.NewLine);
          foreach (var item in insuranceFile.Assemblies)
            writer.WriteLine(item);
          writer.Flush();
        }
      }
    }

    /// <summary>
    /// Returns an instance of <see cref="InsuranceFile"/> with data read from the specified file.
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="InvalidCastException"></exception>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static InsuranceFile Read(string fileName)
    {
      using (var str = File.Open(fileName, FileMode.Open, FileAccess.Read))
      {
        using (var reader = new StreamReader(str))
        {
          var machineId = reader.ReadLine().Substring("MachineId=".Length);
          var creationDateTime = reader.ReadLine().Substring("CreationDate=".Length);
          reader.ReadLine();  // Skip empty line
          var assemblies = new List<AssemblyName>();
          while (!reader.EndOfStream)
          {
            var line = reader.ReadLine();
            if (line == "") break;
            try
            {
              assemblies.Add(new AssemblyName(line));
            }
            catch (ArgumentException e)
            {
              throw new InvalidCastException("Cannot convert \"" + line + "\" to an assembly name.", e);
            }
          }
          return new InsuranceFile(fileName, machineId, DateTime.Parse(creationDateTime), assemblies);
        }
      }
    }

    #endregion

  }
}
