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
  partial class EngineSettingsPage
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
      this._tabEngineSettingsMain = new System.Windows.Forms.TabControl();
      this._tabEngineSettingsRegistry = new System.Windows.Forms.TabPage();
      this._splitContainerEngineSettingsMain = new System.Windows.Forms.SplitContainer();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this._btnEngineSettingsRegistryDelete = new System.Windows.Forms.Button();
      this._btnEngineSettingsRegistryNew = new System.Windows.Forms.Button();
      this._btnEngineSettingsRegistryDown = new System.Windows.Forms.Button();
      this._listEngineSettingsRegistry = new System.Windows.Forms.ListBox();
      this._btnEngineSettingsRegistryUp = new System.Windows.Forms.Button();
      this._gbRegistryEngineRuleConfiguration = new System.Windows.Forms.GroupBox();
      this.label3 = new System.Windows.Forms.Label();
      this._cmbRegistryRuleVirtualizationType = new System.Windows.Forms.ComboBox();
      this._btnRegistryEngineRuleConfigurationApply = new System.Windows.Forms.Button();
      this._txtRegistryRuleValueName = new System.Windows.Forms.TextBox();
      this._txtRegistryRuleKeyName = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this._tabEngineSettingsFileSystem = new System.Windows.Forms.TabPage();
      this._tabEngineSettingsMain.SuspendLayout();
      this._tabEngineSettingsRegistry.SuspendLayout();
      this._splitContainerEngineSettingsMain.Panel1.SuspendLayout();
      this._splitContainerEngineSettingsMain.Panel2.SuspendLayout();
      this._splitContainerEngineSettingsMain.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this._gbRegistryEngineRuleConfiguration.SuspendLayout();
      this.SuspendLayout();
      // 
      // _tabEngineSettingsMain
      // 
      this._tabEngineSettingsMain.Controls.Add(this._tabEngineSettingsRegistry);
      this._tabEngineSettingsMain.Controls.Add(this._tabEngineSettingsFileSystem);
      this._tabEngineSettingsMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this._tabEngineSettingsMain.Location = new System.Drawing.Point(0, 0);
      this._tabEngineSettingsMain.Name = "_tabEngineSettingsMain";
      this._tabEngineSettingsMain.SelectedIndex = 0;
      this._tabEngineSettingsMain.Size = new System.Drawing.Size(678, 429);
      this._tabEngineSettingsMain.TabIndex = 0;
      // 
      // _tabEngineSettingsRegistry
      // 
      this._tabEngineSettingsRegistry.Controls.Add(this._splitContainerEngineSettingsMain);
      this._tabEngineSettingsRegistry.Location = new System.Drawing.Point(4, 22);
      this._tabEngineSettingsRegistry.Name = "_tabEngineSettingsRegistry";
      this._tabEngineSettingsRegistry.Padding = new System.Windows.Forms.Padding(3);
      this._tabEngineSettingsRegistry.Size = new System.Drawing.Size(670, 403);
      this._tabEngineSettingsRegistry.TabIndex = 0;
      this._tabEngineSettingsRegistry.Text = "Registry Engine";
      this._tabEngineSettingsRegistry.UseVisualStyleBackColor = true;
      // 
      // _splitContainerEngineSettingsMain
      // 
      this._splitContainerEngineSettingsMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this._splitContainerEngineSettingsMain.Location = new System.Drawing.Point(3, 3);
      this._splitContainerEngineSettingsMain.Name = "_splitContainerEngineSettingsMain";
      // 
      // _splitContainerEngineSettingsMain.Panel1
      // 
      this._splitContainerEngineSettingsMain.Panel1.Controls.Add(this.tableLayoutPanel1);
      // 
      // _splitContainerEngineSettingsMain.Panel2
      // 
      this._splitContainerEngineSettingsMain.Panel2.Controls.Add(this._gbRegistryEngineRuleConfiguration);
      this._splitContainerEngineSettingsMain.Size = new System.Drawing.Size(664, 397);
      this._splitContainerEngineSettingsMain.SplitterDistance = 221;
      this._splitContainerEngineSettingsMain.TabIndex = 0;
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 4;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      this.tableLayoutPanel1.Controls.Add(this._btnEngineSettingsRegistryDelete, 3, 1);
      this.tableLayoutPanel1.Controls.Add(this._btnEngineSettingsRegistryNew, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this._btnEngineSettingsRegistryDown, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this._listEngineSettingsRegistry, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this._btnEngineSettingsRegistryUp, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.68766F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.312343F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(221, 397);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // _btnEngineSettingsRegistryDelete
      // 
      this._btnEngineSettingsRegistryDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                                            | System.Windows.Forms.AnchorStyles.Left)
                                                                                           | System.Windows.Forms.AnchorStyles.Right)));
      this._btnEngineSettingsRegistryDelete.Location = new System.Drawing.Point(168, 367);
      this._btnEngineSettingsRegistryDelete.Name = "_btnEngineSettingsRegistryDelete";
      this._btnEngineSettingsRegistryDelete.Size = new System.Drawing.Size(50, 27);
      this._btnEngineSettingsRegistryDelete.TabIndex = 4;
      this._btnEngineSettingsRegistryDelete.Text = "Delete";
      this._btnEngineSettingsRegistryDelete.UseVisualStyleBackColor = true;
      this._btnEngineSettingsRegistryDelete.Click += new System.EventHandler(this._btnEngineSettingsRegistryDelete_Click);
      // 
      // _btnEngineSettingsRegistryNew
      // 
      this._btnEngineSettingsRegistryNew.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                                         | System.Windows.Forms.AnchorStyles.Left)
                                                                                        | System.Windows.Forms.AnchorStyles.Right)));
      this._btnEngineSettingsRegistryNew.Location = new System.Drawing.Point(113, 367);
      this._btnEngineSettingsRegistryNew.Name = "_btnEngineSettingsRegistryNew";
      this._btnEngineSettingsRegistryNew.Size = new System.Drawing.Size(49, 27);
      this._btnEngineSettingsRegistryNew.TabIndex = 3;
      this._btnEngineSettingsRegistryNew.Text = "New";
      this._btnEngineSettingsRegistryNew.UseVisualStyleBackColor = true;
      this._btnEngineSettingsRegistryNew.Click += new System.EventHandler(this._btnEngineSettingsRegistryNew_Click);
      // 
      // _btnEngineSettingsRegistryDown
      // 
      this._btnEngineSettingsRegistryDown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                                          | System.Windows.Forms.AnchorStyles.Left)
                                                                                         | System.Windows.Forms.AnchorStyles.Right)));
      this._btnEngineSettingsRegistryDown.Location = new System.Drawing.Point(58, 367);
      this._btnEngineSettingsRegistryDown.Name = "_btnEngineSettingsRegistryDown";
      this._btnEngineSettingsRegistryDown.Size = new System.Drawing.Size(49, 27);
      this._btnEngineSettingsRegistryDown.TabIndex = 2;
      this._btnEngineSettingsRegistryDown.Text = "Down";
      this._btnEngineSettingsRegistryDown.UseVisualStyleBackColor = true;
      this._btnEngineSettingsRegistryDown.Click += new System.EventHandler(this._btnEngineSettingsRegistryDown_Click);
      // 
      // _listEngineSettingsRegistry
      // 
      this.tableLayoutPanel1.SetColumnSpan(this._listEngineSettingsRegistry, 4);
      this._listEngineSettingsRegistry.Dock = System.Windows.Forms.DockStyle.Fill;
      this._listEngineSettingsRegistry.FormattingEnabled = true;
      this._listEngineSettingsRegistry.Location = new System.Drawing.Point(3, 3);
      this._listEngineSettingsRegistry.Name = "_listEngineSettingsRegistry";
      this._listEngineSettingsRegistry.Size = new System.Drawing.Size(215, 355);
      this._listEngineSettingsRegistry.TabIndex = 0;
      this._listEngineSettingsRegistry.SelectedIndexChanged += new System.EventHandler(this._listEngineSettingsRegistry_SelectedIndexChanged);
      // 
      // _btnEngineSettingsRegistryUp
      // 
      this._btnEngineSettingsRegistryUp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                                        | System.Windows.Forms.AnchorStyles.Left)
                                                                                       | System.Windows.Forms.AnchorStyles.Right)));
      this._btnEngineSettingsRegistryUp.Location = new System.Drawing.Point(3, 367);
      this._btnEngineSettingsRegistryUp.Name = "_btnEngineSettingsRegistryUp";
      this._btnEngineSettingsRegistryUp.Size = new System.Drawing.Size(49, 27);
      this._btnEngineSettingsRegistryUp.TabIndex = 1;
      this._btnEngineSettingsRegistryUp.Text = "Up";
      this._btnEngineSettingsRegistryUp.UseVisualStyleBackColor = true;
      this._btnEngineSettingsRegistryUp.Click += new System.EventHandler(this._btnEngineSettingsRegistryUp_Click);
      // 
      // _gbRegistryEngineRuleConfiguration
      // 
      this._gbRegistryEngineRuleConfiguration.Controls.Add(this.label3);
      this._gbRegistryEngineRuleConfiguration.Controls.Add(this._cmbRegistryRuleVirtualizationType);
      this._gbRegistryEngineRuleConfiguration.Controls.Add(this._btnRegistryEngineRuleConfigurationApply);
      this._gbRegistryEngineRuleConfiguration.Controls.Add(this._txtRegistryRuleValueName);
      this._gbRegistryEngineRuleConfiguration.Controls.Add(this._txtRegistryRuleKeyName);
      this._gbRegistryEngineRuleConfiguration.Controls.Add(this.label2);
      this._gbRegistryEngineRuleConfiguration.Controls.Add(this.label1);
      this._gbRegistryEngineRuleConfiguration.Location = new System.Drawing.Point(3, 3);
      this._gbRegistryEngineRuleConfiguration.Name = "_gbRegistryEngineRuleConfiguration";
      this._gbRegistryEngineRuleConfiguration.Size = new System.Drawing.Size(433, 149);
      this._gbRegistryEngineRuleConfiguration.TabIndex = 0;
      this._gbRegistryEngineRuleConfiguration.TabStop = false;
      this._gbRegistryEngineRuleConfiguration.Text = "Registry Engine Rule - Configuration";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(5, 86);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(93, 13);
      this.label3.TabIndex = 13;
      this.label3.Text = "Virtualization Type";
      // 
      // _cmbRegistryRuleVirtualizationType
      // 
      this._cmbRegistryRuleVirtualizationType.FormattingEnabled = true;
      this._cmbRegistryRuleVirtualizationType.Location = new System.Drawing.Point(104, 83);
      this._cmbRegistryRuleVirtualizationType.Name = "_cmbRegistryRuleVirtualizationType";
      this._cmbRegistryRuleVirtualizationType.Size = new System.Drawing.Size(323, 21);
      this._cmbRegistryRuleVirtualizationType.TabIndex = 12;
      // 
      // _btnRegistryEngineRuleConfigurationApply
      // 
      this._btnRegistryEngineRuleConfigurationApply.Location = new System.Drawing.Point(322, 110);
      this._btnRegistryEngineRuleConfigurationApply.Name = "_btnRegistryEngineRuleConfigurationApply";
      this._btnRegistryEngineRuleConfigurationApply.Size = new System.Drawing.Size(105, 23);
      this._btnRegistryEngineRuleConfigurationApply.TabIndex = 11;
      this._btnRegistryEngineRuleConfigurationApply.Text = "Apply Changes";
      this._btnRegistryEngineRuleConfigurationApply.UseVisualStyleBackColor = true;
      this._btnRegistryEngineRuleConfigurationApply.Click += new System.EventHandler(this._btnRegistryEngineRuleConfigurationApply_Click);
      // 
      // _txtRegistryRuleValueName
      // 
      this._txtRegistryRuleValueName.Location = new System.Drawing.Point(104, 56);
      this._txtRegistryRuleValueName.Name = "_txtRegistryRuleValueName";
      this._txtRegistryRuleValueName.Size = new System.Drawing.Size(323, 20);
      this._txtRegistryRuleValueName.TabIndex = 10;
      // 
      // _txtRegistryRuleKeyName
      // 
      this._txtRegistryRuleKeyName.Location = new System.Drawing.Point(104, 27);
      this._txtRegistryRuleKeyName.Name = "_txtRegistryRuleKeyName";
      this._txtRegistryRuleKeyName.Size = new System.Drawing.Size(323, 20);
      this._txtRegistryRuleKeyName.TabIndex = 9;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(5, 59);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(65, 13);
      this.label2.TabIndex = 8;
      this.label2.Text = "Value Name";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(5, 30);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(66, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "Registry Key";
      // 
      // _tabEngineSettingsFileSystem
      // 
      this._tabEngineSettingsFileSystem.Location = new System.Drawing.Point(4, 22);
      this._tabEngineSettingsFileSystem.Name = "_tabEngineSettingsFileSystem";
      this._tabEngineSettingsFileSystem.Padding = new System.Windows.Forms.Padding(3);
      this._tabEngineSettingsFileSystem.Size = new System.Drawing.Size(670, 403);
      this._tabEngineSettingsFileSystem.TabIndex = 1;
      this._tabEngineSettingsFileSystem.Text = "FileSystemEngine";
      this._tabEngineSettingsFileSystem.UseVisualStyleBackColor = true;
      // 
      // EngineSettingsPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._tabEngineSettingsMain);
      this.Name = "EngineSettingsPage";
      this.Size = new System.Drawing.Size(678, 429);
      this._tabEngineSettingsMain.ResumeLayout(false);
      this._tabEngineSettingsRegistry.ResumeLayout(false);
      this._splitContainerEngineSettingsMain.Panel1.ResumeLayout(false);
      this._splitContainerEngineSettingsMain.Panel2.ResumeLayout(false);
      this._splitContainerEngineSettingsMain.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this._gbRegistryEngineRuleConfiguration.ResumeLayout(false);
      this._gbRegistryEngineRuleConfiguration.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl _tabEngineSettingsMain;
    private System.Windows.Forms.TabPage _tabEngineSettingsRegistry;
    private System.Windows.Forms.TabPage _tabEngineSettingsFileSystem;
    private System.Windows.Forms.SplitContainer _splitContainerEngineSettingsMain;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.ListBox _listEngineSettingsRegistry;
    private System.Windows.Forms.Button _btnEngineSettingsRegistryDelete;
    private System.Windows.Forms.Button _btnEngineSettingsRegistryNew;
    private System.Windows.Forms.Button _btnEngineSettingsRegistryDown;
    private System.Windows.Forms.Button _btnEngineSettingsRegistryUp;
    private System.Windows.Forms.GroupBox _gbRegistryEngineRuleConfiguration;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox _cmbRegistryRuleVirtualizationType;
    private System.Windows.Forms.Button _btnRegistryEngineRuleConfigurationApply;
    private System.Windows.Forms.TextBox _txtRegistryRuleValueName;
    private System.Windows.Forms.TextBox _txtRegistryRuleKeyName;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
  }
}