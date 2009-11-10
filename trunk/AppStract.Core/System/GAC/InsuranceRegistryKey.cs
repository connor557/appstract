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
using System.Reflection;
using System.Reflection.GAC;
using AppStract.Utilities.Helpers;
using Microsoft.Win32;

namespace AppStract.Core.System.GAC
{
  /// <summary>
  /// Represents an insurance as saved to the local host's registry.
  /// </summary>
  internal sealed class InsuranceRegistryKey : InsuranceBase
  {

    #region Variables

    private readonly RegistryKey _registryKey;

    #endregion

    #region Properties

    public RegistryKey RegistryKey
    {
      get { return _registryKey; }
    }

    #endregion

    #region Constructors

    public InsuranceRegistryKey(RegistryKey registryKey, InstallerDescription installerDescription, string machineId, DateTime creationDateTime, IEnumerable<AssemblyName> assemblies)
      : base(registryKey.Name.Substring(registryKey.Name.LastIndexOf('\\') + 1), installerDescription, machineId, creationDateTime, assemblies)
    {
      _registryKey = registryKey;
    }

    ~InsuranceRegistryKey()
    {
      if (_registryKey != null)
        _registryKey.Close();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Writes the specified <see cref="InsuranceRegistryKey"/> to the registry key represented by the specified <see cref="RegistryKey"/>.
    /// </summary>
    /// <param name="insuranceRegistryKey"></param>
    /// <returns></returns>
    public static void Write(InsuranceRegistryKey insuranceRegistryKey)
    {
      if (insuranceRegistryKey.RegistryKey.GetValueNames().Length != 0)
        throw new Exception();
      var regKey = insuranceRegistryKey.RegistryKey;
      regKey.SetValue("machineId", insuranceRegistryKey.MachineId);
      regKey.SetValue("creationDateTime", insuranceRegistryKey.CreationDateTime.ToString(_DateTimeFormat));
      var i = 0;
      foreach (var assembly in insuranceRegistryKey.Assemblies)
        regKey.SetValue("assembly" + ++i, assembly.ToString(), RegistryValueKind.String);
      // Write the InstallerDescription
      regKey = regKey.CreateSubKey("Installer");
      if (regKey == null) throw new Exception();
      regKey.SetValue("type", insuranceRegistryKey.InstallerDescription.Type, RegistryValueKind.String);
      regKey.SetValue("id", insuranceRegistryKey.InstallerDescription.Id, RegistryValueKind.String);
      regKey.SetValue("descr", insuranceRegistryKey.InstallerDescription.Description, RegistryValueKind.String);
    }

    /// <summary>
    /// Tries to build an instance of <see cref="InsuranceRegistryKey"/> from data read from the specified registry key.
    /// </summary>
    /// <param name="registryKey"></param>
    /// <param name="insuranceRegistryKey"></param>
    /// <returns></returns>
    public static bool TryRead(RegistryKey registryKey, out InsuranceRegistryKey insuranceRegistryKey)
    {
      try
      {
        insuranceRegistryKey = Read(registryKey);
        return true;
      }
      catch
      {
        insuranceRegistryKey = null;
        return false;
      }
    }

    /// <summary>
    /// Returns an instance of <see cref="InsuranceRegistryKey"/> built from data read from the specified registry key.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown if any of the values specified in the registrykey is invalid.
    /// =OR=
    /// An <see cref="ArgumentException"/> is thrown if the specified registrykey doesn't contain an "Installer" subkey.
    /// </exception>
    /// <param name="registryKey"></param>
    /// <returns></returns>
    public static InsuranceRegistryKey Read(RegistryKey registryKey)
    {
      var values = new List<string>(registryKey.GetValueNames());
      if (values.Count < 2) throw new Exception();
      var machineId = registryKey.GetValue("machineId").ToString();
      var creationDatetime = registryKey.GetValue("creationDateTime").ToString();
      if (machineId == null)
        throw new ArgumentException("The specified registry key doesn't contain a value for \"machineId\"", "registryKey");
      if (creationDatetime == null)
        throw new ArgumentException("The specified registry key doesn't contain a value for \"creationDateTime\"", "registryKey");
      // Read the InstallerDescription
      InstallerDescription installer;
      using (var installerKey = registryKey.OpenSubKey("Installer"))
      {
        if (installerKey == null)
          throw new ArgumentException("The specified registry key doesn't contain a subkey for \"Installer\"",
                                      "registryKey");
        installer = ReadInstallerDescription(installerKey);
      }
      // Read the insured assemblies
      var assemblies = new List<AssemblyName>(values.Count - 2);
      foreach (var value in values)
        if (value.StartsWith("assembly"))
          assemblies.Add(new AssemblyName(registryKey.GetValue(value).ToString()));
      return new InsuranceRegistryKey(registryKey, installer, machineId, DateTime.Parse(creationDatetime), assemblies);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns an <see cref="InstallerDescription"/> built from data read from the specified registry key.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown if any of the values specified in the registrykey is invalid.
    /// </exception>
    /// <param name="regKey"></param></param>
    /// <returns></returns>
    private static InstallerDescription ReadInstallerDescription(RegistryKey regKey)
    {
      var tmp = regKey.GetValue("type");
      if (tmp == null)
        throw new ArgumentException("The specified registry key doesn't contain a value for \"type\"", "regKey");
      var id = regKey.GetValue("id");
      if (id == null)
        throw new ArgumentException("The specified registry key doesn't contain a value for \"id\"", "regKey");
      var descr = regKey.GetValue("descr");
      if (descr == null)
        throw new ArgumentException("The specified registry key doesn't contain a value for \"descr\"", "regKey");
      InstallerType type;
      if (!ParserHelper.TryParseEnum(tmp.ToString(), out type))
        throw new ArgumentException("The specified registry key contains an invalid value for \"type\"", "regKey");
      switch (type)
      {
        case InstallerType.File:
          return InstallerDescription.CreateForFile(descr.ToString(), id.ToString());
        case InstallerType.Installer:
          return InstallerDescription.CreateForInstaller(descr.ToString(), id.ToString());
        case InstallerType.OpaqueString:
          return InstallerDescription.CreateForOpaqueString(descr.ToString(), id.ToString());
      }
      throw new Exception();
    }

    #endregion

  }
}
