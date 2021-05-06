// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SpawnAttachedWeaponOnSpawnedWeapon
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SpawnAttachedWeaponOnSpawnedWeapon : GameNetworkMessage
  {
    public SpawnedItemEntity SpawnedWeapon { get; private set; }

    public int AttachmentIndex { get; private set; }

    public int ForcedIndex { get; private set; }

    public SpawnAttachedWeaponOnSpawnedWeapon(
      SpawnedItemEntity spawnedWeapon,
      int attachmentIndex,
      int forcedIndex)
    {
      this.SpawnedWeapon = spawnedWeapon;
      this.AttachmentIndex = attachmentIndex;
      this.ForcedIndex = forcedIndex;
    }

    public SpawnAttachedWeaponOnSpawnedWeapon()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.SpawnedWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as SpawnedItemEntity;
      this.AttachmentIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponAttachmentIndexCompressionInfo, ref bufferReadValid);
      this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.SpawnedWeapon);
      GameNetworkMessage.WriteIntToPacket(this.AttachmentIndex, CompressionMission.WeaponAttachmentIndexCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Items;

    protected override string OnGetLogFormat() => "SpawnAttachedWeaponOnSpawnedWeapon with Spawned Weapon ID: " + (object) this.SpawnedWeapon.Id.Id + " AttachmentIndex: " + (object) this.AttachmentIndex + " Attached Weapon ID: " + (object) this.ForcedIndex;
  }
}
