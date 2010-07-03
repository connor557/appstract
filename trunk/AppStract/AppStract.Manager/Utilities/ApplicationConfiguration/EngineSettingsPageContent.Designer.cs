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
  partial class EngineSettingsPageContent
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
      this._splitContainerEngineSettingsMain = new System.Windows.Forms.SplitContainer();
      this._tableEngineSettingsList = new System.Windows.Forms.TableLayoutPanel();
      this._btnDeleteRule = new System.Windows.Forms.Button();
      this._btnNewRule = new System.Windows.Forms.Button();
      this._btnRuleDown = new System.Windows.Forms.Button();
      this._listRules = new System.Windows.Forms.ListBox();
      this._btnRuleUp = new System.Windows.Forms.Button();
      this._gbEngineRuleConfiguration = new System.Windows.Forms.GroupBox();
      this._lblRuleSelector = new System.Windows.Forms.Label();
      this._cmbRuleSelector = new System.Windows.Forms.ComboBox();
      this._btnApplyRule = new System.Windows.Forms.Button();
      this._txtKeyItem = new System.Windows.Forms.TextBox();
      this._txtKey = new System.Windows.Forms.TextBox();
      this._lblKeyItem = new System.Windows.Forms.Label();
      this._lblKey = new System.Windows.Forms.Label();
      this._splitContainerEngineSettingsMain.Panel1.SuspendLayout();
      this._splitContainerEngineSettingsMain.Panel2.SuspendLayout();
      this._splitContainerEngineSettingsMain.SuspendLayout();
      this._tableEngineSettingsList.SuspendLayout();
      this._gbEngineRuleConfiguration.SuspendLayout();
      this.SuspendLayout();
      // 
      // _splitContainerEngineSettingsMain
      // 
      this._splitContainerEngineSettingsMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this._splitContainerEngineSettingsMain.Location = new System.Drawing.Point(0, 0);
      this._splitContainerEngineSettingsMain.Name = "_splitContainerEngineSettingsMain";
      // 
      // _splitContainerEngineSettingsMain.Panel1
      // 
      this._splitContainerEngineSettingsMain.Panel1.Controls.Add(this._tableEngineSettingsList);
      // 
      // _splitContainerEngineSettingsMain.Panel2
      // 
      this._splitContainerEngineSettingsMain.Panel2.Controls.Add(this._gbEngineRuleConfiguration);
      this._splitContainerEngineSettingsMain.Size = new System.Drawing.Size(670, 403);
      this._splitContainerEngineSettingsMain.SplitterDistance = 222;
      this._splitContainerEngineSettingsMain.TabIndex = 1;
      // 
      // _tableEngineSettingsList
      // 
      this._tableEngineSettingsList.ColumnCount = 4;
      this._tableEngineSettingsList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this._tableEngineSettingsList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this._tableEngineSettingsList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this._tableEngineSettingsList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this._tableEngineSettingsList.Controls.Add(this._btnDeleteRule, 3, 1);
      this._tableEngineSettingsList.Controls.Add(this._btnNewRule, 2, 1);
      this._tableEngineSettingsList.Controls.Add(this._btnRuleDown, 1, 1);
      this._tableEngineSettingsList.Controls.Add(this._listRules, 0, 0);
      this._tableEngineSettingsList.Controls.Add(this._btnRuleUp, 0, 1);
      this._tableEngineSettingsList.Dock = System.Windows.Forms.DockStyle.Fill;
      this._tableEngineSettingsList.Location = new System.Drawing.Point(0, 0);
      this._tableEngineSettingsList.Name = "_tableEngineSettingsList";
      this._tableEngineSettingsList.RowCount = 2;
      this._tableEngineSettingsList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.68766F));
      this._tableEngineSettingsList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.312343F));
      this._tableEngineSettingsList.Size = new System.Drawing.Size(222, 403);
      this._tableEngineSettingsList.TabIndex = 0;
      // 
      // _btnDeleteRule
      // 
      this._btnDeleteRule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._btnDeleteRule.Location = new System.Drawing.Point(168, 372);
      this._btnDeleteRule.Name = "_btnDeleteRule";
      this._btnDeleteRule.Size = new System.Drawing.Size(51, 28);
      this._btnDeleteRule.TabIndex = 4;
      this._btnDeleteRule.Text = "Delete";
      this._btnDeleteRule.UseVisualStyleBackColor = true;
      this._btnDeleteRule.Click += new System.EventHandler(this.DeleteRule);
      // 
      // _btnNewRule
      // 
      this._btnNewRule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._btnNewRule.Location = new System.Drawing.Point(113, 372);
      this._btnNewRule.Name = "_btnNewRule";
      this._btnNewRule.Size = new System.Drawing.Size(49, 28);
      this._btnNewRule.TabIndex = 3;
      this._btnNewRule.Text = "New";
      this._btnNewRule.UseVisualStyleBackColor = true;
      this._btnNewRule.Click += new System.EventHandler(this.NewRule);
      // 
      // _btnRuleDown
      // 
      this._btnRuleDown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._btnRuleDown.Location = new System.Drawing.Point(58, 372);
      this._btnRuleDown.Name = "_btnRuleDown";
      this._btnRuleDown.Size = new System.Drawing.Size(49, 28);
      this._btnRuleDown.TabIndex = 2;
      this._btnRuleDown.Text = "Down";
      this._btnRuleDown.UseVisualStyleBackColor = true;
      this._btnRuleDown.Click += new System.EventHandler(this.RuleDown);
      // 
      // _tableEngineSettingsList
      // 
      this._tableEngineSettingsList.SetColumnSpan(this._listRules, 4);
      //
      // _listRules
      //
      this._listRules.Dock = System.Windows.Forms.DockStyle.Fill;
      this._listRules.FormattingEnabled = true;
      this._listRules.Location = new System.Drawing.Point(3, 3);
      this._listRules.Name = "_listRules";
      this._listRules.Size = new System.Drawing.Size(216, 363);
      this._listRules.TabIndex = 0;
      this._listRules.SelectedIndexChanged += new System.EventHandler(this.SelectedRuleChanged);
      // 
      // _btnRuleUp
      // 
      this._btnRuleUp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this._btnRuleUp.Location = new System.Drawing.Point(3, 372);
      this._btnRuleUp.Name = "_btnRuleUp";
      this._btnRuleUp.Size = new System.Drawing.Size(49, 28);
      this._btnRuleUp.TabIndex = 1;
      this._btnRuleUp.Text = "Up";
      this._btnRuleUp.UseVisualStyleBackColor = true;
      this._btnRuleUp.Click += new System.EventHandler(this.RuleUp);
      // 
      // _gbEngineRuleConfiguration
      // 
      this._gbEngineRuleConfiguration.Controls.Add(this._lblRuleSelector);
      this._gbEngineRuleConfiguration.Controls.Add(this._cmbRuleSelector);
      this._gbEngineRuleConfiguration.Controls.Add(this._btnApplyRule);
      this._gbEngineRuleConfiguration.Controls.Add(this._txtKeyItem);
      this._gbEngineRuleConfiguration.Controls.Add(this._txtKey);
      this._gbEngineRuleConfiguration.Controls.Add(this._lblKeyItem);
      this._gbEngineRuleConfiguration.Controls.Add(this._lblKey);
      this._gbEngineRuleConfiguration.Location = new System.Drawing.Point(3, 3);
      this._gbEngineRuleConfiguration.Name = "_gbEngineRuleConfiguration";
      this._gbEngineRuleConfiguration.Size = new System.Drawing.Size(433, 149);
      this._gbEngineRuleConfiguration.TabIndex = 0;
      this._gbEngineRuleConfiguration.TabStop = false;
      this._gbEngineRuleConfiguration.Text = Title + " Engine Rule Configuration";
      // 
      // _lblRuleSelector
      // 
      this._lblRuleSelector.AutoSize = true;
      this._lblRuleSelector.Location = new System.Drawing.Point(5, 86);
      this._lblRuleSelector.Name = "_lblRuleSelector";
      this._lblRuleSelector.Size = new System.Drawing.Size(93, 13);
      this._lblRuleSelector.TabIndex = 13;
      this._lblRuleSelector.Text = RuleSelectorText;
      // 
      // _cmbRuleSelector
      // 
      this._cmbRuleSelector.FormattingEnabled = true;
      this._cmbRuleSelector.Location = new System.Drawing.Point(104, 83);
      this._cmbRuleSelector.Name = "_cmbRuleSelector";
      this._cmbRuleSelector.Size = new System.Drawing.Size(323, 21);
      this._cmbRuleSelector.TabIndex = 12;
      // 
      // _btnApplyRule
      // 
      this._btnApplyRule.Location = new System.Drawing.Point(322, 110);
      this._btnApplyRule.Name = "_btnApplyRule";
      this._btnApplyRule.Size = new System.Drawing.Size(105, 23);
      this._btnApplyRule.TabIndex = 11;
      this._btnApplyRule.Text = "Apply Changes";
      this._btnApplyRule.UseVisualStyleBackColor = true;
      this._btnApplyRule.Click += new System.EventHandler(this.ApplySelectedRule);
      // 
      // _txtKey
      // 
      this._txtKeyItem.Location = new System.Drawing.Point(104, 56);
      this._txtKeyItem.Name = "_txtKey";
      this._txtKeyItem.Size = new System.Drawing.Size(323, 20);
      this._txtKeyItem.TabIndex = 10;
      // 
      // _txtKeyItem
      // 
      this._txtKey.Location = new System.Drawing.Point(104, 27);
      this._txtKey.Name = "_txtKeyItem";
      this._txtKey.Size = new System.Drawing.Size(323, 20);
      this._txtKey.TabIndex = 9;
      // 
      // _lblKeyItem
      // 
      this._lblKeyItem.AutoSize = true;
      this._lblKeyItem.Location = new System.Drawing.Point(5, 59);
      this._lblKeyItem.Name = "_lblKeyItem";
      this._lblKeyItem.Size = new System.Drawing.Size(54, 13);
      this._lblKeyItem.TabIndex = 8;
      this._lblKeyItem.Text = KeyItemText;
      // 
      // _lblKey
      // 
      this._lblKey.AutoSize = true;
      this._lblKey.Location = new System.Drawing.Point(5, 30);
      this._lblKey.Name = "_lblKey";
      this._lblKey.Size = new System.Drawing.Size(61, 13);
      this._lblKey.TabIndex = 7;
      this._lblKey.Text = KeyText;
      // 
      // FileSystemEngineSettingsPageContent
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._splitContainerEngineSettingsMain);
      this.Name = "FileSystemEngineSettingsPageContent";
      this.Size = new System.Drawing.Size(670, 403);
      this._splitContainerEngineSettingsMain.Panel1.ResumeLayout(false);
      this._splitContainerEngineSettingsMain.Panel2.ResumeLayout(false);
      this._splitContainerEngineSettingsMain.ResumeLayout(false);
      this._tableEngineSettingsList.ResumeLayout(false);
      this._gbEngineRuleConfiguration.ResumeLayout(false);
      this._gbEngineRuleConfiguration.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer _splitContainerEngineSettingsMain;
    private System.Windows.Forms.TableLayoutPanel _tableEngineSettingsList;
    private System.Windows.Forms.Button _btnDeleteRule;
    private System.Windows.Forms.Button _btnNewRule;
    private System.Windows.Forms.Button _btnRuleDown;
    private System.Windows.Forms.Button _btnRuleUp;
    private System.Windows.Forms.ListBox _listRules;
    private System.Windows.Forms.GroupBox _gbEngineRuleConfiguration;
    private System.Windows.Forms.Button _btnApplyRule;
    private System.Windows.Forms.TextBox _txtKey;
    private System.Windows.Forms.TextBox _txtKeyItem;
    private System.Windows.Forms.Label _lblKey;
    private System.Windows.Forms.Label _lblKeyItem;
    private System.Windows.Forms.Label _lblRuleSelector;
    private System.Windows.Forms.ComboBox _cmbRuleSelector;
  }
}
