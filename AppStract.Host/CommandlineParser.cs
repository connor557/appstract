#region Copyright (C) 2008-2009 Simon Allaeys

/*
    Copyright (C) 2008-2009 Simon Allaeys
 
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

namespace AppStract.Host
{
  /// <summary>
  /// Parses an array of commandline argument strings
  /// to by the application useable <see cref="CommandlineOption"/>s with associated values.
  /// </summary>
  public class CommandlineParser
  {

    #region Variables

    /// <summary>
    /// The arguments, as supplied to the constructor.
    /// </summary>
    private readonly string[] _arguments;
    /// <summary>
    /// The parsed arguments, only initialized after <see cref="Parse"/> has been called.
    /// </summary>
    private Dictionary<CommandlineOption, string> _options;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="CommandlineParser"/>.
    /// </summary>
    /// <param name="arguments">The arguments to parse.</param>
    public CommandlineParser(string[] arguments)
    {
      _arguments = arguments;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether there's a value defined for the specified <see cref="CommandlineOption"/>.
    /// </summary>
    /// <param name="option"><see cref="CommandlineOption"/> to check for an associated value.</param>
    /// <returns></returns>
    public bool IsDefined(CommandlineOption option)
    {
      if (_options == null)
        Parse();
      return _options.ContainsKey(option);
    }

    /// <summary>
    /// Returns the associated value for the specified <see cref="CommandlineOption"/>.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown when no value is associated to <paramref name="option"/>.
    /// </exception>
    /// <param name="option"><see cref="CommandlineOption"/> to return the associated value for.</param>
    /// <returns>The value associated to the specified <see cref="CommandlineOption"/>.</returns>
    public string GetOption(CommandlineOption option)
    {
      if (!IsDefined(option))
        throw new ArgumentException("There is no value associated to " + option);
      return _options[option];
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Parses the arguments to <see cref="_options"/>.
    /// </summary>
    private void Parse()
    {
      var options = new Dictionary<CommandlineOption, string>(_arguments.Length);
      foreach (var arg in _arguments)
      {
        KeyValuePair<CommandlineOption, string> parsedValue;
        if (TryParse(arg, out parsedValue))
          options.Add(parsedValue.Key, parsedValue.Value);
      }
      _options = options;
    }

    /// <summary>
    /// Tries to parse the argument specified to a pair of a <see cref="CommandlineOption"/> and a string.
    /// </summary>
    /// <param name="arg">The argument to parse.</param>
    /// <param name="result">The <see cref="CommandlineOption"/> and it's associated value.</param>
    /// <returns>Whether parsing succeeded.</returns>
    private static bool TryParse(string arg, out KeyValuePair<CommandlineOption, string> result)
    {
      result = new KeyValuePair<CommandlineOption, string>();
      var args = arg.Split(new[] {"="}, 2, StringSplitOptions.RemoveEmptyEntries);
      if (args.Length != 2)
        return false;
      var optionType = typeof (CommandlineOption);
      string enumString = null;
      var enumMembers = Enum.GetNames(optionType);
      foreach (var member in enumMembers)
      {
        if (member.ToLowerInvariant() != args[0].ToLowerInvariant())
          continue;
        enumString = member;
        break;
      }
      if (enumString == null)
        return false;
      var key = (CommandlineOption)Enum.Parse(optionType, enumString);
      result = new KeyValuePair<CommandlineOption, string>(key, args[1]);
      return true;
    }

    #endregion

  }
}
