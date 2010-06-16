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
using AppStract.Core.Data.Application;
using AppStract.Manager.ApplicationConfiguration;

namespace AppStract.Manager
{
  public partial class FrmApplicationConfiguration : Form
  {

    #region Variables

    private string _dataFile;
    private ApplicationData _data;

    #endregion

    #region Constructors

    public FrmApplicationConfiguration()
    {
      InitializeComponent();
      _tabPageApplicationFiles.Controls.Add(new ApplicationFilesPage { Dock = DockStyle.Fill });
      _tabPageEngineSettings.Controls.Add(new EngineSettingsPage { Dock = DockStyle.Fill });
    }

    #endregion

    #region Public Methods

    public void LoadApplicationData(ApplicationData data)
    {
      _data = data;
      _saveToolStripMenuItem.Enabled = true;
      _saveAsToolStripMenuItem.Enabled = true;
      foreach (Control tabPage in _tabMain.TabPages)
        foreach (IApplicationConfigurationPage page in tabPage.Controls)
          page.BindDataSource(data);
    }

    #endregion

    #region Private Methods

    private bool VerifyClosingFile()
    {
      if (_data == null) return true;
      var dialogResult = MessageBox.Show("Do you want to save your changes?", "Save changes?",
                                         MessageBoxButtons.YesNoCancel,
                                         MessageBoxIcon.Question);
      if (dialogResult == DialogResult.Cancel)
        return false;
      if (dialogResult == DialogResult.Yes)
        saveToolStripMenuItem_Click(_saveToolStripMenuItem, new System.EventArgs());
      return true;
    }

    #endregion

    #region Form EventHandlers

    private void FrmApplicationConfiguration_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = !VerifyClosingFile();
    }

    private void newToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      if (!VerifyClosingFile()) return;
      _dataFile = null;
      LoadApplicationData(new ApplicationData());
    }

    private void openToolStripMenuItem_Click(object sender, System.EventArgs e)
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

    private void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      if (string.IsNullOrEmpty(_dataFile))
      {
        saveAsToolStripMenuItem_Click(sender, e);
        return;
      }
      if (!ApplicationData.Save(_data, _dataFile))
        MessageBox.Show("Failed to save the data.\r\nCheck logs for more information.", "Error", MessageBoxButtons.OK);
    }

    private void saveAsToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      var dialog = new SaveFileDialog();
      if (_dataFile != null)
        dialog.FileName = _dataFile;
      if (dialog.ShowDialog() != DialogResult.OK)
        return;
      _dataFile = dialog.FileName;
      saveToolStripMenuItem_Click(sender, e);
    }

    private void closeToolStripMenuItem_Click(object sender, System.EventArgs e)
    {
      Close();
    }

    #endregion

  }
}
