using System;

namespace AppStract.Engine.Virtualization.Hooking
{
  /// <summary>
  /// Represents the method able to install an API hook.
  /// </summary>
  /// <param name="targetEntryPoint">The target entry point that should be hooked.</param>
  /// <param name="hookHandler">A handler with the same signature as the original entry point.</param>
  /// <param name="callback">An uninterpreted callback.</param>
  public delegate void HookInstaller(IntPtr targetEntryPoint, Delegate hookHandler, object callback);

  /// <summary>
  /// Interface representing a type able to provide API hooks.
  /// </summary>
  public interface IHookProvider
  {

    /// <summary>
    /// Initializes the current provider.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Installs all hooks known by the current <see cref="IHookProvider"/>.
    /// </summary>
    /// <exception cref="HookingException">
    /// A <see cref="HookingException"/> is thrown if the installation of any of the API hooks fails.
    /// </exception>
    /// <param name="installHookDelegate">A delegate to the method to use for installing the API hooks.</param>
    void InstallHooks(HookInstaller installHookDelegate);

  }
}
