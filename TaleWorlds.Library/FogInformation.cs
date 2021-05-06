// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.FogInformation
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public struct FogInformation
  {
    public float Density;
    public Vec3 Color;
    public float Falloff;

    public void DeserializeFrom(IReader reader)
    {
      this.Density = reader.ReadFloat();
      this.Color = reader.ReadVec3();
      this.Falloff = reader.ReadFloat();
    }

    public void SerializeTo(IWriter writer)
    {
      writer.WriteFloat(this.Density);
      writer.WriteVec3(this.Color);
      writer.WriteFloat(this.Falloff);
    }
  }
}
