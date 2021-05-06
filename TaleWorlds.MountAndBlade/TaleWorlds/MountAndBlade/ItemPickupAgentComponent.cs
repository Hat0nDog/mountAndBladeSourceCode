// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ItemPickupAgentComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class ItemPickupAgentComponent : AgentComponent
  {
    private const float AvoidPickUpIfLookAgentIsCloseDistance = 20f;
    private const float AvoidPickUpIfLookAgentIsCloseDistanceSquared = 400f;
    private UseObjectAgentComponent _useObjectAgentComponent;
    private GameEntity[] _tempPickableEntities = new GameEntity[16];
    private UIntPtr[] _pickableItemsId = new UIntPtr[16];
    private bool _shieldIsBroken;
    private SpawnedItemEntity _itemToPickUp;
    private MissionTimer _tickTimer;
    private bool _disablePickUpForAgent;

    public ItemPickupAgentComponent(Agent agent)
      : base(agent)
    {
      this._useObjectAgentComponent = this.Agent.GetComponent<UseObjectAgentComponent>();
      this.Agent.OnAgentWieldedItemChange += new Action(this.DisablePickUpForAgentIfNeeded);
      this.Agent.OnAgentMountedStateChanged += new Action(this.DisablePickUpForAgentIfNeeded);
      this.RearmTickTimer();
    }

    private void DisablePickUpForAgentIfNeeded()
    {
      this._disablePickUpForAgent = true;
      if (this.Agent.MountAgent != null)
        return;
      if (this._shieldIsBroken)
      {
        this._disablePickUpForAgent = false;
      }
      else
      {
        for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.Weapon4; ++index)
        {
          MissionWeapon missionWeapon = this.Agent.Equipment[index];
          if (!missionWeapon.IsEmpty && missionWeapon.IsAnyConsumable(out WeaponComponentData _))
          {
            this._disablePickUpForAgent = false;
            break;
          }
        }
      }
    }

    private void RearmTickTimer() => this._tickTimer = new MissionTimer(2.5f + MBRandom.RandomFloat);

    protected internal override void OnTickAsAI(float dt)
    {
      if (this.Agent.Mission.MissionEnded())
      {
        this._itemToPickUp = (SpawnedItemEntity) null;
      }
      else
      {
        if (this._itemToPickUp != null && !this._itemToPickUp.MovingAgents.ContainsKey(this.Agent))
          this._itemToPickUp = (SpawnedItemEntity) null;
        if (this._itemToPickUp != null && (NativeObject) this._itemToPickUp.GameEntity == (NativeObject) null)
          this.Agent.StopUsingGameObject(false);
        if (this._tickTimer.Check(true))
        {
          EquipmentIndex wieldedItemIndex = this.Agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
          WeaponComponentData weaponComponentData = wieldedItemIndex == EquipmentIndex.None ? (WeaponComponentData) null : this.Agent.Equipment[wieldedItemIndex].CurrentUsageItem;
          bool flag = weaponComponentData != null && weaponComponentData.IsRangedWeapon;
          Agent targetAgent = this.Agent.GetTargetAgent();
          if (this.CheckEquipmentForAgentForPickUpAvailability() && this.Agent.CanBeAssignedForScriptedMovement() && (this.Agent.IsAlarmed() && (this.Agent.GetAgentFlags() & AgentFlag.CanAttack) != AgentFlag.None) && !this.IsInImportantCombatAction() && (targetAgent == null || (!flag || this.IsAnyConsumableDepleted() || ((double) this.Agent.GetTargetRange() >= (double) this.Agent.GetMissileRange() || this.Agent.GetLastTargetVisibilityState() != AITargetVisibilityState.TargetIsClear)) && (double) targetAgent.Position.DistanceSquared(this.Agent.Position) > 400.0))
          {
            float forwardUnlimitedSpeed = this.Agent.MaximumForwardUnlimitedSpeed;
            if (this._itemToPickUp == null)
            {
              this._itemToPickUp = this.SelectPickableItem(this.Agent.Position - new Vec3(forwardUnlimitedSpeed, forwardUnlimitedSpeed, 1f), this.Agent.Position + new Vec3(forwardUnlimitedSpeed, forwardUnlimitedSpeed, 1.8f));
              if (this._itemToPickUp != null)
                this.RequestMoveToItem(this._itemToPickUp);
            }
          }
        }
        if (this._itemToPickUp == null || (this.Agent.AIStateFlags & Agent.AIStateFlag.UseObjectMoving) == Agent.AIStateFlag.None || !this.Agent.CanReachAndUseObject((UsableMissionObject) this._itemToPickUp, this.Agent.Frame.origin.DistanceSquared(this._itemToPickUp.GetUserFrameForAgent(this.Agent).Origin.Position)))
          return;
        this.Agent.UseGameObject((UsableMissionObject) this._itemToPickUp);
      }
    }

    private bool IsInImportantCombatAction()
    {
      Agent.ActionCodeType currentActionType = this.Agent.GetCurrentActionType(1);
      switch (currentActionType)
      {
        case Agent.ActionCodeType.ReadyRanged:
        case Agent.ActionCodeType.ReleaseRanged:
        case Agent.ActionCodeType.ReleaseThrowing:
        case Agent.ActionCodeType.ReadyMelee:
        case Agent.ActionCodeType.ReleaseMelee:
          return true;
        default:
          return currentActionType == Agent.ActionCodeType.DefendShield;
      }
    }

    private bool IsAnyConsumableDepleted()
    {
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.Weapon4; ++index)
      {
        MissionWeapon missionWeapon = this.Agent.Equipment[index];
        if (!missionWeapon.IsEmpty && missionWeapon.IsAnyConsumable(out WeaponComponentData _) && missionWeapon.Amount == (short) 0)
          return true;
      }
      return false;
    }

    private bool CheckEquipmentForAgentForPickUpAvailability()
    {
      if (this._disablePickUpForAgent)
        return false;
      if (this._shieldIsBroken)
        return true;
      for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.Weapon4; ++index)
      {
        MissionWeapon missionWeapon = this.Agent.Equipment[index];
        if (!missionWeapon.IsEmpty && missionWeapon.IsAnyConsumable(out WeaponComponentData _) && (int) missionWeapon.Amount <= (int) missionWeapon.ModifiedMaxAmount >> 1)
          return true;
      }
      return false;
    }

    protected internal override void OnWeaponHPChanged(ItemObject item, int hitPoints)
    {
      if (hitPoints > 0)
        return;
      switch (item.PrimaryWeapon.WeaponClass)
      {
        case WeaponClass.SmallShield:
        case WeaponClass.LargeShield:
          this._shieldIsBroken = true;
          break;
      }
    }

    public SpawnedItemEntity SelectPickableItem(Vec3 bMin, Vec3 bMax)
    {
      Agent targetAgent = this.Agent.GetTargetAgent();
      Vec3 v1 = targetAgent == null ? Vec3.Invalid : targetAgent.Position - this.Agent.Position;
      int num1 = this.Agent.Mission.Scene.SelectEntitiesInBoxWithScriptComponent<SpawnedItemEntity>(ref bMin, ref bMax, this._tempPickableEntities, this._pickableItemsId);
      float num2 = 0.0f;
      SpawnedItemEntity spawnedItemEntity = (SpawnedItemEntity) null;
      for (int index = 0; index < num1; ++index)
      {
        SpawnedItemEntity firstScriptOfType = this._tempPickableEntities[index].GetFirstScriptOfType<SpawnedItemEntity>();
        int num3;
        if (firstScriptOfType != null)
        {
          MissionWeapon weaponCopy = firstScriptOfType.WeaponCopy;
          if (!weaponCopy.IsEmpty)
          {
            weaponCopy = firstScriptOfType.WeaponCopy;
            num3 = weaponCopy.IsShield() || firstScriptOfType.IsStuckMissile() ? 1 : (firstScriptOfType.IsQuiverAndNotEmpty() ? 1 : 0);
            goto label_5;
          }
        }
        num3 = 0;
label_5:
        if (num3 != 0 && !firstScriptOfType.HasUser && (firstScriptOfType.MovingAgents.Count == 0 || firstScriptOfType.MovingAgents.ContainsKey(this.Agent)) && firstScriptOfType.GameEntityWithWorldPosition.WorldPosition.GetNavMesh() != UIntPtr.Zero)
        {
          Vec3 v2 = firstScriptOfType.GetUserFrameForAgent(this.Agent).Origin.Position - this.Agent.Position;
          double num4 = (double) v2.Normalize();
          if (targetAgent == null || (double) v1.Length - (double) Vec3.DotProduct(v1, v2) > (double) targetAgent.MaximumForwardUnlimitedSpeed * 3.0)
          {
            EquipmentIndex slotToPickUp = MissionEquipment.SelectWeaponPickUpSlot(this.Agent, firstScriptOfType.WeaponCopy, firstScriptOfType.IsStuckMissile());
            WorldPosition worldPosition = firstScriptOfType.GameEntityWithWorldPosition.WorldPosition;
            if (slotToPickUp != EquipmentIndex.None && worldPosition.GetNavMesh() != UIntPtr.Zero && (this.IsItemAvailable(firstScriptOfType, slotToPickUp) && this.Agent.CanMoveDirectlyToPosition(in worldPosition)))
            {
              float itemScore = this.GetItemScore(firstScriptOfType);
              if ((double) itemScore > (double) num2)
              {
                spawnedItemEntity = firstScriptOfType;
                num2 = itemScore;
              }
            }
          }
        }
      }
      return spawnedItemEntity;
    }

    private float GetItemScore(SpawnedItemEntity item)
    {
      if (!item.WeaponCopy.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CannotBePickedUp))
      {
        WeaponClass weaponClass = item.WeaponCopy.Item.PrimaryWeapon.WeaponClass;
        if (this.Agent.HadSameTypeOfConsumableOrShieldOnSpawn(weaponClass))
        {
          switch (weaponClass)
          {
            case WeaponClass.Arrow:
              return 80f;
            case WeaponClass.Bolt:
              return 80f;
            case WeaponClass.Stone:
              return 20f;
            case WeaponClass.Boulder:
              return 0.0f;
            case WeaponClass.ThrowingAxe:
              return 60f;
            case WeaponClass.ThrowingKnife:
              return 50f;
            case WeaponClass.Javelin:
              return 70f;
            case WeaponClass.SmallShield:
            case WeaponClass.LargeShield:
              return 100f;
            default:
              throw new MBException("This pickable item not scored: " + weaponClass.ToString());
          }
        }
      }
      return 0.0f;
    }

    private bool IsItemAvailable(SpawnedItemEntity item, EquipmentIndex slotToPickUp)
    {
      if (!this.Agent.CanReachAndUseObject((UsableMissionObject) item, 0.0f) || !this.Agent.ObjectHasVacantPosition((UsableMissionObject) item) || item.MovingAgents.Count > 0)
        return false;
      WeaponClass weaponClass = item.WeaponCopy.Item.PrimaryWeapon.WeaponClass;
      switch (weaponClass)
      {
        case WeaponClass.Arrow:
        case WeaponClass.Bolt:
        case WeaponClass.ThrowingAxe:
        case WeaponClass.ThrowingKnife:
        case WeaponClass.Javelin:
          if (item.WeaponCopy.Amount > (short) 0)
          {
            MissionWeapon missionWeapon = this.Agent.Equipment[slotToPickUp];
            if (!missionWeapon.IsEmpty)
            {
              missionWeapon = this.Agent.Equipment[slotToPickUp];
              if (missionWeapon.Item.PrimaryWeapon.WeaponClass == weaponClass)
              {
                missionWeapon = this.Agent.Equipment[slotToPickUp];
                int amount = (int) missionWeapon.Amount;
                missionWeapon = this.Agent.Equipment[slotToPickUp];
                int num = (int) missionWeapon.ModifiedMaxAmount >> 1;
                if (amount <= num)
                  return true;
                break;
              }
              break;
            }
            break;
          }
          break;
        case WeaponClass.SmallShield:
        case WeaponClass.LargeShield:
          return this.Agent.Equipment[slotToPickUp].IsEmpty && this._shieldIsBroken;
      }
      return false;
    }

    internal void ItemPickupDone(SpawnedItemEntity spawnedItemEntity)
    {
      if (spawnedItemEntity.WeaponCopy.Item.PrimaryWeapon.IsShield)
        this._shieldIsBroken = false;
      this._itemToPickUp = (SpawnedItemEntity) null;
    }

    private void RequestMoveToItem(SpawnedItemEntity item)
    {
      if (item.MovingAgents.Any<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>())
        item.MovingAgents.First<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>((Func<KeyValuePair<Agent, UsableMissionObject.MoveInfo>, bool>) (ma => ma.Key != null)).Key.StopUsingGameObject(false);
      this._useObjectAgentComponent.MoveToUsableGameObject((UsableMissionObject) item);
    }
  }
}
