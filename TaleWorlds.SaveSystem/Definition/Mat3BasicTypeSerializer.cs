// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.Mat3BasicTypeSerializer
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
  internal class Mat3BasicTypeSerializer : IBasicTypeSerializer
  {
    void IBasicTypeSerializer.Serialize(IWriter writer, object value)
    {
      Mat3 mat3 = (Mat3) value;
      writer.WriteVec3(mat3.s);
      writer.WriteVec3(mat3.f);
      writer.WriteVec3(mat3.u);
    }

    object IBasicTypeSerializer.Deserialize(IReader reader)
    {
      Vec3 s = reader.ReadVec3();
      Vec3 vec3_1 = reader.ReadVec3();
      Vec3 vec3_2 = reader.ReadVec3();
      Vec3 f = vec3_1;
      Vec3 u = vec3_2;
      return (object) new Mat3(s, f, u);
    }
  }
}
