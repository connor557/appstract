namespace HelloVirtualWorld.TabPages
{
  partial class FileSystemTab
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
      this._btnCreateFile = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // _btnCreateFile
      // 
      this._btnCreateFile.Location = new System.Drawing.Point(26, 31);
      this._btnCreateFile.Name = "_btnCreateFile";
      this._btnCreateFile.Size = new System.Drawing.Size(158, 23);
      this._btnCreateFile.TabIndex = 0;
      this._btnCreateFile.Text = "Create File";
      this._btnCreateFile.UseVisualStyleBackColor = true;
      this._btnCreateFile.Click += new System.EventHandler(this.CreateFile);
      // 
      // FileSystemTab
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._btnCreateFile);
      this.Name = "FileSystemTab";
      this.Size = new System.Drawing.Size(344, 262);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button _btnCreateFile;
  }
}
