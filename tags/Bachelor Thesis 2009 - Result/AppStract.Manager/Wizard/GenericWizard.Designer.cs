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

namespace AppStract.Manager.Wizard
{
  partial class GenericWizard<T>
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
      this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this._panelLogo = new System.Windows.Forms.Panel();
      this._panelContent = new System.Windows.Forms.Panel();
      this._buttonCancel = new System.Windows.Forms.Button();
      this._buttonNext = new System.Windows.Forms.Button();
      this._buttonBack = new System.Windows.Forms.Button();
      this._panelSteps = new System.Windows.Forms.Panel();
      this._tableLayoutPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // _tableLayoutPanel
      // 
      this._tableLayoutPanel.BackColor = System.Drawing.Color.White;
      this._tableLayoutPanel.ColumnCount = 6;
      this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
      this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
      this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
      this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
      this._tableLayoutPanel.Controls.Add(this._panelLogo, 0, 0);
      this._tableLayoutPanel.Controls.Add(this._panelContent, 2, 0);
      this._tableLayoutPanel.Controls.Add(this._buttonCancel, 4, 2);
      this._tableLayoutPanel.Controls.Add(this._buttonNext, 3, 2);
      this._tableLayoutPanel.Controls.Add(this._buttonBack, 2, 2);
      this._tableLayoutPanel.Controls.Add(this._panelSteps, 0, 1);
      this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
      this._tableLayoutPanel.Name = "_tableLayoutPanel";
      this._tableLayoutPanel.RowCount = 3;
      this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
      this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this._tableLayoutPanel.Size = new System.Drawing.Size(594, 474);
      this._tableLayoutPanel.TabIndex = 0;
      // 
      // _panelLogo
      // 
      this._panelLogo.BackColor = System.Drawing.Color.Transparent;
      this._panelLogo.BackgroundImage = global::AppStract.Manager.Properties.Resources.logo;
      this._tableLayoutPanel.SetColumnSpan(this._panelLogo, 2);
      this._panelLogo.Dock = System.Windows.Forms.DockStyle.Top;
      this._panelLogo.Location = new System.Drawing.Point(3, 3);
      this._panelLogo.Name = "_panelLogo";
      this._panelLogo.Size = new System.Drawing.Size(134, 134);
      this._panelLogo.TabIndex = 1;
      // 
      // _panelContent
      // 
      this._panelContent.BackColor = System.Drawing.Color.Transparent;
      this._tableLayoutPanel.SetColumnSpan(this._panelContent, 4);
      this._panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
      this._panelContent.Location = new System.Drawing.Point(143, 3);
      this._panelContent.Name = "_panelContent";
      this._tableLayoutPanel.SetRowSpan(this._panelContent, 2);
      this._panelContent.Size = new System.Drawing.Size(448, 418);
      this._panelContent.TabIndex = 2;
      // 
      // _buttonCancel
      // 
      this._buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this._buttonCancel.Location = new System.Drawing.Point(506, 437);
      this._buttonCancel.Name = "_buttonCancel";
      this._buttonCancel.Size = new System.Drawing.Size(75, 23);
      this._buttonCancel.TabIndex = 3;
      this._buttonCancel.Text = "Cancel";
      this._buttonCancel.UseVisualStyleBackColor = true;
      // 
      // _buttonNext
      // 
      this._buttonNext.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._buttonNext.Location = new System.Drawing.Point(407, 437);
      this._buttonNext.Name = "_buttonNext";
      this._buttonNext.Size = new System.Drawing.Size(74, 23);
      this._buttonNext.TabIndex = 4;
      this._buttonNext.Tag = "1";
      this._buttonNext.Text = "Next >";
      this._buttonNext.UseVisualStyleBackColor = true;
      this._buttonNext.Click += new System.EventHandler(this.ButtonStep_ClickEventHandler);
      // 
      // _buttonBack
      // 
      this._buttonBack.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this._buttonBack.Location = new System.Drawing.Point(326, 437);
      this._buttonBack.Name = "_buttonBack";
      this._buttonBack.Size = new System.Drawing.Size(75, 23);
      this._buttonBack.TabIndex = 5;
      this._buttonBack.Tag = "-1";
      this._buttonBack.Text = "< Back";
      this._buttonBack.UseVisualStyleBackColor = true;
      this._buttonBack.Click += new System.EventHandler(this.ButtonStep_ClickEventHandler);
      // 
      // _panelSteps
      // 
      this._panelSteps.BackColor = System.Drawing.Color.Transparent;
      this._tableLayoutPanel.SetColumnSpan(this._panelSteps, 2);
      this._panelSteps.Dock = System.Windows.Forms.DockStyle.Fill;
      this._panelSteps.Location = new System.Drawing.Point(3, 143);
      this._panelSteps.Name = "_panelSteps";
      this._panelSteps.Size = new System.Drawing.Size(134, 278);
      this._panelSteps.TabIndex = 6;
      // 
      // GenericWizard
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(594, 474);
      this.Controls.Add(this._tableLayoutPanel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MaximumSize = new System.Drawing.Size(600, 500);
      this.MinimumSize = new System.Drawing.Size(600, 500);
      this.Name = "GenericWizard";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = "New Portable Application Wizard";
      this.Load += new System.EventHandler(this.GenericWizard_LoadEventHandler);
      this._tableLayoutPanel.ResumeLayout(false);
      this.ResumeLayout(false);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
    private System.Windows.Forms.Panel _panelLogo;
    private System.Windows.Forms.Panel _panelContent;
    private System.Windows.Forms.Button _buttonCancel;
    private System.Windows.Forms.Button _buttonNext;
    private System.Windows.Forms.Button _buttonBack;
    private System.Windows.Forms.Panel _panelSteps;
  }
}