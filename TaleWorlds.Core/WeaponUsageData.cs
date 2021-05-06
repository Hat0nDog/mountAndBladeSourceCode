﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.WeaponUsageData
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableStruct(10041)]
  public struct WeaponUsageData
  {
    public readonly string WeaponUsageDataId;
    public readonly WeaponClass WeaponClass;
    public readonly WeaponFlags WeaponFlags;
    public readonly string ItemUsageFeatures;
    public readonly bool RotatedInHand;
    public readonly bool UseCenterOfMassAsHandBase;

    public static void AutoGeneratedStaticCollectObjectsWeaponUsageData(
      object o,
      List<object> collectedObjects)
    {
      ((WeaponUsageData) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    private void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
    {
    }

    public bool IsPolearm => this.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip);

    public bool IsTwoHanded => this.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand);

    public bool IsOneHandedCloseWeapon => this.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.MeleeWeapon) && !this.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.NotUsableWithOneHand);

    public bool IsTwoHandedPolearm => this.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.WideGrip);

    public bool IsTwoHandedCloseWeapon => this.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand) && !this.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.WideGrip);

    public WeaponUsageData(
      string weaponUsageDataId,
      WeaponClass weaponClass,
      WeaponFlags weaponFlags,
      string itemUsageFeatures,
      bool rotatedInHand = false,
      bool useCenterOfMassAsHandBase = false)
    {
      this.WeaponUsageDataId = weaponUsageDataId;
      this.WeaponClass = weaponClass;
      this.WeaponFlags = weaponFlags;
      this.ItemUsageFeatures = itemUsageFeatures;
      this.RotatedInHand = rotatedInHand;
      this.UseCenterOfMassAsHandBase = useCenterOfMassAsHandBase;
    }
  }
}
