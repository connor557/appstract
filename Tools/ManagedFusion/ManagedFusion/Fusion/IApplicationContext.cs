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

using System.Runtime.InteropServices;

namespace ManagedFusion.Fusion
{
  /// <summary>
  /// Undocumented.
  /// </summary>
  [ComImport, Guid("7C23FF90-33AF-11D3-95DA-00A024A85B51"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IApplicationContext
  {
    void SetContextNameObject(
      IAssemblyName pName);

    void GetContextNameObject(
      out IAssemblyName ppName);

    void Set(
      [MarshalAs(UnmanagedType.LPWStr)] string szName,
      int pvValue,
      uint cbValue,
      uint dwFlags);
    
    void Get(
      [MarshalAs(UnmanagedType.LPWStr)] string szName,
      out int pvValue,
      ref uint pcbValue,
      uint dwFlags);
    
    void GetDynamicDirectory(
      [Out] out int wzDynamicDir,
      ref uint pdwSize);
  }
}
