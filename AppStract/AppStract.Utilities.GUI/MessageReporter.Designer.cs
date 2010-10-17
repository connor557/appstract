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

namespace AppStract.Utilities.GUI
{
  partial class MessageReporter
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
      this._layoutMain = new System.Windows.Forms.TableLayoutPanel();
      this._button1 = new System.Windows.Forms.Button();
      this._button2 = new System.Windows.Forms.Button();
      this._button3 = new System.Windows.Forms.Button();
      this._layoutContent = new System.Windows.Forms.TableLayoutPanel();
      this._btnExpand = new System.Windows.Forms.Button();
      this._lblMessage = new System.Windows.Forms.Label();
      this._txtMessageDetail = new System.Windows.Forms.RichTextBox();
      this._layoutMain.SuspendLayout();
      this._layoutContent.SuspendLayout();
      this.SuspendLayout();
      // 
      // _layoutMain
      // 
      this._layoutMain.ColumnCount = 4;
      this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this._layoutMain.Controls.Add(this._button1, 1, 1);
      this._layoutMain.Controls.Add(this._button2, 2, 1);
      this._layoutMain.Controls.Add(this._button3, 3, 1);
      this._layoutMain.Controls.Add(this._layoutContent, 0, 0);
      this._layoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this._layoutMain.Location = new System.Drawing.Point(0, 0);
      this._layoutMain.Name = "_layoutMain";
      this._layoutMain.RowCount = 2;
      this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
      this._layoutMain.Size = new System.Drawing.Size(504, 124);
      this._layoutMain.TabIndex = 0;
      // 
      // _button1
      // 
      this._button1.Location = new System.Drawing.Point(207, 93);
      this._button1.Name = "_button1";
      this._button1.Size = new System.Drawing.Size(93, 27);
      this._button1.TabIndex = 1;
      this._button1.Text = "button1";
      this._button1.UseVisualStyleBackColor = true;
      this._button1.Click += new System.EventHandler(this.ButtonClose_Click);
      // 
      // _button2
      // 
      this._button2.Location = new System.Drawing.Point(307, 93);
      this._button2.Name = "_button2";
      this._button2.Size = new System.Drawing.Size(93, 27);
      this._button2.TabIndex = 2;
      this._button2.Text = "button2";
      this._button2.UseVisualStyleBackColor = true;
      this._button2.Click += new System.EventHandler(this.ButtonClose_Click);
      // 
      // _button3
      // 
      this._button3.Location = new System.Drawing.Point(407, 93);
      this._button3.Name = "_button3";
      this._button3.Size = new System.Drawing.Size(93, 27);
      this._button3.TabIndex = 3;
      this._button3.Text = "button3";
      this._button3.UseVisualStyleBackColor = true;
      this._button3.Click += new System.EventHandler(this.ButtonClose_Click);
      // 
      // _layoutContent
      // 
      this._layoutContent.ColumnCount = 3;
      this._layoutMain.SetColumnSpan(this._layoutContent, 4);
      this._layoutContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this._layoutContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this._layoutContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
      this._layoutContent.Controls.Add(this._btnExpand, 2, 1);
      this._layoutContent.Controls.Add(this._lblMessage, 1, 0);
      this._layoutContent.Controls.Add(this._txtMessageDetail, 1, 2);
      this._layoutContent.Dock = System.Windows.Forms.DockStyle.Fill;
      this._layoutContent.Location = new System.Drawing.Point(3, 3);
      this._layoutContent.Name = "_layoutContent";
      this._layoutContent.RowCount = 3;
      this._layoutContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
      this._layoutContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
      this._layoutContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
      this._layoutContent.Size = new System.Drawing.Size(498, 84);
      this._layoutContent.TabIndex = 4;
      // 
      // _btnExpand
      // 
      this._btnExpand.Dock = System.Windows.Forms.DockStyle.Fill;
      this._btnExpand.Image = global::AppStract.Utilities.GUI.Properties.Resources.Down;
      this._btnExpand.Location = new System.Drawing.Point(469, 55);
      this._btnExpand.Name = "_btnExpand";
      this._btnExpand.Size = new System.Drawing.Size(26, 28);
      this._btnExpand.TabIndex = 0;
      this._btnExpand.UseVisualStyleBackColor = true;
      this._btnExpand.Click += new System.EventHandler(this.ButtonExpand_Click);
      // 
      // _lblMessage
      // 
      this._lblMessage.AutoSize = true;
      this._lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
      this._lblMessage.Location = new System.Drawing.Point(23, 0);
      this._lblMessage.Name = "_lblMessage";
      this._lblMessage.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
      this._layoutContent.SetRowSpan(this._lblMessage, 2);
      this._lblMessage.Size = new System.Drawing.Size(440, 86);
      this._lblMessage.TabIndex = 1;
      this._lblMessage.Text = "_lblMessage";
      // 
      // _txtMessageDetail
      // 
      this._txtMessageDetail.Dock = System.Windows.Forms.DockStyle.Fill;
      this._txtMessageDetail.Location = new System.Drawing.Point(23, 89);
      this._txtMessageDetail.Name = "_txtMessageDetail";
      this._txtMessageDetail.ReadOnly = true;
      this._txtMessageDetail.Size = new System.Drawing.Size(440, 1);
      this._txtMessageDetail.TabIndex = 2;
      this._txtMessageDetail.Text = "";
      // 
      // MessageReporter
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(504, 124);
      this.Controls.Add(this._layoutMain);
      this.MaximizeBox = false;
      this.MaximumSize = new System.Drawing.Size(520, 160);
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(370, 160);
      this.Name = "MessageReporter";
      this.ShowInTaskbar = false;
      this.Text = "MessageReporter";
      this._layoutMain.ResumeLayout(false);
      this._layoutContent.ResumeLayout(false);
      this._layoutContent.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel _layoutMain;
    private System.Windows.Forms.Button _button1;
    private System.Windows.Forms.Button _button2;
    private System.Windows.Forms.Button _button3;
    private System.Windows.Forms.TableLayoutPanel _layoutContent;
    private System.Windows.Forms.Button _btnExpand;
    private System.Windows.Forms.Label _lblMessage;
    private System.Windows.Forms.RichTextBox _txtMessageDetail;

  }
}