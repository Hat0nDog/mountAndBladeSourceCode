// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.Vec2BasicTypeSerializer
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
  internal class Vec2BasicTypeSerializer : IBasicTypeSerializer
  {
    void IBasicTypeSerializer.Serialize(IWriter writer, object value)
    {
      Vec2 vec2 = (Vec2) value;
      writer.WriteVec2(vec2);
    }

    object IBasicTypeSerializer.Deserialize(IReader reader) => (object) reader.ReadVec2();
  }
}
