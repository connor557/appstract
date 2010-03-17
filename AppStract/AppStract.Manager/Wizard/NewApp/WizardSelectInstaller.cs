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

using System.IO;
using System.Windows.Forms;

namespace AppStract.Manager.Wizard.NewApp
{
  /// <summary>
  /// The page of the wizard where the user can select the to-be-used paths.
  /// </summary>
  partial class WizardSelectInstaller : UserControl, IWizardItem<NewApplicationState>
  {

    #region Variables

    /// <summary>
    /// Current state of the wizard.
    /// </summary>
    private readonly NewApplicationState _wizardState;

    #endregion

    #region Constructors

    public WizardSelectInstaller(NewApplicationState wizardState)
    {
      InitializeComponent();
      _wizardState = wizardState;
      _textBoxOutputFolder.Text = wizardState.InstallerOutputDestination;
      _textBoxInstallerLocation.Text = _wizardState.InstallerExecutable;
    }

    #endregion

    #region Private Members

    /// <summary>
    /// Calls the StateChangedEvent if any listeners are available.
    /// </summary>
    private void CallStateChangedEvent()
    {
      if (StateChanged != null)
        StateChanged(AcceptableContent, _wizardState);
    }

    #endregion

    #region Private EventHandlers

    private void TextBox_TextChanged(object sender, System.EventArgs e)
    {
      SaveState();
      CallStateChangedEvent();
    }

    private void _buttonLocationBrowse_Click(object sender, System.EventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.RestoreDirectory = true;
      openFileDialog.Filter = "Executables (*.exe)|*.exe|Windows installer files (*.msi)|*.msi";
      if (!string.IsNullOrEmpty(_textBoxInstallerLocation.Text)
          && File.Exists(_textBoxInstallerLocation.Text))
        openFileDialog.FileName = _textBoxInstallerLocation.Text;
      if (openFileDialog.ShowDialog() == DialogResult.OK)
        _textBoxInstallerLocation.Text = openFileDialog.FileName;
    }

    private void _buttonBrowseOutput_Click(object sender, System.EventArgs e)
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      if (!string.IsNullOrEmpty(_textBoxOutputFolder.Text)
          && Directory.Exists(_textBoxOutputFolder.Text))
        folderBrowserDialog.SelectedPath = _textBoxOutputFolder.Text;
      if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        _textBoxOutputFolder.Text = folderBrowserDialog.SelectedPath;
    }

    #endregion

    #region IWizardItem Members

    public event WizardStateChangedEventHandler<NewApplicationState> StateChanged;

    public bool AcceptableContent
    {
      get { return _textBoxOutputFolder.Text != "" && _textBoxInstallerLocation.Text != ""; }
    }

    public NewApplicationState State
    {
      get { return _wizardState; }
    }

    public void SaveState()
    {
      _wizardState.InstallerExecutable = _textBoxInstallerLocation.Text;
      _wizardState.InstallerOutputDestination = _textBoxOutputFolder.Text;
    }

    public void UpdateContent()
    {
      if (!string.IsNullOrEmpty(_wizardState.InstallerExecutable))
        _textBoxInstallerLocation.Text = _wizardState.InstallerExecutable;
      if (!string.IsNullOrEmpty(_wizardState.InstallerOutputDestination))
        _textBoxOutputFolder.Text = _wizardState.InstallerOutputDestination;
    }

    #endregion

  }
}
