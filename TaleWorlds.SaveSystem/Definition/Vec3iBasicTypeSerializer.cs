// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.Vec3iBasicTypeSerializer
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
  internal class Vec3iBasicTypeSerializer : IBasicTypeSerializer
  {
    void IBasicTypeSerializer.Serialize(IWriter writer, object value)
    {
      Vec3i vec3 = (Vec3i) value;
      writer.WriteVec3Int(vec3);
    }

    object IBasicTypeSerializer.Deserialize(IReader reader) => (object) reader.ReadVec3Int();
  }
}
