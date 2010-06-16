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
using AppStract.Manager.Wizard;

namespace AppStract.Manager.Packaging.PreConfiguration
{
  /// <summary>
  /// The last step of the wizard.
  /// </summary>
  partial class WizardFinish : UserControl, IWizardItem<PreConfigurationState>
  {

    #region Variables

    /// <summary>
    /// Current state of the wizard.
    /// </summary>
    private readonly PreConfigurationState _wizardState;

    #endregion

    #region Constructors

    public WizardFinish(PreConfigurationState wizardState)
    {
      InitializeComponent();
      _wizardState = wizardState;
    }

    #endregion

    #region IWizardItem<NewApplicationState> Members

    public event WizardStateChangedEventHandler<PreConfigurationState> StateChanged;

    public bool AcceptableContent
    {
      get { return true; }
    }

    public PreConfigurationState State
    {
      get { return _wizardState; }
    }

    public void SaveState()
    {

    }

    public void UpdateContent()
    {

    }

    #endregion

  }
}
