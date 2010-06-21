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
  partial class WizardSelectExecutable
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
      this._listBoxItems = new System.Windows.Forms.ListBox();
      this._labelHeaderHelp = new System.Windows.Forms.Label();
      this._labelHeader = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // _listBoxItems
      // 
      this._listBoxItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this._listBoxItems.FormattingEnabled = true;
      this._listBoxItems.Location = new System.Drawing.Point(38, 89);
      this._listBoxItems.Name = "_listBoxItems";
      this._listBoxItems.Size = new System.Drawing.Size(378, 299);
      this._listBoxItems.Sorted = true;
      this._listBoxItems.TabIndex = 2;
      this._listBoxItems.SelectedIndexChanged += new System.EventHandler(this._listBoxItems_SelectedIndexChanged);
      // 
      // _labelHeaderHelp
      // 
      this._labelHeaderHelp.AutoSize = true;
      this._labelHeaderHelp.Location = new System.Drawing.Point(35, 54);
      this._labelHeaderHelp.Name = "_labelHeaderHelp";
      this._labelHeaderHelp.Size = new System.Drawing.Size(371, 13);
      this._labelHeaderHelp.TabIndex = 10;
      this._labelHeaderHelp.Text = "You can select the executable to start the new portable application with here.";
      // 
      // _labelHeader
      // 
      this._labelHeader.AutoSize = true;
      this._labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._labelHeader.Location = new System.Drawing.Point(19, 22);
      this._labelHeader.Name = "_labelHeader";
      this._labelHeader.Size = new System.Drawing.Size(206, 24);
      this._labelHeader.TabIndex = 9;
      this._labelHeader.Text = "Select an executable";
      // 
      // WizardSelectExecutable
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Transparent;
      this.Controls.Add(this._labelHeaderHelp);
      this.Controls.Add(this._labelHeader);
      this.Controls.Add(this._listBoxItems);
      this.Name = "WizardSelectExecutable";
      this.Size = new System.Drawing.Size(438, 408);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox _listBoxItems;
    private System.Windows.Forms.Label _labelHeaderHelp;
    private System.Windows.Forms.Label _labelHeader;
  }
}
