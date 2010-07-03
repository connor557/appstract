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

namespace AppStract.Manager.Utilities.ApplicationConfiguration
{
  partial class EngineSettingsPage
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this._tabEngineSettingsMain = new System.Windows.Forms.TabControl();
      this._tabEngineSettingsRegistry = new System.Windows.Forms.TabPage();
      this._registryEngineSettingsPageContent = new AppStract.Manager.Utilities.ApplicationConfiguration.RegistryEngineSettingsPageContent();
      this._tabEngineSettingsFileSystem = new System.Windows.Forms.TabPage();
      this._fileSystemEngineSettingsPageContent = new AppStract.Manager.Utilities.ApplicationConfiguration.FileSystemEngineSettingsPageContent();
      this._tabEngineSettingsMain.SuspendLayout();
      this._tabEngineSettingsRegistry.SuspendLayout();
      this._tabEngineSettingsFileSystem.SuspendLayout();
      this.SuspendLayout();
      // 
      // _tabEngineSettingsMain
      // 
      this._tabEngineSettingsMain.Controls.Add(this._tabEngineSettingsRegistry);
      this._tabEngineSettingsMain.Controls.Add(this._tabEngineSettingsFileSystem);
      this._tabEngineSettingsMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this._tabEngineSettingsMain.Location = new System.Drawing.Point(0, 0);
      this._tabEngineSettingsMain.Name = "_tabEngineSettingsMain";
      this._tabEngineSettingsMain.SelectedIndex = 0;
      this._tabEngineSettingsMain.Size = new System.Drawing.Size(678, 429);
      this._tabEngineSettingsMain.TabIndex = 0;
      // 
      // _tabEngineSettingsRegistry
      // 
      this._tabEngineSettingsRegistry.Controls.Add(this._registryEngineSettingsPageContent);
      this._tabEngineSettingsRegistry.Location = new System.Drawing.Point(4, 22);
      this._tabEngineSettingsRegistry.Name = "_tabEngineSettingsRegistry";
      this._tabEngineSettingsRegistry.Padding = new System.Windows.Forms.Padding(3);
      this._tabEngineSettingsRegistry.Size = new System.Drawing.Size(670, 403);
      this._tabEngineSettingsRegistry.TabIndex = 0;
      this._tabEngineSettingsRegistry.Text = "Registry Engine";
      this._tabEngineSettingsRegistry.UseVisualStyleBackColor = true;
      // 
      // _registryEngineSettingsPageContent
      // 
      this._registryEngineSettingsPageContent.Dock = System.Windows.Forms.DockStyle.Fill;
      this._registryEngineSettingsPageContent.Location = new System.Drawing.Point(3, 3);
      this._registryEngineSettingsPageContent.Name = "_registryEngineSettingsPageContent";
      this._registryEngineSettingsPageContent.Size = new System.Drawing.Size(664, 397);
      this._registryEngineSettingsPageContent.TabIndex = 0;
      // 
      // _tabEngineSettingsFileSystem
      // 
      this._tabEngineSettingsFileSystem.Controls.Add(this._fileSystemEngineSettingsPageContent);
      this._tabEngineSettingsFileSystem.Location = new System.Drawing.Point(4, 22);
      this._tabEngineSettingsFileSystem.Name = "_tabEngineSettingsFileSystem";
      this._tabEngineSettingsFileSystem.Padding = new System.Windows.Forms.Padding(3);
      this._tabEngineSettingsFileSystem.Size = new System.Drawing.Size(670, 403);
      this._tabEngineSettingsFileSystem.TabIndex = 1;
      this._tabEngineSettingsFileSystem.Text = "FileSystemEngine";
      this._tabEngineSettingsFileSystem.UseVisualStyleBackColor = true;
      // 
      // _fileSystemEngineSettingsPageContent
      // 
      this._fileSystemEngineSettingsPageContent.Dock = System.Windows.Forms.DockStyle.Fill;
      this._fileSystemEngineSettingsPageContent.Location = new System.Drawing.Point(3, 3);
      this._fileSystemEngineSettingsPageContent.Name = "_fileSystemEngineSettingsPageContent";
      this._fileSystemEngineSettingsPageContent.Size = new System.Drawing.Size(664, 397);
      this._fileSystemEngineSettingsPageContent.TabIndex = 0;
      // 
      // EngineSettingsPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._tabEngineSettingsMain);
      this.Name = "EngineSettingsPage";
      this.Size = new System.Drawing.Size(678, 429);
      this._tabEngineSettingsMain.ResumeLayout(false);
      this._tabEngineSettingsRegistry.ResumeLayout(false);
      this._tabEngineSettingsFileSystem.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl _tabEngineSettingsMain;
    private System.Windows.Forms.TabPage _tabEngineSettingsRegistry;
    private System.Windows.Forms.TabPage _tabEngineSettingsFileSystem;
    private AppStract.Manager.Utilities.ApplicationConfiguration.RegistryEngineSettingsPageContent _registryEngineSettingsPageContent;
    private AppStract.Manager.Utilities.ApplicationConfiguration.FileSystemEngineSettingsPageContent _fileSystemEngineSettingsPageContent;
  }
}