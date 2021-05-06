// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetWeaponReloadPhase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SetWeaponReloadPhase : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public EquipmentIndex EquipmentIndex { get; private set; }

    public short ReloadPhase { get; private set; }

    public SetWeaponReloadPhase(Agent agent, EquipmentIndex equipmentIndex, short reloadPhase)
    {
      this.Agent = agent;
      this.EquipmentIndex = equipmentIndex;
      this.ReloadPhase = reloadPhase;
    }

    public SetWeaponReloadPhase()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.EquipmentIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
      this.ReloadPhase = (short) GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponReloadPhaseCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket((int) this.EquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) this.ReloadPhase, CompressionMission.WeaponReloadPhaseCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.EquipmentDetailed;

    protected override string OnGetLogFormat() => "Set Reload Phase: " + (object) this.ReloadPhase + " for weapon with EquipmentIndex: " + (object) this.EquipmentIndex + " on Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
