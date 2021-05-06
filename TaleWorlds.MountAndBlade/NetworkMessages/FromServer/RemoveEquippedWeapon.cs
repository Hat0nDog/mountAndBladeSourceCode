// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.RemoveEquippedWeapon
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class RemoveEquippedWeapon : GameNetworkMessage
  {
    public EquipmentIndex SlotIndex { get; private set; }

    public Agent Agent { get; private set; }

    public RemoveEquippedWeapon(Agent a, EquipmentIndex slot)
    {
      this.Agent = a;
      this.SlotIndex = slot;
    }

    public RemoveEquippedWeapon()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket((int) this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.SlotIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "Remove equipped weapon from SlotIndex: " + (object) this.SlotIndex + " on agent: " + this.Agent.Name + " with index: " + (object) this.Agent.Index;
  }
}
