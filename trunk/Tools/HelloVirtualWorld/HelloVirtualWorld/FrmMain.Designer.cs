namespace HelloVirtualWorld
{
  partial class FrmMain
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
      this._tabControl = new System.Windows.Forms.TabControl();
      this._tabFiles = new System.Windows.Forms.TabPage();
      this._tabRegistry = new System.Windows.Forms.TabPage();
      this._tabControl.SuspendLayout();
      this.SuspendLayout();
      // 
      // _tabControl
      // 
      this._tabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
      this._tabControl.Controls.Add(this._tabFiles);
      this._tabControl.Controls.Add(this._tabRegistry);
      this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this._tabControl.Location = new System.Drawing.Point(0, 0);
      this._tabControl.Multiline = true;
      this._tabControl.Name = "_tabControl";
      this._tabControl.SelectedIndex = 0;
      this._tabControl.Size = new System.Drawing.Size(252, 127);
      this._tabControl.TabIndex = 0;
      // 
      // _tabFiles
      // 
      this._tabFiles.Location = new System.Drawing.Point(23, 4);
      this._tabFiles.Name = "_tabFiles";
      this._tabFiles.Padding = new System.Windows.Forms.Padding(3);
      this._tabFiles.Size = new System.Drawing.Size(225, 119);
      this._tabFiles.TabIndex = 0;
      this._tabFiles.Text = "FileSystem";
      this._tabFiles.UseVisualStyleBackColor = true;
      // 
      // _tabRegistry
      // 
      this._tabRegistry.Location = new System.Drawing.Point(23, 4);
      this._tabRegistry.Name = "_tabRegistry";
      this._tabRegistry.Padding = new System.Windows.Forms.Padding(3);
      this._tabRegistry.Size = new System.Drawing.Size(696, 486);
      this._tabRegistry.TabIndex = 1;
      this._tabRegistry.Text = "Registry";
      this._tabRegistry.UseVisualStyleBackColor = true;
      // 
      // FrmMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(252, 127);
      this.Controls.Add(this._tabControl);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FrmMain";
      this.Text = "Hello Virtual World!";
      this._tabControl.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl _tabControl;
    private System.Windows.Forms.TabPage _tabFiles;
    private System.Windows.Forms.TabPage _tabRegistry;

  }
}

