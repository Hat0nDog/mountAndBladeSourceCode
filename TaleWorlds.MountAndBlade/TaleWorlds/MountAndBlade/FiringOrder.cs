// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FiringOrder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public struct FiringOrder
  {
    internal readonly FiringOrder.RangedWeaponUsageOrderEnum OrderEnum;
    public static readonly FiringOrder FiringOrderFireAtWill = new FiringOrder(FiringOrder.RangedWeaponUsageOrderEnum.FireAtWill);
    public static readonly FiringOrder FiringOrderHoldYourFire = new FiringOrder(FiringOrder.RangedWeaponUsageOrderEnum.HoldYourFire);

    private FiringOrder(FiringOrder.RangedWeaponUsageOrderEnum orderEnum) => this.OrderEnum = orderEnum;

    public OrderType OrderType => this.OrderEnum != FiringOrder.RangedWeaponUsageOrderEnum.FireAtWill ? OrderType.HoldFire : OrderType.FireAtWill;

    internal FiringOrder.RangedWeaponUsageOrderEnum GetNativeEnum() => this.OrderEnum;

    public override bool Equals(object obj) => obj is FiringOrder firingOrder && firingOrder == this;

    public override int GetHashCode() => (int) this.OrderEnum;

    public static bool operator !=(FiringOrder f1, FiringOrder f2) => f1.OrderEnum != f2.OrderEnum;

    public static bool operator ==(FiringOrder f1, FiringOrder f2) => f1.OrderEnum == f2.OrderEnum;

    internal enum RangedWeaponUsageOrderEnum
    {
      FireAtWill,
      HoldYourFire,
    }
  }
}
