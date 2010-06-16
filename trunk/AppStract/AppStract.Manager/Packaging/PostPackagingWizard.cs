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

using System;
using System.Collections.Generic;
using AppStract.Core.Data.Application;
using AppStract.Core.Virtualization.Process.Packaging;
using AppStract.Manager.Packaging.PostConfiguration;
using AppStract.Manager.Wizard;

namespace AppStract.Manager.Packaging
{

  /// <summary>
  /// A wizard to guide the user through the setup of his new portable application.
  /// </summary>
  public class PostPackagingWizard : GenericWizard<PostConfigurationState>
  {

    #region Variables

    /// <summary>
    /// The current state of the wizard.
    /// </summary>
    private readonly PostConfigurationState _state;
    /// <summary>
    /// The <see cref="PackagedApplication"/> to base the wizard on.
    /// </summary>
    private readonly PackagedApplication _installerResult;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the result of the current wizard.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// <exception cref="ApplicationException">
    /// An ApplicationException is thrown when the result is accessed before the wizard has finished.
    /// -OR-
    /// An ApplicationException is thrown when the result is accessed if the wizard hasn't successfully finished.
    /// </exception>
    /// </exception>
    public ApplicationData Result
    {
      get
      {
        if (DialogResult != System.Windows.Forms.DialogResult.OK)
          throw new ApplicationException("The result of the NewAppWizard can't be acquired as long as the wizard isn't finished");
        var data = new ApplicationData();
        data.Files.Executable = new ApplicationFile(_state.Executable);
        data.Files.RootDirectory = new ApplicationFile("");
        data.Files.DatabaseFileSystem = new ApplicationFile(_installerResult.FileSystemDatabase);
        data.Files.DatabaseRegistry = new ApplicationFile(_installerResult.RegistryDatabase);
        return data;
      }
    }

    /// <summary>
    /// Gets the current state of the wizard.
    /// </summary>
    public PostConfigurationState State
    {
      get { return _state; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="PostPackagingWizard"/>.
    /// </summary>
    /// <param name="packagedApplication"></param>
    public PostPackagingWizard(PackagedApplication packagedApplication)
    {
      _installerResult = packagedApplication;
      _state = new PostConfigurationState();
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Initializes the UserControls by setting the _steps variable.
    /// </summary>
    protected override IList<WizardPage> WizardPages()
    {
      List<WizardPage> pages = new List<WizardPage>(5);
      pages.Add(new WizardPage("Introduction", new WizardIntroduction()));
      pages.Add(new WizardPage("Executable", new WizardSelectExecutable(_installerResult.Executables, _state)));
      //pages.Add(new WizardPage("Settings", new WizardSettings(_state)));
      pages.Add(new WizardPage("Finish", new WizardFinish(_state)));
      return pages;
    }

    #endregion

  }
}
