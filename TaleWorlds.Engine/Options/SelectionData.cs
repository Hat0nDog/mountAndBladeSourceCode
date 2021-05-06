// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Options.SelectionData
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

namespace TaleWorlds.Engine.Options
{
  public struct SelectionData
  {
    public bool IsLocalizationId;
    public string Data;

    public SelectionData(bool isLocalizationId, string data)
    {
      this.IsLocalizationId = isLocalizationId;
      this.Data = data;
    }
  }
}
