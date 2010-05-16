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
using System.Runtime.Serialization;
using System.Threading;
using AppStract.Utilities.Extensions;

namespace AppStract.Core.Virtualization.Engine
{
  /// <summary>
  /// The abstract base class for a collection of <see cref="EngineRule{TIdentifier,TRule}"/>.
  /// </summary>
  /// <remarks>
  /// This class and it's inheritors must be able to be marshaled across application domain boundaries,
  /// therefore this class implements <see cref="ISerializable"/> and has the <see cref="SerializableAttribute"/>.
  /// 
  /// Inheriting classes must always define a constructor with the following parameters: (<see cref="SerializationInfo"/>, <see cref="StreamingContext"/>)
  /// which calls the matching constructor of <see cref="EngineRuleCollectionBase{TIdentifier,TRule}"/>.
  /// In case the inheriting class adds extra class variables, <see cref="GetObjectData"/> must be overridden with a method
  /// still calling the <see cref="GetObjectData"/> implementation of <see cref="EngineRuleCollectionBase{TIdentifier,TRule}"/>
  /// </remarks>
  /// <typeparam name="TIdentifier">The type of object identifying a <see cref="EngineRule{TIdentifier,TRule}"/>.</typeparam>
  /// <typeparam name="TRule">The type of of object representing an engine rule.</typeparam>
  [Serializable]
  public abstract class EngineRuleCollectionBase<TIdentifier, TRule> : ISerializable
  {

    #region Variables

    /// <summary>
    /// The collection of <see cref="EngineRule{TIdentifier,TRule}"/> defined in the current <see cref="EngineRuleCollectionBase{TIdentifier,TRule}"/>.
    /// </summary>
    private readonly List<EngineRule<TIdentifier, TRule>> _rules;
    /// <summary>
    /// Object to acquire a lock on when reading or changing the <see cref="_rules"/> collection.
    /// </summary>
    private readonly ReaderWriterLockSlim _rulesLock;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new empty instance of <see cref="EngineRuleCollectionBase{TIdentifier,TRule}"/>.
    /// </summary>
    protected EngineRuleCollectionBase()
    {
      _rules = new List<EngineRule<TIdentifier, TRule>>();
      _rulesLock = new ReaderWriterLockSlim();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineRuleCollectionBase{TIdentifier,TRule}"/>,
    /// based on the given <see cref="SerializationInfo"/>.
    /// </summary>
    protected EngineRuleCollectionBase(SerializationInfo info, StreamingContext context)
    {
      var rules = info.GetValue("Rules", typeof(byte[])) as byte[];
      _rules = rules.ToObject<List<EngineRule<TIdentifier, TRule>>>();
      _rulesLock = new ReaderWriterLockSlim();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Determines whether a rule for the specified <see cref="identifier"/> is defined
    /// in the current <see cref="EngineRuleCollectionBase{TIdentifier,TRule}"/>.
    /// </summary>
    /// <param name="identifier">
    /// The <typeparamref name="TIdentifier"/> to determine the existence of an associated <typeparamref name="TRule"/> for.
    /// </param>
    /// <returns></returns>
    public bool HasRule(TIdentifier identifier)
    {
      TRule rule;
      return HasRule(identifier, out rule);
    }

    /// <summary>
    /// Determines whether a rule for the specified <see cref="identifier"/> is defined
    /// in the current <see cref="EngineRuleCollectionBase{TIdentifier,TRule}"/>.
    /// </summary>
    /// <param name="identifier">
    /// The <typeparamref name="TIdentifier"/> to determine the existence of an associated <typeparamref name="TRule"/> for.
    /// </param>
    /// <param name="rule">
    /// The first <typeparamref name="TRule"/> associated to the given <typeparamref name="TIdentifier"/>.
    /// </param>
    /// <returns></returns>
    public bool HasRule(TIdentifier identifier, out TRule rule)
    {
      rule = default(TRule);
      using (_rulesLock.EnterDisposableReadLock())
      {
        var i = GetRuleIndex(identifier);
        if (i == -1)
          return false;
        rule = _rules[i].Rule;
        return true;
      }
    }

    /// <summary>
    /// Gets the first <typeparamref name="TRule"/> associated to the given <typeparamref name="TIdentifier"/>.
    /// </summary>
    /// <exception cref="EngineException">
    /// An <see cref="EngineException"/> is thrown if no rule is associated to <paramref name="identifier"/>.
    /// <param name="identifier">
    /// The <typeparamref name="TIdentifier"/> to return the first defined <typeparamref name="TRule"/> for.
    /// </param>
    /// <returns></returns>
    public TRule GetRule(TIdentifier identifier)
    {
      TRule result;
      if (!HasRule(identifier, out result))
        throw new EngineException("No rule specified for identifier \"" + identifier + "\"");
      return result;
    }

    /// <summary>
    /// Removes the engine rule matching <paramref name="identifier"/> and <paramref name="rule"/>
    /// from the current <see cref="EngineRuleCollectionBase{TIdentifier,TRule}"/>.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="rule"></param>
    /// <returns></returns>
    public bool RemoveRule(TIdentifier identifier, TRule rule)
    {
      using (_rulesLock.EnterDisposableWriteLock())
        return _rules.Remove(new EngineRule<TIdentifier, TRule>(identifier, rule));
    }

    /// <summary>
    /// Associates a <typeparamref name="TRule"/> to the specified <typeparamref name="TIdentifier"/>.
    /// </summary>
    /// <remarks>
    /// If a <typeparamref name="TRule"/> is already defined for <paramref name="identifier"/>,
    /// the old <typeparamref name="TRule"/> is overwritten with <paramref name="rule"/>.
    /// Otherwise, a new association is added to the collection.</remarks>
    /// <param name="identifier">The <typeparamref name="TIdentifier"/> to set a <typeparamref name="TRule"/> for.</param>
    /// <param name="rule"> The <typeparamref name="TRule"/> to associate to <paramref name="identifier"/>.</param>
    public void SetRule(TIdentifier identifier, TRule rule)
    {
      using (_rulesLock.EnterDisposableWriteLock())
      {
        var i = GetRuleIndex(identifier);
        if (i == -1)
          _rules.Add(new EngineRule<TIdentifier, TRule>(identifier, rule));
        else
          _rules[i].Rule = rule;
      }
    }

    /// <summary>
    /// Returns a string representation that represents the current <see cref="EngineRuleCollectionBase{TIdentifier,TRule}"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "Count = " + _rules.Count;
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Returns whether or not <paramref name="value"/> equals <paramref name="otherValue"/>.
    /// </summary>
    /// <param name="value">The first <see cref="TIdentifier"/> to compare, or null.</param>
    /// <param name="otherValue">The second <see cref="TIdentifier"/> to compare, or null.</param>
    /// <returns></returns>
    protected abstract bool Matches(TIdentifier value, TIdentifier otherValue);

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the zero-based index of the first occurrence of a <typeparamref name="TIdentifier"/> in <see cref="_rules"/>.
    /// </summary>
    /// <remarks>
    /// A read lock should be acquired on <see cref="_rulesLock"/> while calling this method.
    /// </remarks>
    /// <param name="identifier"> The <typeparamref name="TIdentifier"/> to locate in <see cref="_rules"/>.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of <paramref name="identifier"/> within <see cref="_rules"/>, if found; otherwise, –1.
    /// </returns>
    private int GetRuleIndex(TIdentifier identifier)
    {
      for (int i = 0; i < _rules.Count; i++)
        if (Matches(_rules[i].Identifier, identifier))
          return i;
      return -1;
    }

    #endregion

    #region ISerializable Members

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      var rules = _rules.ToByteArray();
      info.AddValue("Rules", rules);
    }

    #endregion

  }
}
