// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.SunInformation
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public struct SunInformation
  {
    public float Altitude;
    public float Angle;
    public Vec3 Color;
    public float Brightness;
    public float MaxBrightness;
    public float Size;
    public float RayStrength;

    public void DeserializeFrom(IReader reader)
    {
      this.Altitude = reader.ReadFloat();
      this.Angle = reader.ReadFloat();
      this.Color = reader.ReadVec3();
      this.Brightness = reader.ReadFloat();
      this.MaxBrightness = reader.ReadFloat();
      this.Size = reader.ReadFloat();
      this.RayStrength = reader.ReadFloat();
    }

    public void SerializeTo(IWriter writer)
    {
      writer.WriteFloat(this.Altitude);
      writer.WriteFloat(this.Angle);
      writer.WriteVec3(this.Color);
      writer.WriteFloat(this.Brightness);
      writer.WriteFloat(this.MaxBrightness);
      writer.WriteFloat(this.Size);
      writer.WriteFloat(this.RayStrength);
    }
  }
}
