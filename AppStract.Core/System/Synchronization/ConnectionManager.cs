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

using System.Runtime.Remoting;
using EasyHook;

namespace AppStract.Core.System.Synchronization
{
  /// <summary>
  /// <see cref="ConnectionManager"/> manages the connection used for inter-process communication
  /// between the current server process and a guest process.
  /// </summary>
  public class ConnectionManager
  {

    #region Variables

    /// <summary>
    /// Flag indicating whether <see cref="Initialize"/> is called.
    /// </summary>
    private bool _initialized;
    /// <summary>
    /// The object responsible for the synchronization
    /// between the server process and the virtualized process.
    /// </summary>
    private readonly IProcessSynchronizer _processSynchronizer;
    /// <summary>
    /// Name of the remoting-channel, created by RemoteHooking.IpcCreateServer
    /// </summary>
    private string _channelName;
    /// <summary>
    /// Mutex for synchronization between threads performing actions on the current instance.
    /// </summary>
    private readonly object _syncRoot;

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether the connection is initialized and can be used for IPC.
    /// </summary>
    public bool Initialized
    {
      get { return _initialized; }
    }

    /// <summary>
    /// Name of the remoting-channel connecting the guest process to the server process.
    /// </summary>
    /// <exception cref="CoreException">
    /// A <see cref="CoreException"/> is thrown if the channel name is requested before the <see cref="Initialize"/> method is called.
    /// </exception>
    public string ChannelName
    {
      get
      {
        if (!_initialized)
          throw new CoreException("The resources have to be initialized before a channel name can be provided.");
        return _channelName;
      }
    }

    /// <summary>
    /// Gets the <see cref="IProcessSynchronizer"/> providing synchronization logics
    /// between the server and the virtualized guest process.
    /// </summary>
    public IProcessSynchronizer ProcessSynchronizer
    {
      get { return _processSynchronizer; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="ConnectionManager"/>.
    /// Before the underlying IPC-channel can be used, <see cref="Initialize"/> must be called.
    /// </summary>
    /// <param name="processSynchronizer"></param>
    public ConnectionManager(IProcessSynchronizer processSynchronizer)
    {
      _syncRoot = new object();
      _processSynchronizer = processSynchronizer;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the connection between the server and guest process.
    /// </summary>
    public void Initialize()
    {
      lock (_syncRoot)
      {
        if (_initialized) return;
        ProcessSynchronizerInterface.SProcessSynchronizer = _processSynchronizer;
        RemoteHooking.IpcCreateServer<ProcessSynchronizerInterface>(ref _channelName, WellKnownObjectMode.Singleton);
        _initialized = true;
      }
    }

    #endregion

  }
}