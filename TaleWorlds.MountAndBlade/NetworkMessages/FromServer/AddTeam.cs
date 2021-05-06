// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.AddTeam
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class AddTeam : GameNetworkMessage
  {
    public MBTeam Team { get; private set; }

    public BattleSideEnum Side { get; private set; }

    public uint Color { get; private set; }

    public uint Color2 { get; private set; }

    public string BannerCode { get; private set; }

    public bool IsPlayerGeneral { get; private set; }

    public bool IsPlayerSergeant { get; private set; }

    public AddTeam(TaleWorlds.MountAndBlade.Team team)
    {
      this.Team = team.MBTeam;
      this.Side = team.Side;
      this.Color = team.Color;
      this.Color2 = team.Color2;
      this.BannerCode = team.Banner != null ? TaleWorlds.Core.BannerCode.CreateFrom(team.Banner).Code : string.Empty;
      this.IsPlayerGeneral = team.IsPlayerGeneral;
      this.IsPlayerSergeant = team.IsPlayerSergeant;
    }

    public AddTeam()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Team = GameNetworkMessage.ReadMBTeamReferenceFromPacket(CompressionMission.TeamCompressionInfo, ref bufferReadValid);
      this.Side = (BattleSideEnum) GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref bufferReadValid);
      this.Color = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref bufferReadValid);
      this.Color2 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref bufferReadValid);
      this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      this.IsPlayerGeneral = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.IsPlayerSergeant = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMBTeamReferenceToPacket(this.Team, CompressionMission.TeamCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) this.Side, CompressionMission.TeamSideCompressionInfo);
      GameNetworkMessage.WriteUintToPacket(this.Color, CompressionGeneric.ColorCompressionInfo);
      GameNetworkMessage.WriteUintToPacket(this.Color2, CompressionGeneric.ColorCompressionInfo);
      GameNetworkMessage.WriteStringToPacket(this.BannerCode);
      GameNetworkMessage.WriteBoolToPacket(this.IsPlayerGeneral);
      GameNetworkMessage.WriteBoolToPacket(this.IsPlayerSergeant);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

    protected override string OnGetLogFormat() => "Add team with side: " + (object) this.Side;
  }
}
