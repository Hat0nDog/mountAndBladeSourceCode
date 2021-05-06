// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.EquipWeaponWithNewEntity
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class EquipWeaponWithNewEntity : GameNetworkMessage
  {
    public MissionWeapon Weapon { get; private set; }

    public EquipmentIndex SlotIndex { get; private set; }

    public Agent Agent { get; private set; }

    public EquipWeaponWithNewEntity(Agent agent, EquipmentIndex slot, MissionWeapon weapon)
    {
      this.Agent = agent;
      this.SlotIndex = slot;
      this.Weapon = weapon;
    }

    public EquipWeaponWithNewEntity()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
      GameNetworkMessage.WriteIntToPacket((int) this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid, true);
      this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref bufferReadValid);
      this.SlotIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat()
    {
      if (this.Agent == null)
        return "Not equipping weapon because there is no agent to equip it to,";
      object[] objArray = new object[8];
      objArray[0] = (object) "Equip weapon with name: ";
      MissionWeapon weapon = this.Weapon;
      TextObject textObject;
      if (weapon.IsEmpty)
      {
        textObject = TextObject.Empty;
      }
      else
      {
        weapon = this.Weapon;
        textObject = weapon.Item.Name;
      }
      objArray[1] = (object) textObject;
      objArray[2] = (object) " from SlotIndex: ";
      objArray[3] = (object) this.SlotIndex;
      objArray[4] = (object) " on agent: ";
      objArray[5] = (object) this.Agent.Name;
      objArray[6] = (object) " with index: ";
      objArray[7] = (object) this.Agent.Index;
      return string.Concat(objArray);
    }
  }
}
