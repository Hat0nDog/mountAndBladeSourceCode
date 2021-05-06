// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.InMemDriver
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.IO;

namespace TaleWorlds.SaveSystem
{
  public class InMemDriver : ISaveDriver
  {
    private byte[] _data;

    public void SetFileName(string fileName)
    {
    }

    public void Save(int version, MetaData metaData, GameData gameData)
    {
      byte[] data = gameData.GetData();
      MemoryStream memoryStream = new MemoryStream();
      metaData.Add(nameof (version), version.ToString());
      metaData.Serialize((Stream) memoryStream);
      memoryStream.Write(data, 0, data.Length);
      this._data = memoryStream.GetBuffer();
    }

    public MetaData LoadMetaData()
    {
      MemoryStream memoryStream = new MemoryStream(this._data);
      MetaData metaData = MetaData.Deserialize((Stream) memoryStream);
      memoryStream.Close();
      return metaData;
    }

    public LoadData Load()
    {
      MemoryStream memoryStream = new MemoryStream(this._data);
      MetaData metaData = MetaData.Deserialize((Stream) memoryStream);
      byte[] numArray = new byte[memoryStream.Length - memoryStream.Position];
      memoryStream.Read(numArray, 0, numArray.Length);
      GameData from = GameData.CreateFrom(numArray);
      return new LoadData(metaData, from);
    }

    public SaveGameFileInfo[] GetSaveGameFileInfos() => new SaveGameFileInfo[0];

    public string[] GetSaveGameFileNames() => new string[0];

    public void Delete(string fileName) => this._data = new byte[0];
  }
}
