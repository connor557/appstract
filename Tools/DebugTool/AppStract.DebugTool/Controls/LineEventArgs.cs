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
using System.Windows.Forms;

namespace AppStract.DebugTool.Controls
{
  public class LineEventArgs : EventArgs
  {

    #region Variables

    private int _lineNumber;
    private bool _lineNumberIsKnown;
    private readonly RichTextBoxLineNumbers _lines;

    #endregion

    #region Properties

    public int LineNumber
    {
      get
      {
        if (!_lineNumberIsKnown)
        {
          _lineNumber = _lines.GetLineNumberFromPosition(MouseEventArgs.Y);
          _lineNumberIsKnown = true;
        }
        return _lineNumber;
      }
    }
    public MouseEventArgs MouseEventArgs { get; private set; }

    #endregion

    #region Constructors

    internal LineEventArgs(RichTextBoxLineNumbers sender, MouseEventArgs e)
    {
      _lines = sender;
      MouseEventArgs = e;
    }

    #endregion

  }
}
