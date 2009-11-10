#region Copyright (C) 2009-2010 Simon Allaeys

/*
    Copyright (C) 2009-2010 Simon Allaeys
 
    This file is part of AppStract

    AppStract is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    AppStract is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with AppStract.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System.IO;
using System.Runtime.Interop.Fusion;

namespace System.Reflection.GAC
{
  /// <summary>
  /// Descriptor class for applications manipulating the GAC using an instance of <see cref="AssemblyCache"/>.
  /// </summary>
  [Serializable]
  public class InstallerDescription
  {

    #region Variables

    private readonly InstallerType _installerType;
    private readonly string _uniqueId;
    private readonly string _applicationDescription;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the type of the installer described by the current <see cref="InstallerDescription"/>.
    /// </summary>
    public InstallerType Type
    {
      get { return _installerType; }
    }

    /// <summary>
    /// Gets the unique identifier of the installer described by the current <see cref="InstallerDescription"/>.
    /// </summary>
    public string Id
    {
      get { return _uniqueId; }
    }

    /// <summary>
    /// Gets the description of the installer described by the current <see cref="InstallerDescription"/>.
    /// </summary>
    public string Description
    {
      get { return _applicationDescription; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="InstallerDescription"/> from the values specified.
    /// </summary>
    /// <param name="installerType"></param>
    /// <param name="applicationDescription"></param>
    /// <param name="uniqueId"></param>
    private InstallerDescription(InstallerType installerType, string uniqueId, string applicationDescription)
    {
      _installerType = installerType;
      _uniqueId = uniqueId;
      _applicationDescription = applicationDescription;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="InstallerDescription"/> using the data specified in the given <see cref="FusionInstallReference"/>.
    /// </summary>
    /// <param name="fusionInstallReference"></param>
    internal InstallerDescription(FusionInstallReference fusionInstallReference)
    {
      _installerType = InstallerTypeExt.FromGuid(fusionInstallReference.GuidScheme);
      _applicationDescription = fusionInstallReference.NonCannonicalData;
      _uniqueId = fusionInstallReference.Identifier;
    }

    #endregion

    #region Static Methods

    public static InstallerDescription CreateForInstaller(string installerName, string installerIdentifier)
    {
      return new InstallerDescription(InstallerType.Installer, installerIdentifier, installerName);
    }

    public static InstallerDescription CreateForFile(string description, string fileName)
    {
      if (!File.Exists(fileName))
        throw new FileNotFoundException("An instance of InstallReference can only be created for an existing file.", fileName);
      return new InstallerDescription(InstallerType.File, fileName, description);
    }

    public static InstallerDescription CreateForOpaqueString(string description, string opaqueString)
    {
      return new InstallerDescription(InstallerType.OpaqueString, opaqueString, description);
    }

    #endregion

    #region Public Methods

    public override string ToString()
    {
      return "[" + _installerType + "] " + _uniqueId;
    }

    #endregion

    #region Internal Methods

    internal FusionInstallReference ToFusionStruct()
    {
      var result = new FusionInstallReference(
        _installerType.AsGuid(),
        _uniqueId,
        _applicationDescription);
      return result;
    }

    #endregion

  }
}
