// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.TeamSetIsEnemyOf
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class TeamSetIsEnemyOf : GameNetworkMessage
  {
    public MBTeam Team1 { get; private set; }

    public MBTeam Team2 { get; private set; }

    public bool IsEnemyOf { get; private set; }

    public TeamSetIsEnemyOf(Team team1, Team team2, bool isEnemyOf)
    {
      this.Team1 = team1.MBTeam;
      this.Team2 = team2.MBTeam;
      this.IsEnemyOf = isEnemyOf;
    }

    public TeamSetIsEnemyOf()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team1, CompressionMission.TeamCompressionInfo);
      GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team2, CompressionMission.TeamCompressionInfo);
      GameNetworkMessage.WriteBoolToPacket(this.IsEnemyOf);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Team1 = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref bufferReadValid);
      this.Team2 = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref bufferReadValid);
      this.IsEnemyOf = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

    protected override string OnGetLogFormat() => this.Team1.ToString() + " is now " + (this.IsEnemyOf ? (object) "" : (object) "not an ") + "enemy of " + (object) this.Team2;
  }
}
