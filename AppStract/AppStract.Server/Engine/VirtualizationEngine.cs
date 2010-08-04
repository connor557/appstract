﻿#region Copyright (C) 2009-2010 Simon Allaeys

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
using AppStract.Core.System.IPC;
using AppStract.Server.Engine.FileSystem;
using AppStract.Server.Engine.Hooking;
using AppStract.Server.Engine.Registry;

namespace AppStract.Server.Engine
{
  /// <summary>
  /// Manages the complete virtual environment.
  /// </summary>
  public class VirtualizationEngine
  {

    #region Variables

    /// <summary>
    /// Manages the API hooks.
    /// </summary>
    private readonly HookManager _hookManager;
    /// <summary>
    /// Manages the synchronization cycles between the current process and the host process.
    /// </summary>
    private readonly SynchronizationBus _syncBus;
    /// <summary>
    /// Indicates whether or not the virtualization engine is up and running.
    /// </summary>
    private bool _isRunning;

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether or not the virtualization engine is up and running.
    /// </summary>
    public bool IsRunning
    {
      get { return _isRunning; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Private constructor, instantiates the class variables.
    /// </summary>
    /// <param name="synchronizationBus"></param>
    private VirtualizationEngine(SynchronizationBus synchronizationBus)
    {
      _hookManager = new HookManager();
      _syncBus = synchronizationBus;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Starts the virtualization engine.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if the virtualization engine is already running,
    /// which can be checked with the <see cref="IsRunning"/> property.
    /// </exception>
    /// <returns>Whether or not the virtualization engine is running.</returns>
    public bool StartEngine()
    {
      if (_isRunning)
        throw new ApplicationException("The virtualization engine is already running.");
      _isRunning = true;
      _syncBus.AutoFlush = true;
      try
      {
        _hookManager.InstallHooks();
      }
      catch (HookingException e)
      {
        GuestCore.Log.Critical("Failed to start the virtualization engine.", e);
        return false;
      }
      return true;
    }

    /// <summary>
    /// Ensures that the current thread won't be intercepted anymore by the virtualization engine.
    /// </summary>
    /// <remarks>
    /// The hooking exclusion only works for the current underlying native thread of the current managed thread,
    /// it is not guaranteed that the underlying native thread will never change. Therefore it is recommended
    /// that the returned object is disposed as soon as the application logic allows it.
    /// <br />
    /// A thread can call this method recursively. The hook exclusion is undone when all of the returned objects are disposed.
    /// </remarks>
    /// <returns>An object that, when disposed, will reactivate the virtualization engine for the current thread.</returns>
    public IDisposable GetEngineProcessingSpace()
    {
      return _hookManager.ThreadACL.GetHookingExclusion();
    }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Initializes a new instance of <see cref="VirtualizationEngine"/>.
    /// </summary>
    /// <param name="resourceSynchronizer">
    /// The <see cref="ISynchronizer"/> to use for synchronization between the current (guest) process and the host process.
    /// </param>
    /// <param name="resourceLoader">
    /// The <see cref="IResourceLoader"/> to use for loading the required resources.
    /// </param>
    /// <returns></returns>
    public static VirtualizationEngine InitializeEngine(ISynchronizer resourceSynchronizer, IResourceLoader resourceLoader)
    {
      var syncBus = new SynchronizationBus(resourceSynchronizer, resourceLoader);
      var engine = new VirtualizationEngine(syncBus);
      var fileSystemProvider = new FileSystemProvider(syncBus, resourceLoader.FileSystemRoot);
      var registryProvider = new RegistryProvider(syncBus);
      engine._hookManager.RegisterHookProvider(new FileSystemHookProvider(fileSystemProvider));
      engine._hookManager.RegisterHookProvider(new RegistryHookProvider(registryProvider));
      return engine;
    }

    #endregion

  }
}
