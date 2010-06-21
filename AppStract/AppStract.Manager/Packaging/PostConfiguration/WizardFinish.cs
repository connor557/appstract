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

using System.Windows.Forms;
using AppStract.Manager.Wizard;

namespace AppStract.Manager.Packaging.PostConfiguration
{
  /// <summary>
  /// The last step of the wizard.
  /// </summary>
  public partial class WizardFinish : UserControl, IWizardItem<PostConfigurationState>
  {

    #region Variables

    private readonly PostConfigurationState _state;

    #endregion

    #region Constructors

    public WizardFinish(PostConfigurationState state)
    {
      InitializeComponent();
      _state = state;
    }

    #endregion

    #region IWizardItem Members

    public event WizardStateChangedEventHandler<PostConfigurationState> StateChanged;

    public bool AcceptableContent
    {
      get { return true; }
    }

    public PostConfigurationState State
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
