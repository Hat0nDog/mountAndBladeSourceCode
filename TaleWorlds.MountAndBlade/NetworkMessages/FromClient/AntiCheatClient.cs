// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.AntiCheatClient
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class AntiCheatClient : GameNetworkMessage
  {
    public byte[] Data { get; private set; }

    public AntiCheatClient(byte[] data) => this.Data = data;

    public AntiCheatClient()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.Data.Length, CompressionBasic.DebugIntNonCompressionInfo);
      GameNetworkMessage.WriteByteArrayToPacket(this.Data, 0, this.Data.Length);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      byte[] buffer = new byte[GameNetworkMessage.ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid)];
      GameNetworkMessage.ReadByteArrayFromPacket(buffer, 0, buffer.Length, ref bufferReadValid);
      this.Data = buffer;
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Messaging;

    protected override string OnGetLogFormat() => "Receiving Anti Cheat Client message";
  }
}
