// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.Mat2BasicTypeSerializer
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
  internal class Mat2BasicTypeSerializer : IBasicTypeSerializer
  {
    void IBasicTypeSerializer.Serialize(IWriter writer, object value)
    {
      Mat2 mat2 = (Mat2) value;
      writer.WriteVec2(mat2.s);
      writer.WriteVec2(mat2.f);
    }

    object IBasicTypeSerializer.Deserialize(IReader reader)
    {
      Vec2 vec2_1 = reader.ReadVec2();
      Vec2 vec2_2 = reader.ReadVec2();
      return (object) new Mat2(vec2_1.x, vec2_1.y, vec2_2.x, vec2_2.y);
    }
  }
}
