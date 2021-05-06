// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.ISaveDriver
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

namespace TaleWorlds.SaveSystem
{
  public interface ISaveDriver
  {
    void SetFileName(string fileName);

    void Save(int version, MetaData metaData, GameData gameData);

    SaveGameFileInfo[] GetSaveGameFileInfos();

    string[] GetSaveGameFileNames();

    MetaData LoadMetaData();

    LoadData Load();

    void Delete(string fileName);
  }
}
