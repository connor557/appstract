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
using AppStract.Manager.Packaging.PreConfiguration;
using AppStract.Utilities.GUI.Wizard;

namespace AppStract.Manager.Packaging
{

  /// <summary>
  /// A wizard to guide the user through the installation of his new portable application.
  /// </summary>
  public class PrePackagingWizard : GenericWizard<PreConfigurationState>
  {

    #region Variables

    /// <summary>
    /// The state of the current wizard.
    /// </summary>
    private readonly PreConfigurationState _state;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the result of the current wizard.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An ApplicationException is thrown when the result is accessed before the wizard has finished.
    /// -OR-
    /// An ApplicationException is thrown when the result is accessed if the wizard hasn't successfully finished.
    /// </exception>
    public PreConfigurationState Result
    {
      get
      {
        if (DialogResult != System.Windows.Forms.DialogResult.OK)
          throw new ApplicationException("The result of the NewAppWizard can't be acquired as long as the wizard isn't finished");
        return _state;
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="PrePackagingWizard"/>.
    /// </summary>
    public PrePackagingWizard()
    {
      _state = new PreConfigurationState();
    }

    #endregion

    #region Protected Methods

    protected override IList<WizardPage> WizardPages()
    {
      List<WizardPage> pages = new List<WizardPage>(5);
      pages.Add(new WizardPage("Introduction", new WizardIntroduction()));
      pages.Add(new WizardPage("Specify Installer", new WizardSelectInstaller(_state)));
      pages.Add(new WizardPage("Finish", new WizardFinish(_state)));
      return pages;
    }

    #endregion

  }
}
