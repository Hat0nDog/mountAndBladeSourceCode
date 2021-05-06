// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.SkyInformation
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public struct SkyInformation
  {
    public float Brightness;

    public void DeserializeFrom(IReader reader) => this.Brightness = reader.ReadFloat();

    public void SerializeTo(IWriter writer) => writer.WriteFloat(this.Brightness);
  }
}
