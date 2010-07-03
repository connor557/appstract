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
using AppStract.Core.Virtualization.Engine;
using AppStract.Core.Virtualization.Engine.FileSystem;
using AppStract.Core.Virtualization.Engine.Registry;

namespace AppStract.Manager.Utilities.ApplicationConfiguration
{
  public partial class EngineSettingsPage : UserControl, IApplicationConfigurationPage
  {

    #region Variables

    private ApplicationData _data;

    #endregion

    #region Constructors

    public EngineSettingsPage()
    {
      InitializeComponent();
      Enabled = false;
    }

    #endregion

    #region Private Methods

    private void FileSystemRuleCollectionUpdateEventHandler(EngineRuleCollection ruleCollection)
    {
      _data.Settings.FileSystemEngineRuleCollection = (FileSystemRuleCollection) ruleCollection;
    }

    private void RegistryRuleCollectionUpdateEventHandler(EngineRuleCollection ruleCollection)
    {
      _data.Settings.RegistryEngineRuleCollection = (RegistryRuleCollection)ruleCollection;
    }

    #endregion

    #region IApplicationConfigurationPage Members

    public void BindDataSource(ApplicationData dataSource)
    {
      if (dataSource == null)
        throw new ArgumentNullException("dataSource");
      _data = dataSource;
      if (_data.Settings.FileSystemEngineRuleCollection == null)
        _data.Settings.FileSystemEngineRuleCollection = FileSystemRuleCollection.GetDefaultRuleCollection();
      if (_data.Settings.RegistryEngineRuleCollection == null)
        _data.Settings.RegistryEngineRuleCollection = RegistryRuleCollection.GetDefaultRuleCollection();
      _fileSystemEngineSettingsPageContent.BindRuleCollection(_data.Settings.FileSystemEngineRuleCollection, FileSystemRuleCollectionUpdateEventHandler);
      _registryEngineSettingsPageContent.BindRuleCollection(_data.Settings.RegistryEngineRuleCollection, RegistryRuleCollectionUpdateEventHandler);
      Enabled = true;
    }

    #endregion

  }
}