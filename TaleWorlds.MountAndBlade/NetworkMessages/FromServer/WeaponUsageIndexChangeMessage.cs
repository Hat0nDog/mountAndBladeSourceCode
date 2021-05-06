// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.WeaponUsageIndexChangeMessage
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class WeaponUsageIndexChangeMessage : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public EquipmentIndex SlotIndex { get; private set; }

    public int UsageIndex { get; private set; }

    public WeaponUsageIndexChangeMessage()
    {
    }

    public WeaponUsageIndexChangeMessage(Agent agent, EquipmentIndex slotIndex, int usageIndex)
    {
      this.Agent = agent;
      this.SlotIndex = slotIndex;
      this.UsageIndex = usageIndex;
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.SlotIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
      this.UsageIndex = (int) (short) GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket((int) this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.UsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionDetailed;

    protected override string OnGetLogFormat()
    {
      object[] objArray = new object[8]
      {
        (object) "Set Weapon Usage Index: ",
        (object) this.UsageIndex,
        (object) " for weapon with EquipmentIndex: ",
        (object) this.SlotIndex,
        (object) " on Agent with name: ",
        this.Agent != null ? (object) this.Agent.Name : (object) "null agent",
        (object) " and agent-index: ",
        null
      };
      Agent agent = this.Agent;
      objArray[7] = (object) (agent != null ? agent.Index : -1);
      return string.Concat(objArray);
    }
  }
}
