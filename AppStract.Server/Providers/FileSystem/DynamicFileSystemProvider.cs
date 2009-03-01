﻿#region Copyright (C) 2008-2009 Simon Allaeys

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

using System.Timers;
using AppStract.Server.Communication;
using AppStract.Server.FileSystem;


namespace AppStract.Server.Providers.FileSystem
{
  /// <summary>
  /// <see cref="DynamicFileSystemProvider"/> extends <see cref="FileSystemProvider"/>
  /// by having the ability to release the virtual filesystem after a specified time interval.
  /// </summary>
  public class DynamicFileSystemProvider : FileSystemProvider
  {

    #region Variables

    private bool _released;
    private readonly Timer _releaseTimer;

    #endregion

    #region Properties

    public bool Released
    {
      get { return _released; }
      set { _released = value; }
    }

    #endregion

    #region Constructors

    public DynamicFileSystemProvider(string currentDirectory, IResourceSynchronizer resourceSynchronizer)
      : base(currentDirectory, resourceSynchronizer)
    {
      _released = false;
      _releaseTimer = new Timer(15000);
      _releaseTimer.Elapsed += ReleaseVirtualFileSystem;
    }

    #endregion

    #region Public Methods

    public void Reset()
    {
      _releaseTimer.Enabled = false;
      _releaseTimer.Enabled = true;
    }

    public void Reset(double releaseAfter)
    {
      _releaseTimer.Enabled = false;
      if (releaseAfter < 0)
        return;
      _releaseTimer.Interval = releaseAfter;
      _releaseTimer.Enabled = true;
    }

    public override FileTableEntry GetFile(FileRequest fileRequest)
    {
      if (!_released)
        return base.GetFile(fileRequest);
      return new FileTableEntry(fileRequest.FileName, fileRequest.FileName);
    }

    #endregion

    #region Private Methods

    private void ReleaseVirtualFileSystem(object sender, ElapsedEventArgs e)
    {
      _releaseTimer.Enabled = false;
      _released = true;
    }

    #endregion

  }
}