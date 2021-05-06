// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.AgentSetFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class AgentSetFormation : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public int FormationIndex { get; private set; }

    public AgentSetFormation(Agent agent, int formationIndex)
    {
      this.Agent = agent;
      this.FormationIndex = formationIndex;
    }

    public AgentSetFormation()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Formations | MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "Assign agent with name: " + this.Agent.Name + ", and index: " + (object) this.Agent.Index + " to formation with index: " + (object) this.FormationIndex;
  }
}
