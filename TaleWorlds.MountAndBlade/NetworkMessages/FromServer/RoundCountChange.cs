// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.RoundCountChange
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class RoundCountChange : GameNetworkMessage
  {
    public int RoundCount { get; private set; }

    public RoundCountChange(int roundCount) => this.RoundCount = roundCount;

    public RoundCountChange()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.RoundCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissionRoundCountCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteIntToPacket(this.RoundCount, CompressionMission.MissionRoundCountCompressionInfo);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

    protected override string OnGetLogFormat() => "Change round count to: " + (object) this.RoundCount;
  }
}
