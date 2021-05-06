// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.ReplaceBotWithPlayer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class ReplaceBotWithPlayer : GameNetworkMessage
  {
    public Agent BotAgent { get; private set; }

    public NetworkCommunicator Peer { get; private set; }

    public int Health { get; private set; }

    public int MountHealth { get; private set; }

    public ReplaceBotWithPlayer(NetworkCommunicator peer, Agent botAgent)
    {
      this.Peer = peer;
      this.BotAgent = botAgent;
      this.Health = (int) Math.Ceiling((double) botAgent.Health);
      this.MountHealth = (int) Math.Ceiling((double) this.BotAgent.MountAgent?.Health ?? -1.0);
    }

    public ReplaceBotWithPlayer()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.BotAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.Health = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentHealthCompressionInfo, ref bufferReadValid);
      this.MountHealth = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentHealthCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteAgentReferenceToPacket(this.BotAgent);
      GameNetworkMessage.WriteIntToPacket(this.Health, CompressionMission.AgentHealthCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.MountHealth, CompressionMission.AgentHealthCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Agents;

    protected override string OnGetLogFormat() => "Replace a bot with a player";
  }
}
