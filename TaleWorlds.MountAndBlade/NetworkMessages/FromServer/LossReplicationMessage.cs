// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.LossReplicationMessage
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class LossReplicationMessage : GameNetworkMessage
  {
    internal int LossValue { get; private set; }

    public LossReplicationMessage()
    {
    }

    internal LossReplicationMessage(int lossValue) => this.LossValue = lossValue;

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.LossValue = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.LossValueCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteIntToPacket(this.LossValue, CompressionBasic.LossValueCompressionInfo);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionDetailed;

    protected override string OnGetLogFormat() => nameof (LossReplicationMessage);
  }
}
