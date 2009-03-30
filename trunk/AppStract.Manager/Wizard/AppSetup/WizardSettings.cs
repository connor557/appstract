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
  /// Lets the user change some settings.
  /// </summary>
  public partial class WizardSettings : UserControl, IWizardItem<ApplicationSetupState>
  {

    #region Variables

    private ApplicationSetupState _state;

    #endregion

    #region Constructors

    public WizardSettings(ApplicationSetupState state)
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
      _state.ForceVirtualFileSystem = _checkBoxForceVirtualFileSystem.Checked;
    }

    public void UpdateContent()
    {
      _checkBoxForceVirtualFileSystem.Checked = _state.ForceVirtualFileSystem;
    }

    #endregion

  }
}
