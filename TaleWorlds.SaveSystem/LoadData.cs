// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.LoadData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

namespace TaleWorlds.SaveSystem
{
  public class LoadData
  {
    public MetaData MetaData { get; private set; }

    public GameData GameData { get; private set; }

    public LoadData(MetaData metaData, GameData gameData)
    {
      this.MetaData = metaData;
      this.GameData = gameData;
    }
  }
}
