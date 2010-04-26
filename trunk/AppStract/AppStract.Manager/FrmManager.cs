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
using AppStract.Core;
using AppStract.Core.Data.Application;
using AppStract.Core.Virtualization.Process.Packaging;
using AppStract.Manager.Wizard;

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
      CoreBus.Log.Message("Starting wizard to create a new application package.");
      // Gather the necessary information from the user.
      var preWizard = new NewApplication();
      Hide();
      try
      {
        if (preWizard.ShowDialog() != System.Windows.Forms.DialogResult.OK)
          return;
        // Start packaging.
        PackagedApplication package;
        try
        {
          CoreBus.Log.Message("Initializing packaging logics for \"{0}\".", preWizard.Result.InstallerExecutable);
          package = new Packager(preWizard.Result.InstallerExecutable,
                                 preWizard.Result.InstallerOutputDestination).CreatePackage();
        }
        catch (PackageException ex)
        {
          CoreBus.Log.Error("Packaging failed", ex);
          MessageBox.Show("Failed to package the application.\r\nPlease check the log files for more information.",
                          "Packaging failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
          // ToDo: Clean up first!
          return;
        }
        CoreBus.Log.Message("Packaging succeeded. Starting wizard for initial configuration.");
        // Packaging succeeded, now gather the required configuration data from the user.
        var postWizard = new ApplicationSetup(package);
        if (postWizard.ShowDialog() != System.Windows.Forms.DialogResult.OK)
          return; /// ToDo: Clean up first!
        // Save the resulting data.
        var dataFilename = System.IO.Path.Combine(preWizard.Result.InstallerOutputDestination,
                                                  CoreBus.Configuration.Application.DefaultApplicationDataFile);
        if (!ApplicationData.Save(postWizard.Result, dataFilename))
          // ToDo: Add some proper error handling here!
          // ToDo: Clean up?
          MessageBox.Show("Failed to save the application data to " + dataFilename);
        // Start the application, if requested.
        if (preWizard.Result.Autostart)
          CoreManager.StartProcess(dataFilename);
      }
      finally
      {
        Show();
      }
    }

    private void _btnCleanSystem_Click(object sender, System.EventArgs e)
    {
      FrmCleanUp frm = new FrmCleanUp();
      Hide();
      frm.ShowDialog();
      Show();
    }

    #endregion

  }
}
