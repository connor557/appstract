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

namespace AppStract.Manager.Utilities
{
  partial class ApplicationConfigurationUtility
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this._menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this._newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this._openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this._saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this._saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this._toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this._closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this._tabMain = new System.Windows.Forms.TabControl();
      this._tabPageApplicationFiles = new System.Windows.Forms.TabPage();
      this._tabPageEngineSettings = new System.Windows.Forms.TabPage();
      this._menuStrip.SuspendLayout();
      this._tabMain.SuspendLayout();
      this.SuspendLayout();
      // 
      // _menuStrip
      // 
      this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this._menuStrip.Location = new System.Drawing.Point(0, 0);
      this._menuStrip.Name = "_menuStrip";
      this._menuStrip.Size = new System.Drawing.Size(741, 24);
      this._menuStrip.TabIndex = 0;
      this._menuStrip.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._newToolStripMenuItem,
            this._openToolStripMenuItem,
            this._saveToolStripMenuItem,
            this._saveAsToolStripMenuItem,
            this._toolStripSeparator1,
            this._closeToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // _newToolStripMenuItem
      // 
      this._newToolStripMenuItem.Name = "_newToolStripMenuItem";
      this._newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
      this._newToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
      this._newToolStripMenuItem.Text = "New";
      this._newToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
      // 
      // _openToolStripMenuItem
      // 
      this._openToolStripMenuItem.Name = "_openToolStripMenuItem";
      this._openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
      this._openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
      this._openToolStripMenuItem.Text = "Open...";
      this._openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
      // 
      // _saveToolStripMenuItem
      // 
      this._saveToolStripMenuItem.Enabled = false;
      this._saveToolStripMenuItem.Name = "_saveToolStripMenuItem";
      this._saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
      this._saveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
      this._saveToolStripMenuItem.Text = "Save";
      this._saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
      // 
      // _saveAsToolStripMenuItem
      // 
      this._saveAsToolStripMenuItem.Enabled = false;
      this._saveAsToolStripMenuItem.Name = "_saveAsToolStripMenuItem";
      this._saveAsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
      this._saveAsToolStripMenuItem.Text = "Save As...";
      this._saveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
      // 
      // _toolStripSeparator1
      // 
      this._toolStripSeparator1.Name = "_toolStripSeparator1";
      this._toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
      // 
      // _closeToolStripMenuItem
      // 
      this._closeToolStripMenuItem.Name = "_closeToolStripMenuItem";
      this._closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
      this._closeToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
      this._closeToolStripMenuItem.Text = "Close";
      this._closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
      // 
      // _tabMain
      // 
      this._tabMain.Controls.Add(this._tabPageApplicationFiles);
      this._tabMain.Controls.Add(this._tabPageEngineSettings);
      this._tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this._tabMain.Location = new System.Drawing.Point(0, 24);
      this._tabMain.Name = "_tabMain";
      this._tabMain.SelectedIndex = 0;
      this._tabMain.Size = new System.Drawing.Size(741, 416);
      this._tabMain.TabIndex = 1;
      // 
      // _tabPageApplicationFiles
      // 
      this._tabPageApplicationFiles.Location = new System.Drawing.Point(4, 22);
      this._tabPageApplicationFiles.Name = "_tabPageApplicationFiles";
      this._tabPageApplicationFiles.Padding = new System.Windows.Forms.Padding(3);
      this._tabPageApplicationFiles.Size = new System.Drawing.Size(733, 390);
      this._tabPageApplicationFiles.TabIndex = 0;
      this._tabPageApplicationFiles.Text = "Files";
      this._tabPageApplicationFiles.UseVisualStyleBackColor = true;
      // 
      // _tabPageEngineSettings
      // 
      this._tabPageEngineSettings.Location = new System.Drawing.Point(4, 22);
      this._tabPageEngineSettings.Name = "_tabPageEngineSettings";
      this._tabPageEngineSettings.Padding = new System.Windows.Forms.Padding(3);
      this._tabPageEngineSettings.Size = new System.Drawing.Size(733, 390);
      this._tabPageEngineSettings.TabIndex = 1;
      this._tabPageEngineSettings.Text = "Engine Settings";
      this._tabPageEngineSettings.UseVisualStyleBackColor = true;
      // 
      // ApplicationConfigurationUtility
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(741, 440);
      this.Controls.Add(this._tabMain);
      this.Controls.Add(this._menuStrip);
      this.MainMenuStrip = this._menuStrip;
      this.Name = "ApplicationConfigurationUtility";
      this.Text = "Application Configuration";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmApplicationConfiguration_FormClosing);
      this._menuStrip.ResumeLayout(false);
      this._menuStrip.PerformLayout();
      this._tabMain.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip _menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.TabControl _tabMain;
    private System.Windows.Forms.TabPage _tabPageApplicationFiles;
    private System.Windows.Forms.TabPage _tabPageEngineSettings;
    private System.Windows.Forms.ToolStripMenuItem _newToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem _openToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem _saveToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem _saveAsToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator _toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem _closeToolStripMenuItem;
  }
}