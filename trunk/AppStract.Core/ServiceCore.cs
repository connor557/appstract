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
using System.Collections.Generic;
using AppStract.Core.Logging;

namespace AppStract.Core
{
  public static class ServiceCore
  {

    #region Variables

    private static Logger _logger;
    private static readonly IDictionary<Type, object> _services;

    #endregion

    #region Properties

    public static Logger Log
    {
      get { return _logger; }
      set { _logger = value; }
    }

    #endregion

    #region Constructors

    static ServiceCore()
    {

      _services = new Dictionary<Type, object>();
    }

    #endregion

    #region Methods

    public static T Get<T>() where T : class 
    {
      Type serviceType = typeof (T);
      object service;
      if (_services.TryGetValue(serviceType, out service))
        return (T) service;
      throw new ServiceNotFoundException(string.Format("ServiceCore doesn't contain an instance of a service of type {0}.", serviceType));
    }

    public static bool TryRegister<T>(T service) where T : class
    {
      return TryRegister(service, false);
    }

    public static bool TryRegister<T>(T service, bool overwrite) where T : class
    {
      Type serviceType = typeof(T);
      if (_services.ContainsKey(serviceType))
      {
        if (!overwrite)
          return false;
        _services[serviceType] = service;
        return true;
      }
      _services.Add(serviceType, service);
      return true;
    }

    public static bool IsRegistered<T>() where T : class
    {
      return _services.ContainsKey(typeof (T));
    }

    #endregion

  }
}
 