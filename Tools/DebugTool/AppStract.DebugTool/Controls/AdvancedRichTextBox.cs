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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace AppStract.DebugTool.Controls
{
  public class AdvancedRichTextBox : Control
  {

    #region Variables

    private RichTextBoxLineNumbers _lineNumbers;
    private RichTextBox _textBox;

    #endregion

    #region Variables - Events

    private LineChangedEventHandler _mouseClickEvent;
    private LineChangedEventHandler _mouseDoubleClickEvent;
    private LineChangedEventHandler _mouseDownEvent;
    private LineChangedEventHandler _mouseMoveEvent;
    private LineChangedEventHandler _mouseUpEvent;
    private LineChangedEventHandler _mouseWheelEvent;
    private readonly object _mouseClickEventSyncRoot = new object();
    private readonly object _mouseDoubleClickEventSyncRoot = new object();
    private readonly object _mouseDownEventSyncRoot = new object();
    private readonly object _mouseMoveEventSyncRoot = new object();
    private readonly object _mouseUpEventSyncRoot = new object();
    private readonly object _mouseWheelEventSyncRoot = new object();

    #endregion

    #region Properties

    public bool DisplayLineNumbers
    {
      get { return _lineNumbers.Visible; }
      set { _lineNumbers.Visible = value; }
    }

    public string[] Lines
    {
      get { return _textBox.Lines; }
      set { _textBox.Lines = value; }
    }

    #endregion

    #region Property Overrides

    public override string Text
    {
      get { return _textBox.Text; }
      set { _textBox.Text = value; }
    }

    public override Font Font
    {
      get { return base.Font; }
      set
      {
        base.Font = value;
        _textBox.Font = value;
      }
    }

    #endregion

    #region Events

    public new event LineChangedEventHandler MouseClick
    {
      add { lock (_mouseClickEventSyncRoot) _mouseClickEvent += value; }
      remove { lock (_mouseClickEventSyncRoot) _mouseClickEvent -= value; }
    }
    public new event LineChangedEventHandler MouseDoubleClick
    {
      add { lock (_mouseDoubleClickEventSyncRoot) _mouseDoubleClickEvent += value; }
      remove { lock (_mouseDoubleClickEventSyncRoot) _mouseDoubleClickEvent -= value; }
    }
    public new event LineChangedEventHandler MouseDown
    {
      add { lock (_mouseDownEventSyncRoot) _mouseDownEvent += value; }
      remove { lock (_mouseDownEventSyncRoot) _mouseDownEvent -= value; }
    }
    public new event LineChangedEventHandler MouseMove
    {
      add { lock (_mouseMoveEventSyncRoot) _mouseMoveEvent += value; }
      remove { lock (_mouseMoveEventSyncRoot) _mouseMoveEvent -= value; }
    }
    public new event LineChangedEventHandler MouseUp
    {
      add { lock (_mouseUpEventSyncRoot) _mouseUpEvent += value; }
      remove { lock (_mouseUpEventSyncRoot) _mouseUpEvent -= value; }
    }
    public new event LineChangedEventHandler MouseWheel
    {
      add { lock (_mouseWheelEventSyncRoot) _mouseWheelEvent += value; }
      remove { lock (_mouseWheelEventSyncRoot) _mouseWheelEvent -= value; }
    }

    #endregion

    #region Constructors

    public AdvancedRichTextBox()
    {
      InitializeComponent();
    }

    #endregion

    #region Private Methods

    private void InitializeComponent()
    {
      _textBox = new RichTextBox();
      _lineNumbers = new RichTextBoxLineNumbers();
      SuspendLayout();
      // _textBox
      _textBox.Dock = DockStyle.Fill;
      _textBox.AllowDrop = true;
      _textBox.Name = "_textBox";
      _textBox.Size = new Size(472, 250);
      _textBox.WordWrap = false;
      _textBox.DragDrop += OnDragDrop;
      _textBox.DragEnter += OnDragEnter;
      _textBox.MouseClick += OnChildMouseClick;
      _textBox.MouseDoubleClick += OnChildMouseDoubleClick;
      _textBox.MouseDown += OnChildMouseDown;
      _textBox.MouseMove += OnChildMouseMove;
      _textBox.MouseUp += OnChildMouseUp;
      _textBox.MouseWheel += OnChildMouseWheel;
      // _lineNumbers
      _lineNumbers.Dock = DockStyle.Left;
      _lineNumbers.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
      _lineNumbers.Margin = new Padding(0);
      _lineNumbers.Name = "_lineNumbers";
      _lineNumbers.Offset = new Size(0, 0);
      _lineNumbers.Padding = new Padding(0, 0, 2, 0);
      _lineNumbers.ParentRichTextBox = _textBox;
      _lineNumbers.Size = new Size(28, 250);
      _lineNumbers.MouseClick += OnChildMouseClick;
      _lineNumbers.MouseDoubleClick += OnChildMouseDoubleClick;
      _lineNumbers.MouseDown += OnChildMouseDown;
      _lineNumbers.MouseMove += OnChildMouseMove;
      _lineNumbers.MouseUp += OnChildMouseUp;
      _lineNumbers.MouseWheel += OnChildMouseWheel;
      // this
      Resize += OnResized;
      Controls.Add(_textBox);
      Controls.Add(_lineNumbers);
      Name = "AdvancedRichTextBox";
      Size = new Size(500, 250);
      ResumeLayout(false);
    }

    #endregion

    #region Protected EventHandlers

    protected virtual void OnChildMouseClick(object sender, MouseEventArgs e)
    {
      CallMouseEvent(_mouseClickEvent, _mouseClickEventSyncRoot, e);
    }

    protected virtual void OnChildMouseClick(object sender, LineEventArgs e)
    {
      CallMouseEvent(_mouseClickEvent, _mouseClickEventSyncRoot, e);
    }

    protected virtual void OnChildMouseDoubleClick(object sender, MouseEventArgs e)
    {
      CallMouseEvent(_mouseDoubleClickEvent, _mouseDoubleClickEventSyncRoot, e);
    }

    protected virtual void OnChildMouseDoubleClick(object sender, LineEventArgs e)
    {
      CallMouseEvent(_mouseDoubleClickEvent, _mouseDoubleClickEventSyncRoot, e);
    }

    protected virtual void OnChildMouseDown(object sender, MouseEventArgs e)
    {
      CallMouseEvent(_mouseDownEvent, _mouseDownEventSyncRoot, e);
    }

    protected virtual void OnChildMouseDown(object sender, LineEventArgs e)
    {
      CallMouseEvent(_mouseDownEvent, _mouseDownEventSyncRoot, e);
    }

    protected virtual void OnChildMouseMove(object sender, MouseEventArgs e)
    {
      CallMouseEvent(_mouseMoveEvent, _mouseMoveEventSyncRoot, e);
    }

    protected virtual void OnChildMouseMove(object sender, LineEventArgs e)
    {
      CallMouseEvent(_mouseMoveEvent, _mouseMoveEventSyncRoot, e);
    }

    protected virtual void OnChildMouseUp(object sender, MouseEventArgs e)
    {
      CallMouseEvent(_mouseUpEvent, _mouseUpEventSyncRoot, e);
    }

    protected virtual void OnChildMouseUp(object sender, LineEventArgs e)
    {
      CallMouseEvent(_mouseUpEvent, _mouseUpEventSyncRoot, e);
    }

    protected virtual void OnChildMouseWheel(object sender, MouseEventArgs e)
    {
      CallMouseEvent(_mouseWheelEvent, _mouseWheelEventSyncRoot, e);
    }

    protected virtual void OnChildMouseWheel(object sender, LineEventArgs e)
    {
      CallMouseEvent(_mouseWheelEvent, _mouseWheelEventSyncRoot, e);
    }

    protected virtual void OnDragEnter(object sender, DragEventArgs e)
    {
      e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                   ? DragDropEffects.Copy
                   : DragDropEffects.None;
    }

    protected virtual void OnDragDrop(object sender, DragEventArgs e)
    {
      var data = (object[]) e.Data.GetData(DataFormats.FileDrop);
      if (data == null) return;
      var fileName = data[0].ToString();
      foreach (RichTextBoxStreamType streamType in Enum.GetValues(typeof (RichTextBoxStreamType)))
      {
        try
        {
          _textBox.LoadFile(fileName, streamType);
          return;
        } catch {}
      }
      MessageBox.Show("DragDrop function failed");
    }

    protected virtual void OnResized(object sender, EventArgs e)
    {
      Region = new Region(new Rectangle(new Point(0, 0), Size));
    }

    #endregion

    #region Protected EventHandlers Overrides

    protected override void OnMouseClick(MouseEventArgs e) { }

    protected override void OnMouseDoubleClick(MouseEventArgs e) { }

    protected override void OnMouseDown(MouseEventArgs e) { }

    protected override void OnMouseMove(MouseEventArgs e) { }

    protected override void OnMouseUp(MouseEventArgs e) { }

    protected override void OnMouseWheel(MouseEventArgs e) { }

    #endregion

    #region Private Methods

    private void CallMouseEvent(LineChangedEventHandler eventHandler, object syncroot, MouseEventArgs e)
    {
      CallMouseEvent(eventHandler, syncroot, new LineEventArgs(_lineNumbers, e));
    }

    private void CallMouseEvent(LineChangedEventHandler eventHandler, object syncroot, LineEventArgs e)
    {
      lock (syncroot)
        if (eventHandler != null)
          eventHandler(this, e);
    }

    #endregion

  }
}
