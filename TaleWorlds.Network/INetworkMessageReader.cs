// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.INetworkMessageReader
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;

namespace TaleWorlds.Network
{
  public interface INetworkMessageReader
  {
    int ReadInt32();

    short ReadInt16();

    bool ReadBoolean();

    byte ReadByte();

    string ReadString();

    float ReadFloat();

    long ReadInt64();

    ulong ReadUInt64();

    Guid ReadGuid();

    byte[] ReadByteArray();
  }
}
