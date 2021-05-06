// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.AgentSetTeam
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class AgentSetTeam : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public MBTeam Team { get; private set; }

    public AgentSetTeam(Agent agent, TaleWorlds.MountAndBlade.Team team)
    {
      this.Agent = agent;
      this.Team = team != null ? team.MBTeam : MBTeam.InvalidTeam;
    }

    public AgentSetTeam()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.Team = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team, CompressionMission.TeamCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Agents;

    protected override string OnGetLogFormat() => "Assign agent with name: " + this.Agent.Name + ", and index: " + (object) this.Agent.Index + " to team: " + (object) this.Team;
  }
}
