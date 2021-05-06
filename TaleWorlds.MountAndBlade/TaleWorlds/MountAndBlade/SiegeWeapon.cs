// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeWeapon
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
  public abstract class SiegeWeapon : UsableMachine, ITargetable
  {
    protected bool _spawnedFromSpawner;
    private bool _forcedUse;
    [EditableScriptComponentVariable(true)]
    internal string RemoveOnDeployTag = "";
    private List<GameEntity> _removeOnDeployEntities;
    [EditableScriptComponentVariable(true)]
    internal string AddOnDeployTag = "";
    private List<GameEntity> _addOnDeployEntities;
    private const string TargetingEntityTag = "targeting_entity";
    private GameEntity _targetingEntity;
    private List<Formation> _potentialUsingFormations;

    public abstract SiegeEngineType GetSiegeEngineType();

    public bool ForcedUse
    {
      get => this._forcedUse;
      set
      {
        if (this.ForcedUse == value)
          return;
        this.OnForcedUseChanged(value);
        this._forcedUse = value;
      }
    }

    public bool IsUsed => Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => t.Side == this.Side)).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).Any<Formation>((Func<Formation, bool>) (f => f.IsUsingMachine((UsableMachine) this)));

    private bool CalculateIsSufficientlyManned()
    {
      for (int index = 0; index < Mission.Current.Teams.Count; ++index)
      {
        Team team = Mission.Current.Teams[index];
        if (team.Side == this.Side)
        {
          foreach (Formation formation in team.FormationsIncludingSpecial)
          {
            if (formation.IsUsingMachine((UsableMachine) this) && (formation.arrangement.UnitCount > 1 || formation.arrangement.UnitCount > 0 && !formation.HasPlayer))
              return true;
          }
        }
      }
      return false;
    }

    public virtual BattleSideEnum Side => BattleSideEnum.Attacker;

    protected internal override void OnInit()
    {
      base.OnInit();
      this.ForcedUse = true;
      this._potentialUsingFormations = new List<Formation>();
      this.GameEntity.SetAnimationSoundActivation(true);
      this._removeOnDeployEntities = Mission.Current.Scene.FindEntitiesWithTag(this.RemoveOnDeployTag).ToList<GameEntity>();
      this._addOnDeployEntities = Mission.Current.Scene.FindEntitiesWithTag(this.AddOnDeployTag).ToList<GameEntity>();
      foreach (StandingPoint standingPoint in this.StandingPoints)
        standingPoint.ClearWaypointEntities();
      this._targetingEntity = this.GameEntity.CollectChildrenEntitiesWithTag("targeting_entity").FirstOrDefault<GameEntity>();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this.GameEntity.IsVisibleIncludeParents() && !GameNetwork.IsClientOrReplay ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (GameNetwork.IsClientOrReplay || !this.GameEntity.IsVisibleIncludeParents())
        return;
      if (this.IsDisabledForBattleSide(this.Side))
      {
        foreach (Formation formation in this.Users.Where<Agent>((Func<Agent, bool>) (u => !u.IsMainAgent && u.Formation != null && u.Formation.Team.Side == this.Side)).Select<Agent, Formation>((Func<Agent, Formation>) (u => u.Formation)).Distinct<Formation>())
          formation.StopUsingMachine((UsableMachine) this);
      }
      else
      {
        if (!this.ForcedUse || this.CalculateIsSufficientlyManned())
          return;
        bool flag1 = false;
        for (int index = 0; index < Mission.Current.Teams.Count; ++index)
        {
          Team team = Mission.Current.Teams[index];
          if (team.Side == this.Side)
          {
            foreach (Formation formation in team.FormationsIncludingSpecial)
            {
              if (formation.MovementOrder.OrderEnum != MovementOrder.MovementOrderEnum.Retreat && (formation.arrangement.UnitCount > 1 || formation.arrangement.UnitCount > 0 && !formation.HasPlayer) && !formation.Detachments.Contains((IDetachment) this))
              {
                bool flag2 = formation.CountOfUnits >= this.MaxUserCount;
                if (!flag1)
                {
                  if (flag2)
                  {
                    flag1 = true;
                    this._potentialUsingFormations.Clear();
                    this._potentialUsingFormations.Add(formation);
                  }
                  else
                    this._potentialUsingFormations.Add(formation);
                }
                else if (flag2)
                  this._potentialUsingFormations.Add(formation);
              }
            }
          }
        }
        if (this._potentialUsingFormations.Count > 0)
        {
          Formation formation = this._potentialUsingFormations.MinBy<Formation, float>((Func<Formation, float>) (puf => puf.QuerySystem.AveragePosition.DistanceSquared(this.GameEntity.GlobalPosition.AsVec2)));
          formation.StartUsingMachine((UsableMachine) this, !formation.IsAIControlled);
        }
        this._potentialUsingFormations.Clear();
      }
    }

    internal virtual void OnDeploymentStateChanged(bool isDeployed)
    {
      foreach (GameEntity removeOnDeployEntity in this._removeOnDeployEntities)
      {
        removeOnDeployEntity.SetVisibilityExcludeParents(!isDeployed);
        StrategicArea firstScriptOfType = removeOnDeployEntity.GetFirstScriptOfType<StrategicArea>();
        if (firstScriptOfType != null)
        {
          firstScriptOfType.OnParentGameEntityVisibilityChanged(!isDeployed);
        }
        else
        {
          foreach (StrategicArea strategicArea in removeOnDeployEntity.GetChildren().Where<GameEntity>((Func<GameEntity, bool>) (c => c.HasScriptOfType<StrategicArea>())).Select<GameEntity, StrategicArea>((Func<GameEntity, StrategicArea>) (c => c.GetFirstScriptOfType<StrategicArea>())))
            strategicArea.OnParentGameEntityVisibilityChanged(!isDeployed);
        }
      }
      foreach (GameEntity addOnDeployEntity in this._addOnDeployEntities)
      {
        addOnDeployEntity.SetVisibilityExcludeParents(isDeployed);
        StrategicArea firstScriptOfType = addOnDeployEntity.GetFirstScriptOfType<StrategicArea>();
        if (firstScriptOfType != null)
        {
          firstScriptOfType.OnParentGameEntityVisibilityChanged(isDeployed);
        }
        else
        {
          foreach (StrategicArea strategicArea in addOnDeployEntity.GetChildren().Where<GameEntity>((Func<GameEntity, bool>) (c => c.HasScriptOfType<StrategicArea>())).Select<GameEntity, StrategicArea>((Func<GameEntity, StrategicArea>) (c => c.GetFirstScriptOfType<StrategicArea>())))
            strategicArea.OnParentGameEntityVisibilityChanged(isDeployed);
        }
      }
      if (this._addOnDeployEntities.Count <= 0 && this._removeOnDeployEntities.Count <= 0)
        return;
      foreach (UsableMissionObject standingPoint in this.StandingPoints)
        standingPoint.RefreshGameEntityWithWorldPosition();
    }

    public override bool HasWaitFrame
    {
      get
      {
        if (!base.HasWaitFrame)
          return false;
        return !(this is IPrimarySiegeWeapon) || !(this as IPrimarySiegeWeapon).HasCompletedAction();
      }
    }

    public override bool IsDeactivated => this.IsDisabled || (NativeObject) this.GameEntity == (NativeObject) null || !this.GameEntity.IsVisibleIncludeParents() || base.IsDeactivated;

    public void OnForcedUseChanged(bool value)
    {
    }

    protected float GetUserMultiplierOfWeapon() => this.UserCount == 0 ? 0.1f : (float) (0.699999988079071 + 0.300000011920929 * (double) this.UserCount / (double) this.MaxUserCount);

    protected virtual float GetDistanceMultiplierOfWeapon(Vec3 weaponPos) => (double) this.GetMinimumDistanceBetweenPositions(weaponPos) > 20.0 ? 0.4f : 1f;

    protected virtual float GetMinimumDistanceBetweenPositions(Vec3 position) => this.GameEntity.GlobalPosition.DistanceSquared(position);

    protected float GetHitpointMultiplierofWeapon() => this.DestructionComponent != null ? Math.Max(1f, 2f - (float) Math.Log10((double) this.DestructionComponent.HitPoint / (double) this.DestructionComponent.MaxHitPoint * 10.0 + 1.0)) : 1f;

    public GameEntity GetTargetEntity() => (NativeObject) this._targetingEntity != (NativeObject) null ? this._targetingEntity : this.GameEntity;

    public BattleSideEnum GetSide() => this.Side;

    public GameEntity Entity() => this.GameEntity;

    public abstract TargetFlags GetTargetFlags();

    public abstract float GetTargetValue(List<Vec3> weaponPos);
  }
}
