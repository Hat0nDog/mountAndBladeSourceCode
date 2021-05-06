// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.QuaternionBasicTypeSerializer
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Definition
{
  internal class QuaternionBasicTypeSerializer : IBasicTypeSerializer
  {
    void IBasicTypeSerializer.Serialize(IWriter writer, object value)
    {
      Quaternion quaternion = (Quaternion) value;
      writer.WriteFloat(quaternion.X);
      writer.WriteFloat(quaternion.Y);
      writer.WriteFloat(quaternion.Z);
      writer.WriteFloat(quaternion.W);
    }

    object IBasicTypeSerializer.Deserialize(IReader reader)
    {
      double num1 = (double) reader.ReadFloat();
      float num2 = reader.ReadFloat();
      float num3 = reader.ReadFloat();
      float num4 = reader.ReadFloat();
      double num5 = (double) num2;
      double num6 = (double) num3;
      double num7 = (double) num4;
      return (object) new Quaternion((float) num1, (float) num5, (float) num6, (float) num7);
    }
  }
}
