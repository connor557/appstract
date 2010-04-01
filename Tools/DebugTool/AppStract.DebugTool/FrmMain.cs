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
using AppStract.DebugTool.TabPages;

namespace AppStract.DebugTool
{
  public partial class FrmMain : Form
  {
    public FrmMain()
    {
      InitializeComponent();
      SetTabPages();
      SelectedTabChanged(this, new EventArgs());
    }

    private void SetTabPages()
    {
      _tabMain.TabPages["_tabLogAnalyzer"].Controls.Add(new LogAnalyzerPage { Dock = DockStyle.Fill });
    }

    private void SelectedTabChanged(object sender, EventArgs e)
    {
      Text = _tabMain.SelectedTab.Text + " - AppStract Debug Tools";
    }
  }

}
