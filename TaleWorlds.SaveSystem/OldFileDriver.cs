// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.OldFileDriver
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.IO;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
  public class OldFileDriver : ISaveDriver
  {
    private string _fileName;

    public void SetFileName(string fileName) => this._fileName = FileDriver.GetSaveFileFullPath(fileName);

    public void Save(int version, MetaData metaData, GameData gameData)
    {
      byte[] data = gameData.GetData();
      FileStream fileStream = new FileStream(this._fileName, FileMode.CreateNew);
      metaData.Add("Version", version.ToString());
      metaData.Serialize((Stream) fileStream);
      fileStream.Write(data, 0, data.Length);
      fileStream.Close();
    }

    public MetaData LoadMetaData()
    {
      if (File.Exists(this._fileName))
      {
        FileStream fileStream = new FileStream(this._fileName, FileMode.Open);
        MetaData metaData = MetaData.Deserialize((Stream) fileStream);
        fileStream.Close();
        return metaData;
      }
      Debug.Print("No save files exist.");
      return (MetaData) null;
    }

    public LoadData Load()
    {
      if (File.Exists(this._fileName))
      {
        FileStream fileStream = new FileStream(this._fileName, FileMode.Open);
        MetaData metaData = MetaData.Deserialize((Stream) fileStream);
        byte[] numArray = new byte[fileStream.Length - fileStream.Position];
        fileStream.Read(numArray, 0, numArray.Length);
        GameData from = GameData.CreateFrom(numArray);
        return new LoadData(metaData, from);
      }
      Debug.Print("No save files exist.");
      return (LoadData) null;
    }

    public SaveGameFileInfo[] GetSaveGameFileInfos() => new FileDriver().GetSaveGameFileInfos();

    public string[] GetSaveGameFileNames() => new FileDriver().GetSaveGameFileNames();

    public void Delete(string fileName)
    {
      FileDriver fileDriver = new FileDriver();
      fileDriver.SetFileName(this._fileName);
      fileDriver.Delete(fileName);
    }
  }
}
