// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.PingReplication
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class PingReplication : GameNetworkMessage
  {
    public const int MaxPingToReplicate = 1023;

    internal NetworkCommunicator Peer { get; private set; }

    internal int PingValue { get; private set; }

    public PingReplication()
    {
    }

    internal PingReplication(NetworkCommunicator peer, int ping)
    {
      this.Peer = peer;
      this.PingValue = ping;
      if (this.PingValue <= 1023)
        return;
      this.PingValue = 1023;
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid, true);
      this.PingValue = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PingValueCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteIntToPacket(this.PingValue, CompressionBasic.PingValueCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionDetailed;

    protected override string OnGetLogFormat() => nameof (PingReplication);
  }
}
