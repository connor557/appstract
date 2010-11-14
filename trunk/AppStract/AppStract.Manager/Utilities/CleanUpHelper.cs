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
using System.Linq;
using System.Windows.Forms;
using AppStract.Host;
using AppStract.Utilities.ManagedFusion.Insuring;

namespace AppStract.Manager.Utilities
{
  public partial class CleanUpHelper : Form
  {

    public CleanUpHelper()
    {
      InitializeComponent();
      foreach (var insurance
        in CleanUpInsurance.LoadFromSystem(HostCore.Configuration.Application.GacCleanUpInsuranceFolder,
                                           HostCore.Configuration.Application.GacCleanUpInsuranceRegistryKey))
        _listInsurances.Items.Add(insurance, true);
    }

    private void FrmCleanUp_Shown(object sender, EventArgs e)
    {
      if (_listInsurances.Items.Count != 0)
        return;
      MessageBox.Show("No leaked resources found on your machine.", "No resources leaked", MessageBoxButtons.OK);
      DialogResult = DialogResult.OK;
      Close();
    }

    private void Cancel(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void CleanSelected(object sender, EventArgs e)
    {
      var indices = new int[_listInsurances.CheckedIndices.Count];
      _listInsurances.CheckedIndices.CopyTo(indices, 0);
      foreach (var insurance in _listInsurances.CheckedItems.OfType<CleanUpInsurance>().Where(i => i != null))
        insurance.Dispose(true);
      MessageBox.Show("Selected items are cleaned.", "Done!", MessageBoxButtons.OK);
      DialogResult = DialogResult.OK;
      Close();
    }

  }
}
