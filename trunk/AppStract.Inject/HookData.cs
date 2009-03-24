

using System;
using EasyHook;

namespace AppStract.Inject
{

  /// <summary>
  /// The data needed for creating a managed hook.
  /// </summary>
  internal struct HookData
  {

    #region Variables

    /// <summary>
    /// Target entry point that should be hooked.
    /// </summary>
    private readonly IntPtr _targetEntryPoint;
    /// <summary>
    /// Handler with the same signature as the original entry point that will
    /// be invoked for every call that has passed the Fiber Deadlock Barrier
    /// and various integrity checks.
    /// </summary>
    private readonly Delegate _handler;
    /// <summary>
    /// Uninterpreted callback that will later be available through <seealso cref="HookRuntimeInfo.Callback"/>.
    /// </summary>
    private readonly object _callback;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the target entry point that should be hooked.
    /// </summary>
    public IntPtr TargetEntryPoint
    {
      get { return _targetEntryPoint; }
    }

    /// <summary>
    /// Gets the handler, which has the same signature as the original entry point
    /// </summary>
    public Delegate Handler
    {
      get { return _handler; }
    }

    /// <summary>
    /// Gets the callback object.
    /// </summary>
    public object Callback
    {
      get { return _callback; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetEntryPoint">
    /// The target entry point that should be hooked.
    /// </param>
    /// <param name="handler">The handler with the same signature as the original entry
    /// point that will be invoked for every call that has passed the Fiber Deadlock Barrier
    /// and various integrity checks.
    /// </param>
    /// <param name="callback">
    /// Uninterpreted callback that will later be available through
    /// <seealso cref="HookRuntimeInfo.Callback"/>.
    /// </param>
    public HookData(IntPtr targetEntryPoint, Delegate handler, object callback)
    {
      _targetEntryPoint = targetEntryPoint;
      _handler = handler;
      _callback = callback;
    }

    #endregion

    #region Public Methods

    // Override ToString() for debug use
    public override string ToString()
    {
      return "[" + _targetEntryPoint + "] " + _handler.Method.Name;
    }

    #endregion

  }
}
