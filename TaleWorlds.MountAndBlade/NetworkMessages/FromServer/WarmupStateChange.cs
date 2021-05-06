// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.WarmupStateChange
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class WarmupStateChange : GameNetworkMessage
  {
    public MultiplayerWarmupComponent.WarmupStates WarmupState { get; private set; }

    public float StateStartTimeInSeconds { get; private set; }

    public WarmupStateChange(
      MultiplayerWarmupComponent.WarmupStates warmupState,
      long stateStartTimeInTicks)
    {
      this.WarmupState = warmupState;
      this.StateStartTimeInSeconds = (float) stateStartTimeInTicks / 1E+07f;
    }

    public WarmupStateChange()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket((int) this.WarmupState, CompressionMission.MissionRoundStateCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.WarmupState = (MultiplayerWarmupComponent.WarmupStates) GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissionRoundStateCompressionInfo, ref bufferReadValid);
      this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionDetailed;

    protected override string OnGetLogFormat() => "Warmup state set to " + (object) this.WarmupState;
  }
}
