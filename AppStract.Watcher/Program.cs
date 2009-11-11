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

    private static Parameters _parameters;
    private static CleanUpInsurance _cleanUpInsurance;

    static void Main(string[] args)
    {
      _parameters = new Parameters(args);
      Console.WriteLine(DateTime.Now + "\tWatching with the following parameters:");
      Console.WriteLine("   Flags\t" + _parameters.Flags);
      Console.WriteLine("   File\t\t" + _parameters.InsuranceFile);
      Console.WriteLine("   Registry\t" + _parameters.InsuranceRegistryKey);
      Console.WriteLine("   Insurance\t" + _parameters.InsuranceId);
      Console.WriteLine("   Process\t" + _parameters.ProcessId);
      if (!_parameters.Flags.IsSpecified(CleanUpInsuranceFlags.ByWatchService))
      {
        Console.WriteLine(DateTime.Now + "\tA watch service is not required");
#if DEBUG
        Console.WriteLine(DateTime.Now + "\tPress any key to exit...");
        Console.ReadKey();
#endif
        return;
      }
      _cleanUpInsurance = CleanUpInsurance.LoadFromSystem(_parameters.InsuranceFile, _parameters.InsuranceRegistryKey,
                                                          _parameters.InsuranceId);
      if (_cleanUpInsurance == null)
      {
        Console.WriteLine(DateTime.Now + "\tCan't read the required data for IID." + _parameters.InsuranceId +
                          " from \"" + _parameters.InsuranceFile + "\"");
#if DEBUG
        Console.WriteLine(DateTime.Now + "\tPress any key to exit...");
        Console.ReadKey();
#endif
        return;
      }
      // Check if ByWatchService is the only flag specified.
      // If so, clean up the file or registry key used to pass data to the current watcher.
      if (!_parameters.Flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile)
          && !_parameters.Flags.IsSpecified(CleanUpInsuranceFlags.TrackByRegistry))
        _cleanUpInsurance.Dispose();
      Console.WriteLine(DateTime.Now + "\tRetrieving a handle for the process with PID." + _parameters.ProcessId);
      var process = Process.GetProcessById(_parameters.ProcessId);
      Console.WriteLine(DateTime.Now + "\tWaiting for the process to exit...");
      process.WaitForExit();
      Console.WriteLine(DateTime.Now + "\tProcess has exited");
      Console.WriteLine(DateTime.Now + "\tInvoking cleanup procedure...");
      try
      {
        var cache = new AssemblyCache(_cleanUpInsurance.Installer);
        foreach (var assembly in _cleanUpInsurance.Assemblies)
        {
          var disposition = cache.UninstallAssembly(assembly);
          Console.WriteLine("   [" + disposition + "]\t" + assembly.FullName);
        }
        Console.WriteLine(DateTime.Now + "\tFinished cleanup procedure");
      }
      catch (UnauthorizedAccessException e)
      {
        Console.WriteLine(DateTime.Now + "\tFAILED to uninstall any of the following assemblies...");
        foreach (var assembly in _cleanUpInsurance.Assemblies)
          Console.WriteLine("   " + assembly.FullName);
        Console.WriteLine("\n" + e + "\n");
#if DEBUG
        Console.WriteLine(DateTime.Now + "\tPress any key to exit...");
        Console.ReadKey();
#endif
        return;
      }
      Console.WriteLine(DateTime.Now + "\tDisposing insurance...");
      _cleanUpInsurance.Dispose();
      Console.WriteLine(DateTime.Now + "\tInsurance is disposed");
      Console.WriteLine(DateTime.Now + "\tExiting...");
#if DEBUG
      Console.WriteLine(DateTime.Now + "\tPress any key to exit...");
      Console.ReadKey();
#endif
    }

  }
}
