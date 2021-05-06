// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorAdvance
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
  public sealed class BehaviorAdvance : BehaviorComponent
  {
    private bool _isInShieldWallDistance;
    private bool _switchedToShieldWallRecently;
    private Timer _switchedToShieldWallTimer;
    private Vec2 _reformPosition = Vec2.Invalid;

    public BehaviorAdvance(Formation formation)
      : base(formation)
    {
      this.BehaviorCoherence = 0.8f;
      this._switchedToShieldWallTimer = new Timer(0.0f, 0.0f);
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      if (this._switchedToShieldWallRecently && !this._switchedToShieldWallTimer.Check(Mission.Current.Time) && (double) this.formation.QuerySystem.FormationDispersedness > 2.0)
      {
        WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
        if (this._reformPosition.IsValid)
        {
          medianPosition.SetVec2(this._reformPosition);
        }
        else
        {
          this._reformPosition = this.formation.QuerySystem.AveragePosition + (this.formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this.formation.QuerySystem.AveragePosition).Normalized() * 5f;
          medianPosition.SetVec2(this._reformPosition);
        }
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
      }
      else
      {
        this._switchedToShieldWallRecently = false;
        bool flag = false;
        Vec2 vec2_1;
        if (this.formation.QuerySystem.ClosestEnemyFormation != null && this.formation.QuerySystem.ClosestEnemyFormation.IsCavalryFormation)
        {
          Vec2 vec2_2 = this.formation.QuerySystem.AveragePosition - this.formation.QuerySystem.ClosestEnemyFormation.AveragePosition;
          double num1 = (double) vec2_2.Normalize();
          Vec2 currentVelocity = this.formation.QuerySystem.ClosestEnemyFormation.CurrentVelocity;
          float num2 = currentVelocity.Normalize();
          if (num1 < 30.0 && (double) num2 > 2.0 && (double) vec2_2.DotProduct(currentVelocity) > 0.5)
          {
            flag = true;
            WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
            if (this._reformPosition.IsValid)
            {
              medianPosition.SetVec2(this._reformPosition);
            }
            else
            {
              vec2_1 = this.formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this.formation.QuerySystem.AveragePosition;
              this._reformPosition = this.formation.QuerySystem.AveragePosition + vec2_1.Normalized() * 5f;
              medianPosition.SetVec2(this._reformPosition);
            }
            this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
          }
        }
        if (flag)
          return;
        this._reformPosition = Vec2.Invalid;
        WorldPosition position = this.formation.Team.QuerySystem.EnemyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (t => t.Team.FormationsIncludingSpecial)).Count<Formation>() == 1 ? this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition : this.formation.QuerySystem.Team.MedianTargetFormationPosition;
        vec2_1 = this.formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this.formation.QuerySystem.AveragePosition;
        Vec2 direction = vec2_1.Normalized();
        this.CurrentOrder = MovementOrder.MovementOrderMove(position);
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
      }
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this._isInShieldWallDistance = false;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this.formation.IsInfantry())
      {
        bool flag = false;
        if (this.formation.QuerySystem.ClosestEnemyFormation != null && this.formation.QuerySystem.IsUnderRangedAttack)
        {
          float num = this.formation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2);
          if ((double) num < 6400.0 + (this._isInShieldWallDistance ? 3600.0 : 0.0) && (double) num > 100.0 - (this._isInShieldWallDistance ? 75.0 : 0.0))
            flag = true;
        }
        if (flag != this._isInShieldWallDistance)
        {
          this._isInShieldWallDistance = flag;
          if (this._isInShieldWallDistance)
          {
            if (this.formation.QuerySystem.HasShield)
              this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
            else
              this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
            this._switchedToShieldWallRecently = true;
            this._switchedToShieldWallTimer.Reset(Mission.Current.Time, 5f);
          }
          else
            this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
        }
      }
      this.formation.MovementOrder = this.CurrentOrder;
    }

    protected override float GetAiWeight() => 1f;
  }
}
