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
  partial class CleanUpHelper
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
      this.label1 = new System.Windows.Forms.Label();
      this._listInsurances = new System.Windows.Forms.CheckedListBox();
      this._btnCleanSelected = new System.Windows.Forms.Button();
      this._btnCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(23, 26);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(330, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "While scanning the system the following leaked resources are found:";
      // 
      // _listInsurances
      // 
      this._listInsurances.FormattingEnabled = true;
      this._listInsurances.Location = new System.Drawing.Point(36, 51);
      this._listInsurances.Name = "_listInsurances";
      this._listInsurances.Size = new System.Drawing.Size(309, 109);
      this._listInsurances.TabIndex = 1;
      // 
      // _btnCleanSelected
      // 
      this._btnCleanSelected.Location = new System.Drawing.Point(107, 166);
      this._btnCleanSelected.Name = "_btnCleanSelected";
      this._btnCleanSelected.Size = new System.Drawing.Size(246, 33);
      this._btnCleanSelected.TabIndex = 2;
      this._btnCleanSelected.Text = "Clean selected items";
      this._btnCleanSelected.UseVisualStyleBackColor = true;
      this._btnCleanSelected.Click += new System.EventHandler(this.CleanSelected);
      // 
      // _btnCancel
      // 
      this._btnCancel.Location = new System.Drawing.Point(26, 166);
      this._btnCancel.Name = "_btnCancel";
      this._btnCancel.Size = new System.Drawing.Size(75, 33);
      this._btnCancel.TabIndex = 3;
      this._btnCancel.Text = "Cancel";
      this._btnCancel.UseVisualStyleBackColor = true;
      this._btnCancel.Click += new System.EventHandler(this.Cancel);
      // 
      // FrmCleanUp
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(391, 221);
      this.Controls.Add(this._btnCancel);
      this.Controls.Add(this._btnCleanSelected);
      this.Controls.Add(this._listInsurances);
      this.Controls.Add(this.label1);
      this.Name = "FrmCleanUp";
      this.Text = "AppStract - System CleanUp Utility";
      this.Shown += new System.EventHandler(this.FrmCleanUp_Shown);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckedListBox _listInsurances;
    private System.Windows.Forms.Button _btnCleanSelected;
    private System.Windows.Forms.Button _btnCancel;
  }
}