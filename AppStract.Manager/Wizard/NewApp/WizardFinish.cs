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

using System.Windows.Forms;

namespace AppStract.Manager.Wizard.NewApp
{
  /// <summary>
  /// The last step of the wizard.
  /// </summary>
  partial class WizardFinish : UserControl, IWizardItem<NewApplicationState>
  {

    #region Variables

    /// <summary>
    /// Current state of the wizard.
    /// </summary>
    private readonly NewApplicationState _wizardState;

    #endregion

    #region Constructors

    public WizardFinish(NewApplicationState wizardState)
    {
      InitializeComponent();
      _wizardState = wizardState;
    }

    #endregion

    #region IWizardItem<NewApplicationState> Members

    public event WizardStateChangedEventHandler<NewApplicationState> StateChanged;

    public bool AcceptableContent
    {
      get { return true; }
    }

    public NewApplicationState State
    {
      get { return _wizardState; }
    }

    public void SaveState()
    {
      _wizardState.Autostart = _chkAutostart.Checked;
    }

    public void UpdateContent()
    {
      _chkAutostart.Checked = _wizardState.Autostart;
    }

    #endregion

  }
}
