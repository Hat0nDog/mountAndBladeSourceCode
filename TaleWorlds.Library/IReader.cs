// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.IReader
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public interface IReader
  {
    ISerializableObject ReadSerializableObject();

    int ReadInt();

    short ReadShort();

    string ReadString();

    Color ReadColor();

    bool ReadBool();

    float ReadFloat();

    uint ReadUInt();

    ulong ReadULong();

    long ReadLong();

    byte ReadByte();

    byte[] ReadBytes(int length);

    Vec2 ReadVec2();

    Vec3 ReadVec3();

    Vec3i ReadVec3Int();

    sbyte ReadSByte();

    ushort ReadUShort();

    double ReadDouble();
  }
}
