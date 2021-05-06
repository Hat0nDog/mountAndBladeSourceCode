// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBEquipmentMissionExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public static class MBEquipmentMissionExtensions
  {
    public static SkinMask GetSkinMeshesMask(this Equipment equipment)
    {
      SkinMask skinMask = SkinMask.AllVisible;
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; ++equipmentIndex)
      {
        if (equipmentIndex == EquipmentIndex.NumAllWeaponSlots || equipmentIndex == EquipmentIndex.Body || (equipmentIndex == EquipmentIndex.Leg || equipmentIndex == EquipmentIndex.Gloves) || equipmentIndex == EquipmentIndex.Cape)
        {
          EquipmentElement equipmentElement = equipment[(int) equipmentIndex];
          if (equipmentElement.Item != null)
          {
            equipmentElement = equipment[(int) equipmentIndex];
            if (equipmentElement.Item.HasArmorComponent)
            {
              int num = (int) skinMask;
              equipmentElement = equipment[(int) equipmentIndex];
              int meshesMask = (int) equipmentElement.Item.ArmorComponent.MeshesMask;
              skinMask = (SkinMask) (num & meshesMask);
            }
          }
        }
      }
      return skinMask;
    }
  }
}
