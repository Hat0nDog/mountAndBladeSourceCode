// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionEquipment
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class MissionEquipment
  {
    private readonly MissionWeapon[] _weaponSlots;

    public MissionEquipment() => this._weaponSlots = new MissionWeapon[5];

    public MissionEquipment(Equipment spawnEquipment, Banner banner)
    {
      this._weaponSlots = new MissionWeapon[5];
      for (EquipmentIndex index1 = EquipmentIndex.WeaponItemBeginSlot; index1 < EquipmentIndex.NumAllWeaponSlots; ++index1)
      {
        MissionWeapon[] weaponSlots = this._weaponSlots;
        int index2 = (int) index1;
        EquipmentElement equipmentElement = spawnEquipment[index1];
        ItemObject itemObject = equipmentElement.Item;
        equipmentElement = spawnEquipment[index1];
        ItemModifier itemModifier = equipmentElement.ItemModifier;
        Banner banner1 = banner;
        MissionWeapon missionWeapon = new MissionWeapon(itemObject, itemModifier, banner1);
        weaponSlots[index2] = missionWeapon;
      }
    }

    public MissionWeapon this[int index]
    {
      get => this._weaponSlots[index];
      set => this._weaponSlots[index] = value;
    }

    public MissionWeapon this[EquipmentIndex index]
    {
      get => this._weaponSlots[(int) index];
      set => this[(int) index] = value;
    }

    public void FillFrom(Equipment sourceEquipment, Banner banner)
    {
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
        this[index] = new MissionWeapon(sourceEquipment[index].Item, sourceEquipment[index].ItemModifier, banner);
    }

    public float GetTotalWeightOfWeapons()
    {
      float num = 0.0f;
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
      {
        MissionWeapon missionWeapon = this[index];
        if (!missionWeapon.IsEmpty)
        {
          if (missionWeapon.CurrentUsageItem.IsShield)
          {
            if (missionWeapon.HitPoints > (short) 0)
              num += missionWeapon.GetWeight();
          }
          else
            num += missionWeapon.GetWeight();
        }
      }
      return num;
    }

    public static EquipmentIndex SelectWeaponPickUpSlot(
      Agent agentPickingUp,
      MissionWeapon weaponBeingPickedUp,
      bool isStuckMissile)
    {
      EquipmentIndex equipmentIndex1 = EquipmentIndex.None;
      if (weaponBeingPickedUp.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DropOnWeaponChange | ItemFlags.DropOnAnyAction))
      {
        equipmentIndex1 = EquipmentIndex.Weapon4;
      }
      else
      {
        Agent.HandIndex index1 = weaponBeingPickedUp.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.HeldInOffHand) ? Agent.HandIndex.OffHand : Agent.HandIndex.MainHand;
        EquipmentIndex wieldedItemIndex = agentPickingUp.GetWieldedItemIndex(index1);
        MissionWeapon missionWeapon1 = wieldedItemIndex != EquipmentIndex.None ? agentPickingUp.Equipment[wieldedItemIndex] : MissionWeapon.Invalid;
        if (isStuckMissile)
        {
          bool flag1 = false;
          bool flag2 = false;
          bool isConsumable = weaponBeingPickedUp.Item.PrimaryWeapon.IsConsumable;
          if (isConsumable)
          {
            flag1 = !missionWeapon1.IsEmpty && missionWeapon1.IsEqualTo(weaponBeingPickedUp) && missionWeapon1.HasEnoughSpaceForAmount((int) weaponBeingPickedUp.Amount);
            flag2 = !missionWeapon1.IsEmpty && missionWeapon1.IsSameType(weaponBeingPickedUp) && missionWeapon1.HasEnoughSpaceForAmount((int) weaponBeingPickedUp.Amount);
          }
          EquipmentIndex equipmentIndex2 = EquipmentIndex.None;
          EquipmentIndex equipmentIndex3 = EquipmentIndex.None;
          EquipmentIndex equipmentIndex4 = EquipmentIndex.None;
          for (EquipmentIndex index2 = EquipmentIndex.WeaponItemBeginSlot; index2 < EquipmentIndex.Weapon4; ++index2)
          {
            if (isConsumable)
            {
              if (equipmentIndex3 != EquipmentIndex.None && !agentPickingUp.Equipment[index2].IsEmpty && (agentPickingUp.Equipment[index2].IsEqualTo(weaponBeingPickedUp) && agentPickingUp.Equipment[index2].HasEnoughSpaceForAmount((int) weaponBeingPickedUp.Amount)))
              {
                equipmentIndex3 = index2;
                continue;
              }
              if (equipmentIndex4 == EquipmentIndex.None && !agentPickingUp.Equipment[index2].IsEmpty && (agentPickingUp.Equipment[index2].IsSameType(weaponBeingPickedUp) && agentPickingUp.Equipment[index2].HasEnoughSpaceForAmount((int) weaponBeingPickedUp.Amount)))
              {
                equipmentIndex4 = index2;
                continue;
              }
            }
            if (equipmentIndex2 == EquipmentIndex.None && agentPickingUp.Equipment[index2].IsEmpty)
              equipmentIndex2 = index2;
          }
          if (flag1)
            equipmentIndex1 = wieldedItemIndex;
          else if (equipmentIndex3 != EquipmentIndex.None)
            equipmentIndex1 = equipmentIndex4;
          else if (flag2)
            equipmentIndex1 = wieldedItemIndex;
          else if (equipmentIndex4 != EquipmentIndex.None)
            equipmentIndex1 = equipmentIndex4;
          else if (equipmentIndex2 != EquipmentIndex.None)
            equipmentIndex1 = equipmentIndex2;
        }
        else
        {
          bool isConsumable = weaponBeingPickedUp.Item.PrimaryWeapon.IsConsumable;
          if (isConsumable && weaponBeingPickedUp.Amount == (short) 0)
          {
            equipmentIndex1 = EquipmentIndex.None;
          }
          else
          {
            if (index1 == Agent.HandIndex.OffHand && wieldedItemIndex != EquipmentIndex.None)
            {
              for (int index2 = 0; index2 < 4; ++index2)
              {
                if ((EquipmentIndex) index2 != wieldedItemIndex && !agentPickingUp.Equipment[index2].IsEmpty && agentPickingUp.Equipment[index2].Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.HeldInOffHand))
                {
                  equipmentIndex1 = wieldedItemIndex;
                  break;
                }
              }
            }
            MissionWeapon missionWeapon2;
            if (equipmentIndex1 == EquipmentIndex.None && isConsumable)
            {
              for (EquipmentIndex index2 = EquipmentIndex.WeaponItemBeginSlot; index2 < EquipmentIndex.Weapon4; ++index2)
              {
                missionWeapon2 = agentPickingUp.Equipment[index2];
                if (!missionWeapon2.IsEmpty)
                {
                  missionWeapon2 = agentPickingUp.Equipment[index2];
                  if (missionWeapon2.IsSameType(weaponBeingPickedUp))
                  {
                    missionWeapon2 = agentPickingUp.Equipment[index2];
                    int amount = (int) missionWeapon2.Amount;
                    missionWeapon2 = agentPickingUp.Equipment[index2];
                    int modifiedMaxAmount = (int) missionWeapon2.ModifiedMaxAmount;
                    if (amount < modifiedMaxAmount)
                    {
                      equipmentIndex1 = index2;
                      break;
                    }
                  }
                }
              }
            }
            if (equipmentIndex1 == EquipmentIndex.None)
            {
              for (EquipmentIndex index2 = EquipmentIndex.WeaponItemBeginSlot; index2 < EquipmentIndex.Weapon4; ++index2)
              {
                missionWeapon2 = agentPickingUp.Equipment[index2];
                if (missionWeapon2.IsEmpty)
                {
                  equipmentIndex1 = index2;
                  break;
                }
              }
            }
            if (equipmentIndex1 == EquipmentIndex.None)
            {
              for (EquipmentIndex index2 = EquipmentIndex.WeaponItemBeginSlot; index2 < EquipmentIndex.Weapon4; ++index2)
              {
                missionWeapon2 = agentPickingUp.Equipment[index2];
                if (!missionWeapon2.IsEmpty)
                {
                  missionWeapon2 = agentPickingUp.Equipment[index2];
                  if (missionWeapon2.IsAnyConsumable(out WeaponComponentData _))
                  {
                    missionWeapon2 = agentPickingUp.Equipment[index2];
                    if (missionWeapon2.Amount == (short) 0)
                    {
                      equipmentIndex1 = index2;
                      break;
                    }
                  }
                }
              }
            }
            if (equipmentIndex1 == EquipmentIndex.None && !missionWeapon1.IsEmpty)
              equipmentIndex1 = wieldedItemIndex;
            if (equipmentIndex1 == EquipmentIndex.None)
              equipmentIndex1 = EquipmentIndex.WeaponItemBeginSlot;
          }
        }
      }
      return equipmentIndex1;
    }

    public bool HasAmmo(
      EquipmentIndex equipmentIndex,
      out int rangedUsageIndex,
      out bool hasLoadedAmmo,
      out bool noAmmoInThisSlot)
    {
      hasLoadedAmmo = false;
      noAmmoInThisSlot = false;
      MissionWeapon weaponSlot = this._weaponSlots[(int) equipmentIndex];
      rangedUsageIndex = weaponSlot.GetRangedUsageIndex();
      if (rangedUsageIndex >= 0)
      {
        if (weaponSlot.Ammo > (short) 0)
        {
          hasLoadedAmmo = true;
          return true;
        }
        noAmmoInThisSlot = weaponSlot.IsAnyConsumable(out WeaponComponentData _) && weaponSlot.Amount == (short) 0;
        for (EquipmentIndex equipmentIndex1 = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex1 < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex1)
        {
          MissionWeapon missionWeapon = this[(int) equipmentIndex1];
          if (!missionWeapon.IsEmpty && missionWeapon.HasAnyUsageWithWeaponClass(weaponSlot.GetWeaponComponentDataForUsage(rangedUsageIndex).AmmoClass) && missionWeapon.Amount > (short) 0)
            return true;
        }
      }
      return false;
    }

    public int GetAmmoAmount(WeaponClass ammoClass)
    {
      int num1 = 0;
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
      {
        MissionWeapon missionWeapon = this[(int) equipmentIndex];
        if (!missionWeapon.IsEmpty)
        {
          missionWeapon = this[(int) equipmentIndex];
          if (missionWeapon.CurrentUsageItem.WeaponClass == ammoClass)
          {
            int num2 = num1;
            missionWeapon = this[(int) equipmentIndex];
            int amount = (int) missionWeapon.Amount;
            num1 = num2 + amount;
          }
        }
      }
      return num1;
    }

    public int GetAmmoSlotIndexOfWeapon(WeaponClass ammoClass)
    {
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
      {
        if (!this[(int) equipmentIndex].IsEmpty && this[(int) equipmentIndex].CurrentUsageItem.WeaponClass == ammoClass)
          return (int) equipmentIndex;
      }
      return -1;
    }

    public int GetMaxAmmo(WeaponClass ammoClass)
    {
      int num1 = 0;
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
      {
        MissionWeapon missionWeapon = this[(int) equipmentIndex];
        if (!missionWeapon.IsEmpty)
        {
          missionWeapon = this[(int) equipmentIndex];
          if (missionWeapon.CurrentUsageItem.WeaponClass == ammoClass)
          {
            int num2 = num1;
            missionWeapon = this[(int) equipmentIndex];
            int modifiedMaxAmount = (int) missionWeapon.ModifiedMaxAmount;
            num1 = num2 + modifiedMaxAmount;
          }
        }
      }
      return num1;
    }

    public void GetAmmoCountAndIndexOfType(
      ItemObject.ItemTypeEnum itemType,
      out int ammoCount,
      out EquipmentIndex eIndex,
      EquipmentIndex equippedIndex = EquipmentIndex.None)
    {
      ItemObject.ItemTypeEnum ammoTypeForItemType = ItemObject.GetAmmoTypeForItemType(itemType);
      ItemObject itemObject;
      if (equippedIndex != EquipmentIndex.None)
      {
        itemObject = this[equippedIndex].Item;
        ammoCount = 0;
      }
      else
      {
        itemObject = (ItemObject) null;
        ammoCount = -1;
      }
      eIndex = equippedIndex;
      if (ammoTypeForItemType == ItemObject.ItemTypeEnum.Invalid)
        return;
      for (EquipmentIndex index = EquipmentIndex.Weapon3; index >= EquipmentIndex.WeaponItemBeginSlot; --index)
      {
        if (!this[index].IsEmpty && this[index].Item.Type == ammoTypeForItemType)
        {
          int amount = (int) this[index].Amount;
          if (amount > 0)
          {
            if (itemObject == null)
            {
              eIndex = index;
              itemObject = this[index].Item;
              ammoCount = amount;
            }
            else if (itemObject.Id == this[index].Item.Id)
              ammoCount += amount;
          }
        }
      }
    }

    public static bool DoesWeaponFitToSlot(EquipmentIndex slotIndex, MissionWeapon weapon) => weapon.IsEmpty || (!weapon.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.DropOnWeaponChange | ItemFlags.DropOnAnyAction) ? slotIndex >= EquipmentIndex.WeaponItemBeginSlot && slotIndex < EquipmentIndex.Weapon4 : slotIndex == EquipmentIndex.Weapon4);

    public void CheckLoadedAmmos()
    {
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
      {
        if (!this[index].IsEmpty && this[index].Item.PrimaryWeapon.WeaponClass == WeaponClass.Crossbow)
        {
          EquipmentIndex eIndex;
          this.GetAmmoCountAndIndexOfType(this[index].Item.Type, out int _, out eIndex);
          if (eIndex != EquipmentIndex.None)
          {
            MissionWeapon ammoWeapon = this._weaponSlots[(int) eIndex].Consume(Math.Min(this[index].MaxAmmo, this._weaponSlots[(int) eIndex].Amount));
            this._weaponSlots[(int) index].ReloadAmmo(ammoWeapon, (short) 2);
          }
        }
      }
    }

    public void SetUsageIndexOfSlot(EquipmentIndex slotIndex, int usageIndex) => this._weaponSlots[(int) slotIndex].CurrentUsageIndex = usageIndex;

    public void SetReloadPhaseOfSlot(EquipmentIndex slotIndex, short reloadPhase) => this._weaponSlots[(int) slotIndex].ReloadPhase = reloadPhase;

    public void SetAmountOfSlot(
      EquipmentIndex slotIndex,
      short dataValue,
      bool addOverflowToMaxAmount = false)
    {
      if (addOverflowToMaxAmount)
      {
        short extraValue = (short) ((int) dataValue - (int) this._weaponSlots[(int) slotIndex].Amount);
        if (extraValue > (short) 0)
          this._weaponSlots[(int) slotIndex].AddExtraModifiedMaxValue(extraValue);
      }
      this._weaponSlots[(int) slotIndex].Amount = dataValue;
    }

    public void SetHitPointsOfSlot(
      EquipmentIndex slotIndex,
      short dataValue,
      bool addOverflowToMaxHitPoints = false)
    {
      if (addOverflowToMaxHitPoints)
      {
        short extraValue = (short) ((int) dataValue - (int) this._weaponSlots[(int) slotIndex].HitPoints);
        if (extraValue > (short) 0)
          this._weaponSlots[(int) slotIndex].AddExtraModifiedMaxValue(extraValue);
      }
      this._weaponSlots[(int) slotIndex].HitPoints = dataValue;
    }

    public void SetReloadedAmmoOfSlot(
      EquipmentIndex slotIndex,
      EquipmentIndex ammoSlotIndex,
      short totalAmmo)
    {
      if (ammoSlotIndex == EquipmentIndex.None)
      {
        this._weaponSlots[(int) slotIndex].SetAmmo(MissionWeapon.Invalid);
      }
      else
      {
        MissionWeapon weaponSlot = this._weaponSlots[(int) ammoSlotIndex];
        weaponSlot.Amount = totalAmmo;
        this._weaponSlots[(int) slotIndex].SetAmmo(weaponSlot);
      }
    }

    public void SetConsumedAmmoOfSlot(EquipmentIndex slotIndex, short count) => this._weaponSlots[(int) slotIndex].ConsumeAmmo(count);

    public void AttachWeaponToWeaponInSlot(
      EquipmentIndex slotIndex,
      ref MissionWeapon weapon,
      ref MatrixFrame attachLocalFrame)
    {
      this._weaponSlots[(int) slotIndex].AttachWeapon(weapon, ref attachLocalFrame);
    }

    public bool HasShield()
    {
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
      {
        WeaponComponentData currentUsageItem = this._weaponSlots[(int) equipmentIndex].CurrentUsageItem;
        if (currentUsageItem != null && currentUsageItem.IsShield)
          return true;
      }
      return false;
    }

    public bool HasRangedWeapon(WeaponClass requiredAmmoClass = WeaponClass.Undefined)
    {
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
      {
        WeaponComponentData currentUsageItem = this._weaponSlots[(int) equipmentIndex].CurrentUsageItem;
        if (currentUsageItem != null && currentUsageItem.IsRangedWeapon && (requiredAmmoClass == WeaponClass.Undefined || currentUsageItem.AmmoClass == requiredAmmoClass))
          return true;
      }
      return false;
    }

    public int GetLongestRangedWeaponWithAimingError(out float inaccuracy, Agent agent)
    {
      int num1 = -1;
      float num2 = -1f;
      inaccuracy = -1f;
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
      {
        int rangedUsageIndex;
        bool hasLoadedAmmo;
        bool noAmmoInThisSlot;
        if (this.HasAmmo(equipmentIndex, out rangedUsageIndex, out hasLoadedAmmo, out noAmmoInThisSlot) && !noAmmoInThisSlot && (hasLoadedAmmo || !agent.HasMount || (!this._weaponSlots[(int) equipmentIndex].GetWeaponComponentDataForUsage(rangedUsageIndex).WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CantReloadOnHorseback) || agent.GetAgentFlags().HasFlag((Enum) AgentFlag.CanReloadAllXBowsMounted))))
        {
          int missileSpeedForUsage = this._weaponSlots[(int) equipmentIndex].GetModifiedMissileSpeedForUsage(rangedUsageIndex);
          double num3 = (double) missileSpeedForUsage * 0.707106709480286;
          float num4 = (float) (num3 * 0.1019366979599);
          float num5 = (float) Math.Sqrt(2.0 * (num3 * (double) num4 * 0.5) * 0.1019366979599);
          float val2 = (float) num3 * (num4 + num5);
          WeaponComponentData componentDataForUsage = this._weaponSlots[(int) equipmentIndex].GetWeaponComponentDataForUsage(rangedUsageIndex);
          int effectiveSkill = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(agent.Character, agent.Origin, agent.Formation, componentDataForUsage.RelevantSkill);
          float weaponInaccuracy = MissionGameModels.Current.AgentStatCalculateModel.GetWeaponInaccuracy(agent, componentDataForUsage, effectiveSkill);
          float num6 = Math.Min(2.5f / MathF.Tan(Math.Max(0.5f / (float) missileSpeedForUsage, weaponInaccuracy * 0.5f)), val2);
          if ((double) num2 < (double) num6)
          {
            inaccuracy = weaponInaccuracy;
            num2 = num6;
            num1 = (int) equipmentIndex;
          }
        }
      }
      return num1;
    }

    public void SetGlossMultipliersOfWeaponsRandomly(int seed)
    {
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumAllWeaponSlots; ++equipmentIndex)
        this._weaponSlots[(int) equipmentIndex].SetRandomGlossMultiplier(seed);
    }
  }
}
