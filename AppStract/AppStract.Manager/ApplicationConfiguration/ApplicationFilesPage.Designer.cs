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

namespace AppStract.Manager.ApplicationConfiguration
{
  partial class ApplicationFilesPage
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
      this._lblFileSystemRoot = new System.Windows.Forms.Label();
      this._lblFileSystemDatabase = new System.Windows.Forms.Label();
      this._txtFileSystemRootDirectory = new System.Windows.Forms.TextBox();
      this._txtFileSystemDatabase = new System.Windows.Forms.TextBox();
      this._txtRegistryDatabase = new System.Windows.Forms.TextBox();
      this._lblRegistryDatabase = new System.Windows.Forms.Label();
      this._txtExecutable = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // _lblFileSystemRoot
      // 
      this._lblFileSystemRoot.AutoSize = true;
      this._lblFileSystemRoot.Location = new System.Drawing.Point(17, 21);
      this._lblFileSystemRoot.Name = "_lblFileSystemRoot";
      this._lblFileSystemRoot.Size = new System.Drawing.Size(128, 13);
      this._lblFileSystemRoot.TabIndex = 0;
      this._lblFileSystemRoot.Text = "FileSystem Root Directory";
      // 
      // _lblFileSystemDatabase
      // 
      this._lblFileSystemDatabase.AutoSize = true;
      this._lblFileSystemDatabase.Location = new System.Drawing.Point(17, 47);
      this._lblFileSystemDatabase.Name = "_lblFileSystemDatabase";
      this._lblFileSystemDatabase.Size = new System.Drawing.Size(106, 13);
      this._lblFileSystemDatabase.TabIndex = 1;
      this._lblFileSystemDatabase.Text = "FileSystem Database";
      // 
      // _txtFileSystemRootDirectory
      // 
      this._txtFileSystemRootDirectory.Location = new System.Drawing.Point(151, 18);
      this._txtFileSystemRootDirectory.Name = "_txtFileSystemRootDirectory";
      this._txtFileSystemRootDirectory.Size = new System.Drawing.Size(340, 20);
      this._txtFileSystemRootDirectory.TabIndex = 2;
      this._txtFileSystemRootDirectory.TextChanged += new System.EventHandler(this._txt_TextChanged);
      // 
      // _txtFileSystemDatabase
      // 
      this._txtFileSystemDatabase.Location = new System.Drawing.Point(151, 44);
      this._txtFileSystemDatabase.Name = "_txtFileSystemDatabase";
      this._txtFileSystemDatabase.Size = new System.Drawing.Size(340, 20);
      this._txtFileSystemDatabase.TabIndex = 3;
      this._txtFileSystemDatabase.TextChanged += new System.EventHandler(this._txt_TextChanged);
      // 
      // _txtRegistryDatabase
      // 
      this._txtRegistryDatabase.Location = new System.Drawing.Point(151, 70);
      this._txtRegistryDatabase.Name = "_txtRegistryDatabase";
      this._txtRegistryDatabase.Size = new System.Drawing.Size(340, 20);
      this._txtRegistryDatabase.TabIndex = 5;
      this._txtRegistryDatabase.TextChanged += new System.EventHandler(this._txt_TextChanged);
      // 
      // _lblRegistryDatabase
      // 
      this._lblRegistryDatabase.AutoSize = true;
      this._lblRegistryDatabase.Location = new System.Drawing.Point(17, 73);
      this._lblRegistryDatabase.Name = "_lblRegistryDatabase";
      this._lblRegistryDatabase.Size = new System.Drawing.Size(94, 13);
      this._lblRegistryDatabase.TabIndex = 4;
      this._lblRegistryDatabase.Text = "Registry Database";
      // 
      // _txtExecutable
      // 
      this._txtExecutable.Location = new System.Drawing.Point(151, 96);
      this._txtExecutable.Name = "_txtExecutable";
      this._txtExecutable.Size = new System.Drawing.Size(340, 20);
      this._txtExecutable.TabIndex = 7;
      this._txtExecutable.TextChanged += new System.EventHandler(this._txt_TextChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(17, 99);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(101, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Process Executable";
      // 
      // ApplicationFilesPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._txtExecutable);
      this.Controls.Add(this.label1);
      this.Controls.Add(this._txtRegistryDatabase);
      this.Controls.Add(this._lblRegistryDatabase);
      this.Controls.Add(this._txtFileSystemDatabase);
      this.Controls.Add(this._txtFileSystemRootDirectory);
      this.Controls.Add(this._lblFileSystemDatabase);
      this.Controls.Add(this._lblFileSystemRoot);
      this.Name = "ApplicationFilesPage";
      this.Size = new System.Drawing.Size(661, 355);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label _lblFileSystemRoot;
    private System.Windows.Forms.Label _lblFileSystemDatabase;
    private System.Windows.Forms.TextBox _txtFileSystemRootDirectory;
    private System.Windows.Forms.TextBox _txtFileSystemDatabase;
    private System.Windows.Forms.TextBox _txtRegistryDatabase;
    private System.Windows.Forms.Label _lblRegistryDatabase;
    private System.Windows.Forms.TextBox _txtExecutable;
    private System.Windows.Forms.Label label1;

  }
}
