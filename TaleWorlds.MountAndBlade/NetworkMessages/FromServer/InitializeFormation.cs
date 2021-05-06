// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.InitializeFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class InitializeFormation : GameNetworkMessage
  {
    public int FormationIndex { get; private set; }

    public Team Team { get; private set; }

    public string BannerCode { get; private set; }

    public InitializeFormation(Formation formation, Team team, string bannerCode)
    {
      this.FormationIndex = (int) formation.FormationIndex;
      this.Team = team;
      this.BannerCode = !bannerCode.IsStringNoneOrEmpty() ? bannerCode : string.Empty;
    }

    public InitializeFormation()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref bufferReadValid);
      this.Team = GameNetworkMessage.ReadTeamReferenceFromPacket(ref bufferReadValid);
      this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
      GameNetworkMessage.WriteTeamReferenceToPacket(this.Team);
      GameNetworkMessage.WriteStringToPacket(this.BannerCode);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Peers;

    protected override string OnGetLogFormat() => "Initialize formation with index: " + (object) this.FormationIndex + ", for team: " + (object) this.Team.Side;
  }
}
