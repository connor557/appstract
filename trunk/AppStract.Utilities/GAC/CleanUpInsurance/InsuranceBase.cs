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

using System.Collections.Generic;

namespace System.Reflection.GAC
{
  /// <summary>
  /// Base class for insurances on assembly cache cleanup.
  /// </summary>
  internal abstract class InsuranceBase
  {

    #region Constants

    /// <summary>
    /// Format to use when converting a <see cref="DateTime"/> from and to a <see cref="string"/>.
    /// </summary>
    protected const string _DateTimeFormat = "dd/MM/yyyy HH:mm:ss";

    #endregion

    #region Variables

    private readonly InstallerDescription _installerDescription;
    private readonly string _insuranceId;
    private readonly string _machineId;
    private readonly DateTime _dateTime;
    private readonly List<AssemblyName> _assemblies;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the string which identifies the current insurance.
    /// </summary>
    public string InsuranceIdentifier
    {
      get { return _insuranceId; }
    }

    /// <summary>
    /// Gets the <see cref="InstallerDescription"/> for the application that requested the insurance.
    /// </summary>
    public InstallerDescription InstallerDescription
    {
      get { return _installerDescription; }
    }

    /// <summary>
    /// Gets the identifier for the machine for which this insurance is created.
    /// </summary>
    public string MachineId
    {
      get { return _machineId; }
    }

    /// <summary>
    /// Gets the <see cref="DateTime"/> on which the insurance is created.
    /// </summary>
    public DateTime CreationDateTime
    {
      get { return _dateTime; }
    }

    /// <summary>
    /// Gets the assemblies that are insured by the current insurance.
    /// </summary>
    public IEnumerable<AssemblyName> Assemblies
    {
      get { return _assemblies; }
    }

    #endregion

    #region Constructors

    protected InsuranceBase(string insuranceIdentifier, InstallerDescription installerDescription, string machineId, DateTime creationDateTime, IEnumerable<AssemblyName> assemblies)
    {
      _insuranceId = insuranceIdentifier;
      _installerDescription = installerDescription;
      _machineId = machineId;
      _dateTime = creationDateTime;
      _assemblies = new List<AssemblyName>(assemblies);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Joins the data specified in <paramref name="otherInsurance"/> with the current <see cref="InsuranceBase"/>.
    /// In essence this means that the list of assemblies specified in the current instance is joined with those specified in <paramref name="otherInsurance"/>.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown if <paramref name="otherInsurance"/> doesn't match the current instance.
    /// Whether or not they match can be determined with the <see cref="MatchesWith"/> method.
    /// </exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <param name="otherInsurance">The <see cref="InsuranceBase"/> to join with the current instance.</param>
    public virtual void JoinWith(InsuranceBase otherInsurance)
    {
      if (otherInsurance == null)
        throw new ArgumentNullException("otherInsurance");
      if (_machineId != otherInsurance._machineId)
        throw new ArgumentException();
      if (_dateTime != otherInsurance._dateTime)
        throw new ArgumentException();
      foreach (var item in otherInsurance._assemblies)
        if (!_assemblies.Contains(item))
          _assemblies.Add(item);
    }

    /// <summary>
    /// Determines if the data specified in <paramref name="otherInsurance"/> matches with the current <see cref="InsuranceBase"/>.
    /// </summary>
    /// <remarks>
    /// The data of matching <see cref="InsuranceBase"/> instances can be joined by calling <see cref="JoinWith"/> on one of the instances.
    /// </remarks>
    /// <param name="otherInsurance"><see cref="InsuranceBase"/> to determine matchability for.</param>
    /// <param name="includeAssemblies">Whether to also verify if <paramref name="otherInsurance"/> also specifies the same insured assemblies.</param>
    public virtual bool MatchesWith(InsuranceBase otherInsurance, bool includeAssemblies)
    {
      if (otherInsurance == null
          || _insuranceId != otherInsurance._insuranceId
          || _machineId != otherInsurance._machineId
          || _dateTime != otherInsurance._dateTime)
        return false;
      if (!includeAssemblies)
        return true;
      if (_assemblies.Count != otherInsurance._assemblies.Count)
        return false;
      foreach (var item in otherInsurance._assemblies)
        if (!_assemblies.Contains(item))
          return false;
      return true;
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents the current <see cref="InsuranceBase"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "Insurance [" + _dateTime.ToString(_DateTimeFormat) + "] " + _assemblies.Count + " assemblies";
    }

    #endregion
  }
}
