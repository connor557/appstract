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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AppStract.Manager.Wizard;

namespace AppStract.Manager.Packaging.PostConfiguration
{
  partial class WizardSelectExecutable : UserControl, IWizardItem<PostConfigurationState>
  {

    #region Variables

    private readonly string _rootDirectory;
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

    public WizardSelectExecutable(string rootDirectory, IEnumerable<string> suggestedExecutables, PostConfigurationState state)
    {
      InitializeComponent();
      _rootDirectory = rootDirectory;
      _state = state;
      foreach (string item in suggestedExecutables)
        _listBoxItems.Items.Add(item);
    }

    #endregion

    #region Private EventHandlers

    private void _lnkBrowseExecutable_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      do
      {
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.InitialDirectory = _rootDirectory;
        if (fileDialog.ShowDialog() != DialogResult.OK)
          return;
        if (fileDialog.FileName.StartsWith(_rootDirectory, StringComparison.InvariantCultureIgnoreCase))
        {
          _listBoxItems.Items.Add(fileDialog.FileName.Substring(_rootDirectory.Length));
          _listBoxItems.SelectedIndex = _listBoxItems.Items.Count - 1;
          return;
        }
        // File is outside of the virtual file system, ask user to retry.
        if (MessageBox.Show("The selected executable is invalid. The executable must be a file located under the \""
                            + _rootDirectory + "\" folder.", "Invalid executable", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error)
            == DialogResult.Retry)
          continue;
        return;
      } while (true); 
    }

    private void _listBoxItems_SelectedIndexChanged(object sender, EventArgs e)
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
