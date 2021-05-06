// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.CreateAgentVisuals
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class CreateAgentVisuals : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public int VisualsIndex { get; private set; }

    public BasicCharacterObject Character { get; private set; }

    public Equipment Equipment { get; private set; }

    public int BodyPropertiesSeed { get; private set; }

    public bool IsFemale { get; private set; }

    public int SelectedEquipmentSetIndex { get; private set; }

    public int TroopCountInFormation { get; private set; }

    public CreateAgentVisuals(
      NetworkCommunicator peer,
      AgentBuildData agentBuildData,
      int selectedEquipmentSetIndex,
      int troopCountInFormation = 0)
    {
      this.Peer = peer;
      this.VisualsIndex = agentBuildData.AgentVisualsIndex;
      this.Character = agentBuildData.AgentCharacter;
      this.BodyPropertiesSeed = agentBuildData.AgentEquipmentSeed;
      this.IsFemale = agentBuildData.AgentIsFemale;
      this.Equipment = new Equipment();
      this.Equipment.FillFrom(agentBuildData.AgentOverridenSpawnEquipment);
      this.SelectedEquipmentSetIndex = selectedEquipmentSetIndex;
      this.TroopCountInFormation = troopCountInFormation;
    }

    public CreateAgentVisuals()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.VisualsIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentOffsetCompressionInfo, ref bufferReadValid);
      this.Character = (BasicCharacterObject) GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref bufferReadValid);
      this.Equipment = new Equipment();
      bool flag = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < (flag ? EquipmentIndex.NumEquipmentSetSlots : EquipmentIndex.ArmorItemEndSlot); ++equipmentIndex)
      {
        EquipmentElement itemRosterElement = ModuleNetworkData.ReadItemReferenceFromPacket(MBObjectManager.Instance, ref bufferReadValid);
        if (bufferReadValid)
          this.Equipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, itemRosterElement);
        else
          break;
      }
      this.BodyPropertiesSeed = GameNetworkMessage.ReadIntFromPacket(CompressionGeneric.RandomSeedCompressionInfo, ref bufferReadValid);
      this.IsFemale = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.SelectedEquipmentSetIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref bufferReadValid);
      this.TroopCountInFormation = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteIntToPacket(this.VisualsIndex, CompressionMission.AgentOffsetCompressionInfo);
      GameNetworkMessage.WriteObjectReferenceToPacket((MBObjectBase) this.Character, CompressionBasic.GUIDCompressionInfo);
      bool flag = this.Equipment[EquipmentIndex.ArmorItemEndSlot].Item != null;
      GameNetworkMessage.WriteBoolToPacket(flag);
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < (flag ? EquipmentIndex.NumEquipmentSetSlots : EquipmentIndex.ArmorItemEndSlot); ++equipmentIndex)
        ModuleNetworkData.WriteItemReferenceToPacket(this.Equipment.GetEquipmentFromSlot(equipmentIndex));
      GameNetworkMessage.WriteIntToPacket(this.BodyPropertiesSeed, CompressionGeneric.RandomSeedCompressionInfo);
      GameNetworkMessage.WriteBoolToPacket(this.IsFemale);
      GameNetworkMessage.WriteIntToPacket(this.SelectedEquipmentSetIndex, CompressionBasic.MissionObjectIDCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.TroopCountInFormation, CompressionBasic.PlayerCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Agents;

    protected override string OnGetLogFormat() => "Create AgentVisuals for peer: " + this.Peer.UserName + ", and with Index: " + (object) this.VisualsIndex;
  }
}
