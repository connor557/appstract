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
using AppStract.Engine.Configuration;

namespace AppStract.Manager.Utilities.ApplicationConfiguration
{
  public abstract partial class EngineSettingsPageContent : UserControl
  {

    #region EventHandler Declarations

    public delegate void EngineRuleCollectionUpdated(EngineRuleCollection updatedCollection);

    #endregion

    #region Variables

    private EngineRule _defaultRule = GetNewDefaultEngineRule();
    private EngineRuleCollectionUpdated _notifyUpdateDelegate;

    #endregion

    #region Abstract Members

    protected abstract string Title { get; }
    protected abstract string KeyText { get; }
    protected abstract string KeyItemText { get;}
    protected abstract string RuleSelectorText { get; }
    protected abstract EngineRuleCollection GetEmptyRuleCollection();

    #endregion

    #region Constructors

    protected EngineSettingsPageContent()
    {
      InitializeComponent();
      _cmbRuleSelector.Items.AddRange(Enum.GetNames(typeof(VirtualizationType)));
      _gbEngineRuleConfiguration.Enabled = false;
      _txtKeyItem.Enabled = false;
    }

    #endregion

    #region Public Methods

    public void BindRuleCollection(EngineRuleCollection collection, EngineRuleCollectionUpdated updateHandler)
    {
      _notifyUpdateDelegate = updateHandler;
      _listRules.BeginUpdate();
      _listRules.Items.Clear();
      foreach (var rule in collection)
        _listRules.Items.Add(rule);
      _listRules.EndUpdate();
    }

    #endregion

    #region Private Methods

    private void NotifyUpdate()
    {
      if (_notifyUpdateDelegate == null) return;
      EngineRuleCollection collection = GetEmptyRuleCollection();
      foreach (EngineRule rule in _listRules.Items)
        if (rule != _defaultRule)
          collection.SetRule(rule.Identifier, rule.VirtualizationType);
      _notifyUpdateDelegate(collection);
    }

    #endregion

    #region Form EventHandlers

    private void SelectedRuleChanged(object sender, EventArgs e)
    {
      _txtKeyItem.Enabled = false;
      bool enable = _listRules.SelectedIndex != -1;
      _gbEngineRuleConfiguration.Enabled = enable;
      _btnDeleteRule.Enabled = enable;
      _btnRuleDown.Enabled = enable;
      _btnRuleUp.Enabled = enable;
      if (!enable) return;
      var rule = _listRules.SelectedItem as EngineRule;
      if (rule == null) return;
      _txtKey.Text = rule.Identifier;
      _cmbRuleSelector.SelectedItem = rule.VirtualizationType.ToString();
    }

    private void RuleUp(object sender, EventArgs e)
    {
      var index = _listRules.SelectedIndex;
      if (index <= 0)
        return;
      var item = _listRules.SelectedItem;
      _listRules.Items.RemoveAt(index);
      _listRules.Items.Insert(--index, item);
      _listRules.SelectedIndex = index;
      NotifyUpdate();
    }

    private void RuleDown(object sender, EventArgs e)
    {
      var index = _listRules.SelectedIndex;
      if (index == -1
          || index >= _listRules.Items.Count - 1)
        return;
      var item = _listRules.SelectedItem;
      _listRules.Items.RemoveAt(index);
      _listRules.Items.Insert(++index, item);
      _listRules.SelectedIndex = index;
      NotifyUpdate();
    }

    private void NewRule(object sender, EventArgs e)
    {
      if (!_listRules.Items.Contains(_defaultRule))
      {
        if (_listRules.SelectedIndex != -1)
          _listRules.Items.Insert(_listRules.SelectedIndex, _defaultRule);
        else
          _listRules.Items.Insert(0, _defaultRule);
      }
      _listRules.SelectedItem = _defaultRule;
    }

    private void DeleteRule(object sender, EventArgs e)
    {
      if (_listRules.SelectedItem == null)
        return;
      _listRules.Items.Remove(_listRules.SelectedItem);
      NotifyUpdate();
    }

    private void ApplySelectedRule(object sender, EventArgs e)
    {
      var rule = _listRules.SelectedItem as EngineRule;
      if (rule == null)
      {
        _gbEngineRuleConfiguration.Enabled = false;
        return;
      }
      if (_defaultRule.Identifier == _txtKey.Text)
        return;
      if (rule == _defaultRule)
        _defaultRule = GetNewDefaultEngineRule();
      rule.Identifier = _txtKey.Text;
      if (_cmbRuleSelector.SelectedIndex == -1)
        _cmbRuleSelector.SelectedIndex = 0;
      rule.VirtualizationType
        = (VirtualizationType)Enum.Parse(typeof(VirtualizationType),
                                          _cmbRuleSelector.SelectedItem.ToString());
      var index = _listRules.SelectedIndex;
      _listRules.Items.RemoveAt(index);
      _listRules.Items.Insert(index, rule);
      _listRules.SelectedIndex = index;
      NotifyUpdate();
    }

    #endregion

    #region Private Static Methods

    private static EngineRule GetNewDefaultEngineRule()
    {
      return new EngineRule("New Item", VirtualizationType.Virtual);
    }

    #endregion

  }
}
