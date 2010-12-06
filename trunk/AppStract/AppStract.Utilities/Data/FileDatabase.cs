using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AppStract.Utilities.Data
{
  public abstract class FileDatabase<T> : Database<T>
  {
    #region Variables

    private const string _DataSeperator = "%-#-%";
    private const string _DataItemSeperator = "%:%";
    private const string _DataItemFieldSeperator = "|";
    private readonly string _filename;

    #endregion

    #region Constructors

    protected FileDatabase(string filename)
    {
      _filename = filename;
    }

    #endregion

    #region Public Methods

    public override IEnumerable<T> ReadAll()
    {
      using (var stream = new FileStream(_filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
      {
        var readData = new Dictionary<string, byte[]>();
        var binaryReader = new BinaryReader(stream);
        while (stream.Position < stream.Length)
        {
          var key = binaryReader.ReadString();
          if (key == _DataSeperator)
          {
            // Data item is completely read
            yield return BuildObjectItem(readData);
            continue;
          }
          binaryReader.ReadString();  // Skip to data length field
          int dataLength;
          Int32.TryParse(binaryReader.ReadString(), out dataLength);
          binaryReader.ReadString();  // Skip to data field
          var data = binaryReader.ReadBytes(dataLength);
          readData.Add(key, data);
          binaryReader.ReadString();  // Skip the data item seperator
        }
      }
    }

    #endregion

    #region Protected Methods

    protected override void DoInitialize()
    {
    }

    protected override bool ItemExists(T item)
    {
      return ReadAll().Contains(item);
    }

    protected override void Write(IEnumerator<DatabaseAction<T>> items)
    {
      File.Delete(_filename);
      using (var stream = new FileStream(_filename, FileMode.Create, FileAccess.Write))
      {
        var writer = new BinaryWriter(stream);
        while (items.MoveNext())
        {
          var data = BuildDataItem(items.Current.Item);
          foreach (var dataItem in data)
          {
            writer.Write(dataItem.Key);
            writer.Write(_DataItemFieldSeperator);
            writer.Write(dataItem.Key.Length.ToString());
            writer.Write(_DataItemFieldSeperator);
            writer.Write(dataItem.Key);
            writer.Write(_DataItemSeperator);
          }
          writer.Write(_DataSeperator);
        }
      }
    }

    protected abstract IDictionary<string, byte[]> BuildDataItem(T data);

    protected abstract T BuildObjectItem(IDictionary<string, byte[]> tableItem);

    #endregion
  }
}
