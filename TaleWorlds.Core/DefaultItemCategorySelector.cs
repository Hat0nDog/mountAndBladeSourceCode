// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.DefaultItemCategorySelector
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public class DefaultItemCategorySelector : ItemCategorySelector
  {
    public override ItemCategory GetItemCategoryForItem(ItemObject itemObject)
    {
      if (itemObject.PrimaryWeapon != null)
      {
        WeaponComponentData primaryWeapon = itemObject.PrimaryWeapon;
        if (primaryWeapon.IsMeleeWeapon)
        {
          if (itemObject.Tier == ItemObject.ItemTiers.Tier6 || itemObject.Tier == ItemObject.ItemTiers.Tier5)
            return DefaultItemCategories.MeleeWeapons5;
          if (itemObject.Tier == ItemObject.ItemTiers.Tier4)
            return DefaultItemCategories.MeleeWeapons4;
          if (itemObject.Tier == ItemObject.ItemTiers.Tier3)
            return DefaultItemCategories.MeleeWeapons3;
          return itemObject.Tier != ItemObject.ItemTiers.Tier2 ? DefaultItemCategories.MeleeWeapons1 : DefaultItemCategories.MeleeWeapons2;
        }
        if (primaryWeapon.IsRangedWeapon)
        {
          if (itemObject.Tier == ItemObject.ItemTiers.Tier6 || itemObject.Tier == ItemObject.ItemTiers.Tier5)
            return DefaultItemCategories.RangedWeapons5;
          if (itemObject.Tier == ItemObject.ItemTiers.Tier4)
            return DefaultItemCategories.RangedWeapons4;
          if (itemObject.Tier == ItemObject.ItemTiers.Tier3)
            return DefaultItemCategories.RangedWeapons3;
          return itemObject.Tier != ItemObject.ItemTiers.Tier2 ? DefaultItemCategories.RangedWeapons1 : DefaultItemCategories.RangedWeapons2;
        }
        if (primaryWeapon.IsShield)
        {
          if (itemObject.Tier == ItemObject.ItemTiers.Tier6 || itemObject.Tier == ItemObject.ItemTiers.Tier5)
            return DefaultItemCategories.Shield5;
          if (itemObject.Tier == ItemObject.ItemTiers.Tier4)
            return DefaultItemCategories.Shield4;
          if (itemObject.Tier == ItemObject.ItemTiers.Tier3)
            return DefaultItemCategories.Shield3;
          return itemObject.Tier != ItemObject.ItemTiers.Tier2 ? DefaultItemCategories.Shield1 : DefaultItemCategories.Shield2;
        }
        return primaryWeapon.IsAmmo ? DefaultItemCategories.Arrows : DefaultItemCategories.MeleeWeapons1;
      }
      if (itemObject.HasHorseComponent)
        return DefaultItemCategories.Horse;
      if (itemObject.HasArmorComponent)
      {
        ArmorComponent armorComponent = itemObject.ArmorComponent;
        if (itemObject.Type == ItemObject.ItemTypeEnum.HorseHarness)
        {
          if (itemObject.Tier == ItemObject.ItemTiers.Tier6 || itemObject.Tier == ItemObject.ItemTiers.Tier5)
            return DefaultItemCategories.HorseEquipment5;
          if (itemObject.Tier == ItemObject.ItemTiers.Tier4)
            return DefaultItemCategories.HorseEquipment4;
          if (itemObject.Tier == ItemObject.ItemTiers.Tier3)
            return DefaultItemCategories.HorseEquipment3;
          return itemObject.Tier != ItemObject.ItemTiers.Tier2 ? DefaultItemCategories.HorseEquipment : DefaultItemCategories.HorseEquipment2;
        }
        if (itemObject.Tier == ItemObject.ItemTiers.Tier6 || itemObject.Tier == ItemObject.ItemTiers.Tier5)
          return DefaultItemCategories.UltraArmor;
        if (itemObject.Tier == ItemObject.ItemTiers.Tier4)
          return DefaultItemCategories.HeavyArmor;
        if (itemObject.Tier == ItemObject.ItemTiers.Tier3)
          return DefaultItemCategories.MediumArmor;
        return itemObject.Tier != ItemObject.ItemTiers.Tier2 ? DefaultItemCategories.Garment : DefaultItemCategories.LightArmor;
      }
      return itemObject.HasSaddleComponent ? DefaultItemCategories.HorseEquipment : DefaultItemCategories.Unassigned;
    }
  }
}
