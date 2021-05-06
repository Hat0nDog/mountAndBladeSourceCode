// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.MissionStateChange
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class MissionStateChange : GameNetworkMessage
  {
    public MissionLobbyComponent.MultiplayerGameState CurrentState { get; private set; }

    public float StateStartTimeInSeconds { get; private set; }

    public MissionStateChange(
      MissionLobbyComponent.MultiplayerGameState currentState,
      long stateStartTimeInTicks)
    {
      this.CurrentState = currentState;
      this.StateStartTimeInSeconds = (float) stateStartTimeInTicks / 1E+07f;
    }

    public MissionStateChange()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.CurrentState = (MissionLobbyComponent.MultiplayerGameState) GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.MissionCurrentStateCompressionInfo, ref bufferReadValid);
      if (this.CurrentState != MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
        this.StateStartTimeInSeconds = GameNetworkMessage.ReadFloatFromPacket(CompressionMatchmaker.MissionTimeCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket((int) this.CurrentState, CompressionMatchmaker.MissionCurrentStateCompressionInfo);
      if (this.CurrentState == MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
        return;
      GameNetworkMessage.WriteFloatToPacket(this.StateStartTimeInSeconds, CompressionMatchmaker.MissionTimeCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

    protected override string OnGetLogFormat() => "Mission State has changed to: " + (object) this.CurrentState;
  }
}
