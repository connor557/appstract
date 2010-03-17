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
using AppStract.Utilities.Helpers;
using Microsoft.Win32;

namespace AppStract.Utilities.ManagedFusion.Insuring
{
  /// <summary>
  /// Represents an insurance as saved to the local host's registry.
  /// </summary>
  internal sealed class InsuranceRegistryKey : InsuranceBase
  {

    #region Variables

    private readonly string _registryKeyName;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the subkey of HKEY_CURRENT_USER where the current <see cref="InsuranceRegistryKey"/> stores all its data.
    /// </summary>
    public string RegistryKeyName
    {
      get { return _registryKeyName; }
    }

    #endregion

    #region Constructors

    public InsuranceRegistryKey(string registryKey, Guid guid, InstallerDescription installerDescription, string machineId, DateTime timeStamp, IEnumerable<AssemblyName> assemblies)
      : base(guid, installerDescription, machineId, timeStamp, assemblies)
    {
      _registryKeyName = registryKey;
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
      using (var regKey = Registry.CurrentUser.OpenSubKey(insuranceRegistryKey.RegistryKeyName, true))
      {
        if (regKey == null || regKey.GetValueNames().Length != 0)
          throw new ArgumentException("The specified InsuranceRegistryKey points to an invalid registry key.", "insuranceRegistryKey");
        regKey.SetValue("guid", insuranceRegistryKey.InsuranceIdentifier.ToString(), RegistryValueKind.String);
        regKey.SetValue("machineId", insuranceRegistryKey.MachineId);
        regKey.SetValue("creationDateTime", insuranceRegistryKey.TimeStamp.ToString(_DateTimeFormat));
        var i = 0;
        foreach (var assembly in insuranceRegistryKey.Assemblies)
          regKey.SetValue("assembly" + ++i, assembly.ToString(), RegistryValueKind.String);
        // Write the InstallerDescription
        var regKeyInstaller = regKey.CreateSubKey("Installer");
        if (regKeyInstaller == null) throw new Exception();
        regKeyInstaller.SetValue("type", insuranceRegistryKey.InstallerDescription.Type, RegistryValueKind.String);
        regKeyInstaller.SetValue("id", insuranceRegistryKey.InstallerDescription.Id, RegistryValueKind.String);
        regKeyInstaller.SetValue("descr", insuranceRegistryKey.InstallerDescription.Description, RegistryValueKind.String);
      }
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
      var guidValue = registryKey.GetValue("guid");
      var machineId = registryKey.GetValue("machineId");
      var creationDatetimeValue = registryKey.GetValue("creationDateTime");
      if (guidValue == null)
        throw new ArgumentException("The specified registry key doesn't contain a value for \"guid\"", "registryKey");
      if (machineId == null)
        throw new ArgumentException("The specified registry key doesn't contain a value for \"machineId\"", "registryKey");
      if (creationDatetimeValue == null)
        throw new ArgumentException("The specified registry key doesn't contain a value for \"creationDateTime\"", "registryKey");
      // Construct the GUID
      Guid guid;
      try
      {
        guid = new Guid(guidValue.ToString());
      }
      catch (Exception e)
      {
        throw new ArgumentException("The specified registry key contains a corrupt value for \"guid\"", "registryKey", e);
      }
      // Construct the DateTime
      DateTime creationDateTime;
      if (!DateTime.TryParse(creationDatetimeValue.ToString(), out creationDateTime))
        throw new ArgumentException("The specified registry key contains a corrupt value for \"creationDateTime\"", "registryKey");
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
      return new InsuranceRegistryKey(registryKey.Name.Substring(Registry.CurrentUser.Name.Length), guid, installer,
                                      machineId.ToString(), creationDateTime, assemblies);
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
