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

namespace AppStract.Engine.Configuration
{
  /// <summary>
  /// Represents a rule in the virtualization engine.
  /// </summary>
  [Serializable]
  public sealed class EngineRule : IEquatable<EngineRule>
  {

    #region Public Properties

    /// <summary>
    /// Gets or sets the identifier of the current <see cref="EngineRule"/>.
    /// </summary>
    public string Identifier
    { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="VirtualizationType"/> representing the engine rule.
    /// </summary>
    public VirtualizationType VirtualizationType
    { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes an empty instance of <see cref="EngineRule"/>.
    /// </summary>
    public EngineRule()
    {

    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineRule"/>.
    /// </summary>
    /// <param name="identifier">The identifier of the new <see cref="EngineRule"/>.</param>
    /// <param name="virtualizationType">The <see cref="VirtualizationType"/> representing the engine rule.</param>
    public EngineRule(string identifier, VirtualizationType virtualizationType)
    {
      Identifier = identifier;
      VirtualizationType = virtualizationType;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a string representation of the current <see cref="EngineRule"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "EngineRule: {" + Identifier + " || " + VirtualizationType + "}";
    }

    #endregion

    #region IEquatable<EngineRule> Members

    /// <summary>
    /// Indicates whether the current <see cref="EngineRule"/> is equal to another <see cref="EngineRule"/>.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(EngineRule other)
    {
      return Identifier == other.Identifier
             && VirtualizationType == other.VirtualizationType;
    }

    #endregion

  }
}
