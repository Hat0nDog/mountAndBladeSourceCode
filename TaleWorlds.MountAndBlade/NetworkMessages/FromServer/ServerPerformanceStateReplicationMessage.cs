// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.ServerPerformanceStateReplicationMessage
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class ServerPerformanceStateReplicationMessage : GameNetworkMessage
  {
    internal ServerPerformanceState ServerPerformanceProblemState { get; private set; }

    public ServerPerformanceStateReplicationMessage()
    {
    }

    internal ServerPerformanceStateReplicationMessage(
      ServerPerformanceState serverPerformanceProblemState)
    {
      this.ServerPerformanceProblemState = serverPerformanceProblemState;
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.ServerPerformanceProblemState = (ServerPerformanceState) GameNetworkMessage.ReadIntFromPacket(CompressionBasic.ServerPerformanceStateCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteIntToPacket((int) this.ServerPerformanceProblemState, CompressionBasic.ServerPerformanceStateCompressionInfo);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionDetailed;

    protected override string OnGetLogFormat() => nameof (ServerPerformanceStateReplicationMessage);
  }
}
