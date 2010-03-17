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

namespace AppStract.Manager
{
  partial class FrmManager
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmManager));
      this._btnPackageNew = new System.Windows.Forms.Button();
      this._btnCleanSystem = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // _btnPackageNew
      // 
      this._btnPackageNew.Location = new System.Drawing.Point(12, 12);
      this._btnPackageNew.Name = "_btnPackageNew";
      this._btnPackageNew.Size = new System.Drawing.Size(301, 43);
      this._btnPackageNew.TabIndex = 5;
      this._btnPackageNew.Text = "Package a new application";
      this._btnPackageNew.UseVisualStyleBackColor = true;
      this._btnPackageNew.Click += new System.EventHandler(this._btnPackageNew_Click);
      // 
      // _btnCleanSystem
      // 
      this._btnCleanSystem.Location = new System.Drawing.Point(12, 61);
      this._btnCleanSystem.Name = "_btnCleanSystem";
      this._btnCleanSystem.Size = new System.Drawing.Size(301, 30);
      this._btnCleanSystem.TabIndex = 6;
      this._btnCleanSystem.Text = "Clean the local system";
      this._btnCleanSystem.UseVisualStyleBackColor = true;
      this._btnCleanSystem.Click += new System.EventHandler(this._btnCleanSystem_Click);
      // 
      // FrmManager
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(325, 103);
      this.Controls.Add(this._btnCleanSystem);
      this.Controls.Add(this._btnPackageNew);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FrmManager";
      this.Text = "AppStract Control Center";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button _btnPackageNew;
    private System.Windows.Forms.Button _btnCleanSystem;

  }
}

