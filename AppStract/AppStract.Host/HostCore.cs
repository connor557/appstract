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

using AppStract.Host.Data.Settings;
using AppStract.Host.System;
using AppStract.Utilities.Logging;

namespace AppStract.Host
{
  /// <summary>
  /// The static core class of AppStract,
  /// used as a communication bus to the different core components.
  /// </summary>
  public static class HostCore
  {

    #region Properties

    /// <summary>
    /// The current instance's configuration.
    /// </summary>
    public static Configuration Configuration
    {
      get; set;
    }

    /// <summary>
    /// The current instance's runtime information.
    /// </summary>
    public static Runtime Runtime
    {
      get; set;
    }

    /// <summary>
    /// The current instance's log service.
    /// </summary>
    public static Logger Log
    {
      get; set;
    }

    #endregion

  }
}
 