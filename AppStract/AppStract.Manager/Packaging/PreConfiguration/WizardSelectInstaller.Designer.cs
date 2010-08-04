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

namespace AppStract.Manager.Packaging.PreConfiguration
{
  partial class WizardSelectInstaller
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardSelectInstaller));
      this._labelOutputFolderHelp = new System.Windows.Forms.Label();
      this._labelHeader = new System.Windows.Forms.Label();
      this._labelDestination = new System.Windows.Forms.Label();
      this._textBoxOutputFolder = new System.Windows.Forms.TextBox();
      this._textBoxInstallerLocation = new System.Windows.Forms.TextBox();
      this._labelLocation = new System.Windows.Forms.Label();
      this._labelInstallerLocationHelp = new System.Windows.Forms.Label();
      this._buttonBrowseInstaller = new System.Windows.Forms.Button();
      this._buttonBrowseOutput = new System.Windows.Forms.Button();
      this._labelHeaderHelp = new System.Windows.Forms.Label();
      this._chkShowConfigUtil = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // _labelOutputFolderHelp
      // 
      this._labelOutputFolderHelp.AutoSize = true;
      this._labelOutputFolderHelp.ForeColor = System.Drawing.SystemColors.ControlDark;
      this._labelOutputFolderHelp.Location = new System.Drawing.Point(38, 273);
      this._labelOutputFolderHelp.Name = "_labelOutputFolderHelp";
      this._labelOutputFolderHelp.Size = new System.Drawing.Size(373, 65);
      this._labelOutputFolderHelp.TabIndex = 4;
      this._labelOutputFolderHelp.Text = resources.GetString("_labelOutputFolderHelp.Text");
      // 
      // _labelHeader
      // 
      this._labelHeader.AutoSize = true;
      this._labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._labelHeader.Location = new System.Drawing.Point(19, 22);
      this._labelHeader.Name = "_labelHeader";
      this._labelHeader.Size = new System.Drawing.Size(192, 24);
      this._labelHeader.TabIndex = 3;
      this._labelHeader.Text = "Application Installer";
      // 
      // _labelDestination
      // 
      this._labelDestination.AutoSize = true;
      this._labelDestination.Location = new System.Drawing.Point(20, 223);
      this._labelDestination.Name = "_labelDestination";
      this._labelDestination.Size = new System.Drawing.Size(98, 13);
      this._labelDestination.TabIndex = 5;
      this._labelDestination.Text = "Output Destination:";
      // 
      // _textBoxOutputFolder
      // 
      this._textBoxOutputFolder.Location = new System.Drawing.Point(23, 250);
      this._textBoxOutputFolder.Name = "_textBoxOutputFolder";
      this._textBoxOutputFolder.Size = new System.Drawing.Size(318, 20);
      this._textBoxOutputFolder.TabIndex = 6;
      this._textBoxOutputFolder.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
      // 
      // _textBoxInstallerLocation
      // 
      this._textBoxInstallerLocation.Location = new System.Drawing.Point(23, 144);
      this._textBoxInstallerLocation.Name = "_textBoxInstallerLocation";
      this._textBoxInstallerLocation.Size = new System.Drawing.Size(318, 20);
      this._textBoxInstallerLocation.TabIndex = 9;
      this._textBoxInstallerLocation.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
      // 
      // _labelLocation
      // 
      this._labelLocation.AutoSize = true;
      this._labelLocation.Location = new System.Drawing.Point(20, 117);
      this._labelLocation.Name = "_labelLocation";
      this._labelLocation.Size = new System.Drawing.Size(90, 13);
      this._labelLocation.TabIndex = 8;
      this._labelLocation.Text = "Installer Location:";
      // 
      // _labelInstallerLocationHelp
      // 
      this._labelInstallerLocationHelp.AutoSize = true;
      this._labelInstallerLocationHelp.ForeColor = System.Drawing.SystemColors.ControlDark;
      this._labelInstallerLocationHelp.Location = new System.Drawing.Point(38, 167);
      this._labelInstallerLocationHelp.Name = "_labelInstallerLocationHelp";
      this._labelInstallerLocationHelp.Size = new System.Drawing.Size(344, 26);
      this._labelInstallerLocationHelp.TabIndex = 7;
      this._labelInstallerLocationHelp.Text = "The selected file is expected to be an installer. At the end of this wizard,\r\nit " +
          "will be started and used to create your new portable application from.";
      // 
      // _buttonBrowseInstaller
      // 
      this._buttonBrowseInstaller.Location = new System.Drawing.Point(347, 142);
      this._buttonBrowseInstaller.Name = "_buttonBrowseInstaller";
      this._buttonBrowseInstaller.Size = new System.Drawing.Size(75, 23);
      this._buttonBrowseInstaller.TabIndex = 10;
      this._buttonBrowseInstaller.Text = "Browse...";
      this._buttonBrowseInstaller.UseVisualStyleBackColor = true;
      this._buttonBrowseInstaller.Click += new System.EventHandler(this.BtnLocationBrowse_Click);
      // 
      // _buttonBrowseOutput
      // 
      this._buttonBrowseOutput.Location = new System.Drawing.Point(347, 247);
      this._buttonBrowseOutput.Name = "_buttonBrowseOutput";
      this._buttonBrowseOutput.Size = new System.Drawing.Size(75, 23);
      this._buttonBrowseOutput.TabIndex = 11;
      this._buttonBrowseOutput.Text = "Browse...";
      this._buttonBrowseOutput.UseVisualStyleBackColor = true;
      this._buttonBrowseOutput.Click += new System.EventHandler(this.BtnBrowseOutput_Click);
      // 
      // _labelHeaderHelp
      // 
      this._labelHeaderHelp.AutoSize = true;
      this._labelHeaderHelp.Location = new System.Drawing.Point(35, 54);
      this._labelHeaderHelp.Name = "_labelHeaderHelp";
      this._labelHeaderHelp.Size = new System.Drawing.Size(312, 13);
      this._labelHeaderHelp.TabIndex = 12;
      this._labelHeaderHelp.Text = "You can select the installer for the new portable application here.";
      // 
      // _chkShowConfigUtil
      // 
      this._chkShowConfigUtil.AutoSize = true;
      this._chkShowConfigUtil.Location = new System.Drawing.Point(23, 365);
      this._chkShowConfigUtil.Name = "_chkShowConfigUtil";
      this._chkShowConfigUtil.Size = new System.Drawing.Size(341, 17);
      this._chkShowConfigUtil.TabIndex = 13;
      this._chkShowConfigUtil.Text = "Show the Engine Configuration Utility before launching the installer.";
      this._chkShowConfigUtil.UseVisualStyleBackColor = true;
      // 
      // WizardSelectInstaller
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Transparent;
      this.Controls.Add(this._chkShowConfigUtil);
      this.Controls.Add(this._labelHeaderHelp);
      this.Controls.Add(this._buttonBrowseOutput);
      this.Controls.Add(this._buttonBrowseInstaller);
      this.Controls.Add(this._textBoxInstallerLocation);
      this.Controls.Add(this._labelLocation);
      this.Controls.Add(this._labelInstallerLocationHelp);
      this.Controls.Add(this._textBoxOutputFolder);
      this.Controls.Add(this._labelDestination);
      this.Controls.Add(this._labelOutputFolderHelp);
      this.Controls.Add(this._labelHeader);
      this.Name = "WizardSelectInstaller";
      this.Size = new System.Drawing.Size(438, 408);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label _labelOutputFolderHelp;
    private System.Windows.Forms.Label _labelHeader;
    private System.Windows.Forms.Label _labelDestination;
    private System.Windows.Forms.TextBox _textBoxOutputFolder;
    private System.Windows.Forms.TextBox _textBoxInstallerLocation;
    private System.Windows.Forms.Label _labelLocation;
    private System.Windows.Forms.Label _labelInstallerLocationHelp;
    private System.Windows.Forms.Button _buttonBrowseInstaller;
    private System.Windows.Forms.Button _buttonBrowseOutput;
    private System.Windows.Forms.Label _labelHeaderHelp;
    private System.Windows.Forms.CheckBox _chkShowConfigUtil;
  }
}
