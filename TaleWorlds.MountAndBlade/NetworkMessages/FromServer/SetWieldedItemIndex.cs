// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetWieldedItemIndex
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SetWieldedItemIndex : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public bool IsLeftHand { get; private set; }

    public bool IsWieldedInstantly { get; private set; }

    public bool IsWieldedOnSpawn { get; private set; }

    public EquipmentIndex WieldedItemIndex { get; private set; }

    public int MainHandCurrentUsageIndex { get; private set; }

    public SetWieldedItemIndex(
      Agent agent,
      bool isLeftHand,
      bool isWieldedInstantly,
      bool isWieldedOnSpawn,
      EquipmentIndex wieldedItemIndex,
      int mainHandCurUsageIndex)
    {
      this.Agent = agent;
      this.IsLeftHand = isLeftHand;
      this.IsWieldedInstantly = isWieldedInstantly;
      this.IsWieldedOnSpawn = isWieldedOnSpawn;
      this.WieldedItemIndex = wieldedItemIndex;
      this.MainHandCurrentUsageIndex = mainHandCurUsageIndex;
    }

    public SetWieldedItemIndex()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.IsLeftHand = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.IsWieldedInstantly = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.IsWieldedOnSpawn = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.WieldedItemIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref bufferReadValid);
      this.MainHandCurrentUsageIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponUsageIndexCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteBoolToPacket(this.IsLeftHand);
      GameNetworkMessage.WriteBoolToPacket(this.IsWieldedInstantly);
      GameNetworkMessage.WriteBoolToPacket(this.IsWieldedOnSpawn);
      GameNetworkMessage.WriteIntToPacket((int) this.WieldedItemIndex, CompressionMission.WieldSlotCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.MainHandCurrentUsageIndex, CompressionMission.WeaponUsageIndexCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.EquipmentDetailed;

    protected override string OnGetLogFormat() => "Set Wielded Item Index to: " + (object) this.WieldedItemIndex + " on Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
