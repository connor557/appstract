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
using AppStract.Utilities.Helpers;
using AppStract.Utilities.ManagedFusion.Insuring;

namespace AppStract.Watcher
{
  /// <summary>
  /// Holds the parameters for the current watcher.
  /// The parameters are parsed from the arguments provided to <see cref="Program.Main"/>.
  /// </summary>
  internal struct Parameters
  {

    #region Variables

    /// <summary>
    /// The string representing the folder in which the insurances are kept.
    /// It is expected that <see cref="InsuranceId"/> is a file contained in this folder.
    /// </summary>
    public readonly string InsuranceFile;
    /// <summary>
    /// The string representing the registry key in which the insurances are kept.
    /// It is expected that the key is a subkey of HKEY_CURRENT_USER and that <see cref="InsuranceId"/> is a subkey of this key.
    /// </summary>
    public readonly string InsuranceRegistryKey;
    /// <summary>
    /// The flags describing the insurance methods.
    /// </summary>
    public readonly CleanUpInsuranceFlags Flags;
    /// <summary>
    /// The identifier of the insurance.
    /// </summary>
    public readonly Guid InsuranceId;
    /// <summary>
    /// The identifier of the process to watch.
    /// </summary>
    public readonly int ProcessId;

    #endregion

    #region Constructors

    public Parameters(IEnumerable<string> parameters)
      : this()
    {
      if (parameters == null) return;
      foreach (var param in parameters)
      {
        if (param.StartsWith("IID="))
          InsuranceId = new Guid(param.Substring("IID=".Length));
        else if (param.StartsWith("FILE="))
          InsuranceFile = param.Substring("FILE=".Length);
        else if (param.StartsWith("REG="))
          InsuranceRegistryKey = param.Substring("REG=".Length);
        else if (param.StartsWith("PID="))
          Int32.TryParse(param.Substring("PID=".Length), out ProcessId);
        else if (param.StartsWith("FLAGS="))
        {
          int flags;
          Int32.TryParse(param.Substring("FLAGS=".Length), out flags);
          ParserHelper.TryParseEnum(flags, out Flags);
        }
      }
    }

    #endregion

  }
}
