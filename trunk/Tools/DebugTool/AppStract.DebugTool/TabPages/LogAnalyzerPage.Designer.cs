namespace AppStract.DebugTool.TabPages
{
  partial class LogAnalyzerPage
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
      this._pnlTop = new System.Windows.Forms.Panel();
      this._toolStripLog = new System.Windows.Forms.ToolStrip();
      this._btnOpen = new System.Windows.Forms.ToolStripButton();
      this._btnSave = new System.Windows.Forms.ToolStripButton();
      this._seperator1 = new System.Windows.Forms.ToolStripSeparator();
      this._btnGoBackRelated = new System.Windows.Forms.ToolStripButton();
      this._btnGoParentStatement = new System.Windows.Forms.ToolStripButton();
      this._btnGoChildStatement = new System.Windows.Forms.ToolStripButton();
      this._btnGoNextRelated = new System.Windows.Forms.ToolStripButton();
      this._btnFind = new System.Windows.Forms.ToolStripButton();
      this._txtFind = new System.Windows.Forms.ToolStripTextBox();
      this._seperator2 = new System.Windows.Forms.ToolStripSeparator();
      this._btnHandlesToNames = new System.Windows.Forms.ToolStripButton();
      this._pnlContent = new System.Windows.Forms.Panel();
      this._txtContent = new AppStract.DebugTool.Controls.AdvancedRichTextBox();
      this._pnlTop.SuspendLayout();
      this._toolStripLog.SuspendLayout();
      this._pnlContent.SuspendLayout();
      this.SuspendLayout();
      // 
      // _pnlTop
      // 
      this._pnlTop.Controls.Add(this._toolStripLog);
      this._pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
      this._pnlTop.Location = new System.Drawing.Point(0, 0);
      this._pnlTop.Name = "_pnlTop";
      this._pnlTop.Size = new System.Drawing.Size(850, 23);
      this._pnlTop.TabIndex = 0;
      // 
      // _toolStripLog
      // 
      this._toolStripLog.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btnOpen,
            this._btnSave,
            this._seperator1,
            this._btnGoBackRelated,
            this._btnGoParentStatement,
            this._btnGoChildStatement,
            this._btnGoNextRelated,
            this._btnFind,
            this._txtFind,
            this._seperator2,
            this._btnHandlesToNames});
      this._toolStripLog.Location = new System.Drawing.Point(0, 0);
      this._toolStripLog.Name = "_toolStripLog";
      this._toolStripLog.Size = new System.Drawing.Size(850, 25);
      this._toolStripLog.TabIndex = 0;
      this._toolStripLog.Text = "Log Analyzer Toolstrip";
      // 
      // _btnOpen
      // 
      this._btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this._btnOpen.Image = global::AppStract.DebugTool.Properties.Resources.open;
      this._btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._btnOpen.Name = "_btnOpen";
      this._btnOpen.Size = new System.Drawing.Size(23, 22);
      this._btnOpen.Text = "Open";
      this._btnOpen.Click += new System.EventHandler(this._btnOpen_Click);
      // 
      // _btnSave
      // 
      this._btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this._btnSave.Image = global::AppStract.DebugTool.Properties.Resources.save;
      this._btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._btnSave.Name = "_btnSave";
      this._btnSave.Size = new System.Drawing.Size(23, 22);
      this._btnSave.Text = "Save";
      this._btnSave.ToolTipText = "Save";
      this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
      // 
      // _seperator1
      // 
      this._seperator1.Name = "_seperator1";
      this._seperator1.Size = new System.Drawing.Size(6, 25);
      // 
      // _btnGoBackRelated
      // 
      this._btnGoBackRelated.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this._btnGoBackRelated.Image = global::AppStract.DebugTool.Properties.Resources.skip_backward;
      this._btnGoBackRelated.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._btnGoBackRelated.Name = "_btnGoBackRelated";
      this._btnGoBackRelated.Size = new System.Drawing.Size(23, 22);
      this._btnGoBackRelated.Text = "Go to previous related statement";
      // 
      // _btnGoParentStatement
      // 
      this._btnGoParentStatement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this._btnGoParentStatement.Image = global::AppStract.DebugTool.Properties.Resources.rewind;
      this._btnGoParentStatement.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._btnGoParentStatement.Name = "_btnGoParentStatement";
      this._btnGoParentStatement.Size = new System.Drawing.Size(23, 22);
      this._btnGoParentStatement.Text = "Go to parent statement";
      // 
      // _btnGoChildStatement
      // 
      this._btnGoChildStatement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this._btnGoChildStatement.Image = global::AppStract.DebugTool.Properties.Resources.fast_forward;
      this._btnGoChildStatement.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._btnGoChildStatement.Name = "_btnGoChildStatement";
      this._btnGoChildStatement.Size = new System.Drawing.Size(23, 22);
      this._btnGoChildStatement.Text = "Go to first child statement";
      // 
      // _btnGoNextRelated
      // 
      this._btnGoNextRelated.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this._btnGoNextRelated.Image = global::AppStract.DebugTool.Properties.Resources.skip_forward;
      this._btnGoNextRelated.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._btnGoNextRelated.Name = "_btnGoNextRelated";
      this._btnGoNextRelated.Size = new System.Drawing.Size(23, 22);
      this._btnGoNextRelated.Text = "Go to next related statement";
      // 
      // _btnFind
      // 
      this._btnFind.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this._btnFind.BackColor = System.Drawing.SystemColors.Window;
      this._btnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this._btnFind.Image = global::AppStract.DebugTool.Properties.Resources.search;
      this._btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._btnFind.Name = "_btnFind";
      this._btnFind.Size = new System.Drawing.Size(23, 22);
      this._btnFind.Text = "Find";
      this._btnFind.ToolTipText = "Find";
      // 
      // _txtFind
      // 
      this._txtFind.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this._txtFind.Name = "_txtFind";
      this._txtFind.Size = new System.Drawing.Size(150, 25);
      this._txtFind.ToolTipText = "Find";
      // 
      // _seperator2
      // 
      this._seperator2.Name = "_seperator2";
      this._seperator2.Size = new System.Drawing.Size(6, 25);
      // 
      // _btnHandlesToNames
      // 
      this._btnHandlesToNames.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this._btnHandlesToNames.Image = global::AppStract.DebugTool.Properties.Resources.shuffle_off;
      this._btnHandlesToNames.ImageTransparentColor = System.Drawing.Color.Magenta;
      this._btnHandlesToNames.Name = "_btnHandlesToNames";
      this._btnHandlesToNames.Size = new System.Drawing.Size(23, 22);
      this._btnHandlesToNames.Text = "Replace handles by associated names";
      this._btnHandlesToNames.ToolTipText = "Replace handles by associated names";
      this._btnHandlesToNames.Click += new System.EventHandler(this._btnHandlesToNames_Click);
      // 
      // _pnlContent
      // 
      this._pnlContent.Controls.Add(this._txtContent);
      this._pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
      this._pnlContent.Location = new System.Drawing.Point(0, 23);
      this._pnlContent.Name = "_pnlContent";
      this._pnlContent.Size = new System.Drawing.Size(850, 491);
      this._pnlContent.TabIndex = 2;
      // 
      // _txtContent
      // 
      this._txtContent.DisplayLineNumbers = true;
      this._txtContent.Dock = System.Windows.Forms.DockStyle.Fill;
      this._txtContent.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this._txtContent.Lines = new string[0];
      this._txtContent.Location = new System.Drawing.Point(0, 0);
      this._txtContent.Name = "_txtContent";
      this._txtContent.Size = new System.Drawing.Size(850, 491);
      this._txtContent.TabIndex = 0;
      this._txtContent.MouseUp += new AppStract.DebugTool.Controls.LineChangedEventHandler(this._txtContent_MouseClick);
      // 
      // LogAnalyzerPage
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this._pnlContent);
      this.Controls.Add(this._pnlTop);
      this.ForeColor = System.Drawing.SystemColors.ControlText;
      this.Name = "LogAnalyzerPage";
      this.Size = new System.Drawing.Size(850, 514);
      this._pnlTop.ResumeLayout(false);
      this._pnlTop.PerformLayout();
      this._toolStripLog.ResumeLayout(false);
      this._toolStripLog.PerformLayout();
      this._pnlContent.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel _pnlTop;
    private System.Windows.Forms.Panel _pnlContent;
    private AppStract.DebugTool.Controls.AdvancedRichTextBox _txtContent;
    private System.Windows.Forms.ToolStrip _toolStripLog;
    private System.Windows.Forms.ToolStripButton _btnOpen;
    private System.Windows.Forms.ToolStripButton _btnSave;
    private System.Windows.Forms.ToolStripSeparator _seperator1;
    private System.Windows.Forms.ToolStripButton _btnGoBackRelated;
    private System.Windows.Forms.ToolStripButton _btnGoParentStatement;
    private System.Windows.Forms.ToolStripButton _btnGoChildStatement;
    private System.Windows.Forms.ToolStripButton _btnGoNextRelated;
    private System.Windows.Forms.ToolStripButton _btnFind;
    private System.Windows.Forms.ToolStripTextBox _txtFind;
    private System.Windows.Forms.ToolStripSeparator _seperator2;
    private System.Windows.Forms.ToolStripButton _btnHandlesToNames;
  }
}
