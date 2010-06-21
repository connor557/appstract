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
  partial class WizardIntroduction
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
      this._labelContent = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // _labelHeader
      // 
      this._labelHeader.AutoSize = true;
      this._labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._labelHeader.Location = new System.Drawing.Point(19, 22);
      this._labelHeader.Name = "_labelHeader";
      this._labelHeader.Size = new System.Drawing.Size(389, 24);
      this._labelHeader.TabIndex = 0;
      this._labelHeader.Text = "Configure your New Portable Application";
      // 
      // _labelContent
      // 
      this._labelContent.AutoSize = true;
      this._labelContent.Location = new System.Drawing.Point(36, 96);
      this._labelContent.Name = "_labelContent";
      this._labelContent.Size = new System.Drawing.Size(304, 52);
      this._labelContent.TabIndex = 1;
      this._labelContent.Text = "This wizard helps you configuring your new portable application\r\nfor a first time" +
          " use.\r\n\r\nTo continue, click Next.";
      // 
      // WizardIntroduction
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Transparent;
      this.Controls.Add(this._labelContent);
      this.Controls.Add(this._labelHeader);
      this.Name = "WizardIntroduction";
      this.Size = new System.Drawing.Size(438, 408);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label _labelHeader;
    private System.Windows.Forms.Label _labelContent;
  }
}
