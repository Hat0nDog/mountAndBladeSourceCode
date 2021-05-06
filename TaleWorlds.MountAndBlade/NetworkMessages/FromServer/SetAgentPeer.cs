// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetAgentPeer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SetAgentPeer : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public NetworkCommunicator Peer { get; private set; }

    public SetAgentPeer(Agent agent, NetworkCommunicator peer)
    {
      this.Agent = agent;
      this.Peer = peer;
    }

    public SetAgentPeer()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid, true);
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid, true);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Peers | MultiplayerMessageFilter.Agents;

    protected override string OnGetLogFormat()
    {
      if (this.Agent == null)
        return "Ignoring the message for invalid agent.";
      return "Set NetworkPeer " + (this.Peer != null ? (object) "" : (object) "(to NULL) ") + "on Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
    }
  }
}
