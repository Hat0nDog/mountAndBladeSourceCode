// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetWeaponNetworkData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SetWeaponNetworkData : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public EquipmentIndex WeaponEquipmentIndex { get; private set; }

    public short DataValue { get; private set; }

    public SetWeaponNetworkData(Agent agent, EquipmentIndex weaponEquipmentIndex, short dataValue)
    {
      this.Agent = agent;
      this.WeaponEquipmentIndex = weaponEquipmentIndex;
      this.DataValue = dataValue;
    }

    public SetWeaponNetworkData()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.WeaponEquipmentIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
      this.DataValue = (short) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemDataCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket((int) this.WeaponEquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) this.DataValue, CompressionMission.ItemDataCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.EquipmentDetailed;

    protected override string OnGetLogFormat() => "Set Network data: " + (object) this.DataValue + " for weapon with EquipmentIndex: " + (object) this.WeaponEquipmentIndex + " on Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
