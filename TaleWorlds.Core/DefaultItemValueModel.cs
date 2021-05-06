// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.DefaultItemValueModel
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public class DefaultItemValueModel : ItemValueModel
  {
    private float CalculateArmorTier(ArmorComponent armorComponent)
    {
      float num = (float) (1.20000004768372 * (double) armorComponent.HeadArmor + 1.0 * (double) armorComponent.BodyArmor + 1.0 * (double) armorComponent.LegArmor + 1.0 * (double) armorComponent.ArmArmor);
      if (armorComponent.Item.ItemType == ItemObject.ItemTypeEnum.LegArmor)
        num *= 1.6f;
      else if (armorComponent.Item.ItemType == ItemObject.ItemTypeEnum.HandArmor)
        num *= 1.7f;
      else if (armorComponent.Item.ItemType == ItemObject.ItemTypeEnum.HeadArmor)
        num *= 1.2f;
      else if (armorComponent.Item.ItemType == ItemObject.ItemTypeEnum.Cape)
        num *= 1.8f;
      return (float) ((double) num * 0.100000001490116 - 0.400000005960464);
    }

    private float CalculateHorseTier(HorseComponent horseComponent) => (float) ((horseComponent.IsPackAnimal ? 25.0 : 0.0 + 0.600000023841858 * (double) horseComponent.Maneuver + (double) horseComponent.Speed + 1.5 * (double) horseComponent.ChargeDamage + 0.100000001490116 * (double) horseComponent.HitPoints) / 13.0 - 8.0);

    private float CalculateSaddleTier(SaddleComponent saddleComponent) => 0.0f;

    private float CalculateWeaponTier(WeaponComponent weaponComponent)
    {
      WeaponDesign weaponDesign = weaponComponent.Item?.WeaponDesign;
      if (!(weaponDesign != (WeaponDesign) null))
        return this.CalculateTierNonCraftedWeapon(weaponComponent);
      float tierCraftedWeapon = this.CalculateTierCraftedWeapon(weaponDesign);
      return (float) (0.600000023841858 * (double) this.CalculateTierMeleeWeapon(weaponComponent) + 0.400000005960464 * (double) tierCraftedWeapon);
    }

    private float CalculateTierMeleeWeapon(WeaponComponent weaponComponent)
    {
      WeaponComponentData weapon = weaponComponent.Weapons[0];
      float num1 = Math.Max((float) weapon.ThrustDamage * this.GetFactor(weapon.ThrustDamageType) * MathF.Pow((float) weapon.ThrustSpeed * 0.01f, 1.5f), (float) weapon.SwingDamage * this.GetFactor(weapon.SwingDamageType) * MathF.Pow((float) weapon.SwingSpeed * 0.01f, 1.5f) * 1.1f);
      if (weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.NotUsableWithOneHand))
        num1 *= 0.8f;
      if (weapon.WeaponClass == WeaponClass.ThrowingKnife || weapon.WeaponClass == WeaponClass.ThrowingKnife || weapon.WeaponClass == WeaponClass.Javelin)
        num1 *= 1.2f;
      float num2 = (float) weapon.WeaponLength * 0.01f;
      return (float) (0.0599999986588955 * ((double) num1 * (1.0 + (double) num2)) - 3.5);
    }

    private float GetFactor(DamageTypes swingDamageType)
    {
      if (swingDamageType == DamageTypes.Blunt)
        return 1.3f;
      return swingDamageType != DamageTypes.Pierce ? 1f : 1.15f;
    }

    private float CalculateTierNonCraftedWeapon(WeaponComponent weaponComponent)
    {
      ItemObject itemObject = weaponComponent.Item;
      switch (itemObject != null ? itemObject.ItemType : ItemObject.ItemTypeEnum.Invalid)
      {
        case ItemObject.ItemTypeEnum.Arrows:
        case ItemObject.ItemTypeEnum.Bolts:
        case ItemObject.ItemTypeEnum.Bullets:
          return this.CalculateAmmoTier(weaponComponent);
        case ItemObject.ItemTypeEnum.Shield:
          return this.CalculateShieldTier(weaponComponent);
        case ItemObject.ItemTypeEnum.Bow:
        case ItemObject.ItemTypeEnum.Crossbow:
        case ItemObject.ItemTypeEnum.Pistol:
        case ItemObject.ItemTypeEnum.Musket:
          return this.CalculateRangedWeaponTier(weaponComponent);
        default:
          return 0.0f;
      }
    }

    private float CalculateRangedWeaponTier(WeaponComponent weaponComponent)
    {
      WeaponComponentData weapon = weaponComponent.Weapons[0];
      ItemObject itemObject = weaponComponent.Item;
      double num1;
      switch (itemObject != null ? itemObject.ItemType : ItemObject.ItemTypeEnum.Invalid)
      {
        case ItemObject.ItemTypeEnum.Crossbow:
          num1 = 0.699999988079071;
          break;
        case ItemObject.ItemTypeEnum.Musket:
          num1 = 0.5;
          break;
        default:
          num1 = 1.0;
          break;
      }
      double num2 = (double) weapon.ThrustDamage * 0.200000002980232 + (double) weapon.ThrustSpeed * 0.0199999995529652 + (double) weapon.Accuracy * 0.0199999995529652;
      return (float) (num1 * num2 - 11.0);
    }

    private float CalculateShieldTier(WeaponComponent weaponComponent)
    {
      WeaponComponentData weapon = weaponComponent.Weapons[0];
      return (float) (((double) weapon.MaxDataValue + 3.0 * (double) weapon.BodyArmor + (double) weapon.ThrustSpeed) / (6.0 + (double) weaponComponent.Item.Weight) * 0.129999995231628 - 3.0);
    }

    private float CalculateAmmoTier(WeaponComponent weaponComponent)
    {
      WeaponComponentData weapon = weaponComponent.Weapons[0];
      return (float) weapon.MissileDamage + (float) Math.Max(0, (int) weapon.MaxDataValue - 20) * 0.1f;
    }

    private float CalculateTierCraftedWeapon(WeaponDesign craftingData)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      foreach (WeaponDesignElement usedPiece in craftingData.UsedPieces)
      {
        if (usedPiece.CraftingPiece.IsValid)
        {
          num1 += usedPiece.CraftingPiece.PieceTier;
          ++num2;
          foreach ((CraftingMaterials craftingMaterials3, int num8) in (IEnumerable<(CraftingMaterials, int)>) usedPiece.CraftingPiece.MaterialsUsed)
          {
            int temp_40;
            switch (craftingMaterials3)
            {
              case CraftingMaterials.Iron1:
                temp_40 = 1;
                break;
              case CraftingMaterials.Iron2:
                temp_40 = 2;
                break;
              case CraftingMaterials.Iron3:
                temp_40 = 3;
                break;
              case CraftingMaterials.Iron4:
                temp_40 = 4;
                break;
              case CraftingMaterials.Iron5:
                temp_40 = 5;
                break;
              case CraftingMaterials.Iron6:
                temp_40 = 6;
                break;
              case CraftingMaterials.Wood:
                temp_40 = -1;
                break;
              default:
                temp_40 = -1;
                break;
            }
            int local_10 = temp_40;
            if (local_10 >= 0)
            {
              num3 += num8 * local_10;
              num4 += num8;
            }
          }
        }
      }
      if (num4 > 0 && num2 > 0)
        return (float) (0.400000005960464 * (double) (1.25f * (float) num1 / (float) num2) + 0.600000023841858 * ((double) num3 * 1.29999995231628 / ((double) num4 + 0.600000023841858) - 1.29999995231628));
      return num2 > 0 ? (float) num1 / (float) num2 : 0.1f;
    }

    public override int CalculateValue(ItemObject item)
    {
      float num1 = 1f;
      if (item.ItemComponent != null)
        num1 = this.GetEquipmentValueFromTier(item.Tierf);
      float num2 = 1f;
      if (item.ItemComponent is ArmorComponent)
        num2 = item.ItemType == ItemObject.ItemTypeEnum.BodyArmor ? 120f : (item.ItemType == ItemObject.ItemTypeEnum.HandArmor ? 120f : (item.ItemType == ItemObject.ItemTypeEnum.LegArmor ? 120f : 100f));
      else if (item.ItemComponent is WeaponComponent)
        num2 = 100f;
      else if (item.ItemComponent is HorseComponent)
        num2 = 100f;
      else if (item.ItemComponent is SaddleComponent)
        num2 = 100f;
      else if (item.ItemComponent is TradeItemComponent)
        num2 = 100f;
      return (int) ((double) num2 * (double) num1 * (1.0 + 0.200000002980232 * ((double) item.Appearance - 1.0)) + 100.0 * (double) Math.Max(0.0f, item.Appearance - 1f));
    }

    private float GetWeaponPriceFactor(ItemObject item) => 100f;

    public override float GetEquipmentValueFromTier(float itemTierf) => (float) Math.Pow(3.0, (double) MathF.Clamp(itemTierf, -1f, 7.5f));

    public override float CalculateTier(ItemObject item)
    {
      if (item.ItemComponent is ArmorComponent)
        return this.CalculateArmorTier(item.ItemComponent as ArmorComponent);
      if (item.ItemComponent is WeaponComponent)
        return this.CalculateWeaponTier(item.ItemComponent as WeaponComponent);
      if (item.ItemComponent is HorseComponent)
        return this.CalculateHorseTier(item.ItemComponent as HorseComponent);
      return item.ItemComponent is SaddleComponent ? this.CalculateSaddleTier(item.ItemComponent as SaddleComponent) : 0.0f;
    }
  }
}
