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

using System.Collections.Generic;
using System.Windows.Forms;
using AppStract.Manager.Wizard;

namespace AppStract.Manager.Packaging.PostConfiguration
{
  partial class WizardSelectExecutable : UserControl, IWizardItem<PostConfigurationState>
  {

    #region Variables

    private readonly PostConfigurationState _state;

    #endregion

    #region Properties

    public string SelectedValue
    {
      get
      {
        return _listBoxItems.SelectedIndex == -1
                 ? null
                 : _listBoxItems.Items[_listBoxItems.SelectedIndex].ToString();
      }
    }

    #endregion

    #region Constructor

    public WizardSelectExecutable(IEnumerable<string> items, PostConfigurationState state)
    {
      InitializeComponent();
      foreach (string item in items)
        _listBoxItems.Items.Add(item);
      _state = state;
    }

    #endregion

    #region Private EventHandlers

    private void _listBoxItems_SelectedIndexChanged(object sender, System.EventArgs e)
    {
      SaveState();
      if (StateChanged != null)
        StateChanged(AcceptableContent, _state);
    }

    #endregion

    #region IWizardItem Members

    public event WizardStateChangedEventHandler<PostConfigurationState> StateChanged;

    public bool AcceptableContent
    {
      get { return _listBoxItems.SelectedIndex != -1; }
    }

    public PostConfigurationState State
    {
      get { return _state; }
    }

    public void SaveState()
    {
      _state.Executable = _listBoxItems.SelectedItem != null
                            ? _listBoxItems.SelectedItem.ToString()
                            : null;
    }

    public void UpdateContent()
    {
      for (int i = 0; i < _listBoxItems.Items.Count; i++)
      {
        if (_listBoxItems.Items[i].ToString() != _state.Executable)
          continue;
        _listBoxItems.SelectedIndex = i;
        return;
      }
    }

    #endregion

  }
}
