// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorSergeantMPInfantry
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
  public class BehaviorSergeantMPInfantry : BehaviorComponent
  {
    private BehaviorSergeantMPInfantry.BehaviorState _behaviorState;
    private List<FlagCapturePoint> _flagpositions;
    private MissionMultiplayerFlagDomination _flagDominationGameMode;

    public BehaviorSergeantMPInfantry(Formation formation)
      : base(formation)
    {
      this._behaviorState = BehaviorSergeantMPInfantry.BehaviorState.Unset;
      this._flagpositions = Mission.Current.ActiveMissionObjects.FindAllWithType<FlagCapturePoint>().ToList<FlagCapturePoint>();
      this._flagDominationGameMode = Mission.Current.GetMissionBehaviour<MissionMultiplayerFlagDomination>();
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      BehaviorSergeantMPInfantry.BehaviorState behaviorState = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null || (!this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedFormation || (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) > (this._behaviorState == BehaviorSergeantMPInfantry.BehaviorState.Attacking ? 3600.0 : 2500.0)) && (!this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsInfantryFormation || (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) > (this._behaviorState == BehaviorSergeantMPInfantry.BehaviorState.Attacking ? 900.0 : 400.0)) ? BehaviorSergeantMPInfantry.BehaviorState.GoingToFlag : BehaviorSergeantMPInfantry.BehaviorState.Attacking;
      if (behaviorState == BehaviorSergeantMPInfantry.BehaviorState.Attacking && (this._behaviorState != BehaviorSergeantMPInfantry.BehaviorState.Attacking || this.CurrentOrder.OrderEnum != MovementOrder.MovementOrderEnum.ChargeToTarget || this.CurrentOrder.TargetFormation.QuerySystem != this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation))
      {
        this._behaviorState = BehaviorSergeantMPInfantry.BehaviorState.Attacking;
        this.CurrentOrder = MovementOrder.MovementOrderChargeToTarget(this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation);
      }
      if (behaviorState != BehaviorSergeantMPInfantry.BehaviorState.GoingToFlag)
        return;
      this._behaviorState = behaviorState;
      WorldPosition position;
      if (this._flagpositions.Any<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != this.formation.Team)))
        position = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, this._flagpositions.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != this.formation.Team)).MinBy<FlagCapturePoint, float>((Func<FlagCapturePoint, float>) (fp => fp.Position.AsVec2.DistanceSquared(this.formation.QuerySystem.AveragePosition))).Position, false);
      else if (this._flagpositions.Any<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == this.formation.Team)))
      {
        position = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, this._flagpositions.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == this.formation.Team)).MinBy<FlagCapturePoint, float>((Func<FlagCapturePoint, float>) (fp => fp.Position.AsVec2.DistanceSquared(this.formation.QuerySystem.AveragePosition))).Position, false);
      }
      else
      {
        position = this.formation.QuerySystem.MedianPosition;
        position.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      if (this.CurrentOrder.OrderEnum != MovementOrder.MovementOrderEnum.Invalid && !(this.CurrentOrder.GetPosition(this.formation).AsVec2 != position.AsVec2))
        return;
      Vec2 direction = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null ? (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized() : this.formation.Direction;
      this.CurrentOrder = MovementOrder.MovementOrderMove(position);
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
    }

    protected internal override void TickOccasionally()
    {
      this._flagpositions.RemoveAll((Predicate<FlagCapturePoint>) (fp => fp.IsDeactivated));
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this.formation.QuerySystem.HasShield && (this._behaviorState == BehaviorSergeantMPInfantry.BehaviorState.Attacking || this._behaviorState == BehaviorSergeantMPInfantry.BehaviorState.GoingToFlag && this.CurrentOrder.GetPosition(this.formation).AsVec2.IsValid && (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.CurrentOrder.GetPosition(this.formation).AsVec2) <= 225.0))
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
      else
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight() => this.formation.QuerySystem.IsInfantryFormation ? 1.2f : 0.0f;

    private enum BehaviorState
    {
      GoingToFlag,
      Attacking,
      Unset,
    }
  }
}
