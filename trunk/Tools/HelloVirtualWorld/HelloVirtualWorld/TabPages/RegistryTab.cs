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
using Microsoft.Win32;

namespace HelloVirtualWorld.TabPages
{
  public partial class RegistryTab : UserControl
  {
    public RegistryTab()
    {
      InitializeComponent();
    }

    private void ReadRegistryValue(object sender, EventArgs e)
    {
      MessageBox.Show(@"Reading value for CURRENT_USER\Environment\Tmp");
      RegistryKey key = Registry.CurrentUser.OpenSubKey("Environment");
      var value = key.GetValue("TMP", null);
      MessageBox.Show("The value is\n" + value);
    }
  }
}
