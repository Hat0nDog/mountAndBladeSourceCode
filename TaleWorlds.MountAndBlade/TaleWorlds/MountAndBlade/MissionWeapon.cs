// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionWeapon
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public struct MissionWeapon
  {
    public static MissionWeapon.OnGetWeaponDataDelegate OnGetWeaponDataHandler;
    public static readonly MissionWeapon Invalid = new MissionWeapon((ItemObject) null, (ItemModifier) null, (Banner) null);
    private readonly List<WeaponComponentData> _weapons;
    public int CurrentUsageIndex;
    private short _dataValue;
    private short _modifiedMaxDataValue;
    private MissionWeapon.MissionSubWeapon _ammoWeapon;
    private List<MissionWeapon.MissionSubWeapon> _attachedWeapons;
    private List<MatrixFrame> _attachedWeaponFrames;

    public ItemObject Item { get; private set; }

    public ItemModifier ItemModifier { get; private set; }

    public int WeaponsCount => this._weapons.Count;

    public WeaponComponentData CurrentUsageItem => this._weapons == null || this._weapons.Count == 0 ? (WeaponComponentData) null : this._weapons[this.CurrentUsageIndex];

    public short ReloadPhase { get; set; }

    public Banner Banner { get; private set; }

    public float GlossMultiplier { get; private set; }

    public short RawDataForNetwork
    {
      get => this._dataValue;
      set => this._dataValue = value;
    }

    public short HitPoints
    {
      get => this._dataValue;
      set => this._dataValue = value;
    }

    public short Amount
    {
      get => this._dataValue;
      set => this._dataValue = value;
    }

    public short Ammo
    {
      get
      {
        MissionWeapon.MissionSubWeapon ammoWeapon = this._ammoWeapon;
        return ammoWeapon == null ? (short) 0 : ammoWeapon.Value._dataValue;
      }
    }

    public MissionWeapon AmmoWeapon
    {
      get
      {
        MissionWeapon.MissionSubWeapon ammoWeapon = this._ammoWeapon;
        return ammoWeapon == null ? MissionWeapon.Invalid : ammoWeapon.Value;
      }
    }

    public short MaxAmmo => this._modifiedMaxDataValue;

    public short ModifiedMaxAmount => this._modifiedMaxDataValue;

    public short ModifiedMaxHitPoints => this._modifiedMaxDataValue;

    public bool IsEmpty => this.CurrentUsageItem == null;

    public MissionWeapon(ItemObject item, ItemModifier itemModifier, Banner banner)
    {
      this.Item = item;
      this.ItemModifier = itemModifier;
      this.Banner = banner;
      this.CurrentUsageIndex = 0;
      this._weapons = new List<WeaponComponentData>(1);
      this._modifiedMaxDataValue = (short) 0;
      if (item != null && item.Weapons != null)
      {
        foreach (WeaponComponentData weapon in (IEnumerable<WeaponComponentData>) item.Weapons)
        {
          this._weapons.Add(weapon);
          if (weapon.IsConsumable || weapon.IsRangedWeapon || weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.HasHitPoints))
          {
            this._modifiedMaxDataValue = weapon.MaxDataValue;
            if (itemModifier != null)
            {
              if (weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.HasHitPoints))
                this._modifiedMaxDataValue = weapon.GetModifiedMaximumHitPoints(itemModifier);
              else if (weapon.IsConsumable)
                this._modifiedMaxDataValue = weapon.GetModifiedStackCount(itemModifier);
            }
          }
        }
      }
      this._dataValue = this._modifiedMaxDataValue;
      this.ReloadPhase = (short) 0;
      this._ammoWeapon = (MissionWeapon.MissionSubWeapon) null;
      this._attachedWeapons = (List<MissionWeapon.MissionSubWeapon>) null;
      this._attachedWeaponFrames = (List<MatrixFrame>) null;
      this.GlossMultiplier = 1f;
    }

    public MissionWeapon(
      ItemObject primaryItem,
      ItemModifier itemModifier,
      Banner banner,
      short dataValue)
      : this(primaryItem, itemModifier, banner)
    {
      this._dataValue = dataValue;
    }

    public MissionWeapon(
      ItemObject primaryItem,
      ItemModifier itemModifier,
      Banner banner,
      short dataValue,
      short reloadPhase,
      MissionWeapon? ammoWeapon)
      : this(primaryItem, itemModifier, banner, dataValue)
    {
      this.ReloadPhase = reloadPhase;
      this._ammoWeapon = ammoWeapon.HasValue ? new MissionWeapon.MissionSubWeapon(ammoWeapon.Value) : (MissionWeapon.MissionSubWeapon) null;
    }

    public TextObject GetModifiedItemName()
    {
      if (this.ItemModifier == null)
        return this.Item.Name;
      TextObject name = this.ItemModifier.Name;
      name.SetTextVariable("ITEMNAME", this.Item.Name);
      return name;
    }

    public bool IsEqualTo(MissionWeapon other) => this.Item == other.Item;

    public bool IsSameType(MissionWeapon other) => this.Item.PrimaryWeapon.WeaponClass == other.Item.PrimaryWeapon.WeaponClass;

    public float GetWeight()
    {
      double num1 = this.Item.PrimaryWeapon.IsConsumable ? (double) this.GetBaseWeight() * (double) this._dataValue : (double) this.GetBaseWeight();
      MissionWeapon.MissionSubWeapon ammoWeapon = this._ammoWeapon;
      double num2 = ammoWeapon != null ? (double) ammoWeapon.Value.GetWeight() : 0.0;
      return (float) (num1 + num2);
    }

    private float GetBaseWeight() => this.Item.Weight;

    public WeaponComponentData GetWeaponComponentDataForUsage(int usageIndex) => this._weapons[usageIndex];

    public int GetGetModifiedArmorForCurrentUsage() => this._weapons[this.CurrentUsageIndex].GetModifiedArmor(this.ItemModifier);

    public int GetModifiedThrustDamageForCurrentUsage() => this._weapons[this.CurrentUsageIndex].GetModifiedThrustDamage(this.ItemModifier);

    public int GetModifiedSwingDamageForCurrentUsage() => this._weapons[this.CurrentUsageIndex].GetModifiedSwingDamage(this.ItemModifier);

    public int GetModifiedMissileDamageForCurrentUsage() => this._weapons[this.CurrentUsageIndex].GetModifiedMissileDamage(this.ItemModifier);

    public int GetModifiedThrustSpeedForCurrentUsage() => this._weapons[this.CurrentUsageIndex].GetModifiedThrustSpeed(this.ItemModifier);

    public int GetModifiedSwingSpeedForCurrentUsage() => this._weapons[this.CurrentUsageIndex].GetModifiedSwingSpeed(this.ItemModifier);

    public int GetModifiedMissileSpeedForCurrentUsage() => this._weapons[this.CurrentUsageIndex].GetModifiedMissileSpeed(this.ItemModifier);

    public int GetModifiedMissileSpeedForUsage(int usageIndex) => this._weapons[usageIndex].GetModifiedMissileSpeed(this.ItemModifier);

    public int GetModifiedHandlingForCurrentUsage() => this._weapons[this.CurrentUsageIndex].GetModifiedHandling(this.ItemModifier);

    public WeaponData GetWeaponData(bool needBatchedVersionForMeshes)
    {
      if (this.IsEmpty || this.Item.WeaponComponent == null)
        return WeaponData.InvalidWeaponData;
      WeaponComponent weaponComponent = this.Item.WeaponComponent;
      WeaponData weaponData = new WeaponData()
      {
        WeaponKind = (int) this.Item.Id.InternalValue,
        ItemHolsterIndices = this.Item.GetItemHolsterIndices(),
        ReloadPhase = this.ReloadPhase,
        Difficulty = this.Item.Difficulty,
        BaseWeight = this.GetBaseWeight(),
        HasFlagAnimation = false,
        WeaponFrame = weaponComponent.PrimaryWeapon.Frame,
        ScaleFactor = this.Item.ScaleFactor,
        Inertia = weaponComponent.PrimaryWeapon.Inertia,
        CenterOfMass = weaponComponent.PrimaryWeapon.CenterOfMass,
        CenterOfMass3D = weaponComponent.PrimaryWeapon.CenterOfMass3D,
        HolsterPositionShift = this.Item.HolsterPositionShift,
        TrailParticleName = weaponComponent.PrimaryWeapon.TrailParticleName,
        AmmoOffset = weaponComponent.PrimaryWeapon.AmmoOffset
      };
      string physicsMaterial = weaponComponent.PrimaryWeapon.PhysicsMaterial;
      weaponData.PhysicsMaterialIndex = string.IsNullOrEmpty(physicsMaterial) ? PhysicsMaterial.InvalidPhysicsMaterial.Index : PhysicsMaterial.GetFromName(physicsMaterial).Index;
      weaponData.FlyingSoundCode = SoundManager.GetEventGlobalIndex(weaponComponent.PrimaryWeapon.FlyingSoundCode);
      weaponData.PassbySoundCode = SoundManager.GetEventGlobalIndex(weaponComponent.PrimaryWeapon.PassbySoundCode);
      weaponData.StickingFrame = weaponComponent.PrimaryWeapon.StickingFrame;
      weaponData.CollisionShape = string.IsNullOrEmpty(this.Item.CollisionBodyName) ? (PhysicsShape) null : PhysicsShape.GetFromResource(this.Item.CollisionBodyName);
      weaponData.Shape = !needBatchedVersionForMeshes || string.IsNullOrEmpty(this.Item.BodyName) ? (PhysicsShape) null : PhysicsShape.GetFromResource(this.Item.BodyName);
      weaponData.DataValue = this._dataValue;
      weaponData.CurrentUsageIndex = this.CurrentUsageIndex;
      int rangedUsageIndex = this.GetRangedUsageIndex();
      WeaponComponentData consumableWeapon;
      if (this.IsAnyConsumable(out consumableWeapon))
        weaponData.AirFrictionConstant = ItemObject.GetAirFrictionConstant(consumableWeapon.WeaponClass);
      else if (rangedUsageIndex >= 0)
        weaponData.AirFrictionConstant = ItemObject.GetAirFrictionConstant(this.GetWeaponComponentDataForUsage(rangedUsageIndex).WeaponClass);
      weaponData.GlossMultiplier = this.GlossMultiplier;
      weaponData.HasLowerHolsterPriority = this.Item.HasLowerHolsterPriority;
      MissionWeapon.OnGetWeaponDataDelegate weaponDataHandler = MissionWeapon.OnGetWeaponDataHandler;
      if (weaponDataHandler != null)
        weaponDataHandler(ref weaponData, this, false, this.Banner, needBatchedVersionForMeshes);
      return weaponData;
    }

    public WeaponStatsData[] GetWeaponStatsData()
    {
      WeaponStatsData[] weaponStatsDataArray = new WeaponStatsData[this._weapons.Count];
      for (int usageIndex = 0; usageIndex < weaponStatsDataArray.Length; ++usageIndex)
        weaponStatsDataArray[usageIndex] = this.GetWeaponStatsDataForUsage(usageIndex);
      return weaponStatsDataArray;
    }

    public WeaponStatsData GetWeaponStatsDataForUsage(int usageIndex)
    {
      WeaponStatsData weaponStatsData = new WeaponStatsData();
      WeaponComponentData weapon = this._weapons[usageIndex];
      weaponStatsData.WeaponClass = (int) weapon.WeaponClass;
      weaponStatsData.AmmoClass = (int) weapon.AmmoClass;
      weaponStatsData.Properties = (uint) this.Item.ItemFlags;
      weaponStatsData.WeaponFlags = (ulong) weapon.WeaponFlags;
      weaponStatsData.ItemUsageIndex = string.IsNullOrEmpty(weapon.ItemUsage) ? -1 : weapon.GetItemUsageIndex();
      weaponStatsData.ThrustSpeed = weapon.GetModifiedThrustSpeed(this.ItemModifier);
      weaponStatsData.SwingSpeed = weapon.GetModifiedSwingSpeed(this.ItemModifier);
      weaponStatsData.MissileSpeed = weapon.GetModifiedMissileSpeed(this.ItemModifier);
      weaponStatsData.ShieldArmor = weapon.GetModifiedArmor(this.ItemModifier);
      weaponStatsData.Accuracy = weapon.Accuracy;
      weaponStatsData.WeaponLength = weapon.WeaponLength;
      weaponStatsData.WeaponBalance = weapon.WeaponBalance;
      weaponStatsData.ThrustDamage = weapon.GetModifiedThrustDamage(this.ItemModifier);
      weaponStatsData.ThrustDamageType = (int) weapon.ThrustDamageType;
      weaponStatsData.SwingDamage = weapon.GetModifiedSwingDamage(this.ItemModifier);
      weaponStatsData.SwingDamageType = (int) weapon.SwingDamageType;
      weaponStatsData.DefendSpeed = weapon.GetModifiedHandling(this.ItemModifier);
      weaponStatsData.SweetSpot = weapon.SweetSpotReach;
      weaponStatsData.MaxDataValue = this._modifiedMaxDataValue;
      weaponStatsData.WeaponFrame = weapon.Frame;
      weaponStatsData.RotationSpeed = weapon.RotationSpeed;
      return weaponStatsData;
    }

    public WeaponData GetAmmoWeaponData(bool needBatchedVersion) => this.AmmoWeapon.GetWeaponData(needBatchedVersion);

    public WeaponStatsData[] GetAmmoWeaponStatsData() => this.AmmoWeapon.GetWeaponStatsData();

    public int GetAttachedWeaponsCount()
    {
      List<MissionWeapon.MissionSubWeapon> attachedWeapons = this._attachedWeapons;
      // ISSUE: explicit non-virtual call
      return attachedWeapons == null ? 0 : __nonvirtual (attachedWeapons.Count);
    }

    public MissionWeapon GetAttachedWeapon(int attachmentIndex) => this._attachedWeapons[attachmentIndex].Value;

    public MatrixFrame GetAttachedWeaponFrame(int attachmentIndex) => this._attachedWeaponFrames[attachmentIndex];

    public bool IsShield() => this._weapons.Count == 1 && this._weapons[0].IsShield;

    public bool IsAnyAmmo() => this._weapons.Any<WeaponComponentData>((Func<WeaponComponentData, bool>) (weapon => weapon.IsAmmo));

    public bool HasAnyUsageWithWeaponClass(WeaponClass weaponClass)
    {
      foreach (WeaponComponentData weapon in this._weapons)
      {
        if (weapon.WeaponClass == weaponClass)
          return true;
      }
      return false;
    }

    public bool HasAnyUsageWithAmmoClass(WeaponClass ammoClass)
    {
      foreach (WeaponComponentData weapon in this._weapons)
      {
        if (weapon.AmmoClass == ammoClass)
          return true;
      }
      return false;
    }

    public bool HasAllUsagesWithAnyWeaponFlag(WeaponFlags flags)
    {
      foreach (WeaponComponentData weapon in this._weapons)
      {
        if (!weapon.WeaponFlags.HasAnyFlag<WeaponFlags>(flags))
          return false;
      }
      return true;
    }

    public void GatherInformationFromWeapon(
      out bool weaponHasMelee,
      out bool weaponHasShield,
      out bool weaponHasPolearm,
      out bool weaponHasNonConsumableRanged,
      out bool weaponHasThrown,
      out WeaponClass rangedAmmoClass)
    {
      weaponHasMelee = false;
      weaponHasShield = false;
      weaponHasPolearm = false;
      weaponHasNonConsumableRanged = false;
      weaponHasThrown = false;
      rangedAmmoClass = WeaponClass.Undefined;
      foreach (WeaponComponentData weapon in this._weapons)
      {
        weaponHasMelee = weaponHasMelee || weapon.IsMeleeWeapon;
        weaponHasShield = weaponHasShield || weapon.IsShield;
        weaponHasPolearm = weapon.IsPolearm;
        if (weapon.IsRangedWeapon)
        {
          weaponHasThrown = weapon.IsConsumable;
          weaponHasNonConsumableRanged = !weaponHasThrown;
          rangedAmmoClass = weapon.AmmoClass;
        }
      }
    }

    public bool IsAnyConsumable(out WeaponComponentData consumableWeapon)
    {
      consumableWeapon = (WeaponComponentData) null;
      foreach (WeaponComponentData weapon in this._weapons)
      {
        if (weapon.IsConsumable)
        {
          consumableWeapon = weapon;
          return true;
        }
      }
      return false;
    }

    public int GetRangedUsageIndex()
    {
      for (int index = 0; index < this._weapons.Count; ++index)
      {
        if (this._weapons[index].IsRangedWeapon)
          return index;
      }
      return -1;
    }

    public MissionWeapon Consume(short count)
    {
      this.Amount -= count;
      return new MissionWeapon(this.Item, this.ItemModifier, this.Banner, count, (short) 0, new MissionWeapon?());
    }

    public void ConsumeAmmo(short count)
    {
      if (count > (short) 0)
      {
        MissionWeapon subWeapon = this._ammoWeapon.Value;
        subWeapon.Amount = count;
        this._ammoWeapon = new MissionWeapon.MissionSubWeapon(subWeapon);
      }
      else
        this._ammoWeapon = (MissionWeapon.MissionSubWeapon) null;
    }

    public void SetAmmo(MissionWeapon ammoWeapon) => this._ammoWeapon = new MissionWeapon.MissionSubWeapon(ammoWeapon);

    public void ReloadAmmo(MissionWeapon ammoWeapon, short reloadPhase)
    {
      if (this._ammoWeapon != null && this._ammoWeapon.Value.Amount >= (short) 0)
        ammoWeapon.Amount += this._ammoWeapon.Value.Amount;
      this._ammoWeapon = new MissionWeapon.MissionSubWeapon(ammoWeapon);
      this.ReloadPhase = reloadPhase;
    }

    public void AttachWeapon(MissionWeapon attachedWeapon, ref MatrixFrame attachFrame)
    {
      if (this._attachedWeapons == null)
      {
        this._attachedWeapons = new List<MissionWeapon.MissionSubWeapon>();
        this._attachedWeaponFrames = new List<MatrixFrame>();
      }
      this._attachedWeapons.Add(new MissionWeapon.MissionSubWeapon(attachedWeapon));
      this._attachedWeaponFrames.Add(attachFrame);
    }

    public void RemoveAttachedWeapon(int attachmentIndex)
    {
      this._attachedWeapons.RemoveAt(attachmentIndex);
      this._attachedWeaponFrames.RemoveAt(attachmentIndex);
    }

    public bool HasEnoughSpaceForAmount(int amount) => (int) this.ModifiedMaxAmount - (int) this.Amount >= amount;

    public void SetRandomGlossMultiplier(int seed) => this.GlossMultiplier = (float) (1.0 + ((double) new Random(seed).NextFloat() * 2.0 - 1.0) * 0.300000011920929);

    internal void AddExtraModifiedMaxValue(short extraValue) => this._modifiedMaxDataValue += extraValue;

    private class MissionSubWeapon
    {
      public MissionWeapon Value { get; private set; }

      public MissionSubWeapon(MissionWeapon subWeapon) => this.Value = subWeapon;
    }

    public delegate void OnGetWeaponDataDelegate(
      ref WeaponData weaponData,
      MissionWeapon weapon,
      bool isFemale,
      Banner banner,
      bool needBatchedVersion);
  }
}
