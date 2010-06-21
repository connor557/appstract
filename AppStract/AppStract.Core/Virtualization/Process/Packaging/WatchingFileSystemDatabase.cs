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

using System.Collections.Generic;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Engine.FileSystem;

namespace AppStract.Core.Virtualization.Process.Packaging
{
  /// <summary>
  /// Watches the incoming <see cref="DatabaseAction{T}"/> for variables needed to created
  /// an instance of <see cref="PackagedApplication"/> when packaging has completed.
  /// </summary>
  internal sealed class WatchingFileSystemDatabase : FileSystemDatabase
  {

    #region Variables

    private readonly List<FileTableEntry> _executables;
    private readonly object _listExecutablesSyncLock;

    #endregion

    #region Properties

    /// <summary>
    /// All detected executables.
    /// </summary>
    public IEnumerable<FileTableEntry> Executables
    {
      get { return _executables; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="WatchingFileSystemDatabase"/>.
    /// </summary>
    /// <param name="connectionString">The connectionstring to use.</param>
    public WatchingFileSystemDatabase(string connectionString)
      : base(connectionString)
    {
      ItemEnqueued += InstallerFileSystemDatabase_ItemEnqueued;
      _executables = new List<FileTableEntry>();
      _listExecutablesSyncLock = new object();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a <see cref="WatchingFileSystemDatabase"/> initialized with the default connectionstring.
    /// </summary>
    /// <param name="filename">The file that will contain the data of the returned <see cref="WatchingFileSystemDatabase"/>.</param>
    /// <returns>A <see cref="WatchingFileSystemDatabase"/> initialized with the default connectionstring.</returns>
    public new static WatchingFileSystemDatabase CreateDefaultDatabase(string filename)
    {
      var fs = FileSystemDatabase.CreateDefaultDatabase(filename);
      return new WatchingFileSystemDatabase(fs.ConnectionString);
    }

    /// <summary>
    /// Returns a string representation of the current <see cref="WatchingFileSystemDatabase"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "[WatchingFileSystemDatabase] " + _connectionString;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// EventHandler for newly enqueued items.
    /// Checks if the <paramref name="item"/> will be needed to create a <see cref="PackagedApplication"/>,
    /// if so it is stored so it can be retrieved when packaging has completed.
    /// </summary>
    /// <param name="sender">The sender of the event, should be the current instance.</param>
    /// <param name="item">Item to check.</param>
    private void InstallerFileSystemDatabase_ItemEnqueued(object sender, DatabaseAction<FileTableEntry> item)
    {
      if (item.ActionType == DatabaseActionType.Set
          || !item.Item.Value.ToUpperInvariant().EndsWith(".EXE"))
        return;
      lock (_listExecutablesSyncLock)
      {
        var itemIndex = _executables.IndexOf(item.Item);
        switch (item.ActionType)
        {
          case DatabaseActionType.Set:
            if (itemIndex == -1)
              _executables.Add(item.Item);
            else
              _executables[itemIndex] = item.Item;
            break;
          case DatabaseActionType.Remove:
            _executables.Remove(item.Item);
            break;
        }
      }
    }

    #endregion

  }
}
