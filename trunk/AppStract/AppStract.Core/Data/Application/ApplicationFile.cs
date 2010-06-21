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
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using AppStract.Utilities.Helpers;
using AppStract.Utilities.Extensions;

namespace AppStract.Core.Data.Application
{
  [Serializable]
  public sealed class ApplicationFile : ISerializable, IXmlSerializable
  {

    #region Variables

    private FileType _type;
    private string _file;

    #endregion

    #region Properties

    public FileType Type
    {
      get { return _type; }
    }

    public string FileName
    {
      get { return _file; }
      set
      {
        _type = GetFileType(value);
        _file = value;
      }
    }

    #endregion

    #region Constructors

    public ApplicationFile() { }

    public ApplicationFile(string file)
    {
      FileName = file;
    }

    private ApplicationFile(SerializationInfo info, StreamingContext context)
    {
      ParserHelper.TryParseEnum(info.GetString("Type"), out _type);
      _file = info.GetString("FileName");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether the curretn <see cref="ApplicationFile"/> exists on the local media.
    /// </summary>
    /// <returns></returns>
    public bool Exists()
    {
      return File.Exists(FileName);
    }

    /// <summary>
    /// Returns a string representation of the current <see cref="ApplicationFile"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "[" + Type + "] " + FileName; 
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the <see cref="FileType"/> for the filename specified.
    /// </summary>
    /// <exception cref="FileNotFoundException">
    /// A <see cref="FileNotFoundException"/> is thrown if the filename is identified as an assembly
    /// while the associated file does not exist.
    /// It's not possible to retrieve the <see cref="FileType"/> of a non-existing assembly.
    /// </exception>
    /// <remarks>
    /// The <paramref name="filename"/> specified is identified as an assembly
    /// if it's not a directory
    /// and if it ends with ".exe" or ".dll".
    /// </remarks>
    /// <param name="filename">The file to determine the <see cref="FileType"/> of.</param>
    /// <returns>The <see cref="FileType"/> of the <paramref name="filename"/> specified.</returns>
    private static FileType GetFileType(string filename)
    {
      filename = filename.ToLowerInvariant();
      if (filename == "" || filename.IsComposedOf(new[] {'.', '\\'}) || Directory.Exists(filename))
        return FileType.Directory;
      if (filename.EndsWith(".db3"))
        return FileType.Database;
      if (filename.EndsWithAny(new[] {".exe", ".dll"}))
      {
        if (!File.Exists(filename))
          throw new FileNotFoundException(
            "The assembly specified does not exist, making it impossible to retrieve its FileType.",
            filename);
        return AssemblyHelper.IsManagedAssembly(filename)
                 ? FileType.Assembly_Managed
                 : FileType.Assembly_Native;
      }
      return FileType.File;
    }

    #endregion

    #region ISerializable Members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("Type", Enum.GetName(typeof(FileType), _type));
      info.AddValue("FileName", _file);
    }

    #endregion

    #region IXmlSerializable Members

    public global::System.Xml.Schema.XmlSchema GetSchema()
    {
      return null;
    }

    public void ReadXml(XmlReader reader)
    {
      if (!ParserHelper.TryParseEnum(reader.GetAttribute("Type"), out _type))
        _type = GetFileType(_file);
      reader.Read();
      _file = reader.ReadElementString("FileName");
      reader.Read();
    }

    public void WriteXml(XmlWriter writer)
    {
      writer.WriteAttributeString("Type", Enum.GetName(typeof(FileType), _type));
      writer.WriteElementString("FileName", _file);
    }

    #endregion

  }
}
