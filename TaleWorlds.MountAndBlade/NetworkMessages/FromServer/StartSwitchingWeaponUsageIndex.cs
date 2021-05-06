// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.StartSwitchingWeaponUsageIndex
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class StartSwitchingWeaponUsageIndex : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public EquipmentIndex EquipmentIndex { get; private set; }

    public int UsageIndex { get; private set; }

    public Agent.UsageDirection CurrentMovementFlagUsageDirection { get; private set; }

    public StartSwitchingWeaponUsageIndex(
      Agent agent,
      EquipmentIndex equipmentIndex,
      int usageIndex,
      Agent.UsageDirection currentMovementFlagUsageDirection)
    {
      this.Agent = agent;
      this.EquipmentIndex = equipmentIndex;
      this.UsageIndex = usageIndex;
      this.CurrentMovementFlagUsageDirection = currentMovementFlagUsageDirection;
    }

    public StartSwitchingWeaponUsageIndex()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.EquipmentIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
      this.UsageIndex = (int) (short) GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref bufferReadValid);
      this.CurrentMovementFlagUsageDirection = (Agent.UsageDirection) GameNetworkMessage.ReadIntFromPacket(CompressionMission.UsageDirectionCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket((int) this.EquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.UsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) this.CurrentMovementFlagUsageDirection, CompressionMission.UsageDirectionCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.EquipmentDetailed;

    protected override string OnGetLogFormat() => "StartSwitchingWeaponUsageIndex: " + (object) this.UsageIndex + " for weapon with EquipmentIndex: " + (object) this.EquipmentIndex + " on Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
