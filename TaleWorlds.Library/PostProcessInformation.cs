// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.PostProcessInformation
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public struct PostProcessInformation
  {
    public float MinExposure;
    public float MaxExposure;
    public float BrightpassThreshold;
    public float MiddleGray;

    public void DeserializeFrom(IReader reader)
    {
      this.MinExposure = reader.ReadFloat();
      this.MaxExposure = reader.ReadFloat();
      this.BrightpassThreshold = reader.ReadFloat();
      this.MiddleGray = reader.ReadFloat();
    }

    public void SerializeTo(IWriter writer)
    {
      writer.WriteFloat(this.MinExposure);
      writer.WriteFloat(this.MaxExposure);
      writer.WriteFloat(this.BrightpassThreshold);
      writer.WriteFloat(this.MiddleGray);
    }
  }
}
