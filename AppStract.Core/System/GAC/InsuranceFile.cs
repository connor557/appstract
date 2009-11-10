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
using System.Reflection.GAC;
using AppStract.Utilities.Helpers;

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
    /// <param name="installerDescription"></param>
    /// <param name="machineId"></param>
    /// <param name="creationDate"></param>
    /// <param name="assemblies"></param>
    public InsuranceFile(string fileName, InstallerDescription installerDescription, string machineId, DateTime creationDate, IEnumerable<AssemblyName> assemblies)
      : base(Path.GetFileNameWithoutExtension(fileName), installerDescription, machineId, creationDate, assemblies)
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
          writer.WriteLine("MachineId={0}" + Environment.NewLine + "CreationDateTime={1}" + Environment.NewLine,
                           insuranceFile.MachineId, insuranceFile.CreationDateTime.ToString(_DateTimeFormat));
          writer.WriteLine("Installer=[Type={0}, Id={1}, Description={2}]" + Environment.NewLine,
                           insuranceFile.InstallerDescription.Type, insuranceFile.InstallerDescription.Id,
                           insuranceFile.InstallerDescription.Description);
          foreach (var item in insuranceFile.Assemblies)
            writer.WriteLine(item);
          writer.Flush();
        }
      }
    }

    /// <summary>
    /// Tries to build an instance of <see cref="InsuranceFile"/> from data read from the specified file.
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    /// <param name="fileName"></param>
    /// <param name="insuranceFile"></param>
    /// <returns></returns>
    public static bool TryRead(string fileName, out InsuranceFile insuranceFile)
    {
      insuranceFile = null;
      using (var str = File.Open(fileName, FileMode.Open, FileAccess.Read))
      {
        using (var reader = new StreamReader(str))
        {
          string machineId, creationDateTime;
          // Get MachineId
          var line = reader.ReadLine();
          if (!ReadValue(line, "MachineId", out machineId))
            return false;
          // Get CreationDate
          line = reader.ReadLine();
          if (!ReadValue(line, "CreationDateTime", out creationDateTime))
            return false;
          // Skip empty line
          reader.ReadLine();
          // Read the InstallerDescription from the file
          var installer = ReadInstallerDescriptionFromLine(reader.ReadLine());
          if (installer == null)
            return false;
          // Skip empty line
          reader.ReadLine();
          // Read the list of insured assemblies
          var assemblies = new List<AssemblyName>();
          while (!reader.EndOfStream)
          {
            line = reader.ReadLine();
            if (line == "") break;
            try
            {
              assemblies.Add(new AssemblyName(line));
            }
            catch (ArgumentException)
            {
              return false;
            }
          }
          insuranceFile = new InsuranceFile(fileName, installer, machineId, DateTime.Parse(creationDateTime), assemblies);
          return true;
        }
      }
    }

    /// <summary>
    /// Returns an instance of <see cref="InsuranceFile"/> built from data read from the specified file.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static InsuranceFile Read(string fileName)
    {
      InsuranceFile result;
      if (!TryRead(fileName, out result))
        throw new ArgumentException("The specified file contains incorrect data.", "fileName");
      return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Reads a <paramref name="value"/> for the specified <paramref name="key"/> from the given <paramref name="line"/>.
    /// </summary>
    /// <param name="line">The line to find the key in and read the value from.</param>
    /// <param name="key">The key to read the value for.</param>
    /// <param name="value">The resulting value.</param>
    /// <returns>Whether the requested value is read.</returns>
    private static bool ReadValue(string line, string key, out string value)
    {
      return ReadValue(line, key, null, out value);
    }

    /// <summary>
    /// Reads a <paramref name="value"/>, delimited by the <paramref name="delimiter"/>, for the specified <paramref name="key"/> from the given <paramref name="line"/>.
    /// </summary>
    /// <param name="line">The line to find the key in and read the value from.</param>
    /// <param name="key">The key to read the value for.</param>
    /// <param name="delimiter">String thats trailing the value.</param>
    /// <param name="value">The resulting value.</param>
    /// <returns>Whether the requested value is read.</returns>
    private static bool ReadValue(string line, string key, string delimiter, out string value)
    {
      value = null;
      if (!line.Contains(key + "="))
        return false;
      var i = line.IndexOf(key + "=") + key.Length + 1; // +1 because of trailing '='
      if (line.Length < i) return false;
      value = line.Substring(i);
      if (delimiter == null)
        return true;
      i = value.IndexOf(delimiter);
      if (i == -1)
        return false;
      value = line.Substring(0, i);
      return true;
    }

    /// <summary>
    /// Returns an instance of <see cref="InstallerDescription"/> built from data read from the given <paramref name="line"/>.
    /// If no instance can be build, null is returned.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    private static InstallerDescription ReadInstallerDescriptionFromLine(string line)
    {
      string irTypeString, irId, irDescr;
      if (!ReadValue(line, "Installer", out line)
          || !ReadValue(line, "Type", ", ", out irTypeString)
          || !ReadValue(line, "Id", ", ", out irId)
          || !ReadValue(line, "Description", "]", out irDescr))
        return null;
      InstallerType irType;
      if (!ParserHelper.TryParseEnum(irTypeString, out irType))
        return null;
      if (irType == InstallerType.File)
        return InstallerDescription.CreateForFile(irDescr, irId);
      if (irType == InstallerType.OpaqueString)
        return InstallerDescription.CreateForOpaqueString(irDescr, irId);
      if (irType == InstallerType.Installer)
        return InstallerDescription.CreateForInstaller(irDescr, irId);
      return null;
    }

    #endregion

  }
}
