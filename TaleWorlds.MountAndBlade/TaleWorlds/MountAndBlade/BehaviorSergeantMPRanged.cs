// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorSergeantMPRanged
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorSergeantMPRanged : BehaviorComponent
  {
    private List<FlagCapturePoint> _flagpositions;
    private Formation _attachedInfantry;
    private MissionMultiplayerFlagDomination _flagDominationGameMode;

    public BehaviorSergeantMPRanged(Formation formation)
      : base(formation)
    {
      this._flagpositions = Mission.Current.ActiveMissionObjects.FindAllWithType<FlagCapturePoint>().ToList<FlagCapturePoint>();
      this._flagDominationGameMode = Mission.Current.GetMissionBehaviour<MissionMultiplayerFlagDomination>();
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      List<Formation> list = this.formation.Team.QuerySystem.EnemyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (t => t.Team.Formations)).ToList<Formation>();
      if (this.formation.Team.Formations.Any<Formation>((Func<Formation, bool>) (f => f != this.formation && f.QuerySystem.IsInfantryFormation)))
      {
        this._attachedInfantry = this.formation.Team.Formations.Where<Formation>((Func<Formation, bool>) (f => f != this.formation && f.QuerySystem.IsInfantryFormation)).MinBy<Formation, float>((Func<Formation, float>) (f => f.QuerySystem.MedianPosition.AsVec2.DistanceSquared(this.formation.QuerySystem.AveragePosition)));
        Formation formation = (Formation) null;
        if (list.Any<Formation>())
        {
          if ((double) this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2.DistanceSquared(this.formation.QuerySystem.AveragePosition) <= 4900.0)
            formation = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation;
          else if (list.Any<Formation>((Func<Formation, bool>) (ef => ef.QuerySystem.IsCavalryFormation || ef.QuerySystem.IsRangedCavalryFormation)))
            formation = list.Where<Formation>((Func<Formation, bool>) (ef => ef.QuerySystem.IsCavalryFormation || ef.QuerySystem.IsRangedCavalryFormation)).MinBy<Formation, float>((Func<Formation, float>) (ecf => ecf.QuerySystem.MedianPosition.AsVec2.DistanceSquared(this.formation.QuerySystem.AveragePosition)));
        }
        Vec2 vec2;
        if (formation != null)
        {
          WorldPosition medianPosition = formation.QuerySystem.MedianPosition;
          Vec2 asVec2_1 = medianPosition.AsVec2;
          medianPosition = this._attachedInfantry.QuerySystem.MedianPosition;
          Vec2 asVec2_2 = medianPosition.AsVec2;
          vec2 = (asVec2_1 - asVec2_2).Normalized();
        }
        else
          vec2 = this._attachedInfantry.Direction;
        Vec2 direction = vec2;
        WorldPosition medianPosition1 = this._attachedInfantry.QuerySystem.MedianPosition;
        medianPosition1.SetVec2(medianPosition1.AsVec2 - direction * (float) (((double) this._attachedInfantry.Depth + (double) this.formation.Depth) / 2.0));
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition1);
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
      }
      else if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && (double) this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2.DistanceSquared(this.formation.QuerySystem.AveragePosition) <= 4900.0)
      {
        Vec2 direction = (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized();
        float num = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2.Distance(this.formation.QuerySystem.AveragePosition);
        WorldPosition medianPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
        if ((double) num > (double) this.formation.QuerySystem.MissileRange)
          medianPosition.SetVec2(medianPosition.AsVec2 - direction * (this.formation.QuerySystem.MissileRange - this.formation.Depth * 0.5f));
        else if ((double) num < (double) this.formation.QuerySystem.MissileRange * 0.400000005960464)
          medianPosition.SetVec2(medianPosition.AsVec2 - direction * (this.formation.QuerySystem.MissileRange * 0.4f));
        else
          medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
      }
      else if (this._flagpositions.Any<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != this.formation.Team)))
      {
        Vec3 position = this._flagpositions.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != this.formation.Team)).MinBy<FlagCapturePoint, float>((Func<FlagCapturePoint, float>) (fp => fp.Position.AsVec2.DistanceSquared(this.formation.QuerySystem.AveragePosition))).Position;
        WorldPosition worldPosition;
        if (this.CurrentOrder.OrderEnum != MovementOrder.MovementOrderEnum.Invalid)
        {
          worldPosition = this.CurrentOrder.GetPosition(this.formation);
          if (!(worldPosition.AsVec2 != position.AsVec2))
            return;
        }
        Vec2 direction;
        if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null)
        {
          direction = this.formation.Direction;
        }
        else
        {
          worldPosition = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition;
          direction = (worldPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized();
        }
        this.CurrentOrder = MovementOrder.MovementOrderMove(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, position, false));
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
      }
      else if (this._flagpositions.Any<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == this.formation.Team)))
      {
        Vec3 position = this._flagpositions.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == this.formation.Team)).MinBy<FlagCapturePoint, float>((Func<FlagCapturePoint, float>) (fp => fp.Position.AsVec2.DistanceSquared(this.formation.QuerySystem.AveragePosition))).Position;
        this.CurrentOrder = MovementOrder.MovementOrderMove(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, position, false));
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      else
      {
        WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
    }

    protected internal override void TickOccasionally()
    {
      this._flagpositions.RemoveAll((Predicate<FlagCapturePoint>) (fp => fp.IsDeactivated));
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight() => this.formation.QuerySystem.IsRangedFormation ? 1.2f : 0.0f;
  }
}
