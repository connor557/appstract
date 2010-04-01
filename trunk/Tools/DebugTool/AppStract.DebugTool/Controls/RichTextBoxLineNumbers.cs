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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AppStract.DebugTool.Controls
{
  [DefaultProperty("ParentRichTextBox")]
  public sealed class RichTextBoxLineNumbers : Control
  {

    #region Imports

    [DllImport("user32.dll", EntryPoint = "SendMessageW", SetLastError = true)]
    private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, IntPtr lParam);

    #endregion

    #region Variables

    private List<LineNumberItem> _items;
    private bool _autoSizing;
    private RichTextBox _parentTextBox;
    private LineNumberDockSide _dockSide = LineNumberDockSide.Left;
    private bool _transparent;
    private bool _gridLinesShow;
    private Color _gridLinesColor = Color.SlateGray;
    private float _gridLinesThickness = 0.5F;
    private DashStyle _gridLinesDashStyle = DashStyle.Dot;
    private bool _borderShow;
    private Color _borderColor;
    private float _borderThickness;
    private DashStyle _borderDashStyle = DashStyle.Solid;
    private bool _gradientShow;
    private LinearGradientMode _gradientDirection = LinearGradientMode.Horizontal;
    private Color _gradientStarColor = Color.FromArgb(0, 0, 0, 0);
    private Color _gradientEndColor = Color.LightSteelBlue;
    private bool _marginLinesShow = true;
    private LineNumberDockSide _marginLinesSide = LineNumberDockSide.Right;
    private float _marginLinesThickness = 1.0F;
    private DashStyle _marginLinesStyle = DashStyle.Solid;
    private Color _marginLinesColor = Color.SlateGray;
    private bool _lineNumbersShow = true;
    private bool _lineNumbersShowLeadingZeroes;
    private bool _lineNumbersShowAsHexadecimal;
    private bool _lineNumbersClipByItemRectangle = true;
    private Size _lineNumbersOffset = new Size(0, 0);
    private string _lineNumbersFormat = "0";
    private ContentAlignment _lineNumbersAlignment = ContentAlignment.MiddleRight;
    private bool _lineNumbersAntiAlias = true;
    private Size _autoSizingSize = new Size(0, 0);

    #endregion

    #region Properties

    /// <summary>
    /// Gets the the list of visible lines as instances of <see cref="LineNumberItem"/>.
    /// </summary>
    public IList<LineNumberItem> Items
    {
      get { return _items; }
    }

    [Description("Use this property to enable line numbering for the chosen RichTextBox.")]
    [Category("Data")]
    public RichTextBox ParentRichTextBox
    {
      get { return _parentTextBox; }
      set
      {
        if (_parentTextBox == value) return;
        DeattachEventHandlersFromParent();
        _parentTextBox = value;
        AttachEventHandlersToParent();
        if (_parentTextBox != null)
        {
          Parent = _parentTextBox.Parent;
          _parentTextBox.Refresh();
        }
        Refresh();
      }
    }

    [Description("Use this property to automatically resize the control (and reposition it if needed).")]
    [Category("Behavior")]
    [DefaultValue(false)]
    public bool AutoSizing
    {
      get { return _autoSizing; }
      set
      {
        if (_autoSizing == value) return;
        _autoSizing = value;
        Refresh();
      }
    }

    [Description("Use this property to dock the LineNumbers to a chosen side of the chosen RichTextBox.")]
    [Category("Layout")]
    [DefaultValue(LineNumberDockSide.Left)]
    public LineNumberDockSide DockSide
    {
      get { return _dockSide; }
      set
      {
        _dockSide = value;
        Refresh();
      }
    }

    [Description("Use this property to enable the control to act as an overlay on top of the RichTextBox.")]
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool Transparent
    {
      get { return _transparent; }
      set
      {
        _transparent = value;
        Refresh();
      }
    }

    #region Properties - Overriden

    [Browsable(false)]
    public override bool AutoSize
    {
      get { return base.AutoSize; }
      set
      {
        base.AutoSize = value;
        Invalidate();
      }
    }

    [Browsable(true)]
    public override Font Font
    {
      get { return base.Font; }
      set
      {
        base.Font = value;
        Refresh();
        Refresh();
      }
    }

    [DefaultValue("")]
    [AmbientValue("")]
    [Browsable(false)]
    public override string Text
    {
      get { return base.Text; }
      set
      {
        base.Text = "";
        Refresh();
      }
    }

    #endregion

    #region Properties - Border

    [Description("BorderLines are shown on all sides of the LineNumber control.")]
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool ShowBorder
    {
      get { return _borderShow; }
      set
      {
        _borderShow = value;
        Refresh();
      }
    }

    [Category("Appearance")]
    public Color BorderColor
    {
      get { return _borderColor; }
      set
      {
        _borderColor = value;
        Refresh();
      }
    }

    [Category("Appearance")]
    [DefaultValue(1)]
    public float BorderThickness
    {
      get { return _borderThickness; }
      set
      {
        _borderThickness = Math.Max(1, Math.Min(255, value));
        Refresh();
      }
    }

    [Category("Appearance")]
    [DefaultValue(DashStyle.Solid)]
    public DashStyle BorderDashStyle
    {
      get { return _borderDashStyle; }
      set
      {
        if (value == DashStyle.Custom)
          value = DashStyle.Solid;
        _borderDashStyle = value;
        Refresh();
      }
    }

    #endregion

    #region Properties - GridLines

    [Description("GridLines are the horizontal divider-lines shown above each LineNumber.")]
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool ShowGridLines
    {
      get { return _gridLinesShow; }
      set
      {
        _gridLinesShow = value;
        Refresh();
      }
    }

    [Category("Appearance")]
    public Color GridLinesColor
    {
      get { return _gridLinesColor; }
      set
      {
        _gridLinesColor = value;
        Refresh();
      }
    }

    [Category("Appearance")]
    [DefaultValue(0.5)]
    public float GridLinesThickness
    {
      get { return _gridLinesThickness; }
      set
      {
        _gridLinesThickness = Math.Max(1, Math.Min(255, value));
        Refresh();
      }
    }

    [Category("Appearance")]
    [DefaultValue(DashStyle.Dot)]
    public DashStyle GridLinesDashStyle
    {
      get { return _gridLinesDashStyle; }
      set
      {
        if (value == DashStyle.Custom)
          value = DashStyle.Solid;
        _gridLinesDashStyle = value;
        Refresh();
      }
    }

    #endregion

    #region Properties - MarginLines

    [Description("MarginLines are shown on the Left or Right (or both in Height-mode) of the LineNumber control.")]
    [Category("Behavior")]
    [DefaultValue(true)]
    public bool ShowMarginLines
    {
      get { return _marginLinesShow; }
      set
      {
        _marginLinesShow = value;
        Refresh();
      }
    }

    [Category("Appearance")]
    [DefaultValue(LineNumberDockSide.Right)]
    public LineNumberDockSide MarginLinesSide
    {
      get { return _marginLinesSide; }
      set
      {
        _marginLinesSide = value;
        Refresh();
      }
    }

    [Category("Appearance")]
    public Color MarginLinesColor
    {
      get { return _marginLinesColor; }
      set
      {
        _marginLinesColor = value;
        Refresh();
      }
    }

    [Category("Appearance")]
    [DefaultValue(1.0F)]
    public float MarginLinesThickness
    {
      get { return _marginLinesThickness; }
      set
      {
        _marginLinesThickness = Math.Max(1, Math.Min(255, value));
        Refresh();
      }
    }

    [Category("Additional Appearance")]
    [DefaultValue(DashStyle.Solid)]
    public DashStyle MarginLinesStyle
    {
      get { return _marginLinesStyle; }
      set
      {
        if (value == DashStyle.Custom)
          value = DashStyle.Solid;
        _marginLinesStyle = value;
        Refresh();
      }
    }

    #endregion

    #region Properties - BackgroundGradient

    [Description("The BackgroundGradient is a gradual blend of two colors, shown in the back of each LineNumber's item-area.")]
    [Category("Appearance")]
    [DefaultValue(false)]
    public bool ShowBackgroundGradient
    {
      get { return _gradientShow; }
      set
      {
        _gradientShow = value;
        Refresh();
      }
    }

    [Category("Appearance")]
    public Color BackgroundGradientAlphaColor
    {
      get { return _gradientStarColor; }
      set
      {
        _gradientStarColor = value;
        Refresh();
      }
    }

    [Category("Appearance")]
    public Color BackgroundGradientBetaColor
    {
      get { return _gradientEndColor; }
      set
      {
        _gradientEndColor = value;
        Refresh();
      }
    }

    [Category("Appearance")]
    [DefaultValue(LinearGradientMode.Horizontal)]
    public LinearGradientMode BackgroundGradientDirection
    {
      get { return _gradientDirection; }
      set
      {
        _gradientDirection = value;
        Refresh();
      }
    }

    #endregion

    #region Properties - LineNumbers

    [Category("LineNumbers")]
    [DefaultValue(true)]
    public bool ShowLineNumbers
    {
      get { return _lineNumbersShow; }
      set
      {
        _lineNumbersShow = value;
        Refresh();
      }
    }

    [Description("Use this to set whether the LineNumbers are allowed to spill out of their item-area, or should be clipped by it.")]
    [Category("LineNumbers")]
    [DefaultValue(true)]
    public bool ClipByItemRectangle
    {
      get { return _lineNumbersClipByItemRectangle; }
      set
      {
        _lineNumbersClipByItemRectangle = value;
        Refresh();
      }
    }

    [Description("Use this to set whether the LineNumbers should have leading zeroes (based on the total amount of textlines).")]
    [Category("LineNumbers")]
    [DefaultValue(false)]
    public bool ShowLeadingZeroes
    {
      get { return _lineNumbersShowLeadingZeroes; }
      set
      {
        _lineNumbersShowLeadingZeroes = value;
        Refresh();
      }
    }

    [Description("Use this to set whether the LineNumbers should be shown as hexadecimal values.")]
    [Category("LineNumbers")]
    [DefaultValue(false)]
    public bool AsHexadecimal
    {
      get { return _lineNumbersShowAsHexadecimal; }
      set
      {
        _lineNumbersShowAsHexadecimal = value;
        Refresh();
      }
    }

    [Description("Use this property to manually reposition the LineNumbers, relative to their current location.")]
    [Category("LineNumbers")]
    public Size Offset
    {
      get { return _lineNumbersOffset; }
      set
      {
        _lineNumbersOffset = value;
        Refresh();
      }
    }

    [Description("Use this to align the LineNumbers to a chosen corner (or center) within their item-area.")]
    [Category("LineNumbers")]
    [DefaultValue(ContentAlignment.MiddleRight)]
    public ContentAlignment Alignment
    {
      get { return _lineNumbersAlignment; }
      set
      {
        _lineNumbersAlignment = value;
        Refresh();
      }
    }

    [Description("Use this to apply Anti-Aliasing to the LineNumbers (high quality). Some fonts will look better without it, though.")]
    [Category("LineNumbers")]
    [DefaultValue(true)]
    public bool AntiAlias
    {
      get { return _lineNumbersAntiAlias; }
      set
      {
        _lineNumbersAntiAlias = value;
        Refresh();
        Refresh();
      }
    }

    #endregion

    #endregion

    #region Events

    [Category("Action")]
    [Browsable(true)]
    public new event LineChangedEventHandler MouseClick;

    #endregion

    #region Constructors

    public RichTextBoxLineNumbers()
    {
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      SetStyle(ControlStyles.ResizeRedraw, true);
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      Margin = new Padding(0);
      Padding = new Padding(0, 0, 2, 0);
      //zTimer.Enabled = true;
      //zTimer.Interval = 200;
      //zTimer.Stop();
      UpdateSizeAndPosition();
      Invalidate();
    }

    #endregion

    #region Public Methods

    public int GetLineNumberFromPosition(int verticalPosition)
    {
      return GetLineNumberItemFromPosition(verticalPosition).LineNumber;
    }

    public LineNumberItem GetLineNumberItemFromPosition(int verticalPosition)
    {
      if (_parentTextBox.Lines.Length == 0 || _items.Count == 0)
      {
        if (verticalPosition >= GetVerticalOffset()
            && TextRenderer.MeasureText("0", _parentTextBox.Font).Height >= verticalPosition)
          return new LineNumberItem(1);
      }
      else if (verticalPosition >= _items[0].Rectangle.Y
               && verticalPosition <= _items[_items.Count - 1].Rectangle.Y + _items[_items.Count - 1].Rectangle.Height)
      {
        foreach (var item in _items)
          if (verticalPosition >= item.Rectangle.Y && verticalPosition <= item.Rectangle.Y + item.Rectangle.Height)
            return item;
      }
      return new LineNumberItem(-1);
    }

    public LineNumberItem GetLineNumberItem(int lineNumber)
    {
      return _items.FirstOrDefault(i => i.LineNumber == lineNumber);
    }

    #endregion

    #region Public Method Overrides

    public override void Refresh()
    {
      base.Refresh();
      UpdateSizeAndPosition();
    }

    #endregion

    #region Protected Method Overrides

    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);
      AutoSize = false;
    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
      if (MouseClick != null)
        MouseClick(this, new LineEventArgs(this, e));
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      if (DesignMode)
        Refresh();
      base.OnSizeChanged(e);
      Invalidate();
    }

    protected override void OnLocationChanged(EventArgs e)
    {
      if (DesignMode)
        Refresh();
      base.OnLocationChanged(e);
      Invalidate();
    }

    /// <summary>
    /// OnPaint will go through the enabled elements (vertical ReminderMessage, GridLines, LineNumbers, BorderLines, MarginLines) and will
    /// draw them if enabled. At the same time, it will build GraphicsPaths for each of those elements (that are enabled), which will be used 
    /// in SeeThroughMode (if it's active): the figures in the GraphicsPaths will form a customized outline for the control by setting them as the 
    /// Region of the LineNumber control. Note: the vertical ReminderMessages are only drawn during designtime. 
    /// </summary>
    /// <param name="e"></param>
    /// <remarks></remarks>
    protected override void OnPaint(PaintEventArgs e)
    {
      e.Graphics.TextRenderingHint
        = _lineNumbersAntiAlias ? TextRenderingHint.AntiAlias : TextRenderingHint.SystemDefault;
      base.OnPaint(e);
      _lineNumbersFormat = _lineNumbersShowAsHexadecimal
                       ? "".PadRight(_parentTextBox.Lines.Length.ToString("X").Length, '0')
                       : "".PadRight(_parentTextBox.Lines.Length.ToString().Length, '0');
      if (_autoSizing)
      {
        var textSize = e.Graphics.MeasureString(_lineNumbersFormat.Replace('0', 'W'), Font);
        _autoSizingSize = new Size((int) Math.Round(textSize.Width), 0);
      }
      _items = GetVisibleLineNumberItems(e.Graphics);
      // GraphicsPaths are needed for transparency
      var gpBorder = new GraphicsPath(FillMode.Winding);
      var gpGridLines = new GraphicsPath(FillMode.Winding);
      var gpLineNumbers = new GraphicsPath(FillMode.Winding);
      var gpMarginLines = new GraphicsPath(FillMode.Winding);
      if (DesignMode)
        DrawReminderMessage(e.Graphics, gpLineNumbers);
      if (_items.Count != 0)
        DrawItems(_items, e.Graphics, gpGridLines, gpLineNumbers);
      if (_marginLinesShow)
        DrawMargins(e.Graphics, gpMarginLines);
      if (_borderShow)
        DrawBorders(e.Graphics, GetControlBounds(), gpBorder);
      if (_transparent)
        MakeTransparent(new[] { gpBorder, gpGridLines, gpLineNumbers, gpMarginLines });
      else
        EnsureControlBounds(GetControlBounds());
      gpGridLines.Dispose();
      gpBorder.Dispose();
      gpMarginLines.Dispose();
      gpLineNumbers.Dispose();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Attaches all needed event handlers to <see cref="ParentRichTextBox"/>.
    /// </summary>
    private void AttachEventHandlersToParent()
    {
      if (_parentTextBox == null) return;
      _parentTextBox.LocationChanged += OnParentChanged;
      _parentTextBox.Move += OnParentChanged;
      _parentTextBox.Resize += OnParentChanged;
      _parentTextBox.DockChanged += OnParentChanged;
      _parentTextBox.TextChanged += OnParentChanged;
      _parentTextBox.VScroll += OnParentScroll;
      _parentTextBox.ContentsResized += OnParentContentsResized;
      _parentTextBox.Disposed += OnParentDisposed;
      _parentTextBox.SizeChanged += OnLinesInvalidated;
    }

    /// <summary>
    /// Deattaches all needed event handlers from <see cref="ParentRichTextBox"/>.
    /// </summary>
    private void DeattachEventHandlersFromParent()
    {
      if (_parentTextBox == null) return;
      _parentTextBox.LocationChanged -= OnParentChanged;
      _parentTextBox.Move -= OnParentChanged;
      _parentTextBox.Resize -= OnParentChanged;
      _parentTextBox.DockChanged -= OnParentChanged;
      _parentTextBox.TextChanged -= OnParentChanged;
      _parentTextBox.VScroll -= OnParentScroll;
      _parentTextBox.ContentsResized -= OnParentContentsResized;
      _parentTextBox.Disposed -= OnParentDisposed;
      _parentTextBox.SizeChanged -= OnLinesInvalidated;
    }

    /// <summary>
    /// Ensures the bounds of the current <see cref="Control"/>.
    /// </summary>
    /// <remarks>
    /// If the control is in a condition that would show it as empty, then a border-region is still drawn regardless of it's borders on/off state.
    /// This is added to make sure that the bounds of the control are never lost (it would remain empty if this was not done).
    /// </remarks>
    /// <param name="controlBounds"></param>
    private void EnsureControlBounds(Rectangle controlBounds)
    {
      var bounds = new GraphicsPath(FillMode.Winding);
      bounds.AddRectangle(controlBounds);
      bounds.CloseFigure();
      Region = new Region(bounds);
    }

    /// <summary>
    /// Gets the bounds of the current <see cref="RichTextBoxLineNumbers"/> as a <see cref="Rectangle"/>.
    /// </summary>
    /// <returns></returns>
    private Rectangle GetControlBounds()
    {
      var borderSmall = (int)Math.Floor(_borderThickness / 2);
      var borderBig = (int)Math.Ceiling(_borderThickness / 2);
      return new Rectangle
      {
        X = borderSmall,
        Y = borderSmall,
        Width = Width - borderBig - borderSmall,
        Height = Height - borderBig - borderSmall
      };
    }

    /// <summary>
    /// Gets the point where the specified <see cref="LineNumberItem"/> should be drawn.
    /// The return value is based on <see cref="Alignment"/>, <paramref name="textSize"/>, and <paramref name="item.Rectangle"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="textSize"></param>
    /// <returns></returns>
    private PointF GetLineNumberDrawingStartPoint(LineNumberItem item, SizeF textSize)
    {
      if (_lineNumbersAlignment == ContentAlignment.TopLeft)
        return new PointF(item.Rectangle.Left + Padding.Left + _lineNumbersOffset.Width, item.Rectangle.Top + Padding.Top + _lineNumbersOffset.Height);
      if (_lineNumbersAlignment == ContentAlignment.MiddleLeft)
        return new PointF(item.Rectangle.Left + Padding.Left + _lineNumbersOffset.Width, item.Rectangle.Top + (item.Rectangle.Height / 2) + _lineNumbersOffset.Height - textSize.Height / 2);
      if (_lineNumbersAlignment == ContentAlignment.BottomLeft)
        return new PointF(item.Rectangle.Left + Padding.Left + _lineNumbersOffset.Width, item.Rectangle.Bottom - Padding.Bottom + 1 + _lineNumbersOffset.Height - textSize.Height);
      if (_lineNumbersAlignment == ContentAlignment.TopCenter)
        return new PointF(item.Rectangle.Width / 2 + _lineNumbersOffset.Width - textSize.Width / 2, item.Rectangle.Top + Padding.Top + _lineNumbersOffset.Height);
      if (_lineNumbersAlignment == ContentAlignment.MiddleCenter)
        return new PointF(item.Rectangle.Width / 2 + _lineNumbersOffset.Width - textSize.Width / 2, item.Rectangle.Top + (item.Rectangle.Height / 2) + _lineNumbersOffset.Height - textSize.Height / 2);
      if (_lineNumbersAlignment == ContentAlignment.BottomCenter)
        return new PointF(item.Rectangle.Width / 2 + _lineNumbersOffset.Width - textSize.Width / 2, item.Rectangle.Bottom - Padding.Bottom + 1 + _lineNumbersOffset.Height - textSize.Height);
      if (_lineNumbersAlignment == ContentAlignment.TopRight)
        return new PointF(item.Rectangle.Right - Padding.Right + _lineNumbersOffset.Width - textSize.Width, item.Rectangle.Top + Padding.Top + _lineNumbersOffset.Height);
      if (_lineNumbersAlignment == ContentAlignment.MiddleRight)
        return new PointF(item.Rectangle.Right - Padding.Right + _lineNumbersOffset.Width - textSize.Width, item.Rectangle.Top + (item.Rectangle.Height / 2) + _lineNumbersOffset.Height - textSize.Height / 2);
      if (_lineNumbersAlignment == ContentAlignment.BottomRight)
        return new PointF(item.Rectangle.Right - Padding.Right + _lineNumbersOffset.Width - textSize.Width, item.Rectangle.Bottom - Padding.Bottom + 1 + _lineNumbersOffset.Height - textSize.Height);
      return new PointF(0, 0);
    }

    /// <summary>
    /// Gets the vertical offset to make the LineNumberItems line up with the textlines in _parentTextBox.
    /// </summary>
    /// <returns></returns>
    private int GetVerticalOffset()
    {
      return _parentTextBox.PointToScreen(_parentTextBox.ClientRectangle.Location).Y
             - PointToScreen(new Point(0, 0)).Y + 1;
    }

    /// <summary>
    /// This Sub determines which textlines are visible in the ParentRichTextBox, and makes LineNumberItems (LineNumber + ItemRectangle)
    /// for each visible line. They are put into the zLNIs List that will be used by the OnPaint event to draw the LineNumberItems. 
    /// </summary>
    /// <remarks></remarks>
    private List<LineNumberItem> GetVisibleLineNumberItems(Graphics graphics)
    {
      var offsetVertical = GetVerticalOffset();
      if (_parentTextBox == null || _parentTextBox.Text.Length == 0)
      {
        if (_parentTextBox == null || _parentTextBox.ReadOnly)
          return new List<LineNumberItem>(0);
        // No lines in textbox, but there is a line where user can start typing
        var textSize = graphics.MeasureString(_lineNumbersFormat.Replace('0', 'W'), Font);
        return new List<LineNumberItem>(1)
                 {new LineNumberItem(1, new RectangleF(0, offsetVertical, Width, textSize.Height))};
      }
      var items = new List<LineNumberItem>(); // Will contain all visible lines
      var lines = _parentTextBox.Lines; // Saving the reference improves performance with non-optimized builds
      var firstLineIndex = GetFirstVisibleLine(_parentTextBox);
      var charIndex = _parentTextBox.GetFirstCharIndexFromLine(firstLineIndex);
      var pos = offsetVertical + _parentTextBox.GetPositionFromCharIndex(charIndex).Y;
      for (var i = firstLineIndex; i < lines.Length && pos < _parentTextBox.Height; i++)
      {
        charIndex += lines[i].Length + 1; // + 1 for new line character
        var nxtPos = offsetVertical + _parentTextBox.GetPositionFromCharIndex(charIndex).Y;
        if (nxtPos <= pos) // Occurs when no next line available
          nxtPos = pos + (int) graphics.MeasureString(lines[i] == "" ? " " : lines[i], Font).Height;
        items.Add(new LineNumberItem(i + 1, new RectangleF(0, pos, Width, nxtPos - pos)));
        pos = nxtPos;
      }
      return items;
    }

    /// <summary>
    /// Makes the current <see cref="RichTextBoxLineNumbers"/> transparent
    /// by matching <see cref="Region"/> to the given <see cref="GraphicsPath"/>s.
    /// </summary>
    /// <param name="graphicPaths"></param>
    private void MakeTransparent(IEnumerable<GraphicsPath> graphicPaths)
    {
      var region = new Region(ClientRectangle);
      region.MakeEmpty();
      foreach (var path in graphicPaths)
        region.Union(path);
      Region = region;
    }

    /// <summary>
    /// Applies the AutoSizing and DockSide settings.
    /// </summary>
    /// <remarks></remarks>
    private void UpdateSizeAndPosition()
    {
      if (AutoSize || Dock == DockStyle.Bottom || Dock == DockStyle.Fill || Dock == DockStyle.Top)
        return;
      var location = Location;
      Size size = Size;
      try
      {
        if (_parentTextBox == null)
        {
          if (_autoSizingSize.Width > 0)
            size.Width = _autoSizingSize.Width;
          if (_autoSizingSize.Height > 0)
            size.Height = _autoSizingSize.Height;
          return;
        }
        if (_dockSide != LineNumberDockSide.None)
        {
          // DockSide is active
          if (_autoSizing && _autoSizingSize.Width > 0)
            size.Width = _autoSizingSize.Width;
          size.Height = _parentTextBox.Height;
          if (_dockSide == LineNumberDockSide.Left)
            location.X = _parentTextBox.Left - size.Width - 1;
          else if (_dockSide == LineNumberDockSide.Right)
            location.X = _parentTextBox.Right + 1;
          location.Y = _parentTextBox.Top;
        }
        else if (_autoSizing)
        {
          // DockSide = None, but AutoSizing is still setting the Width
          if (_autoSizingSize.Width > 0)
            size.Width = _autoSizingSize.Width;
        }
        if (_autoSizing && (Dock == DockStyle.Left || Dock == DockStyle.Right))
        {
          // _parentTextBox is not null for the following cases
          if (_autoSizingSize.Width > 0)
            size.Width = _autoSizingSize.Width;
        }
      }
      finally
      {
        Location = location;
        Size = size;
      }
    }

    #endregion

    #region Private Methods - Drawing

    /// <summary>
    /// Draws <see cref="border"/> on the specified <paramref name="graphics"/>,
    /// and updates <paramref name="gpBorder"/> to hold the paths of the drawn data.
    /// </summary>
    /// <param name="graphics">The <see cref="Graphics"/> to draw on.</param>
    /// <param name="border"></param>
    /// <param name="gpBorder">The <see cref="GraphicsPath"/> to update with the drawn data.</param>
    private void DrawBorders(Graphics graphics, Rectangle border, GraphicsPath gpBorder)
    {
      using (var pen = new Pen(_borderColor, _borderThickness) { DashStyle = _borderDashStyle })
      {
        graphics.DrawRectangle(pen, border);
        gpBorder.AddRectangle(border);
        gpBorder.CloseFigure();
        pen.DashStyle = DashStyle.Solid;
        gpBorder.Widen(pen);
      }
    }

    /// <summary>
    /// Draws a gradient for the specified <see cref="LineNumberItem"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="graphics">The <see cref="Graphics"/> to draw on.</param>
    private void DrawGradient(LineNumberItem item, Graphics graphics)
    {
      using (var gradientBrush
        = new LinearGradientBrush(item.Rectangle, _gradientStarColor, _gradientEndColor, _gradientDirection))
        graphics.FillRectangle(gradientBrush, item.Rectangle);
    }

    /// <summary>
    /// Draws a grid for the specified <see cref="LineNumberItem"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="graphics">The <see cref="Graphics"/> to draw on.</param>
    /// <param name="pen"></param>
    /// <param name="path">The <see cref="GraphicsPath"/> to update with the drawn data.</param>
    private void DrawGridLines(LineNumberItem item, Graphics graphics, Pen pen, GraphicsPath path)
    {
      graphics.DrawLine(pen, new PointF(0, item.Rectangle.Y), new PointF(Width, item.Rectangle.Y));

      // Every item in a GraphicsPath is a closed figure, so instead of adding gridlines as lines, we'll add them
      // as rectangles that loop out of sight. Their height uses the zContentRectangle which is the maxsize of 
      // the ParentRichTextBox's contents. 
      //   
      // Slight adjustment needed when the first item has a negative Y coordinate. 
      // This explains the " - zLNIs(0).Rectangle.Y" (which adds the negative size to the height 
      // to make sure the rectangle's bottompart stays out of sight) 
      path.AddRectangle(new RectangleF(-_gridLinesThickness,
                                       item.Rectangle.Y,
                                       Width + _gridLinesThickness*2,
                                       Height - item.Rectangle.Y + _gridLinesThickness));
      path.CloseFigure();
    }

    /// <summary>
    /// Draws all given <see cref="LineNumberItem"/>s to the specified <see cref="Graphics"/>.
    /// </summary>
    /// <param name="items">The items to draw on the specified <paramref name="graphics"/>.</param>
    /// <param name="graphics">The <see cref="Graphics"/> to draw on.</param>
    /// <param name="gpGridLines">The <see cref="GraphicsPath"/> to update with the drawn gridline-data.</param>
    /// <param name="gpLineNumbers">The <see cref="GraphicsPath"/> to update with the drawn linenumbers-data.</param>
    private void DrawItems(IEnumerable<LineNumberItem> items, Graphics graphics, GraphicsPath gpGridLines, GraphicsPath gpLineNumbers)
    {
      var textFormat =
        new StringFormat(StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip | StringFormatFlags.NoWrap)
          {Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near};
      using (var brush = new SolidBrush(ForeColor))
      using (var pen = new Pen(_gridLinesColor, _gridLinesThickness) {DashStyle = _gridLinesDashStyle})
      {
        foreach (var item in items)
        {
          if (_lineNumbersShow)
            DrawLineNumber(item, graphics, brush, textFormat, gpLineNumbers);
          if (_gradientShow)
            DrawGradient(item, graphics);
          if (_gridLinesShow)
            DrawGridLines(item, graphics, pen, gpGridLines);
        }
        if (!_gridLinesShow) return;
        // Draw as solid to keep the paintingspeed high.
        pen.DashStyle = DashStyle.Solid;
        gpGridLines.Widen(pen);
      }
    }

    /// <summary>
    /// Draws a single <see cref="LineNumberItem"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="graphics">The <see cref="Graphics"/> to draw on.</param>
    /// <param name="brush"></param>
    /// <param name="textFormat"></param>
    /// <param name="path">The <see cref="GraphicsPath"/> to update with the drawn data.</param>
    private void DrawLineNumber(LineNumberItem item, Graphics graphics, Brush brush, StringFormat textFormat, GraphicsPath path)
    {
      var text = _lineNumbersShowAsHexadecimal
                   ? item.LineNumber.ToString("X")
                   : _lineNumbersShowLeadingZeroes
                       ? item.LineNumber.ToString(_lineNumbersFormat)
                       : item.LineNumber.ToString();
      var textSize = graphics.MeasureString(text, Font, new Point(0, 0), textFormat);
      var point = GetLineNumberDrawingStartPoint(item, textSize);
      // TextClipping
      var itemClipRectangle = new RectangleF(point, textSize.ToSize());
      if (_lineNumbersClipByItemRectangle)
      {
        // If selected, the text will be clipped so that it doesn't spill out of its own LineNumberItem-area.
        // Only the part of the text inside the LineNumberItem.Rectangle should be visible, so intersect with the ItemRectangle
        // The SetClip method temporary restricts the drawing area of the control for whatever is drawn next.
        itemClipRectangle.Intersect(item.Rectangle);
        graphics.SetClip(itemClipRectangle);
      }
      // TextDrawing
      graphics.DrawString(text, Font, brush, point, textFormat);
      graphics.ResetClip();
      // The GraphicsPath for the LineNumber is just a rectangle behind the text, to keep the paintingspeed high and avoid ugly artifacts.
      path.AddRectangle(itemClipRectangle);
      path.CloseFigure();
    }

    /// <summary>
    /// Draws the margins on the specified <paramref name="graphics"/>,
    /// and updates <paramref name="gpMarginLines"/> to hold the paths of the drawn data.
    /// </summary>
    /// <param name="graphics">The <see cref="Graphics"/> to draw on.</param>
    /// <param name="gpMarginLines">The <see cref="GraphicsPath"/> to update with the drawn data.</param>
    private void DrawMargins(Graphics graphics, GraphicsPath gpMarginLines)
    {
      if (_marginLinesSide == LineNumberDockSide.None)
        return; // No margins to draw
      using (var pen = new Pen(_marginLinesColor, _marginLinesThickness) { DashStyle = _marginLinesStyle })
      {
        var topLeft = new Point((int)(-_marginLinesThickness), (int)(-_marginLinesThickness));
        var bottomRight = new Point((int)(Width + _marginLinesThickness), (int)(Height + _marginLinesThickness));
        if (_marginLinesSide == LineNumberDockSide.Height || _marginLinesSide == LineNumberDockSide.Left)
        {
          graphics.DrawLine(pen,
                            new Point((int)(Math.Floor(_marginLinesThickness / 2)), 0),
                            new Point((int)(Math.Floor(_marginLinesThickness / 2)), Height - 1));
          topLeft = new Point((int)(Math.Ceiling(_marginLinesThickness / 2)), (int)(-_marginLinesThickness));
        }
        if (_marginLinesSide == LineNumberDockSide.Height || _marginLinesSide == LineNumberDockSide.Right)
        {
          graphics.DrawLine(pen,
                            new Point(Width - (int)Math.Ceiling(_marginLinesThickness / 2), 0),
                            new Point(Width - (int)Math.Ceiling(_marginLinesThickness / 2), Height - 1));
          bottomRight = new Point((int)(Width - Math.Ceiling(_marginLinesThickness / 2)),
                                  (int)(Height + _marginLinesThickness));
        }
        //   GraphicsPath for the MarginLines(s):
        //   MarginLines(s) are drawn as a rectangle connecting the left and right points, which are either inside or 
        //   outside of sight, depending on whether the MarginLines at that side is visible.
        gpMarginLines.AddRectangle(new Rectangle(topLeft, new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y)));
        pen.DashStyle = DashStyle.Solid;
        gpMarginLines.Widen(pen);
      }
    }

    /// <summary>
    /// Paints a reminder message if <see cref="ParentRichTextBox"/> is null.
    /// </summary>
    /// <param name="graphics">The <see cref="Graphics"/> to draw on.</param>
    /// <param name="gpLineNumbers">The <see cref="GraphicsPath"/> to update with the drawn data.</param>
    private void DrawReminderMessage(Graphics graphics, GraphicsPath gpLineNumbers)
    {
      if (_parentTextBox != null) return;
      const string reminderToShow = "-!- Set ParentRichTextBox -!-";
      // Centering and Rotation for the reminder message
      graphics.TranslateTransform(Width / 2.0F, Height / 2.0F);
      graphics.RotateTransform(-90);
      var textFormat = new StringFormat
      {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center
      };
      // Show the reminder message (with small shadow)
      var textSize = graphics.MeasureString(reminderToShow, Font, new Point(0, 0), textFormat);
      graphics.DrawString(reminderToShow, Font, Brushes.WhiteSmoke, 1, 1, textFormat);
      graphics.DrawString(reminderToShow, Font, Brushes.Firebrick, 0, 0, textFormat);
      graphics.ResetTransform();
      var reminderRectangle = new Rectangle((int)(Width / 2.0 - textSize.Height / 2),
                                            (int)(Height / 2.0 - textSize.Width / 2), (int)(textSize.Height),
                                            (int)(textSize.Width));
      gpLineNumbers.AddRectangle(reminderRectangle);
      gpLineNumbers.CloseFigure();

      if (!_autoSizing) return;
      reminderRectangle.Inflate((int)(textSize.Height * 0.2), (int)(textSize.Width * 0.1));
      _autoSizingSize = new Size(reminderRectangle.Width, reminderRectangle.Height);
    }

    #endregion

    #region Private EventHandlers

    private void OnLinesInvalidated(object sender, EventArgs e)
    {
      Invalidate();
    }

    private void OnParentChanged(object sender, EventArgs e)
    {
      if (DesignMode)
        Invalidate();
    }

    private void OnParentScroll(object sender, EventArgs e)
    {
      Invalidate();
    }

    private void OnParentContentsResized(object sender, ContentsResizedEventArgs e)
    {
      Invalidate();
    }

    private void OnParentDisposed(object sender, EventArgs e)
    {
      ParentRichTextBox = null;
      Invalidate();
    }

    #endregion

    #region Private Static Methods

    private static int GetFirstVisibleLine(IWin32Window textBox)
    {
      const int EM_GETFIRSTVISIBLELINE = 0xce;
      return SendMessage(textBox.Handle, EM_GETFIRSTVISIBLELINE, 0, IntPtr.Zero);
    }

    #endregion

  }
}
