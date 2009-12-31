namespace EasyHook
{
  /// <summary>
  /// Interfaces the log service used by the EasyHook library.
  /// </summary>
  public interface IEasyLog
  {
    /// <summary>
    /// Logs an error message.
    /// </summary>
    /// <param name="message"></param>
    void Error(string message);

    /// <summary>
    /// Logs a warning message.
    /// </summary>
    /// <param name="message"></param>
    void Warning(string message);

    /// <summary>
    /// Logs an information message.
    /// </summary>
    /// <param name="message"></param>
    void Information(string message);
  }
}
