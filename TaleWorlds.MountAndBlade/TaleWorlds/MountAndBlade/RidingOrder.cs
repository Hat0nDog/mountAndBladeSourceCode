// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.RidingOrder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public struct RidingOrder
  {
    internal readonly RidingOrder.RidingOrderEnum OrderEnum;
    public static readonly RidingOrder RidingOrderFree = new RidingOrder(RidingOrder.RidingOrderEnum.Free);
    public static readonly RidingOrder RidingOrderMount = new RidingOrder(RidingOrder.RidingOrderEnum.Mount);
    public static readonly RidingOrder RidingOrderDismount = new RidingOrder(RidingOrder.RidingOrderEnum.Dismount);

    private RidingOrder(RidingOrder.RidingOrderEnum orderEnum) => this.OrderEnum = orderEnum;

    public OrderType OrderType
    {
      get
      {
        if (this.OrderEnum == RidingOrder.RidingOrderEnum.Free)
          return OrderType.RideFree;
        return this.OrderEnum != RidingOrder.RidingOrderEnum.Mount ? OrderType.Dismount : OrderType.Mount;
      }
    }

    internal RidingOrder.RidingOrderEnum GetNativeEnum() => this.OrderEnum;

    public override bool Equals(object obj) => obj is RidingOrder ridingOrder && ridingOrder == this;

    public override int GetHashCode() => (int) this.OrderEnum;

    public static bool operator !=(RidingOrder r1, RidingOrder r2) => r1.OrderEnum != r2.OrderEnum;

    public static bool operator ==(RidingOrder r1, RidingOrder r2) => r1.OrderEnum == r2.OrderEnum;

    internal enum RidingOrderEnum
    {
      Free,
      Mount,
      Dismount,
    }
  }
}
