// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.SetMachineRotation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class SetMachineRotation : GameNetworkMessage
  {
    public UsableMachine UsableMachine { get; private set; }

    public float HorizontalRotation { get; private set; }

    public float VerticalRotation { get; private set; }

    public SetMachineRotation(
      UsableMachine missionObject,
      float horizontalRotation,
      float verticalRotation)
    {
      this.UsableMachine = missionObject;
      this.HorizontalRotation = horizontalRotation;
      this.VerticalRotation = verticalRotation;
    }

    public SetMachineRotation()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.UsableMachine = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as UsableMachine;
      this.HorizontalRotation = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.HighResRadianCompressionInfo, ref bufferReadValid);
      this.VerticalRotation = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.HighResRadianCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.UsableMachine);
      GameNetworkMessage.WriteFloatToPacket(this.HorizontalRotation, CompressionBasic.HighResRadianCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.VerticalRotation, CompressionBasic.HighResRadianCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Set rotation of UsableMachine with ID: " + (object) this.UsableMachine.Id + " and with name: " + this.UsableMachine.GameEntity.Name;
  }
}
