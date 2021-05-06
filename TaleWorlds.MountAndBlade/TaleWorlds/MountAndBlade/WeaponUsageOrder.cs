// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.WeaponUsageOrder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public struct WeaponUsageOrder
  {
    internal readonly WeaponUsageOrder.WeaponUsageOrderEnum OrderEnum;
    public static readonly WeaponUsageOrder WeaponUsageOrderUseAny = new WeaponUsageOrder(WeaponUsageOrder.WeaponUsageOrderEnum.UseAnyWeapon);
    public static readonly WeaponUsageOrder WeaponUsageOrderUseOnlyBlunt = new WeaponUsageOrder(WeaponUsageOrder.WeaponUsageOrderEnum.UseBluntWeaponsOnly);

    private WeaponUsageOrder(WeaponUsageOrder.WeaponUsageOrderEnum orderEnum) => this.OrderEnum = orderEnum;

    public OrderType OrderType => this.OrderEnum != WeaponUsageOrder.WeaponUsageOrderEnum.UseAnyWeapon ? OrderType.UseBluntWeaponsOnly : OrderType.UseAnyWeapon;

    internal WeaponUsageOrder.WeaponUsageOrderEnum GetNativeEnum() => this.OrderEnum;

    public override bool Equals(object obj) => obj is WeaponUsageOrder weaponUsageOrder && weaponUsageOrder == this;

    public override int GetHashCode() => (int) this.OrderEnum;

    public static bool operator !=(WeaponUsageOrder wuo1, WeaponUsageOrder wuo2) => wuo1.OrderEnum != wuo2.OrderEnum;

    public static bool operator ==(WeaponUsageOrder wuo1, WeaponUsageOrder wuo2) => wuo1.OrderEnum == wuo2.OrderEnum;

    internal enum WeaponUsageOrderEnum
    {
      UseAnyWeapon,
      UseBluntWeaponsOnly,
    }
  }
}
