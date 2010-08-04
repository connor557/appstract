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

#if !SYNCLOG
using System.Collections.Generic;
using System.Timers;
#endif
using AppStract.Core.System.IPC;
using AppStract.Core.System.Logging;

namespace AppStract.Server
{
  /// <summary>
  /// A <see cref="Logger"/> which sends all received messages to the host process,
  /// by using the <see cref="IServerReporter"/> specified in the constructor.
  /// </summary>
  /// <remarks>
  /// The implementation depends on whether or not the library is compiled with the "SYNCLOG" symbol defined.
  /// If "SYNCLOG" is defined, the log messages are sent synchronously to the host process;
  /// Otherwise, the log messages are queued and asynchronously sent as batches to the host process.
  /// </remarks>
  internal sealed class LogBus : Logger
  {

    #region Variables

    private readonly IServerReporter _serverReporter;
#if !SYNCLOG
    private const double _flushInterval = 100.0;
    private readonly Queue<LogMessage> _queue;
    private readonly Timer _timer;
#endif

    #endregion

    #region Properties
    
#if !SYNCLOG

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="LogBus"/> should synchronize the messages to the host process.
    /// </summary>
    public bool Enabled
    {
      get {
        return _timer.Enabled;
        return true;
      }
      set
      {
        if (_timer.Enabled != value)
          _timer.Enabled = value;
      }
    }

#endif

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="LogBus"/>.
    /// </summary>
    /// <param name="serverReporter">
    /// The <see cref="IServerReporter"/> to use for marshalling log messages to the host process.
    /// </param>
    public LogBus(IServerReporter serverReporter)
    {
      _serverReporter = serverReporter;
      _level = serverReporter.GetRequiredLogLevel();
      _logType = LogType.Other;
      _syncRoot = new object();
#if !SYNCLOG
      _queue = new Queue<LogMessage>();
      _timer = new Timer(_flushInterval);
      _timer.Elapsed += SendMessages;
#endif
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Writes the specified <see cref="LogMessage"/> to the bus.
    /// </summary>
    /// <param name="message"></param>
    protected override void Write(LogMessage message)
    {
      message.Prefix = "Guest " + GuestCore.ProcessId;
#if !SYNCLOG
      lock (_syncRoot)
        _queue.Enqueue(message);
#else
      try
      {
        using (GuestCore.Engine.GetEngineProcessingSpace())
          _serverReporter.ReportMessage(message);
      }
      catch
      {
      }
#endif
    }

    #endregion

    #region Private Methods

#if !SYNCLOG

    /// <summary>
    /// Sends all messages contained in <see cref="_queue"/> to the host process.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SendMessages(object sender, ElapsedEventArgs e)
    {
      LogMessage[] msgs;
      lock (_syncRoot)
      {
        msgs = _queue.ToArray();
        _queue.Clear();
      }
      try
      {
        using (Hooking.Engine.GetEngineProcessingSpace())
          _serverReporter.ReportMessage(msgs);
      }
      catch
      {
        Enabled = false;
      }
    }

#endif

    #endregion

  }
}
