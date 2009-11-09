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
using Microsoft.Win32;

namespace AppStract.Core.System.GAC
{
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

    public InsuranceRegistryKey(RegistryKey registryKey, string machineId, DateTime creationDateTime, IEnumerable<AssemblyName> assemblies)
      : base(registryKey.Name.Substring(registryKey.Name.LastIndexOf('\\') + 1), machineId, creationDateTime, assemblies)
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
    /// ToDo: Make this method more safe and use meaningful exceptions.
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
    }

    /// <summary>
    /// ToDo: Make this method more safe and use meaningful exceptions.
    /// </summary>
    /// <param name="registryKey"></param>
    /// <returns></returns>
    public static InsuranceRegistryKey Read(RegistryKey registryKey)
    {
      var values = new List<string>(registryKey.GetValueNames());
      if (values.Count < 2) throw new Exception();
      if (!values.Contains("machineId"))
        throw new Exception();
      if (!values.Contains("creationDateTime"))
        throw new Exception();
      string machineId = null;
      string creationDatetime = null;
      var assemblies = new List<AssemblyName>(values.Count - 2);
      foreach (var value in values)
      {
        if (value.StartsWith("assembly"))
          assemblies.Add(new AssemblyName(registryKey.GetValue(value).ToString()));
        else if (value == "machineId")
          machineId = registryKey.GetValue(value).ToString();
        else if (value == "creationDateTime")
          creationDatetime = registryKey.GetValue(value).ToString();
      }
      if (machineId == null)
        throw new Exception();
      if (creationDatetime == null)
        throw new Exception();
      return new InsuranceRegistryKey(registryKey, machineId, DateTime.Parse(creationDatetime), assemblies);
    }

    #endregion

  }
}
