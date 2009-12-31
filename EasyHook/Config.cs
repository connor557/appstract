/*
    EasyHook - The reinvention of Windows API hooking
 
    Copyright (C) 2009 Christoph Husse

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

    Please visit http://www.codeplex.com/easyhook for more information
    about the project and latest updates.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography;

namespace EasyHook
{
  /// <summary>
  /// Currently only provides a mechanism to register assemblies in the GAC.
  /// </summary>
  /// <include file='FileMonHost.xml' path='remarks'/>
  public static class Config
  {

    #region Properties

    /// <summary>
    /// Gets or sets the log service used by the EasyHook library.
    /// </summary>
    public static IEasyLog Log { get; set; }

    #endregion

    #region Constructors

    static Config()
    {
      Log = new LogService();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// REQUIRES ADMIN PRIVILEGES. Installs EasyHook and all given user NET assemblies into the GAC and
    /// ensures that all references are cleaned up if the installing application
    /// is shutdown. Cleanup does not depend on the calling application...
    /// </summary>
    /// <remarks>
    /// <para>
    /// ATTENTION: There are some problems when debugging processes whose libraries
    /// are added to the GAC. Visual Studio won't start the debug session! There is
    /// only one chance for you to workaround this issue if you want to install
    /// libraries AND debug them simultanously. This is simply to debug only one process
    /// which is the default setting of Visual Studio. Because the libraries are added
    /// to the GAC AFTER Visual Studio has initialized the debug session, there won't be
    /// any conflicts; at least so far...
    /// </para><para>
    /// In debug versions of EasyHook, you may also check the "Application" event log, which holds additional information
    /// about the GAC registration, after calling this method. In general this method works
    /// transactionally. This means if something goes wrong, the GAC state of all related libraries
    /// won't be violated!
    /// </para><para>
    /// The problem with NET assemblies is that the CLR only searches the GAC and
    /// directories starting with the application base directory for assemblies.
    /// To get injected assemblies working either all of them have to be located 
    /// under the target base directory (which is not suitable in most cases) or
    /// reside in the GAC. 
    /// </para><para>
    /// EasyHook provides a way to automatically register all of its own assemblies
    /// and custom ones temporarily in the GAC. It also ensures
    /// that all of these assemblies are removed if the installing process exists. 
    /// So you don't need to care about and may write applications according to
    /// the XCOPY standard. If your application ships with an installer, you may
    /// statically install all of your assemblies and the ones of EasyHook into the
    /// GAC. In this case just don't call <see cref="Register"/>. 
    /// </para><para>
    /// Of course EasyHook does also take care of multiple processes using the same
    /// injection libraries. So if two processes are sharing some of those DLLs,
    /// a stable reference counter ensures that the libraries are kept in the GAC
    /// if one process is terminated while the other continues running and so continues
    /// holding a proper GAC reference.
    /// </para><para>
    /// Please note that in order to add your library to the GAC, it has to be a valid
    /// NET assembly and expose a so called "Strong Name". Assemblies without a strong
    /// name will be rejected by this method!
    /// </para>
    /// </remarks>
    /// <param name="InDescription">
    /// A description under which the installed files should be referenced. 
    /// </param>
    /// <param name="InUserAssemblies">
    /// A list of user assemblies as relative or absolute paths. 
    /// </param>
    /// <exception cref="System.IO.FileNotFoundException">
    /// At least one of the files specified could not be found!
    /// </exception>
    /// <exception cref="BadImageFormatException">
    /// Unable to load at least one of the given files for reflection. 
    /// </exception>
    /// <exception cref="ArgumentException">
    /// At least one of the given files does not have a strong name.
    /// </exception>
    public static void Register(
        String InDescription,
        params String[] InUserAssemblies)
    {
      List<Assembly> AsmList = new List<Assembly>();
      String RemovalList = "";
      List<String> InstallList = new List<String>();

      /*
       * Read and validate config file...
       */
      List<String> Files = new List<string>();

      Files.Add(typeof(Config).Assembly.Location);

      Files.AddRange(InUserAssemblies);

      for (int i = 0; i < Files.Count; i++)
      {
        Assembly Entry;
        String AsmPath = Path.GetFullPath(Files[i]);

        if (!File.Exists(AsmPath))
          throw new FileNotFoundException("The given assembly \"" + Files[i] + "\" (\"" + AsmPath + "\") does not exist.");

        // just validate that this is a NET assembly with valid metadata...
        try { Entry = Assembly.ReflectionOnlyLoadFrom(AsmPath); }
        catch (Exception ExtInfo)
        {
          throw new BadImageFormatException("Unable to load given assembly \"" + Files[i] + "\" (\"" + AsmPath +
              "\") for reflection. Is this a valid NET assembly?", ExtInfo);
        }

        // is strongly named? (required for GAC)
        AssemblyName Name = AssemblyName.GetAssemblyName(AsmPath);

        if ((Name.Flags & AssemblyNameFlags.PublicKey) == 0)
          throw new ArgumentException("The given user library \"" + Files[i] + "\" is not strongly named!");

        AsmList.Add(Entry);
        InstallList.Add(AsmPath);

        RemovalList += " \"" + Name.Name + "\"";
      }

      /*
       * Install assemblies into GAC ...
       */

      // create unique installation identifier
      byte[] id = new Byte[30];

      new RNGCryptoServiceProvider().GetBytes(id);

      // run cleanup service
      InDescription = InDescription.Replace('"', '\'');

      RunCommand(
        "GACRemover", false, false, "EasyHook32Svc.exe",
        Process.GetCurrentProcess().Id + " \"" + Convert.ToBase64String(id) + "\" \"" + InDescription + "\"" + RemovalList);

      // install assemblies
      IntPtr GacContext = NativeAPI.GacCreateContext();

      try
      {
        foreach (String Assembly in InstallList)
        {
          NativeAPI.GacInstallAssembly(
              GacContext,
              Assembly,
              InDescription,
              Convert.ToBase64String(id));
        }
      }
      finally
      {
        NativeAPI.GacReleaseContext(ref GacContext);
      }
    }

    #endregion

    #region Private Methods

    private static void RunCommand(
        String InFriendlyName,
        Boolean InWaitForExit,
        Boolean InShellExecute,
        String InPath,
        String InArguments)
    {
      InPath = Path.GetFullPath(InPath);

      Process Proc = new Process();
      ProcessStartInfo StartInfo = new ProcessStartInfo(InPath, InArguments);

      if (InShellExecute && InWaitForExit)
        throw new ArgumentException("It is not supported to execute in shell and wait for termination!");

      StartInfo.RedirectStandardOutput = !InShellExecute;
      StartInfo.UseShellExecute = InShellExecute;
      StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
      StartInfo.CreateNoWindow = true;
      StartInfo.WorkingDirectory = Path.GetDirectoryName(InPath);

      Proc.StartInfo = StartInfo;

      try
      {
        if (!Proc.Start())
          throw new Exception();
      }
      catch (Exception ExtInfo)
      {
        throw new ApplicationException("Unable to run internal command.", ExtInfo);
      }

      if (InWaitForExit)
      {
        if (!Proc.WaitForExit(5000))
        {
          Proc.Kill();

          throw new ApplicationException("Unable to run internal command.");
        }

        // save to log entry
        String Output = Proc.StandardOutput.ReadToEnd();

        Log.Information("[" + InFriendlyName + "]: " + Output);
      }
    }

    #endregion

  }
}
