using System;
using System.Diagnostics;

namespace EasyHook
{
  /// <summary>
  /// Default implementation of <see cref="IEasyLog"/>.
  /// </summary>
  internal class LogService : IEasyLog
  {

    #region Private Methods

    /// <summary>
    /// Prints a message to the local systems eventlog.
    /// </summary>
    /// <param name="entry"></param>
    /// <param name="type"></param>
    private static void Print(string entry, EventLogEntryType type)
    {
      switch (type)
      {
        case EventLogEntryType.Error: entry = "[error]: " + entry; break;
        case EventLogEntryType.Information: entry = "[comment]: " + entry; break;
        case EventLogEntryType.Warning: entry = "[warning]: " + entry; break;
      }

      try
      {
        if (EventLog.Exists("Application", "."))
        {
          EventLog eventLog = new EventLog("Application", ".", "EasyHook");

#if !DEBUG
                if(InType == EventLogEntryType.Error)
#endif
          eventLog.WriteEntry(entry, type);
        }
      }
      catch
      {
      }

#if DEBUG
      Console.WriteLine(entry);
#endif
    }

    #endregion

    #region IEasyLog Members

    public void Error(string message)
    {
      Print(message, EventLogEntryType.Error);
    }

    public void Warning(string message)
    {
      Print(message, EventLogEntryType.Warning);
    }

    public void Information(string message)
    {
      Print(message, EventLogEntryType.Information);
    }

    #endregion

  }
}
