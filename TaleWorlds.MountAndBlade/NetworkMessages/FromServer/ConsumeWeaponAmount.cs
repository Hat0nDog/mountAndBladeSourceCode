// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.ConsumeWeaponAmount
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class ConsumeWeaponAmount : GameNetworkMessage
  {
    public MissionObject SpawnedItemEntity { get; private set; }

    public short ConsumedAmount { get; private set; }

    public ConsumeWeaponAmount(TaleWorlds.MountAndBlade.SpawnedItemEntity spawnedItemEntity, short consumedAmount)
    {
      this.SpawnedItemEntity = (MissionObject) spawnedItemEntity;
      this.ConsumedAmount = consumedAmount;
    }

    public ConsumeWeaponAmount()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.SpawnedItemEntity);
      GameNetworkMessage.WriteIntToPacket((int) this.ConsumedAmount, CompressionGeneric.ItemDataValueCompressionInfo);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.SpawnedItemEntity = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.ConsumedAmount = (short) GameNetworkMessage.ReadIntFromPacket(CompressionGeneric.ItemDataValueCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.EquipmentDetailed;

    protected override string OnGetLogFormat() => "Consumed " + (object) this.ConsumedAmount + " from " + (object) (this.SpawnedItemEntity as TaleWorlds.MountAndBlade.SpawnedItemEntity).WeaponCopy.GetModifiedItemName();
  }
}
