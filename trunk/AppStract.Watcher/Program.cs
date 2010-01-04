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
using System.Diagnostics;
using System.Reflection.GAC;

namespace AppStract.Watcher
{
  class Program
  {

    /// <summary>
    /// The parameters provided to the current watcher.
    /// </summary>
    private static Parameters _parameters;
    /// <summary>
    /// The <see cref="CleanUpInsurance"/> for which the current watcher is created.
    /// </summary>
    private static CleanUpInsurance _cleanUpInsurance;

    /// <summary>
    /// Main entry point for <see cref="AppStract.Watcher"/> processes.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
      _parameters = new Parameters(args);
      ReportMessage("Watching with the following parameters:");
      ReportMessage("  Flags\t" + _parameters.Flags);
      ReportMessage("  File\t\t" + _parameters.InsuranceFile);
      ReportMessage("  Registry\t" + _parameters.InsuranceRegistryKey);
      ReportMessage("  Insurance\t" + _parameters.InsuranceId);
      ReportMessage("  Process\t" + _parameters.ProcessId);
      if (!_parameters.Flags.IsSpecified(CleanUpInsuranceFlags.ByWatchService))
      {
        ReportMessage("A watch service is not required");
#if DEBUG
        ReportMessage("Press any key to exit...");
        Console.ReadKey();
#endif
        return;
      }
      if (_parameters.InsuranceId != Guid.Empty)
        _cleanUpInsurance = CleanUpInsurance.LoadFromSystem(_parameters.InsuranceFile, _parameters.InsuranceRegistryKey,
                                                            _parameters.InsuranceId);
      if (_cleanUpInsurance == null)
      {
        ReportMessage("Can't read the required data for IID." + _parameters.InsuranceId +
                          " from \"" + _parameters.InsuranceFile + "\" or \"" + _parameters.InsuranceRegistryKey + "\"");
#if DEBUG
        ReportMessage("Press any key to exit...");
        Console.ReadKey();
#endif
        return;
      }
      ReportMessage("The insurance has been read from the system");
      // If allowed, clean up the file or registry key used to pass data to the current watcher.
      _cleanUpInsurance.Dispose((!_parameters.Flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile)
                                   ? CleanUpInsuranceFlags.TrackByFile
                                   : CleanUpInsuranceFlags.None)
                                | (!_parameters.Flags.IsSpecified(CleanUpInsuranceFlags.TrackByRegistry)
                                     ? CleanUpInsuranceFlags.TrackByRegistry
                                     : CleanUpInsuranceFlags.None));
      ReportMessage("tRetrieving a handle for the process with PID." + _parameters.ProcessId);
      var process = Process.GetProcessById(_parameters.ProcessId);
      ReportMessage("tWaiting for the process to exit...");
      process.WaitForExit();
      ReportMessage("Process has exited");
      ReportMessage("Invoking cleanup procedure...");
      try
      {
        var cache = new AssemblyCache(_cleanUpInsurance.Installer);
        foreach (var assembly in _cleanUpInsurance.Assemblies)
        {
          var disposition = cache.UninstallAssembly(assembly);
          ReportMessage("  [" + disposition + "]  " + assembly.FullName);
        }
        ReportMessage("Finished cleanup procedure");
      }
      catch (UnauthorizedAccessException e)
      {
        ReportMessage("FAILED to uninstall any of the following assemblies...");
        foreach (var assembly in _cleanUpInsurance.Assemblies)
          ReportMessage("  " + assembly.FullName);
        ReportMessage("\n" + e + "\n");
#if DEBUG
        ReportMessage("Press any key to exit...");
        Console.ReadKey();
#endif
        return;
      }
      ReportMessage("Disposing insurance...");
      _cleanUpInsurance.Dispose();
      ReportMessage("Insurance is disposed");
      ReportMessage("Exiting...");
#if DEBUG
      ReportMessage("Press any key to exit...");
      Console.ReadKey();
#endif
    }

    /// <summary>
    /// Reports a message to the user.
    /// </summary>
    /// <param name="message"></param>
    private static void ReportMessage(string message)
    {
      Console.WriteLine(DateTime.Now + "  " + message);
    }

  }
}
