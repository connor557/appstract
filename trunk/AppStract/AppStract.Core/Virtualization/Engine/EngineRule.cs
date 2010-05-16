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

namespace AppStract.Core.Virtualization.Engine
{
  /// <summary>
  /// Represents a rule in the virtualization engine.
  /// </summary>
  /// <typeparam name="TIdentifier">The type of object identifying the <see cref="EngineRule{TIdentifier,TRule}"/>.</typeparam>
  /// <typeparam name="TRule">The type of object representing an engine rule.</typeparam>
  [Serializable]
  public class EngineRule<TIdentifier, TRule> : IEquatable<EngineRule<TIdentifier, TRule>>
  {

    #region Public Properties

    /// <summary>
    /// Gets or sets the identifier of the current <see cref="EngineRule{TIdentifier,TRule}"/>.
    /// </summary>
    public TIdentifier Identifier
    { get; set; }

    /// <summary>
    /// Gets or sets the object representing the engine rule.
    /// </summary>
    public TRule Rule
    { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes an empty instance of <see cref="EngineRule{TIdentifier,TRule}"/>.
    /// </summary>
    public EngineRule()
    {

    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineRule{TIdentifier,TRule}"/>.
    /// </summary>
    /// <param name="identifier">The identifier of the new <see cref="EngineRule{TIdentifier,TRule}"/>.</param>
    /// <param name="rule">The object representing the engine rule.</param>
    public EngineRule(TIdentifier identifier, TRule rule)
    {
      Identifier = identifier;
      Rule = rule;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a string representation of the current <see cref="EngineRule{TIdentifier,TRule}"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "EngineRule: {" + Identifier + " || " + Rule + "}";
    }

    #endregion

    #region IEquatable<EngineRule<TIdentifier,TRule>> Members

    /// <summary>
    /// Indicates whether the current <see cref="EngineRule{TIdentifier,TRule}"/> is equal to another <see cref="EngineRule{TIdentifier,TRule}"/>.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(EngineRule<TIdentifier, TRule> other)
    {
      var idComparer = EqualityComparer<TIdentifier>.Default;
      var ruleComparer = EqualityComparer<TRule>.Default;
      return idComparer.Equals(Identifier, other.Identifier)
             && ruleComparer.Equals(Rule, other.Rule);
    }

    #endregion

  }
}
