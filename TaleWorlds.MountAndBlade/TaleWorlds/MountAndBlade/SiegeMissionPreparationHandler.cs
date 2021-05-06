// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeMissionPreparationHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class SiegeMissionPreparationHandler : MissionLogic
  {
    private const string SallyOutTag = "sally_out";
    private const string AssaultTag = "siege_assault";
    private const string DamageDecalTag = "damage_decal";
    private float[] _wallHitPointPercentages;
    private bool _hasAnySiegeTower;
    private SiegeMissionPreparationHandler.SiegeMissionType _siegeMissionType;

    private Scene MissionScene => Mission.Current.Scene;

    public SiegeMissionPreparationHandler(
      bool isSallyOut,
      bool isReliefForceAttack,
      float[] wallHitPointPercentages,
      bool hasAnySiegeTower)
    {
      this._siegeMissionType = !isSallyOut ? (!isReliefForceAttack ? SiegeMissionPreparationHandler.SiegeMissionType.Assault : SiegeMissionPreparationHandler.SiegeMissionType.ReliefForce) : SiegeMissionPreparationHandler.SiegeMissionType.SallyOut;
      this._wallHitPointPercentages = wallHitPointPercentages;
      this._hasAnySiegeTower = hasAnySiegeTower;
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this.SetUpScene();
    }

    private void SetUpScene()
    {
      this.ArrangeBesiegerDeploymentPointsAndMachines();
      this.ArrangeEntitiesForMissionType();
      this.ArrangeDestructedMeshes();
      if (this._siegeMissionType == SiegeMissionPreparationHandler.SiegeMissionType.Assault)
        return;
      this.ArrangeSiegeMachinesForNonAssaultMission();
    }

    private void ArrangeBesiegerDeploymentPointsAndMachines()
    {
      int num = this._siegeMissionType == SiegeMissionPreparationHandler.SiegeMissionType.Assault ? 1 : 0;
      Debug.Print("{SIEGE} ArrangeBesiegerDeploymentPointsAndMachines", color: Debug.DebugColor.DarkCyan, debugFilter: 64UL);
      Debug.Print("{SIEGE} MissionType: " + (object) this._siegeMissionType, color: Debug.DebugColor.DarkCyan, debugFilter: 64UL);
      if (num != 0)
        return;
      foreach (SynchedMissionObject synchedMissionObject in this.Mission.ActiveMissionObjects.FindAllWithType<SiegeLadder>().ToArray<SiegeLadder>())
        synchedMissionObject.SetDisabledSynched();
    }

    private void ArrangeEntitiesForMissionType()
    {
      string tag = this._siegeMissionType == SiegeMissionPreparationHandler.SiegeMissionType.Assault ? "sally_out" : "siege_assault";
      Debug.Print("{SIEGE} ArrangeEntitiesForMissionType", color: Debug.DebugColor.DarkCyan, debugFilter: 64UL);
      Debug.Print("{SIEGE} MissionType: " + (object) this._siegeMissionType, color: Debug.DebugColor.DarkCyan, debugFilter: 64UL);
      Debug.Print("{SIEGE} TagToBeRemoved: " + tag, color: Debug.DebugColor.DarkCyan, debugFilter: 64UL);
      foreach (GameEntity gameEntity in this.MissionScene.FindEntitiesWithTag(tag).ToList<GameEntity>())
        gameEntity.Remove(77);
    }

    private void ArrangeDestructedMeshes()
    {
      float num1 = 0.0f;
      foreach (float hitPointPercentage in this._wallHitPointPercentages)
        num1 += hitPointPercentage;
      if (!((IEnumerable<float>) this._wallHitPointPercentages).IsEmpty<float>())
        num1 /= (float) this._wallHitPointPercentages.Length;
      float num2 = MBMath.Lerp(0.0f, 0.7f, 1f - num1);
      IEnumerable<SynchedMissionObject> source1 = this.Mission.MissionObjects.OfType<SynchedMissionObject>();
      IEnumerable<DestructableComponent> destructibleComponents = source1.OfType<DestructableComponent>();
      foreach (StrategicArea strategicArea in this.Mission.ActiveMissionObjects.OfType<StrategicArea>().ToList<StrategicArea>())
        strategicArea.DetermineAssociatedDestructibleComponents(destructibleComponents);
      foreach (SynchedMissionObject synchedMissionObject in source1)
      {
        if (this._hasAnySiegeTower && synchedMissionObject.GameEntity.HasTag("tower_merlon"))
        {
          synchedMissionObject.SetVisibleSynched(false, true);
        }
        else
        {
          DestructableComponent firstScriptOfType = synchedMissionObject.GameEntity.GetFirstScriptOfType<DestructableComponent>();
          if (firstScriptOfType != null && firstScriptOfType.CanBeDestroyedInitially && ((double) num2 > 0.0 && (double) MBRandom.RandomFloat <= (double) num2))
            firstScriptOfType.PreDestroy();
        }
      }
      if ((double) num2 >= 0.100000001490116)
      {
        List<GameEntity> list = this.Mission.Scene.FindEntitiesWithTag("damage_decal").ToList<GameEntity>();
        foreach (GameEntity gameEntity in list)
          gameEntity.GetFirstScriptOfType<SynchedMissionObject>().SetVisibleSynched(false);
        for (double num3 = Math.Floor((double) list.Count * (double) num2); num3 > 0.0; --num3)
        {
          GameEntity gameEntity = list[MBRandom.RandomInt(list.Count)];
          list.Remove(gameEntity);
          gameEntity.GetFirstScriptOfType<SynchedMissionObject>().SetVisibleSynched(true);
        }
      }
      List<WallSegment> source2 = new List<WallSegment>();
      List<WallSegment> list1 = this.Mission.ActiveMissionObjects.FindAllWithType<WallSegment>().Where<WallSegment>((Func<WallSegment, bool>) (ws => ws.DefenseSide != FormationAI.BehaviorSide.BehaviorSideNotSet && ws.GameEntity.GetChildren().Any<GameEntity>((Func<GameEntity, bool>) (ge => ge.HasTag("broken_child"))))).ToList<WallSegment>();
      int val2 = 0;
      foreach (float hitPointPercentage in this._wallHitPointPercentages)
      {
        if ((double) Math.Abs(hitPointPercentage) < 9.99999974737875E-06)
          ++val2;
      }
      int num4 = Math.Min(list1.Count<WallSegment>(), val2);
      while (num4 > 0)
      {
        WallSegment randomElement = list1.GetRandomElement<WallSegment>();
        randomElement.OnChooseUsedWallSegment(true);
        list1.Remove(randomElement);
        --num4;
        source2.Add(randomElement);
      }
      foreach (WallSegment wallSegment in list1)
        wallSegment.OnChooseUsedWallSegment(false);
      if ((double) num2 < 0.100000001490116)
        return;
      List<SiegeWeapon> siegeWeaponList = new List<SiegeWeapon>();
      foreach (SiegeWeapon siegeWeapon in this.Mission.ActiveMissionObjects.FindAllWithType<SiegeWeapon>().Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (sw => sw is IPrimarySiegeWeapon)))
      {
        SiegeWeapon primarySiegeWeapon = siegeWeapon;
        if (source2.Any<WallSegment>((Func<WallSegment, bool>) (b => b.DefenseSide == ((IPrimarySiegeWeapon) primarySiegeWeapon).WeaponSide)))
          siegeWeaponList.Add(primarySiegeWeapon);
      }
      siegeWeaponList.ForEach((Action<SiegeWeapon>) (siegeWeaponToRemove => siegeWeaponToRemove.SetDisabledSynched()));
    }

    private void ArrangeSiegeMachinesForNonAssaultMission()
    {
      foreach (GameEntity gameEntity in Mission.Current.GetActiveEntitiesWithScriptComponentOfType<SiegeWeapon>())
      {
        SiegeWeapon firstScriptOfType = gameEntity.GetFirstScriptOfType<SiegeWeapon>();
        SiegeEngineType siegeEngineType = firstScriptOfType.GetSiegeEngineType();
        if (siegeEngineType != DefaultSiegeEngineTypes.Ballista && siegeEngineType != DefaultSiegeEngineTypes.FireBallista && (siegeEngineType != DefaultSiegeEngineTypes.Catapult && siegeEngineType != DefaultSiegeEngineTypes.FireCatapult) && (siegeEngineType != DefaultSiegeEngineTypes.Onager && siegeEngineType != DefaultSiegeEngineTypes.FireOnager))
          firstScriptOfType.Deactivate();
      }
    }

    private enum SiegeMissionType
    {
      Assault,
      SallyOut,
      ReliefForce,
    }
  }
}
