// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SpawnWeaponWithNewEntity
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SpawnWeaponWithNewEntity : GameNetworkMessage
  {
    public MissionWeapon Weapon { get; private set; }

    public Mission.WeaponSpawnFlags WeaponSpawnFlags { get; private set; }

    public int ForcedIndex { get; private set; }

    public MatrixFrame Frame { get; private set; }

    public MissionObject ParentMissionObject { get; private set; }

    public bool IsVisible { get; private set; }

    public bool HasLifeTime { get; private set; }

    public SpawnWeaponWithNewEntity(
      MissionWeapon weapon,
      Mission.WeaponSpawnFlags weaponSpawnFlags,
      int forcedIndex,
      MatrixFrame frame,
      MissionObject parentMissionObject,
      bool isVisible,
      bool hasLifeTime)
    {
      this.Weapon = weapon;
      this.WeaponSpawnFlags = weaponSpawnFlags;
      this.ForcedIndex = forcedIndex;
      this.Frame = frame;
      this.ParentMissionObject = parentMissionObject;
      this.IsVisible = isVisible;
      this.HasLifeTime = hasLifeTime;
    }

    public SpawnWeaponWithNewEntity()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Weapon = ModuleNetworkData.ReadWeaponReferenceFromPacket(MBObjectManager.Instance, ref bufferReadValid);
      this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref bufferReadValid);
      this.WeaponSpawnFlags = (Mission.WeaponSpawnFlags) GameNetworkMessage.ReadIntFromPacket(CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo, ref bufferReadValid);
      this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref bufferReadValid);
      this.ParentMissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.IsVisible = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.HasLifeTime = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      ModuleNetworkData.WriteWeaponReferenceToPacket(this.Weapon);
      GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
      GameNetworkMessage.WriteIntToPacket((int) this.WeaponSpawnFlags, CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.ParentMissionObject);
      GameNetworkMessage.WriteBoolToPacket(this.IsVisible);
      GameNetworkMessage.WriteBoolToPacket(this.HasLifeTime);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Items;

    protected override string OnGetLogFormat() => "Spawn Weapon with name: " + (object) this.Weapon.Item.Name + ", and with ID: " + (object) this.ForcedIndex;
  }
}
