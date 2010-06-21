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

namespace AppStract.Manager.Packaging.PostConfiguration
{
  partial class WizardSettings
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
      this._labelHeader = new System.Windows.Forms.Label();
      this._checkBoxForceVirtualFileSystem = new System.Windows.Forms.CheckBox();
      this._labelHeaderHelp = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // _labelHeader
      // 
      this._labelHeader.AutoSize = true;
      this._labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._labelHeader.Location = new System.Drawing.Point(19, 22);
      this._labelHeader.Name = "_labelHeader";
      this._labelHeader.Size = new System.Drawing.Size(84, 24);
      this._labelHeader.TabIndex = 3;
      this._labelHeader.Text = "Settings";
      // 
      // _checkBoxForceVirtualFileSystem
      // 
      this._checkBoxForceVirtualFileSystem.AutoSize = true;
      this._checkBoxForceVirtualFileSystem.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
      this._checkBoxForceVirtualFileSystem.Checked = true;
      this._checkBoxForceVirtualFileSystem.CheckState = System.Windows.Forms.CheckState.Checked;
      this._checkBoxForceVirtualFileSystem.Enabled = false;
      this._checkBoxForceVirtualFileSystem.Location = new System.Drawing.Point(36, 98);
      this._checkBoxForceVirtualFileSystem.Name = "_checkBoxForceVirtualFileSystem";
      this._checkBoxForceVirtualFileSystem.Size = new System.Drawing.Size(354, 17);
      this._checkBoxForceVirtualFileSystem.TabIndex = 5;
      this._checkBoxForceVirtualFileSystem.Text = "Force my new portable application to always use the virtual filesystem.";
      this._checkBoxForceVirtualFileSystem.UseVisualStyleBackColor = true;
      // 
      // _labelHeaderHelp
      // 
      this._labelHeaderHelp.AutoSize = true;
      this._labelHeaderHelp.Location = new System.Drawing.Point(35, 54);
      this._labelHeaderHelp.Name = "_labelHeaderHelp";
      this._labelHeaderHelp.Size = new System.Drawing.Size(363, 13);
      this._labelHeaderHelp.TabIndex = 8;
      this._labelHeaderHelp.Text = "You can select your preferred settings for the new portable application here.";
      // 
      // WizardSettings
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Transparent;
      this.Controls.Add(this._labelHeaderHelp);
      this.Controls.Add(this._checkBoxForceVirtualFileSystem);
      this.Controls.Add(this._labelHeader);
      this.Name = "WizardSettings";
      this.Size = new System.Drawing.Size(438, 408);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label _labelHeader;
    private System.Windows.Forms.CheckBox _checkBoxForceVirtualFileSystem;
    private System.Windows.Forms.Label _labelHeaderHelp;
  }
}
