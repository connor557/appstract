namespace HelloVirtualWorld.TabPages
{
  partial class RegistryTab
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
      this._btnReadRegistryValue = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // _btnReadRegistryValue
      // 
      this._btnReadRegistryValue.Location = new System.Drawing.Point(26, 31);
      this._btnReadRegistryValue.Name = "_btnReadRegistryValue";
      this._btnReadRegistryValue.Size = new System.Drawing.Size(158, 23);
      this._btnReadRegistryValue.TabIndex = 1;
      this._btnReadRegistryValue.Text = "Read Registry Value";
      this._btnReadRegistryValue.UseVisualStyleBackColor = true;
      this._btnReadRegistryValue.Click += new System.EventHandler(this.ReadRegistryValue);
      // 
      // TabRegistry
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this._btnReadRegistryValue);
      this.Name = "TabRegistry";
      this.Size = new System.Drawing.Size(359, 249);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button _btnReadRegistryValue;

  }
}
