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

namespace AppStract.Manager.Wizard.AppSetup
{
  /// <summary>
  /// The last step of the wizard.
  /// </summary>
  public partial class WizardFinish : UserControl, IWizardItem<ApplicationSetupState>
  {

    #region Variables

    private readonly ApplicationSetupState _state;

    #endregion

    #region Constructors

    public WizardFinish(ApplicationSetupState state)
    {
      InitializeComponent();
      _state = state;
    }

    #endregion

    #region IWizardItem Members

    public event WizardStateChangedEventHandler<ApplicationSetupState> StateChanged;

    public bool AcceptableContent
    {
      get { return true; }
    }

    public ApplicationSetupState State
    {
      get { return _state; }
    }

    public void SaveState()
    {
      _state.StartOnEnd = _checkBoxStartApp.Checked;
    }

    public void UpdateContent()
    {
      _checkBoxStartApp.Checked = _state.StartOnEnd;
    }

    #endregion

  }
}
