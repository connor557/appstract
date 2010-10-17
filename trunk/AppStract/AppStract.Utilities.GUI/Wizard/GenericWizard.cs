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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AppStract.Utilities.GUI.Wizard
{

  /// <summary>
  /// GenericWizard is a form to display usercontrols as a wizard.
  /// </summary>
  /// <remarks>
  /// The usercontrols are expected to be of the size 438 x 408
  /// </remarks>
  public abstract partial class GenericWizard<T> : Form
  {

    #region Variables

    /// <summary>
    /// The current step of the wizard.
    /// </summary>
    private int _currentStep;
    /// <summary>
    /// All available steps for the wizard.
    /// </summary>
    private IList<WizardPage> _steps;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor for GenericWizard.
    /// </summary>
    protected GenericWizard()
    {
      InitializeComponent();
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Returns an IList of WizardPage, holding all pages to display for the current wizard.
    /// The return may not be null.
    /// </summary>
    /// <returns>All pages to display for the current wizard, can't be null.</returns>
    protected abstract IList<WizardPage> WizardPages();

    /// <summary>
    /// Finishes the wizard,
    /// sets DialogResult to DialogResult.OK and calls Close().
    /// </summary>
    protected virtual void FinishWizard()
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    #endregion

    #region Private EventHandlers

    /// <summary>
    /// Occurs whenever the user loads the form.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GenericWizard_LoadEventHandler(object sender, EventArgs e)
    {
      _steps = WizardPages();
      if (_steps == null)
        throw new NullReferenceException("The IList<WizardPages> returned by WizardPages() can't be null.");
      _currentStep = -1;
      InitializeUserControls();
      SetArea(0);
    }

    /// <summary>
    /// Handles a StateChanged event of the IWizardItems.
    /// </summary>
    /// <param name="wizardMayContinue"></param>
    /// <param name="currentState"></param>
    private void Wizard_StateChangedEventHandler(bool wizardMayContinue, T currentState)
    {
      _buttonNext.Enabled = wizardMayContinue && _currentStep < _steps.Count - 1;
    }

    /// <summary>
    /// Handles the Click event for the "Back" and "Next" buttons.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonStep_ClickEventHandler(object sender, EventArgs e)
    {
      if (_panelContent.Controls.Count != 0
        && _panelContent.Controls[0] is IWizardItem<T>)
        ((IWizardItem<T>)_panelContent.Controls[0]).SaveState();
      if (sender == _buttonNext && _buttonNext.Text == "Finish")
        FinishWizard();
      else
        SetArea(_currentStep + Int32.Parse(((Button)sender).Tag.ToString()));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initializes the UserControls by setting the _steps variable.
    /// </summary>
    private void InitializeUserControls()
    {
      int yPos = 30;
      for (int i = 0; i < _steps.Count; i++)
      {
        // Try to attach to the StateChanged event.
        if (_steps[i].UserControl is IWizardItem<T>)
          ((IWizardItem<T>)_steps[i].UserControl).StateChanged += Wizard_StateChangedEventHandler;
        // Create a label for the new step step.
        Label label = new Label();
        label.Text = _steps[i].Text;
        label.Tag = i;
        label.Location = new Point(19, yPos);
        label.ForeColor = SystemColors.ButtonShadow;
        _panelSteps.Controls.Add(label);
        // Increment yPos, so the next step is correctly placed.
        yPos += 35;
      }
    }

    /// <summary>
    /// Sets all controls for the specified <paramref name="index"/>.
    /// </summary>
    /// <param name="index">Index of the step to load.</param>
    private void SetArea(int index)
    {
      if (index == _currentStep || index < 0 || index >= _steps.Count)
        return;
      _currentStep = index;
      // Set the back/next buttons.
      _buttonBack.Enabled = index != 0;
      _buttonNext.Enabled = true;
      _buttonNext.Text = index == _steps.Count - 1 ? "Finish" : "Next >";
      // Select the current step's label.
      SelectStepLabelByIndex(index);
      // Add the current step's UserControl to the panel.
      if (_steps[index].UserControl is IWizardItem<T>)
      {
        ((IWizardItem<T>)_steps[index].UserControl).UpdateContent();
        _buttonNext.Enabled = (_buttonNext.Enabled && ((IWizardItem<T>)_steps[index].UserControl).AcceptableContent);
      }
      _panelContent.Controls.Clear();
      _panelContent.Controls.Add(_steps[index].UserControl);
    }

    /// <summary>
    /// Sets the label with the specified index as selected,
    /// the label in front of it is set as unselected.
    /// </summary>
    /// <param name="index"></param>
    private void SelectStepLabelByIndex(int index)
    {
      foreach (Control control in _panelSteps.Controls)
      {
        int i;
        if (!Int32.TryParse(control.Tag.ToString(), out i))
          continue;
        if (i == index)
          SetLabelSelected(control);
        else if (control.Font.Bold)
          SetLabelUnselected(control);
      }
    }

    #endregion

    #region Private Static Methods

    /// <summary>
    /// Sets the specified <paramref name="label"/> as selected.
    /// </summary>
    /// <param name="label"></param>
    private static void SetLabelSelected(Control label)
    {
      if (label == null)
        return;
      label.Font = new Font(label.Font, FontStyle.Bold);
      label.ForeColor = SystemColors.ControlText;
    }

    /// <summary>
    /// Sets the specified <paramref name="label"/> as unselected.
    /// </summary>
    /// <param name="label"></param>
    private static void SetLabelUnselected(Control label)
    {
      if (label == null)
        return;
      label.Font = new Font(label.Font, FontStyle.Regular);
    }

    #endregion

  }
}
