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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using AppStract.Utilities.Extensions;

namespace AppStract.Engine.Configuration
{
  /// <summary>
  /// Represents a collection of <see cref="EngineRule"/> objects.
  /// </summary>
  /// <remarks>
  /// This class and it's inheritors must be able to be marshaled across application domain boundaries,
  /// therefore this class implements <see cref="ISerializable"/> and has the <see cref="SerializableAttribute"/>.
  /// 
  /// Inheriting classes must always define a constructor with the following parameters: (<see cref="SerializationInfo"/>, <see cref="StreamingContext"/>)
  /// which calls the matching constructor of <see cref="EngineRuleCollection"/>.
  /// In case the inheriting class adds extra class variables, <see cref="GetObjectData"/> must be overridden with a method
  /// still calling the <see cref="GetObjectData"/> implementation of <see cref="EngineRuleCollection"/>.
  /// </remarks>
  [Serializable]
  public abstract class EngineRuleCollection : IEnumerable<EngineRule>, ISerializable
  {

    #region Constants

    protected const char _WildCard = '%';

    #endregion

    #region Variables

    /// <summary>
    /// The collection of <see cref="EngineRule"/> defined in the current <see cref="EngineRuleCollection"/>.
    /// </summary>
    private readonly List<EngineRule> _rules;
    /// <summary>
    /// Object to acquire a lock on when reading or changing the <see cref="_rules"/> collection.
    /// </summary>
    private readonly ReaderWriterLockSlim _rulesLock;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new empty instance of <see cref="EngineRuleCollection"/>.
    /// </summary>
    protected EngineRuleCollection()
    {
      _rules = new List<EngineRule>();
      _rulesLock = new ReaderWriterLockSlim();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="EngineRuleCollection"/>,
    /// based on the given <see cref="SerializationInfo"/>.
    /// </summary>
    protected EngineRuleCollection(SerializationInfo info, StreamingContext context)
    {
      var rules = info.GetValue("Rules", typeof(byte[])) as byte[];
      _rules = rules.ToObject<List<EngineRule>>();
      _rulesLock = new ReaderWriterLockSlim();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the first <see cref="VirtualizationType"/> associated to the given value.
    /// </summary>
    /// <exception cref="Exception">
    /// An <see cref="Exception"/> is thrown if no rule is associated to <paramref name="identifier"/>.
    /// </exception>
    /// <param name="identifier">
    /// The value to return the first defined <see cref="VirtualizationType"/> for.
    /// </param>
    /// <returns></returns>
    public VirtualizationType GetRule(string identifier)
    {
      VirtualizationType result;
      if (!HasRule(identifier, out result))
        throw new Exception("No rule specified for identifier \"" + identifier + "\"");
      return result;
    }

    /// <summary>
    /// Determines whether a rule for the specified <see cref="identifier"/> is defined
    /// in the current <see cref="EngineRuleCollection"/>.
    /// </summary>
    /// <param name="identifier">
    /// The value to determine the existence of an associated <see cref="VirtualizationType"/> for.
    /// </param>
    /// <returns></returns>
    public bool HasRule(string identifier)
    {
      VirtualizationType rule;
      return HasRule(identifier, out rule);
    }

    /// <summary>
    /// Determines whether a rule for the specified <see cref="identifier"/> is defined
    /// in the current <see cref="EngineRuleCollection"/>.
    /// </summary>
    /// <param name="identifier">
    /// The value to determine the existence of an associated <see cref="VirtualizationType"/> for.
    /// </param>
    /// <param name="rule">
    /// The first <see cref="VirtualizationType"/> associated to the given value.
    /// </param>
    /// <returns></returns>
    public bool HasRule(string identifier, out VirtualizationType rule)
    {
      rule = default(VirtualizationType);
      using (_rulesLock.EnterDisposableReadLock())
      {
        var i = GetRuleIndex(identifier);
        if (i == -1)
          return false;
        rule = _rules[i].VirtualizationType;
        return true;
      }
    }

    /// <summary>
    /// Removes the engine rule matching the given <paramref name="identifier"/> and <paramref name="rule"/>
    /// from the current <see cref="EngineRuleCollection"/>.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="rule"></param>
    /// <returns></returns>
    public bool RemoveRule(string identifier, VirtualizationType rule)
    {
      using (_rulesLock.EnterDisposableWriteLock())
        return _rules.Remove(new EngineRule(identifier, rule));
    }

    /// <summary>
    /// Associates a <see cref="VirtualizationType"/> to the specified value.
    /// </summary>
    /// <remarks>
    /// If a <see cref="VirtualizationType"/> is already defined for <paramref name="identifier"/>,
    /// the old <see cref="VirtualizationType"/> is overwritten with <paramref name="rule"/>.
    /// Otherwise, a new association is added to the collection.</remarks>
    /// <param name="identifier">The value to set a <see cref="VirtualizationType"/> for.</param>
    /// <param name="rule"> The <see cref="VirtualizationType"/> to associate to <paramref name="identifier"/>.</param>
    public void SetRule(string identifier, VirtualizationType rule)
    {
      using (_rulesLock.EnterDisposableWriteLock())
      {
        var i = GetRuleIndex(identifier);
        if (i == -1)
          _rules.Add(new EngineRule(identifier, rule));
        else
          _rules[i].VirtualizationType = rule;
      }
    }

    /// <summary>
    /// Returns a string representation that represents the current <see cref="EngineRuleCollection"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "Count = " + _rules.Count;
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Returns the collection of default rules.
    /// </summary>
    /// <returns></returns>
    protected abstract IEnumerable<EngineRule> GetDefaultRules();

    /// <summary>
    /// Returns whether or not <paramref name="value"/> equals <paramref name="otherValue"/>.
    /// </summary>
    /// <remarks>
    /// The default implementation by <see cref="EngineRuleCollection"/> is case insensitive
    /// and supports wildcards in the beginning and end of <paramref name="value"/>.
    /// </remarks>
    /// <param name="value">The first <see cref="string"/> to compare, or null.</param>
    /// <param name="otherValue">The second <see cref="string"/> to compare, or null.</param>
    /// <returns></returns>
    protected virtual bool Matches(string value, string otherValue)
    {
      if (string.IsNullOrEmpty(value))
        return string.IsNullOrEmpty(otherValue);
      if (string.IsNullOrEmpty(otherValue))
        return string.IsNullOrEmpty(value);
      if (value == "%")
        return true;
      value = value.ToLowerInvariant();
      otherValue = otherValue.ToLowerInvariant();
      if (value[0] == _WildCard)
        return value[value.Length - 1] == _WildCard
                 ? otherValue.Contains(value.Substring(1, value.Length - 2))
                 : otherValue.EndsWith(value.Substring(1));
      if (value[value.Length - 1] == _WildCard)
        return otherValue.StartsWith(value.Substring(0, value.Length - 1));
      return value == otherValue;
    }

    /// <summary>
    /// Clears the current collection and populates it with the default rules.
    /// </summary>
    protected void PopulateWithDefaultRules()
    {
      using (_rulesLock.EnterDisposableWriteLock())
      {
        _rules.Clear();
        var defaultRules = GetDefaultRules();
        foreach (var rule in defaultRules)
          _rules.Add(rule);
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the zero-based index of the first occurrence of a value in <see cref="_rules"/>.
    /// </summary>
    /// <remarks>
    /// A read lock should be held on <see cref="_rulesLock"/> while calling this method.
    /// </remarks>
    /// <param name="identifier"> The value to locate in <see cref="_rules"/>.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of <paramref name="identifier"/> within <see cref="_rules"/>, if found; otherwise, –1.
    /// </returns>
    private int GetRuleIndex(string identifier)
    {
      for (int i = 0; i < _rules.Count; i++)
        if (Matches(_rules[i].Identifier, identifier))
          return i;
      return -1;
    }

    #endregion

    #region IEnumerable<EngineRule<string,VirtualizationType>> Members

    public IEnumerator<EngineRule> GetEnumerator()
    {
      return _rules.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _rules.GetEnumerator();
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
