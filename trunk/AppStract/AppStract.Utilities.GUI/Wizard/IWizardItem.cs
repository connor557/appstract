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

namespace AppStract.Utilities.GUI.Wizard
{

  /// <summary>
  /// Interface to an item of a wizard.
  /// </summary>
  public interface IWizardItem<out T>
  {

    /// <summary>
    /// Gets called when the state of a step in the wizard changes.
    /// </summary>
    event WizardStateChangedEventHandler<T> StateChanged;

    /// <summary>
    /// Gets whether the content of the current item is accepted,
    /// this indicates whether the wizard is allowed to continue.
    /// </summary>
    bool AcceptableContent
    {
      get;
    }

    /// <summary>
    /// Gets the current state.
    /// </summary>
    T State
    {
      get;
    }

    /// <summary>
    /// Saves the state..
    /// </summary>
    void SaveState();

    /// <summary>
    /// Updates the UI from the curren state.
    /// </summary>
    void UpdateContent();

  }
}
