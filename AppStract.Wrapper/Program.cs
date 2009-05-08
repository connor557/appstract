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

using System.Threading;
using AppStract.Server;

namespace AppStract.Wrapper
{
  class Program
  {
    /// <summary>
    /// Entry point for the wrapper process.
    /// Hangs until the connection with the server process is lost.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
      while (true)
      {
        Thread.Sleep(500);
        if (GuestCore.Initialized && !GuestCore.ValidConnection)
          return;
      }
    }
  }
}
