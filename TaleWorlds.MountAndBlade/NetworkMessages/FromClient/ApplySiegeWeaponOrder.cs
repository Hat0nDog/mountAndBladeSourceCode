// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.ApplySiegeWeaponOrder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class ApplySiegeWeaponOrder : GameNetworkMessage
  {
    public SiegeWeaponOrderType OrderType { get; private set; }

    public ApplySiegeWeaponOrder(SiegeWeaponOrderType orderType) => this.OrderType = orderType;

    public ApplySiegeWeaponOrder()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.OrderType = (SiegeWeaponOrderType) GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteIntToPacket((int) this.OrderType, CompressionOrder.OrderTypeCompressionInfo);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeaponsDetailed | MultiplayerMessageFilter.Orders;

    protected override string OnGetLogFormat() => "Apply siege order: " + (object) this.OrderType;
  }
}
