// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.IWriter
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public interface IWriter
  {
    void WriteSerializableObject(ISerializableObject serializableObject);

    void WriteByte(byte value);

    void WriteSByte(sbyte value);

    void WriteBytes(byte[] bytes);

    void WriteInt(int value);

    void WriteUInt(uint value);

    void WriteShort(short value);

    void WriteUShort(ushort value);

    void WriteString(string value);

    void WriteColor(Color value);

    void WriteBool(bool value);

    void WriteFloat(float value);

    void WriteDouble(double value);

    void WriteULong(ulong value);

    void WriteLong(long value);

    void WriteVec2(Vec2 vec2);

    void WriteVec3(Vec3 vec3);

    void WriteVec3Int(Vec3i vec3);
  }
}
