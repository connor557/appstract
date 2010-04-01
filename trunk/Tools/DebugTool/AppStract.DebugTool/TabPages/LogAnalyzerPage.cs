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
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace AppStract.DebugTool.TabPages
{
  public partial class LogAnalyzerPage : UserControl
  {

    #region Variables

    private string _originalText;

    #endregion

    #region Constructors

    public LogAnalyzerPage()
    {
      InitializeComponent();
    }

    #endregion

    #region Private Methods

    private void SwitchHandlesToNames()
    {
      _originalText = _txtContent.Text;
      var lines = _txtContent.Lines;
      var knownHandles = new Dictionary<uint, string>
                           {
                             {0x80000000, "HKEY_CLASSES_ROOT"},
                             {0x80000001, "HKEY_CURRENT_USER"},
                             {0x80000002, "HKEY_LOCAL_MACHINE"},
                             {0x80000003, "HKEY_USERS"},
                             {0x80000004, "HKEY_PERFORMANCE_DATA"},
                             {0x80000005, "HKEY_CURRENT_CONFIG"},
                             {0x80000006, "HKEY_DYN_DATA"}
                           };
      for (var i = 0; i < lines.Length; i++)
      {
        string matchingKey;
        if (!lines[i].ContainsAny(knownHandles.Keys.AsEnumerable().ToStrings(), out matchingKey))
          continue;
        lines[i] = lines[i].Replace(matchingKey + @"\\", knownHandles[uint.Parse(matchingKey)] + @"\");
        lines[i] = lines[i].Replace("HKey=" + matchingKey, "HKey=" + knownHandles[uint.Parse(matchingKey)]);
        // Add new handle, if any.
        var handleIndex = lines[i].IndexOf(" => ");
        if (handleIndex == -1) continue;
        var handleString = lines[i].Substring(handleIndex + " => ".Length);
        handleIndex = handleString.IndexOf("HKey=");
        if (handleIndex == -1) continue;
        handleString = handleString.Substring(handleIndex + "HKey=".Length);
        uint handle;
        if (!uint.TryParse(handleString, out handle)
            || knownHandles.ContainsKey(handle))
          continue;
        var openingBracketIndex = lines[i].IndexOf('(');
        if (openingBracketIndex == -1)
          continue;
        var closingBracketIndex = lines[i].IndexOf(')');
        if (closingBracketIndex == -1 || closingBracketIndex <= openingBracketIndex)
          continue;
        var newKey = lines[i].Substring(openingBracketIndex + 1, closingBracketIndex - openingBracketIndex - 1);
        if (newKey.Contains("HKey="))
          newKey = newKey.Substring("HKey=".Length).Replace(" NewSubKey=", @"\");
        knownHandles.Add(handle, newKey);
      }
      _txtContent.Lines = lines;
    }

    #endregion

    #region Private EventHandlers

    private void _btnOpen_Click(object sender, EventArgs e)
    {
      var dialog = new OpenFileDialog();
      if (dialog.ShowDialog() != DialogResult.OK)
        return;
      using (var reader = new StreamReader(dialog.FileName))
        _txtContent.Lines = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
    }

    private void _btnSave_Click(object sender, EventArgs e)
    {
      var dialog = new SaveFileDialog();
      if (dialog.ShowDialog() != DialogResult.OK)
        return;
      using (var writer = new StreamWriter(dialog.FileName))
        writer.Write(_txtContent.Text);
    }

    private void _txtContent_MouseClick(object sender, Controls.LineEventArgs e)
    {
      if (e.MouseEventArgs.Button != MouseButtons.Right)
        return;
      var line = e.LineNumber;
      var menu = new ContextMenu();
      menu.MenuItems.Add("Clicked line " + line);
      menu.Show(_txtContent, e.MouseEventArgs.Location);
    }

    private void _btnHandlesToNames_Click(object sender, EventArgs e)
    {
      _btnHandlesToNames.Checked = !_btnHandlesToNames.Checked;
      _btnHandlesToNames.Image
        = _btnHandlesToNames.Checked ? Properties.Resources.shuffle_on : Properties.Resources.shuffle_off;
      if (_btnHandlesToNames.Checked)
        SwitchHandlesToNames();
      else
        _txtContent.Text = _originalText;
    }

    #endregion


  }
}
