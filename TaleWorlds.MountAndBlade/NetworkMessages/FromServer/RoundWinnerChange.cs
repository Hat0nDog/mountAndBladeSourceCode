// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.RoundWinnerChange
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class RoundWinnerChange : GameNetworkMessage
  {
    public BattleSideEnum RoundWinner { get; private set; }

    public RoundWinnerChange(BattleSideEnum roundWinner) => this.RoundWinner = roundWinner;

    public RoundWinnerChange() => this.RoundWinner = BattleSideEnum.None;

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.RoundWinner = (BattleSideEnum) GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteIntToPacket((int) this.RoundWinner, CompressionMission.TeamSideCompressionInfo);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

    protected override string OnGetLogFormat() => "Change round winner to: " + this.RoundWinner.ToString();
  }
}
