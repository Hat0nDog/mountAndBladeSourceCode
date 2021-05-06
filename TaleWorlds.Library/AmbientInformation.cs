// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.AmbientInformation
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public struct AmbientInformation
  {
    public float EnvironmentMultiplier;
    public Vec3 AmbientColor;
    public float MieScatterStrength;
    public float RayleighConstant;

    public void DeserializeFrom(IReader reader)
    {
      this.EnvironmentMultiplier = reader.ReadFloat();
      this.AmbientColor = reader.ReadVec3();
      this.MieScatterStrength = reader.ReadFloat();
      this.RayleighConstant = reader.ReadFloat();
    }

    public void SerializeTo(IWriter writer)
    {
      writer.WriteFloat(this.EnvironmentMultiplier);
      writer.WriteVec3(this.AmbientColor);
      writer.WriteFloat(this.MieScatterStrength);
      writer.WriteFloat(this.RayleighConstant);
    }
  }
}
