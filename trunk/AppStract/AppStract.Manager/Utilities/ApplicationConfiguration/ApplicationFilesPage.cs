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
using AppStract.Core.Data.Application;

namespace AppStract.Manager.Utilities.ApplicationConfiguration
{
  public partial class ApplicationFilesPage : UserControl, IApplicationConfigurationPage
  {

    #region Variables

    private bool _dataSourceLocked;
    private ApplicationData _data;

    #endregion

    #region Constructors

    public ApplicationFilesPage()
    {
      InitializeComponent();
      Enabled = false;
    }

    #endregion

    #region Private Methods

    private void UpdateDataSource()
    {
      if (_dataSourceLocked) return;
      _data.Files.Executable.FileName = _txtExecutable.Text;
      _data.Files.RootDirectory.FileName = _txtFileSystemRootDirectory.Text;
      _data.Files.RegistryDatabase.FileName = _txtRegistryDatabase.Text;
    }

    #endregion

    #region Form EventHandlers

    private void _txt_TextChanged(object sender, EventArgs e)
    {
      UpdateDataSource();
    }

    #endregion

    #region IApplicationConfigurationPage Members

    public void BindDataSource(ApplicationData dataSource)
    {
      if (dataSource == null)
        throw new ArgumentNullException("dataSource");
      _data = dataSource;
      if (_data.Files.Executable == null)
        _data.Files.Executable = new ApplicationFile();
      if (_data.Files.RootDirectory == null)
        _data.Files.RootDirectory = new ApplicationFile();
      if (_data.Files.RegistryDatabase == null)
        _data.Files.RegistryDatabase = new ApplicationFile();
      _dataSourceLocked = true;
      _txtExecutable.Text = _data.Files.Executable.FileName;
      _txtFileSystemRootDirectory.Text = _data.Files.RootDirectory.FileName;
      _txtRegistryDatabase.Text = _data.Files.RegistryDatabase.FileName;
      _dataSourceLocked = false;
      Enabled = true;
    }

    #endregion

  }
}