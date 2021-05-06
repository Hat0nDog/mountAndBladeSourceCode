// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.AreaInformation
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public struct AreaInformation
  {
    public float Temperature;
    public float Humidity;
    public int AreaType;

    public void DeserializeFrom(IReader reader)
    {
      this.Temperature = reader.ReadFloat();
      this.Humidity = reader.ReadFloat();
      this.AreaType = reader.ReadInt();
    }

    public void SerializeTo(IWriter writer)
    {
      writer.WriteFloat(this.Temperature);
      writer.WriteFloat(this.Humidity);
      writer.WriteInt(this.AreaType);
    }
  }
}
