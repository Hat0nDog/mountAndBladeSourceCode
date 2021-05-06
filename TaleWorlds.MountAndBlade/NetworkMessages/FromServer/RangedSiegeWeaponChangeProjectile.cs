// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.RangedSiegeWeaponChangeProjectile
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class RangedSiegeWeaponChangeProjectile : GameNetworkMessage
  {
    public RangedSiegeWeapon RangedSiegeWeapon { get; private set; }

    public int Index { get; private set; }

    public RangedSiegeWeaponChangeProjectile(RangedSiegeWeapon rangedSiegeWeapon, int index)
    {
      this.RangedSiegeWeapon = rangedSiegeWeapon;
      this.Index = index;
    }

    public RangedSiegeWeaponChangeProjectile()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.RangedSiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as RangedSiegeWeapon;
      this.Index = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.RangedSiegeWeapon);
      GameNetworkMessage.WriteIntToPacket(this.Index, CompressionMission.RangedSiegeWeaponAmmoIndexCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Changed Projectile Type Index to: " + (object) this.Index + " on RangedSiegeWeapon with ID: " + (object) this.RangedSiegeWeapon.Id + " and name: " + this.RangedSiegeWeapon.GameEntity.Name;
  }
}
