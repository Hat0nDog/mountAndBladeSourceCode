// Decompiled with JetBrains decompiler
// Type: TaleWorlds.ObjectSystem.MMOGUIDBasicTypeSerializer
// Assembly: TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 525679E6-68C3-48B8-A030-465E146E69EE
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ObjectSystem.dll

using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.ObjectSystem
{
  internal class MMOGUIDBasicTypeSerializer : IBasicTypeSerializer
  {
    void IBasicTypeSerializer.Serialize(IWriter writer, object value)
    {
      MBGUID mbguid = (MBGUID) value;
      writer.WriteUInt(mbguid.InternalValue);
    }

    object IBasicTypeSerializer.Deserialize(IReader reader) => (object) new MBGUID(reader.ReadUInt());
  }
}
