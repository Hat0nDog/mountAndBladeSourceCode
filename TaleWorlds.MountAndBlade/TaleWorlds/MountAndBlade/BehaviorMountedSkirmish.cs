// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorMountedSkirmish
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorMountedSkirmish : BehaviorComponent
  {
    private bool _engaging = true;

    public BehaviorMountedSkirmish(Formation formation)
      : base(formation)
    {
      this.CalculateCurrentOrder();
      this.BehaviorCoherence = 0.5f;
    }

    protected override void CalculateCurrentOrder()
    {
      WorldPosition position = this.formation.QuerySystem.MedianPosition;
      if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null)
      {
        position.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      else
      {
        int num1 = (double) (this.formation.QuerySystem.AverageAllyPosition - this.formation.Team.QuerySystem.AverageEnemyPosition).LengthSquared <= 3600.0 ? 1 : 0;
        bool engaging = this._engaging;
        this._engaging = num1 != 0 || (this._engaging ? (double) this.formation.QuerySystem.UnderRangedAttackRatio <= (double) this.formation.QuerySystem.MakingRangedAttackRatio && (!this.formation.QuerySystem.FastestSignificantlyLargeEnemyFormation.IsCavalryFormation && !this.formation.QuerySystem.FastestSignificantlyLargeEnemyFormation.IsRangedCavalryFormation || (double) (this.formation.QuerySystem.AveragePosition - this.formation.QuerySystem.FastestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2).LengthSquared / ((double) this.formation.QuerySystem.FastestSignificantlyLargeEnemyFormation.MovementSpeed * (double) this.formation.QuerySystem.FastestSignificantlyLargeEnemyFormation.MovementSpeed) >= 16.0) : (double) (this.formation.QuerySystem.AveragePosition - this.formation.QuerySystem.AverageAllyPosition).LengthSquared <= 3600.0);
        if (this._engaging)
        {
          Vec2 vec2_1 = this.formation.QuerySystem.AveragePosition - this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.AveragePosition;
          vec2_1 = vec2_1.Normalized();
          Vec2 vec2_2 = vec2_1.LeftVec();
          FormationQuerySystem largeEnemyFormation = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation;
          float num2 = (float) (50.0 + ((double) this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation.Width + (double) this.formation.Depth) * 0.5);
          float num3 = 0.0f;
          Formation formation1 = this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation;
          for (int index1 = 0; index1 < Mission.Current.Teams.Count; ++index1)
          {
            Team team = Mission.Current.Teams[index1];
            if (team.IsEnemyOf(this.formation.Team))
            {
              for (int index2 = 0; index2 < team.FormationsIncludingSpecialAndEmpty.Count; ++index2)
              {
                Formation formation2 = team.FormationsIncludingSpecialAndEmpty[index2];
                if (formation2.CountOfUnits > 0 && formation2.QuerySystem != largeEnemyFormation)
                {
                  Vec2 v = formation2.QuerySystem.AveragePosition - largeEnemyFormation.AveragePosition;
                  float num4 = v.Normalize();
                  if ((double) vec2_2.DotProduct(v) > 0.800000011920929 && (double) num4 < (double) num2 && (double) num4 > (double) num3)
                  {
                    num3 = num4;
                    formation1 = formation2;
                  }
                }
              }
            }
          }
          if (((double) this.formation.QuerySystem.RangedCavalryUnitRatio <= 0.949999988079071 ? 0 : (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation == formation1 ? 1 : 0)) == 0)
          {
            bool flag = formation1.QuerySystem.IsCavalryFormation || formation1.QuerySystem.IsRangedCavalryFormation;
            float radius = Math.Min((flag ? 35f : 20f) + (float) (((double) formation1.Depth + (double) this.formation.Width) * 0.5), this.formation.QuerySystem.MissileRange - this.formation.Width * 0.5f);
            BehaviorMountedSkirmish.Ellipse ellipse = new BehaviorMountedSkirmish.Ellipse(formation1.QuerySystem.MedianPosition.AsVec2, radius, (float) ((double) formation1.Width * 0.5 * (flag ? 1.5 : 1.0)), formation1.Direction);
            position.SetVec2(ellipse.GetTargetPos(this.formation.QuerySystem.AveragePosition, 20f));
          }
          else
          {
            this.CurrentOrder = MovementOrder.MovementOrderCharge;
            return;
          }
        }
        else
          position = new WorldPosition(Mission.Current.Scene, new Vec3(this.formation.QuerySystem.AverageAllyPosition, this.formation.Team.QuerySystem.MedianPosition.GetNavMeshZ() + 100f));
      }
      this.CurrentOrder = MovementOrder.MovementOrderMove(position);
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight() => 1f;

    private struct Ellipse
    {
      private readonly Vec2 _center;
      private readonly float _radius;
      private readonly float _halfLength;
      private readonly Vec2 _direction;

      public Ellipse(Vec2 center, float radius, float halfLength, Vec2 direction)
      {
        this._center = center;
        this._radius = radius;
        this._halfLength = halfLength;
        this._direction = direction;
      }

      public Vec2 GetTargetPos(Vec2 position, float distance)
      {
        Vec2 v1 = this._direction.LeftVec();
        Vec2 vec2_1 = this._center + v1 * this._halfLength;
        Vec2 vec2_2 = this._center - v1 * this._halfLength;
        Vec2 vec2_3 = position - this._center;
        bool flag1 = (double) vec2_3.Normalized().DotProduct(this._direction) > 0.0;
        Vec2 vec2_4 = vec2_3.DotProduct(v1) * v1;
        bool flag2 = (double) vec2_4.Length < (double) this._halfLength;
        bool flag3 = true;
        Vec2 vec2_5;
        if (flag2)
        {
          position = this._center + vec2_4 + this._direction * (this._radius * (flag1 ? 1f : -1f));
        }
        else
        {
          flag3 = (double) vec2_4.DotProduct(v1) > 0.0;
          vec2_5 = position - (flag3 ? vec2_1 : vec2_2);
          Vec2 vec2_6 = vec2_5.Normalized();
          position = (flag3 ? vec2_1 : vec2_2) + vec2_6 * this._radius;
        }
        Vec2 vec2_7 = this._center + vec2_4;
        double num1 = 2.0 * Math.PI * (double) this._radius;
        while ((double) distance > 0.0)
        {
          if (flag2 & flag1)
          {
            vec2_5 = vec2_1 - vec2_7;
            double num2;
            if ((double) vec2_5.Length >= (double) distance)
            {
              num2 = (double) distance;
            }
            else
            {
              vec2_5 = vec2_1 - vec2_7;
              num2 = (double) vec2_5.Length;
            }
            float num3 = (float) num2;
            Vec2 vec2_6 = vec2_7;
            vec2_5 = vec2_1 - vec2_7;
            Vec2 vec2_8 = vec2_5.Normalized() * num3;
            position = vec2_6 + vec2_8;
            position += this._direction * this._radius;
            distance -= num3;
            flag2 = false;
            flag3 = true;
          }
          else if (!flag2 & flag3)
          {
            vec2_5 = position - vec2_1;
            Vec2 v2 = vec2_5.Normalized();
            vec2_5 = this._direction;
            double num2 = Math.Acos((double) MBMath.ClampFloat(vec2_5.DotProduct(v2), -1f, 1f));
            double num3 = 2.0 * Math.PI * ((double) distance / num1);
            double num4 = num2 + num3 < Math.PI ? num2 + num3 : Math.PI;
            double num5 = (num4 - num2) / Math.PI * (num1 / 2.0);
            Vec2 direction = this._direction;
            direction.RotateCCW((float) num4);
            position = vec2_1 + direction * this._radius;
            distance -= (float) num5;
            flag2 = true;
            flag1 = false;
          }
          else if (flag2)
          {
            vec2_5 = vec2_2 - vec2_7;
            double num2;
            if ((double) vec2_5.Length >= (double) distance)
            {
              num2 = (double) distance;
            }
            else
            {
              vec2_5 = vec2_2 - vec2_7;
              num2 = (double) vec2_5.Length;
            }
            float num3 = (float) num2;
            Vec2 vec2_6 = vec2_7;
            vec2_5 = vec2_2 - vec2_7;
            Vec2 vec2_8 = vec2_5.Normalized() * num3;
            position = vec2_6 + vec2_8;
            position -= this._direction * this._radius;
            distance -= num3;
            flag2 = false;
            flag3 = false;
          }
          else
          {
            vec2_5 = position - vec2_2;
            Vec2 v2 = vec2_5.Normalized();
            vec2_5 = this._direction;
            double num2 = Math.Acos((double) MBMath.ClampFloat(vec2_5.DotProduct(v2), -1f, 1f));
            double num3 = 2.0 * Math.PI * ((double) distance / num1);
            double num4 = num2 - num3 > 0.0 ? num2 - num3 : 0.0;
            double num5 = num2 - num4;
            double num6 = num5 / Math.PI * (num1 / 2.0);
            Vec2 vec2_6 = v2;
            vec2_6.RotateCCW((float) num5);
            position = vec2_2 + vec2_6 * this._radius;
            distance -= (float) num6;
            flag2 = true;
            flag1 = true;
          }
        }
        return position;
      }
    }
  }
}
