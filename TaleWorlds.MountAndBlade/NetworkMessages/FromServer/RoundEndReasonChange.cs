// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.RoundEndReasonChange
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class RoundEndReasonChange : GameNetworkMessage
  {
    public RoundEndReason RoundEndReason { get; private set; }

    public RoundEndReasonChange() => this.RoundEndReason = RoundEndReason.Invalid;

    public RoundEndReasonChange(RoundEndReason roundEndReason) => this.RoundEndReason = roundEndReason;

    protected override void OnWrite() => GameNetworkMessage.WriteIntToPacket((int) this.RoundEndReason, CompressionMission.RoundEndReasonCompressionInfo);

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.RoundEndReason = (RoundEndReason) GameNetworkMessage.ReadIntFromPacket(CompressionMission.RoundEndReasonCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.GameMode;

    protected override string OnGetLogFormat() => "Change round end reason to: " + this.RoundEndReason.ToString();
  }
}
