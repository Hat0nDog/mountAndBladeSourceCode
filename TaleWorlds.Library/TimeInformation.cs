// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.TimeInformation
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public struct TimeInformation
  {
    public float TimeOfDay;
    public float NightTimeFactor;
    public float DrynessFactor;
    public float WinterTimeFactor;
    public int Season;

    public void DeserializeFrom(IReader reader)
    {
      this.TimeOfDay = reader.ReadFloat();
      this.NightTimeFactor = reader.ReadFloat();
      this.DrynessFactor = reader.ReadFloat();
      this.WinterTimeFactor = reader.ReadFloat();
      this.Season = reader.ReadInt();
    }

    public void SerializeTo(IWriter writer)
    {
      writer.WriteFloat(this.TimeOfDay);
      writer.WriteFloat(this.NightTimeFactor);
      writer.WriteFloat(this.DrynessFactor);
      writer.WriteFloat(this.WinterTimeFactor);
      writer.WriteInt(this.Season);
    }
  }
}
