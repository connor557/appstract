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
  public class CommandlineParser
  {

    #region Variables

    private readonly string[] _arguments;
    private Dictionary<CommandlineOption, object> _options;

    #endregion

    #region Constructors

    public CommandlineParser(string[] arguments)
    {
      _arguments = arguments;
      _options = new Dictionary<CommandlineOption, object>(4);
    }

    #endregion

    #region Public Methods

    public void Parse()
    {
      var options = new Dictionary<CommandlineOption, object>(_arguments.Length);
      foreach (var arg in _arguments)
      {
        KeyValuePair<CommandlineOption, string> parsedValue;
        if (TryParse(arg, out parsedValue))
          options.Add(parsedValue.Key, parsedValue.Value);
      }
      _options = options;
    }

    public bool IsDefined(CommandlineOption option)
    {
      return _options.ContainsKey(option);
    }

    public object GetOption(CommandlineOption option)
    {
      return _options[option];
    }

    #endregion

    #region Private Methods

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
