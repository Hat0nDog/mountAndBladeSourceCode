// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.MessageBuffer
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;

namespace TaleWorlds.Network
{
  internal class MessageBuffer
  {
    internal const int MessageHeaderSize = 4;

    internal MessageBuffer(byte[] buffer, int dataLength)
    {
      this.Buffer = buffer;
      this.DataLength = dataLength;
    }

    internal MessageBuffer(byte[] buffer) => this.Buffer = buffer;

    internal byte[] Buffer { get; private set; }

    internal int DataLength { get; set; }

    internal string GetDebugText() => BitConverter.ToString(this.Buffer, 0, this.DataLength);
  }
}
