// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.Vec2iBasicTypeSerializer
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
  internal class Vec2iBasicTypeSerializer : IBasicTypeSerializer
  {
    void IBasicTypeSerializer.Serialize(IWriter writer, object value)
    {
      Vec2i vec2i = (Vec2i) value;
      writer.WriteFloat((float) vec2i.Item1);
      writer.WriteFloat((float) vec2i.Item2);
    }

    object IBasicTypeSerializer.Deserialize(IReader reader) => (object) new Vec2i(reader.ReadInt(), reader.ReadInt());
  }
}
