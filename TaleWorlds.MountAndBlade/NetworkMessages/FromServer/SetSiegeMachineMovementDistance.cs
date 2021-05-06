// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetSiegeMachineMovementDistance
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetSiegeMachineMovementDistance : GameNetworkMessage
  {
    public UsableMachine UsableMachine { get; private set; }

    public float Distance { get; private set; }

    public SetSiegeMachineMovementDistance(UsableMachine usableMachine, float distance)
    {
      this.UsableMachine = usableMachine;
      this.Distance = distance;
    }

    public SetSiegeMachineMovementDistance()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.UsableMachine = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as UsableMachine;
      this.Distance = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.UsableMachine);
      GameNetworkMessage.WriteFloatToPacket(this.Distance, CompressionBasic.PositionCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Set Movement Distance: " + (object) this.Distance + " of SiegeMachine with ID: " + (object) this.UsableMachine.Id + " and name: " + this.UsableMachine.GameEntity.Name;
  }
}
