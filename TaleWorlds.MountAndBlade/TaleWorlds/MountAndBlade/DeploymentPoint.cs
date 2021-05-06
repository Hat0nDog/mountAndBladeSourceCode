// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DeploymentPoint
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
using TaleWorlds.MountAndBlade.Missions;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
  public class DeploymentPoint : SynchedMissionObject
  {
    private DeploymentPoint.DeploymentPointType _deploymentPointType;
    private List<SiegeLadder> _associatedSiegeLadders;
    private bool _isBreachSideDeploymentPoint;
    public BattleSideEnum Side = BattleSideEnum.Attacker;
    public float Radius = 3f;
    public string SiegeWeaponTag = "dpWeapon";
    private IEnumerable<SynchedMissionObject> _weapons;
    private readonly List<GameEntity> _highlightedEntites = new List<GameEntity>();

    public event Action<DeploymentPoint> OnDeploymentStateChanged;

    public event Action<DeploymentPoint> OnDeploymentPointTypeDetermined;

    public Vec3 DeploymentTargetPosition { get; private set; }

    public WallSegment AssociatedWallSegment { get; private set; }

    public IEnumerable<SynchedMissionObject> DeployableWeapons => this._weapons.Where<SynchedMissionObject>((Func<SynchedMissionObject, bool>) (w => !w.IsDisabled));

    public bool IsDeployed => this.DeployedWeapon != null;

    public SynchedMissionObject DeployedWeapon { get; private set; }

    public SynchedMissionObject DisbandedWeapon { get; private set; }

    protected internal override void OnInit() => this._weapons = (IEnumerable<SynchedMissionObject>) new List<SynchedMissionObject>();

    public override void AfterMissionStart()
    {
      base.OnInit();
      if (GameNetwork.IsClientOrReplay)
        return;
      this._weapons = this.GetWeaponsUnder();
      this._associatedSiegeLadders = new List<SiegeLadder>();
      if (this.DeployableWeapons.IsEmpty<SynchedMissionObject>())
      {
        this.SetVisibleSynched(false);
        this.SetBreachSideDeploymentPoint();
      }
      base.AfterMissionStart();
      if (!GameNetwork.IsClientOrReplay)
        this.DetermineDeploymentPointType();
      this.HideAllWeapons();
    }

    private void SetBreachSideDeploymentPoint()
    {
      this._isBreachSideDeploymentPoint = true;
      this._deploymentPointType = DeploymentPoint.DeploymentPointType.Breach;
      FormationAI.BehaviorSide deploymentPointSide = (this._weapons.FirstOrDefault<SynchedMissionObject>((Func<SynchedMissionObject, bool>) (w => w is SiegeTower)) as IPrimarySiegeWeapon).WeaponSide;
      this.AssociatedWallSegment = Mission.Current.ActiveMissionObjects.FindAllWithType<WallSegment>().FirstOrDefault<WallSegment>((Func<WallSegment, bool>) (ws => ws.DefenseSide == deploymentPointSide));
      this.DeploymentTargetPosition = this.AssociatedWallSegment.GameEntity.GlobalPosition;
    }

    public Vec3 GetDeploymentOrigin() => this.GameEntity.GlobalPosition;

    public DeploymentPoint.DeploymentPointState GetDeploymentPointState()
    {
      switch (this._deploymentPointType)
      {
        case DeploymentPoint.DeploymentPointType.BatteringRam:
          return !this.IsDeployed ? DeploymentPoint.DeploymentPointState.NotDeployed : DeploymentPoint.DeploymentPointState.BatteringRam;
        case DeploymentPoint.DeploymentPointType.TowerLadder:
          return !this.IsDeployed ? DeploymentPoint.DeploymentPointState.SiegeLadder : DeploymentPoint.DeploymentPointState.SiegeTower;
        case DeploymentPoint.DeploymentPointType.Breach:
          return DeploymentPoint.DeploymentPointState.Breach;
        case DeploymentPoint.DeploymentPointType.Ranged:
          return !this.IsDeployed ? DeploymentPoint.DeploymentPointState.NotDeployed : DeploymentPoint.DeploymentPointState.Ranged;
        default:
          MBDebug.ShowWarning("Undefined deployment point type fetched.");
          return DeploymentPoint.DeploymentPointState.NotDeployed;
      }
    }

    public DeploymentPoint.DeploymentPointType GetDeploymentPointType() => this._deploymentPointType;

    public List<SiegeLadder> GetAssociatedSiegeLadders() => this._associatedSiegeLadders;

    private void DetermineDeploymentPointType()
    {
      if (this._isBreachSideDeploymentPoint)
        this._deploymentPointType = DeploymentPoint.DeploymentPointType.Breach;
      else if (this._weapons.Any<SynchedMissionObject>((Func<SynchedMissionObject, bool>) (w => w is BatteringRam)))
      {
        this._deploymentPointType = DeploymentPoint.DeploymentPointType.BatteringRam;
        this.DeploymentTargetPosition = (this._weapons.First<SynchedMissionObject>((Func<SynchedMissionObject, bool>) (w => w is BatteringRam)) as IPrimarySiegeWeapon).TargetCastlePosition.GameEntity.GlobalPosition;
      }
      else if (this._weapons.Any<SynchedMissionObject>((Func<SynchedMissionObject, bool>) (w => w is SiegeTower)))
      {
        SiegeTower tower = this._weapons.FirstOrDefault<SynchedMissionObject>((Func<SynchedMissionObject, bool>) (w => w is SiegeTower)) as SiegeTower;
        this._deploymentPointType = DeploymentPoint.DeploymentPointType.TowerLadder;
        this.DeploymentTargetPosition = tower.TargetCastlePosition.GameEntity.GlobalPosition;
        this._associatedSiegeLadders = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeLadder>().Where<SiegeLadder>((Func<SiegeLadder, bool>) (sl => sl.WeaponSide == tower.WeaponSide)).ToList<SiegeLadder>();
      }
      else
      {
        this._deploymentPointType = DeploymentPoint.DeploymentPointType.Ranged;
        this.DeploymentTargetPosition = Vec3.Invalid;
      }
      Action<DeploymentPoint> pointTypeDetermined = this.OnDeploymentPointTypeDetermined;
      if (pointTypeDetermined == null)
        return;
      pointTypeDetermined(this);
    }

    public IEnumerable<SynchedMissionObject> GetWeaponsUnder()
    {
      List<GameEntity> entities = new List<GameEntity>();
      this.GameEntity.Scene.GetEntities(ref entities);
      IEnumerable<SiegeWeapon> source = entities.Where<GameEntity>((Func<GameEntity, bool>) (se => se.HasScriptOfType<SiegeWeapon>())).Select<GameEntity, SiegeWeapon>((Func<GameEntity, SiegeWeapon>) (se => se.GetScriptComponents<SiegeWeapon>().FirstOrDefault<SiegeWeapon>()));
      IEnumerable<SynchedMissionObject> first = source.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (ssw =>
      {
        if (ssw.GameEntity.HasTag(this.SiegeWeaponTag))
          return true;
        return (NativeObject) ssw.GameEntity.Parent != (NativeObject) null && ssw.GameEntity.Parent.HasTag(this.SiegeWeaponTag);
      })).Select<SiegeWeapon, SynchedMissionObject>((Func<SiegeWeapon, SynchedMissionObject>) (ssw => (SynchedMissionObject) ssw));
      Vec3 globalPosition = this.GameEntity.GlobalPosition;
      float radiusSquared = this.Radius * this.Radius;
      IEnumerable<SynchedMissionObject> second = source.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (ssw => (NativeObject) ssw.GameEntity != (NativeObject) this.GameEntity && (double) ssw.GameEntity.GlobalPosition.DistanceSquared(globalPosition) < (double) radiusSquared)).Select<SiegeWeapon, SynchedMissionObject>((Func<SiegeWeapon, SynchedMissionObject>) (ssw => (SynchedMissionObject) ssw));
      return first.Concat<SynchedMissionObject>(second).Distinct<SynchedMissionObject>();
    }

    public IEnumerable<SpawnerBase> GetSpawnersForEditor()
    {
      List<GameEntity> entities = new List<GameEntity>();
      this.GameEntity.Scene.GetEntities(ref entities);
      IEnumerable<SpawnerBase> source = entities.Where<GameEntity>((Func<GameEntity, bool>) (se => se.HasScriptOfType<SpawnerBase>())).Select<GameEntity, SpawnerBase>((Func<GameEntity, SpawnerBase>) (se => se.GetScriptComponents<SpawnerBase>().FirstOrDefault<SpawnerBase>()));
      IEnumerable<SpawnerBase> first = source.Where<SpawnerBase>((Func<SpawnerBase, bool>) (ssw => ssw.GameEntity.HasTag(this.SiegeWeaponTag))).Select<SpawnerBase, SpawnerBase>((Func<SpawnerBase, SpawnerBase>) (ssw => ssw));
      Vec3 globalPosition = this.GameEntity.GlobalPosition;
      float radiusSquared = this.Radius * this.Radius;
      IEnumerable<SpawnerBase> second = source.Where<SpawnerBase>((Func<SpawnerBase, bool>) (ssw => (NativeObject) ssw.GameEntity != (NativeObject) this.GameEntity && (double) ssw.GameEntity.GlobalPosition.DistanceSquared(globalPosition) < (double) radiusSquared)).Select<SpawnerBase, SpawnerBase>((Func<SpawnerBase, SpawnerBase>) (ssw => ssw));
      return first.Concat<SpawnerBase>(second).Distinct<SpawnerBase>();
    }

    protected internal override void OnEditorInit()
    {
      base.OnEditorInit();
      this._weapons = (IEnumerable<SynchedMissionObject>) null;
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      foreach (GameEntity highlightedEntite in this._highlightedEntites)
        highlightedEntite.SetContourColor(new uint?());
      this._highlightedEntites.Clear();
      if (!MBEditor.IsEntitySelected(this.GameEntity))
        return;
      uint color = 4294901760;
      if ((double) this.Radius > 0.0)
        DebugExtensions.RenderDebugCircleOnTerrain(this.Scene, this.GameEntity.GetGlobalFrame(), this.Radius, color);
      foreach (SpawnerBase spawnerBase in this.GetSpawnersForEditor())
      {
        spawnerBase.GameEntity.SetContourColor(new uint?(color));
        this._highlightedEntites.Add(spawnerBase.GameEntity);
      }
    }

    private void OnDeploymentStateChangedAux(SynchedMissionObject targetObject)
    {
      if (this.IsDeployed)
      {
        targetObject.SetVisibleSynched(true);
        targetObject.SetPhysicsStateSynched(true);
      }
      else
      {
        targetObject.SetVisibleSynched(false);
        targetObject.SetPhysicsStateSynched(false);
      }
      Action<DeploymentPoint> deploymentStateChanged = this.OnDeploymentStateChanged;
      if (deploymentStateChanged != null)
        deploymentStateChanged(this);
      if (!(targetObject is SiegeWeapon siegeWeapon))
        return;
      siegeWeapon.OnDeploymentStateChanged(this.IsDeployed);
    }

    public void Deploy(System.Type t)
    {
      this.DeployedWeapon = this._weapons.First<SynchedMissionObject>((Func<SynchedMissionObject, bool>) (w => SiegeWeaponCollection.GetWeaponType((ScriptComponentBehaviour) w) == t));
      this.OnDeploymentStateChangedAux(this.DeployedWeapon);
      this.ToggleDeploymentPointVisibility(false);
      this.ToggleDeployedWeaponVisibility(true);
    }

    public void Deploy(SiegeWeapon s)
    {
      this.DeployedWeapon = (SynchedMissionObject) s;
      this.DisbandedWeapon = (SynchedMissionObject) null;
      this.OnDeploymentStateChangedAux((SynchedMissionObject) s);
      this.ToggleDeploymentPointVisibility(false);
      this.ToggleDeployedWeaponVisibility(true);
    }

    public ScriptComponentBehaviour Disband()
    {
      this.ToggleDeploymentPointVisibility(true);
      this.ToggleDeployedWeaponVisibility(false);
      this.DisbandedWeapon = this.DeployedWeapon;
      this.DeployedWeapon = (SynchedMissionObject) null;
      this.OnDeploymentStateChangedAux(this.DisbandedWeapon);
      return (ScriptComponentBehaviour) this.DisbandedWeapon;
    }

    public IEnumerable<System.Type> DeployableWeaponTypes => this.DeployableWeapons.Select<SynchedMissionObject, System.Type>(new Func<SynchedMissionObject, System.Type>(SiegeWeaponCollection.GetWeaponType));

    public void Hide()
    {
      this.ToggleDeploymentPointVisibility(false);
      foreach (SynchedMissionObject synchedMissionObject in this.GetWeaponsUnder())
      {
        if (synchedMissionObject != null)
        {
          synchedMissionObject.SetVisibleSynched(false);
          synchedMissionObject.SetPhysicsStateSynched(false);
        }
      }
    }

    public void Show()
    {
      this.ToggleDeploymentPointVisibility(!this.IsDeployed);
      if (!this.IsDeployed)
        return;
      this.ToggleDeployedWeaponVisibility(true);
    }

    private void ToggleDeploymentPointVisibility(bool visible)
    {
      this.SetVisibleSynched(visible);
      this.SetPhysicsStateSynched(visible);
    }

    private void ToggleDeployedWeaponVisibility(bool visible) => this.ToggleWeaponVisibility(visible, this.DeployedWeapon);

    public void ToggleWeaponVisibility(bool visible, SynchedMissionObject weapon)
    {
      if ((NativeObject) weapon.GameEntity.Parent != (NativeObject) null && weapon.GameEntity.Parent.HasScriptOfType<SynchedMissionObject>())
      {
        weapon.GameEntity.Parent.GetScriptComponents<SynchedMissionObject>().First<SynchedMissionObject>().SetVisibleSynched(visible);
        weapon.GameEntity.Parent.GetScriptComponents<SynchedMissionObject>().First<SynchedMissionObject>().SetPhysicsStateSynched(visible);
      }
      else
      {
        weapon?.SetVisibleSynched(visible);
        weapon?.SetPhysicsStateSynched(visible);
      }
    }

    public void HideAllWeapons()
    {
      foreach (SynchedMissionObject deployableWeapon in this.DeployableWeapons)
        this.ToggleWeaponVisibility(false, deployableWeapon);
    }

    public enum DeploymentPointType
    {
      BatteringRam,
      TowerLadder,
      Breach,
      Ranged,
    }

    public enum DeploymentPointState
    {
      NotDeployed,
      BatteringRam,
      SiegeLadder,
      SiegeTower,
      Breach,
      Ranged,
    }
  }
}
