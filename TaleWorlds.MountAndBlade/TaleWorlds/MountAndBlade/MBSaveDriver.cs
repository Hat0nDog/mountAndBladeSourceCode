// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBSaveDriver
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MBSaveDriver : ISaveDriver
  {
    private string _fileName;

    public void SetFileName(string fileName) => this._fileName = fileName;

    public void Save(int version, MetaData metaData, GameData gameData)
    {
      MemoryStream memoryStream = new MemoryStream();
      metaData.Add("Version", version.ToString());
      metaData.Serialize((Stream) memoryStream);
      using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream, CompressionLevel.Fastest, true))
        new BinaryFormatter().Serialize((Stream) deflateStream, (object) gameData);
      ArraySegment<byte> buffer;
      if (memoryStream.TryGetBuffer(out buffer))
        Utilities.SaveFile(this._fileName, buffer.Array);
      memoryStream.Close();
    }

    public MetaData LoadMetaData()
    {
      byte[] buffer = Utilities.LoadFile(this._fileName);
      if (buffer != null)
        return MetaData.Deserialize((Stream) new MemoryStream(buffer));
      Debug.Print("No save files exist.");
      return (MetaData) null;
    }

    public LoadData Load()
    {
      byte[] buffer = Utilities.LoadFile(this._fileName);
      if (buffer != null)
      {
        MemoryStream memoryStream = new MemoryStream(buffer);
        using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream, CompressionMode.Decompress))
          return new LoadData(MetaData.Deserialize((Stream) memoryStream), (GameData) new BinaryFormatter().Deserialize((Stream) deflateStream));
      }
      else
      {
        Debug.Print("No save files exist.");
        return (LoadData) null;
      }
    }

    public SaveGameFileInfo[] GetSaveGameFileInfos()
    {
      List<Tuple<string, byte[]>> saveFileMetadatas = Utilities.GetAllSaveFileMetadatas();
      SaveGameFileInfo[] saveGameFileInfoArray = new SaveGameFileInfo[saveFileMetadatas.Count];
      for (int index = 0; index < saveFileMetadatas.Count; ++index)
      {
        SaveGameFileInfo saveGameFileInfo = new SaveGameFileInfo();
        saveGameFileInfo.Name = saveFileMetadatas[index].Item1;
        MemoryStream memoryStream = new MemoryStream(saveFileMetadatas[index].Item2);
        saveGameFileInfo.MetaData = MetaData.Deserialize((Stream) memoryStream);
        saveGameFileInfoArray[index] = saveGameFileInfo;
      }
      return saveGameFileInfoArray;
    }

    public string[] GetSaveGameFileNames()
    {
      List<Tuple<string, byte[]>> saveFileMetadatas = Utilities.GetAllSaveFileMetadatas();
      string[] strArray = new string[saveFileMetadatas.Count];
      for (int index = 0; index < saveFileMetadatas.Count; ++index)
        strArray[index] = saveFileMetadatas[index].Item1;
      return strArray;
    }

    public void Delete(string fileName) => Utilities.DeleteSaveGameFile(fileName);

    private byte[] Compress(byte[] data)
    {
      MemoryStream memoryStream = new MemoryStream();
      using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream, CompressionLevel.Fastest))
        deflateStream.Write(data, 0, data.Length);
      return memoryStream.ToArray();
    }

    private byte[] Decompress(byte[] data)
    {
      MemoryStream memoryStream1 = new MemoryStream(data);
      MemoryStream memoryStream2 = new MemoryStream();
      using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream1, CompressionMode.Decompress))
        deflateStream.CopyTo((Stream) memoryStream2);
      return memoryStream2.ToArray();
    }
  }
}
