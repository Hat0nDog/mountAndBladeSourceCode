// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.ApplyOrderWithPosition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class ApplyOrderWithPosition : GameNetworkMessage
  {
    public OrderType OrderType { get; private set; }

    public Vec3 Position { get; private set; }

    public ApplyOrderWithPosition(OrderType orderType, Vec3 position)
    {
      this.OrderType = orderType;
      this.Position = position;
    }

    public ApplyOrderWithPosition()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.OrderType = (OrderType) GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref bufferReadValid);
      this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionOrder.OrderPositionCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket((int) this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
      GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionOrder.OrderPositionCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Orders;

    protected override string OnGetLogFormat() => "Apply order: " + (object) this.OrderType + ", to position: " + (object) this.Position;
  }
}
