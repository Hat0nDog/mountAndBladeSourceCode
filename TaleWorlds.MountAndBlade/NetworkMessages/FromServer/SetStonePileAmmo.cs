// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetStonePileAmmo
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetStonePileAmmo : GameNetworkMessage
  {
    public StonePile StonePile { get; private set; }

    public int AmmoCount { get; private set; }

    public SetStonePileAmmo(StonePile stonePile, int ammoCount)
    {
      this.StonePile = stonePile;
      this.AmmoCount = ammoCount;
    }

    public SetStonePileAmmo()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.StonePile = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as StonePile;
      this.AmmoCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.StonePile);
      GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Set ammo left to: " + (object) this.AmmoCount + " on StonePile with ID: " + (object) this.StonePile.Id + " and name: " + this.StonePile.GameEntity.Name;
  }
}
