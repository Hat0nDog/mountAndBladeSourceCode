// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetSiegeTowerGateState
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetSiegeTowerGateState : GameNetworkMessage
  {
    public SiegeTower SiegeTower { get; private set; }

    public SiegeTower.GateState State { get; private set; }

    public SetSiegeTowerGateState(SiegeTower siegeTower, SiegeTower.GateState state)
    {
      this.SiegeTower = siegeTower;
      this.State = state;
    }

    public SetSiegeTowerGateState()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.SiegeTower = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as SiegeTower;
      this.State = (SiegeTower.GateState) GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeTowerGateStateCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.SiegeTower);
      GameNetworkMessage.WriteIntToPacket((int) this.State, CompressionMission.SiegeTowerGateStateCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Set SiegeTower State to: " + (object) this.State + " on SiegeTower with ID: " + (object) this.SiegeTower.Id + " and name: " + this.SiegeTower.GameEntity.Name;
  }
}
