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

using System.Threading;

namespace AppStract.Utilities.Observables
{
  internal abstract class EventRaiser
  {

    #region Public Methods

    /// <summary>
    /// Calls the delegate.
    /// </summary>
    public abstract void Raise();

    /// <summary>
    /// Calls the delegate in a new thread.
    /// </summary>
    public void RaiseAsync()
    {
      ThreadPool.QueueUserWorkItem(Raise);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Calls <see cref="Raise"/>, <paramref name="state"/> is not used.
    /// </summary>
    /// <param name="state">Not used, only here to match <see cref="WaitCallback"/>.</param>
    private void Raise(object state)
    {
      Raise();
    }

    #endregion

  }
}
