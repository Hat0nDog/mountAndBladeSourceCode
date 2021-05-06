// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.RoundStateChange
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class RoundStateChange : GameNetworkMessage
  {
    public MultiplayerRoundState RoundState { get; private set; }

    public float StateStartTimeInSeconds { get; private set; }

    public int RemainingTimeOnPreviousState { get; private set; }

    public RoundStateChange(
      MultiplayerRoundState roundState,
      long stateStartTimeInTicks,
      int remainingTimeOnPreviousState)
    {
      this.RoundState = roundState;
      this.StateStartTimeInSeconds = (float) stateStartTimeInTicks / 1E+07f;
      this.RemainingTimeOnPreviousState = remainingTimeOnPreviousState;
    }

    public RoundStateChange()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.RoundState = (MultiplayerRoundState) GameNetworkMessage.ReadIntFromPacket(CompressionMission.MissionRoundStateCompressionInfo, ref bufferReadValid);
      this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref bufferReadValid);
      this.RemainingTimeOnPreviousState = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RoundTimeCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket((int) this.RoundState, CompressionMission.MissionRoundStateCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.RemainingTimeOnPreviousState, CompressionMission.RoundTimeCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

    protected override string OnGetLogFormat() => "Changing round state to: " + (object) this.RoundState;
  }
}
