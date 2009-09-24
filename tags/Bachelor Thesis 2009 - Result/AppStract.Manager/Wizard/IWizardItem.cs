
namespace AppStract.Manager.Wizard
{

  /// <summary>
  /// Interface to an item of a wizard.
  /// </summary>
  interface IWizardItem<T>
  {

    /// <summary>
    /// Gets called when the state of a step in the wizard changes.
    /// </summary>
    event WizardStateChangedEventHandler<T> StateChanged;

    /// <summary>
    /// Gets whether the content of the current item is accepted,
    /// this indicates whether the wizard is allowed to continue.
    /// </summary>
    bool AcceptableContent
    {
      get;
    }

    /// <summary>
    /// Gets the current state.
    /// </summary>
    T State
    {
      get;
    }

    /// <summary>
    /// Saves the state..
    /// </summary>
    void SaveState();

    /// <summary>
    /// Updates the UI from the curren state.
    /// </summary>
    void UpdateContent();

  }
}
