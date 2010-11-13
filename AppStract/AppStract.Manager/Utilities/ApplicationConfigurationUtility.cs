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
using AppStract.Host.Data.Application;
using AppStract.Manager.Utilities.ApplicationConfiguration;

namespace AppStract.Manager.Utilities
{
  public partial class ApplicationConfigurationUtility : Form
  {

    #region Constants

    private const string _WindowTitle = "Application Configuration";
    private const string _BlockFileInteractionWindowTitle = "Application Configuration [Locked Mode]";

    #endregion

    #region Variables

    private bool _blockFileInteraction;
    private string _dataFile;
    private ApplicationData _data;

    #endregion

    #region Properties

    public bool BlockFileInteraction
    {
      get { return _blockFileInteraction; }
      set
      {
        _blockFileInteraction = value;
        UpdateGlobalGUI();
      }
    }

    #endregion

    #region Constructors

    public ApplicationConfigurationUtility()
      : this(false)
    {
    }

    public ApplicationConfigurationUtility(bool blockFileInteraction)
    {
      InitializeComponent();
      _blockFileInteraction = blockFileInteraction;
      _tabPageApplicationFiles.Controls.Add(new ApplicationFilesPage { Dock = DockStyle.Fill });
      _tabPageEngineSettings.Controls.Add(new EngineSettingsPage { Dock = DockStyle.Fill });
      UpdateGlobalGUI();
    }

    #endregion

    #region Public Methods

    public void LoadApplicationData(ApplicationData data)
    {
      _data = data;
      UpdateGlobalGUI();
      foreach (Control tabPage in _tabMain.TabPages)
        foreach (IApplicationConfigurationPage page in tabPage.Controls)
          page.BindDataSource(data);
    }

    #endregion

    #region Private Methods

    private void UpdateGlobalGUI()
    {
      Text = _blockFileInteraction ? _BlockFileInteractionWindowTitle : _WindowTitle;
      _saveToolStripMenuItem.Enabled = _data != null;
      _saveAsToolStripMenuItem.Enabled = _data != null;
      _newToolStripMenuItem.Enabled = !_blockFileInteraction;
      _openToolStripMenuItem.Enabled = _openToolStripMenuItem.Enabled && !_blockFileInteraction;
      _saveAsToolStripMenuItem.Enabled = _saveAsToolStripMenuItem.Enabled && !_blockFileInteraction;
      _saveToolStripMenuItem.Enabled = _saveToolStripMenuItem.Enabled && !_blockFileInteraction;
    }

    private bool VerifyClosingFile()
    {
      if (_data == null || _blockFileInteraction) return true;
      var dialogResult = MessageBox.Show("Do you want to save your changes?", "Save changes?",
                                         MessageBoxButtons.YesNoCancel,
                                         MessageBoxIcon.Question);
      if (dialogResult == DialogResult.Cancel)
        return false;
      if (dialogResult == DialogResult.Yes)
        SaveToolStripMenuItem_Click(_saveToolStripMenuItem, new System.EventArgs());
      return true;
    }

    #endregion

    #region Form EventHandlers

    private void FrmApplicationConfiguration_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = !VerifyClosingFile();
    }

    private void NewToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      if (!VerifyClosingFile()) return;
      _dataFile = null;
      LoadApplicationData(new ApplicationData());
    }

    private void OpenToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      if (!VerifyClosingFile()) return;
      var dialog = new OpenFileDialog();
      ApplicationData data;
      do
      {
        if (dialog.ShowDialog() != DialogResult.OK)
          return;
        _dataFile = dialog.FileName;
        data = ApplicationData.Load(dialog.FileName);
        if (data != null) break;
        if (MessageBox.Show("An error occured while loading application data from\r\n" + dialog.FileName, "Error",
                            MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) != DialogResult.Retry)
          return;
      } while (true);
      LoadApplicationData(data);
    }

    private void SaveToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      if (string.IsNullOrEmpty(_dataFile))
      {
        SaveAsToolStripMenuItem_Click(sender, e);
        return;
      }
      if (!ApplicationData.Save(_data, _dataFile))
        MessageBox.Show("Failed to save the data.\r\nCheck logs for more information.", "Error", MessageBoxButtons.OK);
    }

    private void SaveAsToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      var dialog = new SaveFileDialog();
      if (_dataFile != null)
        dialog.FileName = _dataFile;
      if (dialog.ShowDialog() != DialogResult.OK)
        return;
      _dataFile = dialog.FileName;
      SaveToolStripMenuItem_Click(sender, e);
    }

    private void CloseToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      Close();
    }

    #endregion

  }
}
