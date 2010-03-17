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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using AppStract.Utilities.ManagedFusion.Fusion;

namespace AppStract.Utilities.ManagedFusion
{
  internal class AssemblyEnumerator : IEnumerator<AssemblyName>
  {

    #region DLL Imports

    /// <summary>
    /// To obtain an instance of the CreateAssemblyEnum API, call the CreateAssemblyNameObject API.
    /// </summary>
    /// <param name="pEnum">Pointer to a memory location that contains the IAssemblyEnum pointer.</param>
    /// <param name="pUnkReserved">Must be null.</param>
    /// <param name="pName">An assembly name that is used to filter the enumeration. Can be null to enumerate all assemblies in the GAC.</param>
    /// <param name="dwFlags">Exactly one bit from the ASM_CACHE_FLAGS enumeration.</param>
    /// <param name="pvReserved">Must be NULL.</param>
    [DllImport("fusion.dll", SetLastError = true, PreserveSig = false)]
    static extern void CreateAssemblyEnum(out IAssemblyEnum pEnum, IntPtr pUnkReserved, IAssemblyName pName,
      AssemblyCacheFlags dwFlags, IntPtr pvReserved);

    #endregion

    #region Variables

    private readonly IAssemblyEnum _enum;
    private AssemblyName _current;

    #endregion

    #region Constructors

    public AssemblyEnumerator()
    {
      CreateAssemblyEnum(out _enum, (IntPtr)0, null, AssemblyCacheFlags.GAC, (IntPtr)0);
    }

    #endregion

    #region IEnumerator<AssemblyName> Members

    public AssemblyName Current
    {
      get { return _current; }
    }

    #endregion

    #region IEnumerator Members

    object System.Collections.IEnumerator.Current
    {
      get { return _current; }
    }

    public bool MoveNext()
    {
      IAssemblyName ae;
      IApplicationContext context;
      var hResult = _enum.GetNextAssembly(out context, out ae, 0);
      _current = ae != null && hResult == 0 ? ae.ToAssemblyName() : null;
      return _current != null;
    }

    public void Reset()
    {
      _enum.Reset();
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {

    }

    #endregion

  }
}
