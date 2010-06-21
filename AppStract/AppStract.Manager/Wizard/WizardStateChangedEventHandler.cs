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

namespace AppStract.Manager.Wizard
{

  /// <summary>
  /// EventHandler for a change in a wizard's state.
  /// </summary>
  /// <param name="wizardMayContinue">True if the wizard is allowed to continue to the next page, otherwise false.</param>
  /// <param name="currentState">Current state of the wizard.</param>
  public delegate void WizardStateChangedEventHandler<TState>(bool wizardMayContinue, TState currentState);

}
