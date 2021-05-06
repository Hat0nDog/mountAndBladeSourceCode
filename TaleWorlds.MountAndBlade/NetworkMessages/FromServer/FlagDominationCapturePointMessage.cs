// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.FlagDominationCapturePointMessage
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class FlagDominationCapturePointMessage : GameNetworkMessage
  {
    public int FlagIndex { get; private set; }

    public Team OwnerTeam { get; private set; }

    public FlagDominationCapturePointMessage()
    {
    }

    public FlagDominationCapturePointMessage(int flagIndex, Team owner)
    {
      this.FlagIndex = flagIndex;
      this.OwnerTeam = owner;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.FlagIndex, CompressionMission.FlagCapturePointIndexCompressionInfo);
      GameNetworkMessage.WriteTeamReferenceToPacket(this.OwnerTeam);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.FlagIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.FlagCapturePointIndexCompressionInfo, ref bufferReadValid);
      this.OwnerTeam = GameNetworkMessage.ReadTeamReferenceFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.GameMode;

    protected override string OnGetLogFormat() => "Flag owner changed.";
  }
}
