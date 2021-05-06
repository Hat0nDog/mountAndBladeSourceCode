// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.UnselectSiegeWeapon
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class UnselectSiegeWeapon : GameNetworkMessage
  {
    public SiegeWeapon SiegeWeapon { get; private set; }

    public UnselectSiegeWeapon(SiegeWeapon siegeWeapon) => this.SiegeWeapon = siegeWeapon;

    public UnselectSiegeWeapon()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.SiegeWeapon = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as SiegeWeapon;
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.SiegeWeapon);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Deselect SiegeWeapon with ID: " + (object) this.SiegeWeapon.Id + " and with name: " + this.SiegeWeapon.GameEntity.Name;
  }
}
