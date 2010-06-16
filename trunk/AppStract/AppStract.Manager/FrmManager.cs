#region Copyright (C) 2008-2009 Simon Allaeys

/*
    Copyright (C) 2008-2009 Simon Allaeys
 
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

using System.Threading;
using System.Windows.Forms;
using AppStract.Manager.Packaging;
using AppStract.Manager.Utilities;

namespace AppStract.Manager
{
  /// <summary>
  /// First (and simplified) implementation of the main form.
  /// The planned features are:
  /// - Start wizard to package applications with from here.
  /// - Let user load packaged applications so he can...
  ///   -> export and import files
  ///   -> edit the registry.
  ///   -> edit the file table.
  ///   -> change settings.
  ///   -> ...
  /// - Provide tools for system recovery
  /// - Provide tools for package recovery
  /// </summary>
  public partial class FrmManager : Form
  {

    #region Constructors

    public FrmManager()
    {
      InitializeComponent();
    }

    #endregion

    #region Private Methods

    private void _btnPackageNew_Click(object sender, System.EventArgs e)
    {
      if (Thread.CurrentThread.Name == null)
        Thread.CurrentThread.Name = "Packager";
      // Gather the necessary information from the user and start packaging.
      Hide();
      PackagingHelper.Start();
      Show();
    }

    private void _btnConfigureApplication_Click(object sender, System.EventArgs e)
    {
      ApplicationConfigurationUtility frm = new ApplicationConfigurationUtility();
      frm.ShowDialog();
    }

    private void _btnCleanSystem_Click(object sender, System.EventArgs e)
    {
      CleanUpHelper frm = new CleanUpHelper();
      frm.ShowDialog();
    }

    #endregion

  }
}
