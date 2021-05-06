// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.ApplyOrderWithFormationAndNumber
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class ApplyOrderWithFormationAndNumber : GameNetworkMessage
  {
    public OrderType OrderType { get; private set; }

    public int FormationIndex { get; private set; }

    public int Number { get; private set; }

    public ApplyOrderWithFormationAndNumber(OrderType orderType, int formationIndex, int number)
    {
      this.OrderType = orderType;
      this.FormationIndex = formationIndex;
      this.Number = number;
    }

    public ApplyOrderWithFormationAndNumber()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.OrderType = (OrderType) GameNetworkMessage.ReadIntFromPacket(CompressionOrder.OrderTypeCompressionInfo, ref bufferReadValid);
      this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref bufferReadValid);
      this.Number = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket((int) this.OrderType, CompressionOrder.OrderTypeCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.Number, CompressionBasic.DebugIntNonCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Formations | MultiplayerMessageFilter.Orders;

    protected override string OnGetLogFormat() => "Apply order: " + (object) this.OrderType + ", to formation with index: " + (object) this.FormationIndex + " and number: " + (object) this.Number;
  }
}
