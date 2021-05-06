// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetRangedSiegeWeaponState
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetRangedSiegeWeaponState : GameNetworkMessage
  {
    public RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

    public RangedSiegeWeapon.WeaponState State { get; private set; }

    public SetRangedSiegeWeaponState(
      RangedSiegeWeapon rangedSiegeWeapon,
      RangedSiegeWeapon.WeaponState state)
    {
      this.RangedSiegeWeapon = rangedSiegeWeapon;
      this.State = state;
    }

    public SetRangedSiegeWeaponState()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.RangedSiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as RangedSiegeWeapon;
      this.State = (RangedSiegeWeapon.WeaponState) GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponStateCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.RangedSiegeWeapon);
      GameNetworkMessage.WriteIntToPacket((int) this.State, CompressionMission.RangedSiegeWeaponStateCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Set RangedSiegeWeapon State to: " + (object) this.State + " on RangedSiegeWeapon with ID: " + (object) this.RangedSiegeWeapon.Id + " and name: " + this.RangedSiegeWeapon.GameEntity.Name;
  }
}
