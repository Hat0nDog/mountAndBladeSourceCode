// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.INetworkMessageWriter
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;

namespace TaleWorlds.Network
{
  public interface INetworkMessageWriter
  {
    void Write(string data);

    void Write(int data);

    void Write(short data);

    void Write(bool data);

    void Write(byte data);

    void Write(float data);

    void Write(long data);

    void Write(ulong data);

    void Write(Guid data);

    void Write(byte[] data);
  }
}
