// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.CreateAgent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class CreateAgent : GameNetworkMessage
  {
    public int AgentIndex { get; private set; }

    public int MountAgentIndex { get; private set; }

    public NetworkCommunicator Peer { get; private set; }

    public BasicCharacterObject Character { get; private set; }

    public Monster Monster { get; private set; }

    public MissionEquipment SpawnMissionEquipment { get; private set; }

    public Equipment SpawnEquipment { get; private set; }

    public BodyProperties BodyPropertiesValue { get; private set; }

    public int BodyPropertiesSeed { get; private set; }

    public bool IsFemale { get; private set; }

    public Team Team { get; private set; }

    public Vec3 Position { get; private set; }

    public Vec2 Direction { get; private set; }

    public int FormationIndex { get; private set; }

    public bool IsPlayerAgent { get; private set; }

    public uint ClothingColor1 { get; private set; }

    public uint ClothingColor2 { get; private set; }

    public CreateAgent(
      Agent agent,
      bool isPlayerAgent,
      Vec3 position,
      Vec2 direction,
      NetworkCommunicator peer)
    {
      this.AgentIndex = agent.Index;
      bool flag = agent.MountAgent != null && agent.MountAgent.RiderAgent == agent;
      this.MountAgentIndex = flag ? agent.MountAgent.Index : -1;
      this.Peer = peer;
      this.Character = agent.Character;
      this.Monster = agent.Monster;
      this.SpawnEquipment = new Equipment();
      this.SpawnMissionEquipment = new MissionEquipment();
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
        this.SpawnMissionEquipment[index] = agent.Equipment[index];
      for (EquipmentIndex equipmentIndex = EquipmentIndex.NumAllWeaponSlots; equipmentIndex < EquipmentIndex.ArmorItemEndSlot; ++equipmentIndex)
        this.SpawnEquipment[equipmentIndex] = agent.SpawnEquipment.GetEquipmentFromSlot(equipmentIndex);
      if (flag)
      {
        this.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot] = agent.MountAgent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot];
        this.SpawnEquipment[EquipmentIndex.HorseHarness] = agent.MountAgent.SpawnEquipment[EquipmentIndex.HorseHarness];
      }
      else
      {
        this.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot] = new EquipmentElement();
        this.SpawnEquipment[EquipmentIndex.HorseHarness] = new EquipmentElement();
      }
      this.BodyPropertiesValue = agent.BodyPropertiesValue;
      this.BodyPropertiesSeed = agent.BodyPropertiesSeed;
      this.IsFemale = agent.IsFemale;
      this.Team = agent.Team;
      this.Position = position;
      this.Direction = direction;
      Formation formation = agent.Formation;
      this.FormationIndex = formation != null ? formation.Index : -1;
      this.ClothingColor1 = agent.ClothingColor1;
      this.ClothingColor2 = agent.ClothingColor2;
      this.IsPlayerAgent = isPlayerAgent;
    }

    public CreateAgent()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Character = (BasicCharacterObject) GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref bufferReadValid);
      this.Monster = (Monster) GameNetworkMessage.ReadObjectReferenceFromPacket(MBObjectManager.Instance, CompressionBasic.GUIDCompressionInfo, ref bufferReadValid);
      this.AgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(CompressionMission.AgentCompressionInfo, ref bufferReadValid);
      this.MountAgentIndex = GameNetworkMessage.ReadAgentIndexFromPacket(CompressionMission.AgentCompressionInfo, ref bufferReadValid);
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.SpawnEquipment = new Equipment();
      this.SpawnMissionEquipment = new MissionEquipment();
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
        this.SpawnMissionEquipment[index] = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref bufferReadValid);
      for (EquipmentIndex equipmentIndex = EquipmentIndex.NumAllWeaponSlots; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; ++equipmentIndex)
        this.SpawnEquipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, ModuleNetworkData.ReadItemReferenceFromPacket(MBObjectManager.Instance, ref bufferReadValid));
      this.IsPlayerAgent = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.BodyPropertiesSeed = !this.IsPlayerAgent ? GameNetworkMessage.ReadIntFromPacket(CompressionGeneric.RandomSeedCompressionInfo, ref bufferReadValid) : 0;
      this.BodyPropertiesValue = GameNetworkMessage.ReadBodyPropertiesFromPacket(ref bufferReadValid);
      this.IsFemale = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.Team = GameNetworkMessage.ReadTeamReferenceFromPacket(ref bufferReadValid);
      this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
      this.Direction = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid).Normalized();
      this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref bufferReadValid);
      this.ClothingColor1 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref bufferReadValid);
      this.ClothingColor2 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteObjectReferenceToPacket((MBObjectBase) this.Character, CompressionBasic.GUIDCompressionInfo);
      GameNetworkMessage.WriteObjectReferenceToPacket((MBObjectBase) this.Monster, CompressionBasic.GUIDCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.AgentIndex, CompressionMission.AgentCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.MountAgentIndex, CompressionMission.AgentCompressionInfo);
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
        ModuleNetworkData.WriteWeaponReferenceToPacket(this.SpawnMissionEquipment[index]);
      for (EquipmentIndex equipmentIndex = EquipmentIndex.NumAllWeaponSlots; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; ++equipmentIndex)
        ModuleNetworkData.WriteItemReferenceToPacket(this.SpawnEquipment.GetEquipmentFromSlot(equipmentIndex));
      GameNetworkMessage.WriteBoolToPacket(this.IsPlayerAgent);
      if (!this.IsPlayerAgent)
        GameNetworkMessage.WriteIntToPacket(this.BodyPropertiesSeed, CompressionGeneric.RandomSeedCompressionInfo);
      BodyProperties bodyProperties = this.BodyPropertiesValue;
      GameNetworkMessage.WriteBodyPropertiesToPacket(in bodyProperties);
      GameNetworkMessage.WriteBoolToPacket(this.IsFemale);
      GameNetworkMessage.WriteTeamReferenceToPacket(this.Team);
      GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
      GameNetworkMessage.WriteVec2ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
      GameNetworkMessage.WriteUintToPacket(this.ClothingColor1, CompressionGeneric.ColorCompressionInfo);
      GameNetworkMessage.WriteUintToPacket(this.ClothingColor2, CompressionGeneric.ColorCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Agents;

    protected override string OnGetLogFormat()
    {
      object[] objArray = new object[4]
      {
        (object) "Create an agent with index: ",
        (object) this.AgentIndex,
        null,
        null
      };
      string str;
      if (this.Peer == null)
        str = "";
      else
        str = ", belonging to peer with Name: " + this.Peer.UserName + ", and peer-index: " + (object) this.Peer.Index;
      objArray[2] = (object) str;
      objArray[3] = this.MountAgentIndex == -1 ? (object) "" : (object) (", owning a mount with index: " + (object) this.MountAgentIndex);
      return string.Concat(objArray);
    }
  }
}
