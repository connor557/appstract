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
      this.lblIntroduction = new System.Windows.Forms.Label();
      this._lnkPackage = new System.Windows.Forms.LinkLabel();
      this._lnkLoad = new System.Windows.Forms.LinkLabel();
      this.SuspendLayout();
      // 
      // lblIntroduction
      // 
      this.lblIntroduction.AutoSize = true;
      this.lblIntroduction.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblIntroduction.Location = new System.Drawing.Point(12, 28);
      this.lblIntroduction.Name = "lblIntroduction";
      this.lblIntroduction.Size = new System.Drawing.Size(300, 48);
      this.lblIntroduction.TabIndex = 2;
      this.lblIntroduction.Text = "Welcome to the AppStract Control Center window.\r\nHere you can either package a ne" +
          "w application,\r\nor you can load a packaged applications.";
      // 
      // _lnkPackage
      // 
      this._lnkPackage.AutoSize = true;
      this._lnkPackage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._lnkPackage.Location = new System.Drawing.Point(131, 44);
      this._lnkPackage.Name = "_lnkPackage";
      this._lnkPackage.Size = new System.Drawing.Size(169, 16);
      this._lnkPackage.TabIndex = 3;
      this._lnkPackage.TabStop = true;
      this._lnkPackage.Text = "package a new application";
      this._lnkPackage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._lnkPackage_LinkClicked);
      // 
      // _lnkLoad
      // 
      this._lnkLoad.AutoSize = true;
      this._lnkLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._lnkLoad.Location = new System.Drawing.Point(79, 60);
      this._lnkLoad.Name = "_lnkLoad";
      this._lnkLoad.Size = new System.Drawing.Size(180, 16);
      this._lnkLoad.TabIndex = 4;
      this._lnkLoad.TabStop = true;
      this._lnkLoad.Text = "load a packaged application";
      this._lnkLoad.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._lnkLoad_LinkClicked);
      // 
      // FrmPackager
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(325, 108);
      this.Controls.Add(this._lnkLoad);
      this.Controls.Add(this._lnkPackage);
      this.Controls.Add(this.lblIntroduction);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FrmPackager";
      this.Text = "AppStract Control Center";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblIntroduction;
    private System.Windows.Forms.LinkLabel _lnkPackage;
    private System.Windows.Forms.LinkLabel _lnkLoad;

  }
}

