// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.AttachWeaponToWeaponInAgentEquipmentSlot
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class AttachWeaponToWeaponInAgentEquipmentSlot : GameNetworkMessage
  {
    public MissionWeapon Weapon { get; private set; }

    public EquipmentIndex SlotIndex { get; private set; }

    public Agent Agent { get; private set; }

    public MatrixFrame AttachLocalFrame { get; private set; }

    public AttachWeaponToWeaponInAgentEquipmentSlot(
      MissionWeapon weapon,
      Agent agent,
      EquipmentIndex slot,
      MatrixFrame attachLocalFrame)
    {
      this.Weapon = weapon;
      this.Agent = agent;
      this.SlotIndex = slot;
      this.AttachLocalFrame = attachLocalFrame;
    }

    public AttachWeaponToWeaponInAgentEquipmentSlot()
    {
    }

    protected override void OnWrite()
    {
      ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket((int) this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
      GameNetworkMessage.WriteVec3ToPacket(this.AttachLocalFrame.origin, CompressionBasic.LocalPositionCompressionInfo);
      GameNetworkMessage.WriteRotationMatrixToPacket(this.AttachLocalFrame.rotation);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref bufferReadValid);
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.SlotIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
      Vec3 o = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.LocalPositionCompressionInfo, ref bufferReadValid);
      Mat3 rot = GameNetworkMessage.ReadRotationMatrixFromPacket(ref bufferReadValid);
      if (bufferReadValid)
        this.AttachLocalFrame = new MatrixFrame(rot, o);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "AttachWeaponToWeaponInAgentEquipmentSlot with name: " + (!this.Weapon.IsEmpty ? (object) this.Weapon.Item.Name : (object) TextObject.Empty) + " to SlotIndex: " + (object) this.SlotIndex + " on agent: " + this.Agent.Name + " with index: " + (object) this.Agent.Index;
  }
}
