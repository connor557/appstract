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
using AppStract.Core.Virtualization.Engine.Registry;
using RegistryRule = AppStract.Core.Virtualization.Engine.EngineRule<string, AppStract.Core.Virtualization.Engine.Registry.AccessMechanism>;

namespace AppStract.Manager.Utilities.ApplicationConfiguration
{
  public partial class EngineSettingsPage : UserControl, IApplicationConfigurationPage
  {

    #region Variables

    private RegistryRule _defaultRegistryRule = GetNewDefaultRegistryRule();
    private ApplicationData _data;

    #endregion

    #region Constructors

    public EngineSettingsPage()
    {
      InitializeComponent();
      _cmbRegistryRuleVirtualizationType.Items.AddRange(Enum.GetNames(typeof(AccessMechanism)));
      _gbRegistryEngineRuleConfiguration.Enabled = false;
      _txtRegistryRuleValueName.Enabled = false;
      Enabled = false;
    }

    #endregion

    #region Private Methods

    private void UpdateDataSource()
    {
      if (_data == null) return;
      var collection = RegistryRuleCollection.GetEmptyRuleCollection();
      foreach (RegistryRule rule in _listEngineSettingsRegistry.Items)
        if (rule != _defaultRegistryRule)
          collection.SetRule(rule.Identifier, rule.Rule);
      _data.Settings.RegistryEngineRuleCollection = collection;
    }

    private static RegistryRule GetNewDefaultRegistryRule()
    {
      return new RegistryRule("New Item", AccessMechanism.Virtual);
    }

    #endregion

    #region Form EventHandlers

    private void _listEngineSettingsRegistry_SelectedIndexChanged(object sender, EventArgs e)
    {
      _txtRegistryRuleValueName.Enabled = false;
      bool enable = _listEngineSettingsRegistry.SelectedIndex != -1;
      _gbRegistryEngineRuleConfiguration.Enabled = enable;
      _btnEngineSettingsRegistryDelete.Enabled = enable;
      _btnEngineSettingsRegistryDown.Enabled = enable;
      _btnEngineSettingsRegistryUp.Enabled = enable;
      if (!enable) return;
      var rule = _listEngineSettingsRegistry.SelectedItem as RegistryRule;
      if (rule == null) return;
      _txtRegistryRuleKeyName.Text = rule.Identifier;
      _cmbRegistryRuleVirtualizationType.SelectedValue = rule.Rule.ToString();
    }

    private void _btnEngineSettingsRegistryUp_Click(object sender, EventArgs e)
    {
      var index = _listEngineSettingsRegistry.SelectedIndex;
      if (index <= 0)
        return;
      var item = _listEngineSettingsRegistry.SelectedItem;
      _listEngineSettingsRegistry.Items.RemoveAt(index);
      _listEngineSettingsRegistry.Items.Insert(--index, item);
      _listEngineSettingsRegistry.SelectedIndex = index;
      UpdateDataSource();
    }

    private void _btnEngineSettingsRegistryDown_Click(object sender, EventArgs e)
    {
      var index = _listEngineSettingsRegistry.SelectedIndex;
      if (index == -1
          || index >= _listEngineSettingsRegistry.Items.Count - 1)
        return;
      var item = _listEngineSettingsRegistry.SelectedItem;
      _listEngineSettingsRegistry.Items.RemoveAt(index);
      _listEngineSettingsRegistry.Items.Insert(++index, item);
      _listEngineSettingsRegistry.SelectedIndex = index;
      UpdateDataSource();
    }

    private void _btnEngineSettingsRegistryNew_Click(object sender, EventArgs e)
    {
      if (!_listEngineSettingsRegistry.Items.Contains(_defaultRegistryRule))
        _listEngineSettingsRegistry.Items.Add(_defaultRegistryRule);
      _listEngineSettingsRegistry.SelectedItem = _defaultRegistryRule;
    }

    private void _btnEngineSettingsRegistryDelete_Click(object sender, EventArgs e)
    {
      if (_listEngineSettingsRegistry.SelectedItem == null)
        return;
      _listEngineSettingsRegistry.Items.Remove(_listEngineSettingsRegistry.SelectedItem);
      UpdateDataSource();
    }

    private void _btnRegistryEngineRuleConfigurationApply_Click(object sender, EventArgs e)
    {
      var rule = _listEngineSettingsRegistry.SelectedItem as RegistryRule;
      if (rule == null)
      {
        _gbRegistryEngineRuleConfiguration.Enabled = false;
        return;
      }
      if (_defaultRegistryRule.Identifier == _txtRegistryRuleKeyName.Text)
        return;
      if (rule == _defaultRegistryRule)
        _defaultRegistryRule = GetNewDefaultRegistryRule();
      rule.Identifier = _txtRegistryRuleKeyName.Text;
      if (_cmbRegistryRuleVirtualizationType.SelectedIndex == -1)
        _cmbRegistryRuleVirtualizationType.SelectedIndex = 0;
      rule.Rule = (AccessMechanism) Enum.Parse(typeof (AccessMechanism), _cmbRegistryRuleVirtualizationType.SelectedItem.ToString());
      var index = _listEngineSettingsRegistry.SelectedIndex;
      _listEngineSettingsRegistry.Items.RemoveAt(index);
      _listEngineSettingsRegistry.Items.Insert(index, rule);
      _listEngineSettingsRegistry.SelectedIndex = index;
      UpdateDataSource();
    }

    #endregion

    #region IApplicationConfigurationPage Members

    public void BindDataSource(ApplicationData dataSource)
    {
      if (dataSource == null)
        throw new ArgumentNullException("dataSource");
      _data = dataSource;
      if (_data.Settings.RegistryEngineRuleCollection == null)
        _data.Settings.RegistryEngineRuleCollection = RegistryRuleCollection.GetDefaultRuleCollection();
      _listEngineSettingsRegistry.BeginUpdate();
      _listEngineSettingsRegistry.Items.Clear();
      foreach (var rule in dataSource.Settings.RegistryEngineRuleCollection)
        _listEngineSettingsRegistry.Items.Add(rule);
      _listEngineSettingsRegistry.EndUpdate();
      Enabled = true;
    }

    #endregion

  }
}