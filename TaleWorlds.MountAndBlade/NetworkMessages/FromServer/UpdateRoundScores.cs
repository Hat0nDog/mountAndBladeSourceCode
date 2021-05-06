// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.UpdateRoundScores
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class UpdateRoundScores : GameNetworkMessage
  {
    public int AttackerTeamScore { get; private set; }

    public int DefenderTeamScore { get; private set; }

    public UpdateRoundScores(int attackerTeamScore, int defenderTeamScore)
    {
      this.AttackerTeamScore = attackerTeamScore;
      this.DefenderTeamScore = defenderTeamScore;
    }

    public UpdateRoundScores()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.AttackerTeamScore = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamScoreCompressionInfo, ref bufferReadValid);
      this.DefenderTeamScore = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamScoreCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.AttackerTeamScore, CompressionMission.TeamScoreCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.DefenderTeamScore, CompressionMission.TeamScoreCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission | MultiplayerMessageFilter.GameMode;

    protected override string OnGetLogFormat() => "Update round score. Attackers: " + (object) this.AttackerTeamScore + ", defenders: " + (object) this.DefenderTeamScore;
  }
}
