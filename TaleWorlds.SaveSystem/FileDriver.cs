// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.FileDriver
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
  public class FileDriver : ISaveDriver
  {
    private const string ApplicationName = "Mount and Blade II Bannerlord";
    private const string ModuleName = "Native";
    private const string SaveDirectoryName = "Game Saves";
    private string _fileName;

    private static string SavePath => Common.PlatformFileHelper.DocumentsPath + "\\Mount and Blade II Bannerlord\\Game Saves\\";

    public static string GetSaveFileFullPath(string saveName)
    {
      Directory.CreateDirectory(FileDriver.SavePath);
      return FileDriver.SavePath + saveName;
    }

    public void SetFileName(string fileName) => this._fileName = FileDriver.GetSaveFileFullPath(fileName);

    public void Save(int version, MetaData metaData, GameData gameData)
    {
      string str = this._fileName + ".tmp";
      if (File.Exists(str))
        File.Delete(str);
      FileStream fileStream = new FileStream(str, FileMode.CreateNew);
      metaData.Add("Version", version.ToString());
      metaData.Serialize((Stream) fileStream);
      using (DeflateStream deflateStream = new DeflateStream((Stream) fileStream, CompressionLevel.Fastest))
        new BinaryFormatter().Serialize((Stream) deflateStream, (object) gameData);
      fileStream.Close();
      if (File.Exists(this._fileName))
        File.Delete(this._fileName);
      File.Move(str, this._fileName);
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
        using (DeflateStream deflateStream = new DeflateStream((Stream) fileStream, CompressionMode.Decompress))
          return new LoadData(MetaData.Deserialize((Stream) fileStream), (GameData) new BinaryFormatter().Deserialize((Stream) deflateStream));
      }
      else
      {
        Debug.Print("No save files exist.");
        return (LoadData) null;
      }
    }

    public SaveGameFileInfo[] GetSaveGameFileInfos()
    {
      string fileName = this._fileName;
      DirectoryInfo directory = Directory.CreateDirectory(FileDriver.SavePath);
      FileInfo[] files = directory.GetFiles("*.sav", SearchOption.AllDirectories);
      List<SaveGameFileInfo> saveGameFileInfoList = new List<SaveGameFileInfo>(files.Length);
      for (int index = 0; index < files.Length; ++index)
      {
        FileInfo fileInfo = files[index];
        string str = fileInfo.FullName.Substring(directory.FullName.Length, fileInfo.FullName.Length - directory.FullName.Length - "sav".Length - 1);
        this.SetFileName(str + ".sav");
        MetaData metaData = SaveManager.LoadMetaData((ISaveDriver) this);
        saveGameFileInfoList.Add(new SaveGameFileInfo()
        {
          Name = str,
          MetaData = metaData
        });
      }
      this._fileName = fileName;
      return saveGameFileInfoList.ToArray();
    }

    public string[] GetSaveGameFileNames()
    {
      List<string> stringList = new List<string>();
      DirectoryInfo directory = Directory.CreateDirectory(FileDriver.SavePath);
      foreach (FileInfo file in directory.GetFiles("*.sav", SearchOption.AllDirectories))
      {
        string str = file.FullName.Substring(directory.FullName.Length, file.FullName.Length - directory.FullName.Length - "sav".Length - 1);
        stringList.Add(str);
      }
      return stringList.ToArray();
    }

    public void Delete(string fileName)
    {
      if (File.Exists(fileName))
        File.Delete(fileName);
      else
        Console.WriteLine("There is no such file.");
    }

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
