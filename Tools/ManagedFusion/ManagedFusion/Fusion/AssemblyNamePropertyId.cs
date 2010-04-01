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

namespace ManagedFusion.Fusion
{
  /// <summary>
  /// Defines the valid names of the name-value pairs in an assembly name.
  /// </summary>
  /// <remarks>
  /// The ASM_NAME enumeration property ID describes the valid names of the name-value pairs in an assembly name.
  /// See the .NET Framework SDK for a description of these properties: <see cref="http://msdn.microsoft.com/en-us/library/aa374201(VS.85).aspx"/>
  /// </remarks>
  internal enum AssemblyNamePropertyId
  {
    /// <summary>
    /// Property ID for the assembly's public key.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_PUBLIC_KEY
    /// Value type: byte array
    /// </remarks>
    PublicKey = 0,
    /// <summary>
    /// Property ID for the assembly's public key token.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_PUBLIC_KEY_TOKEN
    /// Value type: byte array
    /// </remarks>
    PublicKeyToken,
    /// <summary>
    /// Property ID for a reserved name-value pair.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_HASH_VALUE
    /// Value type: byte array
    /// </remarks>
    HashValue,
    /// <summary>
    /// Property ID for the assembly's simple name.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_NAME
    /// Value type: string value
    /// </remarks>
    Name,
    /// <summary>
    /// Property ID for the assembly's major version.
    /// </summary>
    /// <remarks>
    /// Native Name: ASM_NAME_MAJOR_VERSION
    /// Value type: WORD value
    /// </remarks>
    MajorVersion,
    /// <summary>
    /// Property ID for the assembly's minor version.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_MINOR_VERSION
    /// Value type: WORD value
    /// </remarks>
    MinorVersion,
    /// <summary>
    /// Property ID for the assembly's build version.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_BUILD_NUMBER
    /// Value type: WORD value
    /// </remarks>
    BuildNumber,
    /// <summary>
    /// Property ID for the assembly's revision version.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_REVISION_NUMBER
    /// Value type: WORD value
    /// </remarks>
    RevisionNumber,
    /// <summary>
    /// Property ID for the assembly's culture.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_CULTURE
    /// Value type: string value
    /// </remarks>
    Culture,
    /// <summary>
    /// Property ID for a reserved name-value pair.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_PROCESSOR_ID_ARRAY
    /// Value type: undocumented
    /// </remarks>
    ProcessorIdArray,
    /// <summary>
    /// Property ID for a reserved name-value pair.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_OSINFO_ARRAY
    /// Value type: undocumented
    /// </remarks>
    OSInfoArray,
    /// <summary>
    /// Property ID for a reserved name-value pair.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_HASH_ALGID
    /// Value type: DWORD value
    /// </remarks>
    HashAlgorithmId,
    /// <summary>
    /// Property ID for a reserved name-value pair.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_ALIAS
    /// Value type: undocumented
    /// </remarks>
    Alias,
    /// <summary>
    /// Property ID for a reserved name-value pair.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_CODEBASE_URL
    /// Value type: undocumented
    /// </remarks>
    CodebaseUrl,
    /// <summary>
    /// Property ID for a reserved name-value pair.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_CODEBASE_LASTMOD
    /// Value type: <see cref="System.Runtime.InteropServices.ComTypes.FILETIME"/> structure
    /// </remarks>
    CodebaseLastMod,
    /// <summary>
    /// Property ID for the assembly as a simply named assembly that does not have a public key.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_NULL_PUBLIC_KEY
    /// Value type: undocumented
    /// </remarks>
    NullPublicKey,
    /// <summary>
    /// Property ID for the assembly as a simply named assembly that does not have a public key token.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_NULL_PUBLIC_KEY_TOKEN
    /// Value type: undocumented
    /// </remarks>
    NullPublicKeyToken,
    /// <summary>
    /// Property ID for a reserved name-value pair.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_CUSTOM
    /// Value type: string value
    /// </remarks>
    Custom,
    /// <summary>
    /// Property ID for a reserved name-value pair.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_NULL_CUSTOM
    /// Value type: undocumented
    /// </remarks>
    NullCustom,
    /// <summary>
    /// Property ID for a reserved name-value pair.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_MVID
    /// Value type: undocumented
    /// </remarks>
    ModuleVersionId,
    /// <summary>
    /// Reserved.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_NAME_MAX_PARAMS
    /// Value type: undocumented
    /// </remarks>
    MaxParameters
  }
}
