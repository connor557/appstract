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

namespace AppStract.Core.Virtualization.Process
{

  /// <summary>
  /// Represents a method that will handle the event raisen by an exited <see cref="VirtualizedProcess"/>.
  /// </summary>
  /// <param name="sender">The terminated <see cref="VirtualizedProcess"/>.</param>
  /// <param name="exitCode">The <see cref="ExitCode"/>, specified when the sender terminated.</param>
  public delegate void ProcessExitEventHandler(VirtualizedProcess sender, ExitCode exitCode);
  
}
