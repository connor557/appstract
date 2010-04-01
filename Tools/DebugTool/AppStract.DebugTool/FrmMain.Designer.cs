namespace AppStract.DebugTool
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
      this._tabMain = new System.Windows.Forms.TabControl();
      this._tapDatabaseAnalyzer = new System.Windows.Forms.TabPage();
      this._tabDiagnostics = new System.Windows.Forms.TabPage();
      this._tabSystemRestore = new System.Windows.Forms.TabPage();
      this._tabLogAnalyzer = new System.Windows.Forms.TabPage();
      this._tabMain.SuspendLayout();
      this.SuspendLayout();
      // 
      // _tabMain
      // 
      this._tabMain.Alignment = System.Windows.Forms.TabAlignment.Bottom;
      this._tabMain.Controls.Add(this._tabLogAnalyzer);
      this._tabMain.Controls.Add(this._tapDatabaseAnalyzer);
      this._tabMain.Controls.Add(this._tabDiagnostics);
      this._tabMain.Controls.Add(this._tabSystemRestore);
      this._tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
      this._tabMain.Location = new System.Drawing.Point(0, 0);
      this._tabMain.Multiline = true;
      this._tabMain.Name = "_tabMain";
      this._tabMain.SelectedIndex = 0;
      this._tabMain.Size = new System.Drawing.Size(867, 579);
      this._tabMain.TabIndex = 0;
      this._tabMain.SelectedIndexChanged += new System.EventHandler(this.SelectedTabChanged);
      // 
      // _tapDatabaseAnalyzer
      // 
      this._tapDatabaseAnalyzer.Location = new System.Drawing.Point(4, 4);
      this._tapDatabaseAnalyzer.Name = "_tapDatabaseAnalyzer";
      this._tapDatabaseAnalyzer.Padding = new System.Windows.Forms.Padding(3);
      this._tapDatabaseAnalyzer.Size = new System.Drawing.Size(859, 553);
      this._tapDatabaseAnalyzer.TabIndex = 1;
      this._tapDatabaseAnalyzer.Text = "Database Analyzer";
      this._tapDatabaseAnalyzer.UseVisualStyleBackColor = true;
      // 
      // _tabDiagnostics
      // 
      this._tabDiagnostics.Location = new System.Drawing.Point(4, 4);
      this._tabDiagnostics.Name = "_tabDiagnostics";
      this._tabDiagnostics.Size = new System.Drawing.Size(859, 553);
      this._tabDiagnostics.TabIndex = 2;
      this._tabDiagnostics.Text = "Process Diagnostics";
      this._tabDiagnostics.UseVisualStyleBackColor = true;
      // 
      // _tabSystemRestore
      // 
      this._tabSystemRestore.Location = new System.Drawing.Point(4, 4);
      this._tabSystemRestore.Name = "_tabSystemRestore";
      this._tabSystemRestore.Size = new System.Drawing.Size(859, 553);
      this._tabSystemRestore.TabIndex = 3;
      this._tabSystemRestore.Text = "System Restore";
      this._tabSystemRestore.UseVisualStyleBackColor = true;
      // 
      // _tabLogAnalyzer
      // 
      this._tabLogAnalyzer.Location = new System.Drawing.Point(4, 4);
      this._tabLogAnalyzer.Name = "_tabLogAnalyzer";
      this._tabLogAnalyzer.Padding = new System.Windows.Forms.Padding(3);
      this._tabLogAnalyzer.Size = new System.Drawing.Size(859, 553);
      this._tabLogAnalyzer.TabIndex = 4;
      this._tabLogAnalyzer.Text = "Log Analyzer";
      this._tabLogAnalyzer.UseVisualStyleBackColor = true;
      // 
      // FrmMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(867, 579);
      this.Controls.Add(this._tabMain);
      this.Name = "FrmMain";
      this.Text = "AppStract Debug Tools";
      this._tabMain.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl _tabMain;
    private System.Windows.Forms.TabPage _tapDatabaseAnalyzer;
    private System.Windows.Forms.TabPage _tabDiagnostics;
    private System.Windows.Forms.TabPage _tabSystemRestore;
    private System.Windows.Forms.TabPage _tabLogAnalyzer;

  }
}

