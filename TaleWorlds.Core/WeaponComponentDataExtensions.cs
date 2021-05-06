// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.WeaponComponentDataExtensions
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public static class WeaponComponentDataExtensions
  {
    public static int GetModifiedThrustDamage(
      this WeaponComponentData componentData,
      ItemModifier itemModifier)
    {
      return itemModifier != null && componentData.ThrustDamage > 0 ? itemModifier.ModifyDamage(componentData.ThrustDamage) : componentData.ThrustDamage;
    }

    public static int GetModifiedSwingDamage(
      this WeaponComponentData componentData,
      ItemModifier itemModifier)
    {
      return itemModifier != null && componentData.SwingDamage > 0 ? itemModifier.ModifyDamage(componentData.SwingDamage) : componentData.SwingDamage;
    }

    public static int GetModifiedMissileDamage(
      this WeaponComponentData componentData,
      ItemModifier itemModifier)
    {
      return itemModifier != null && componentData.MissileDamage > 0 ? itemModifier.ModifyDamage(componentData.MissileDamage) : componentData.MissileDamage;
    }

    public static int GetModifiedThrustSpeed(
      this WeaponComponentData componentData,
      ItemModifier itemModifier)
    {
      return itemModifier != null && componentData.ThrustSpeed > 0 ? itemModifier.ModifySpeed(componentData.ThrustSpeed) : componentData.ThrustSpeed;
    }

    public static int GetModifiedSwingSpeed(
      this WeaponComponentData componentData,
      ItemModifier itemModifier)
    {
      return itemModifier != null && componentData.SwingSpeed > 0 ? itemModifier.ModifySpeed(componentData.SwingSpeed) : componentData.SwingSpeed;
    }

    public static int GetModifiedMissileSpeed(
      this WeaponComponentData componentData,
      ItemModifier itemModifier)
    {
      return itemModifier != null && componentData.MissileSpeed > 0 ? itemModifier.ModifyMissileSpeed(componentData.MissileSpeed) : componentData.MissileSpeed;
    }

    public static int GetModifiedHandling(
      this WeaponComponentData componentData,
      ItemModifier itemModifier)
    {
      return itemModifier != null && componentData.Handling > 0 ? itemModifier.ModifySpeed(componentData.Handling) : componentData.Handling;
    }

    public static short GetModifiedStackCount(
      this WeaponComponentData componentData,
      ItemModifier itemModifier)
    {
      return itemModifier != null && componentData.MaxDataValue > (short) 0 ? itemModifier.ModifyStackCount(componentData.MaxDataValue) : componentData.MaxDataValue;
    }

    public static short GetModifiedMaximumHitPoints(
      this WeaponComponentData componentData,
      ItemModifier itemModifier)
    {
      return itemModifier != null && componentData.MaxDataValue > (short) 0 ? itemModifier.ModifyHitPoints(componentData.MaxDataValue) : componentData.MaxDataValue;
    }

    public static int GetModifiedArmor(
      this WeaponComponentData componentData,
      ItemModifier itemModifier)
    {
      return itemModifier != null && componentData.BodyArmor > 0 ? itemModifier.ModifyArmor(componentData.BodyArmor) : componentData.BodyArmor;
    }
  }
}
