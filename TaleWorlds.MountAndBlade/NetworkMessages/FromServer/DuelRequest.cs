// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.DuelRequest
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class DuelRequest : GameNetworkMessage
  {
    public Agent RequesterAgent { get; private set; }

    public Agent RequestedAgent { get; private set; }

    public DuelRequest(Agent requesterAgent, Agent requestedAgent)
    {
      this.RequesterAgent = requesterAgent;
      this.RequestedAgent = requestedAgent;
    }

    public DuelRequest()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.RequesterAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.RequestedAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.RequesterAgent);
      GameNetworkMessage.WriteAgentReferenceToPacket(this.RequestedAgent);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.GameMode;

    protected override string OnGetLogFormat() => "Request duel from agent with name: " + this.RequestedAgent.Name + " and index: " + (object) this.RequestedAgent.Index;
  }
}
