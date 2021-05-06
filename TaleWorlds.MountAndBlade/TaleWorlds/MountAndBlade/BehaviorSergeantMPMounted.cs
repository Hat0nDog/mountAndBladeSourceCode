// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorSergeantMPMounted
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
  public class BehaviorSergeantMPMounted : BehaviorComponent
  {
    private List<FlagCapturePoint> _flagpositions;
    private MissionMultiplayerFlagDomination _flagDominationGameMode;

    public BehaviorSergeantMPMounted(Formation formation)
      : base(formation)
    {
      this._flagpositions = Mission.Current.ActiveMissionObjects.FindAllWithType<FlagCapturePoint>().ToList<FlagCapturePoint>();
      this._flagDominationGameMode = Mission.Current.GetMissionBehaviour<MissionMultiplayerFlagDomination>();
      this.CalculateCurrentOrder();
    }

    private MovementOrder UncapturedFlagMoveOrder()
    {
      if (this._flagpositions.Any<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != this.formation.Team)))
      {
        FlagCapturePoint flagCapturePoint = this._flagpositions.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != this.formation.Team)).MinBy<FlagCapturePoint, float>((Func<FlagCapturePoint, float>) (fp => this.formation.Team.QuerySystem.GetLocalEnemyPower(fp.Position.AsVec2)));
        return MovementOrder.MovementOrderMove(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, flagCapturePoint.Position, false));
      }
      if (!this._flagpositions.Any<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == this.formation.Team)))
        return MovementOrder.MovementOrderStop;
      Vec3 position = this._flagpositions.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) == this.formation.Team)).MinBy<FlagCapturePoint, float>((Func<FlagCapturePoint, float>) (fp => fp.Position.AsVec2.DistanceSquared(this.formation.QuerySystem.AveragePosition))).Position;
      return MovementOrder.MovementOrderMove(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, position, false));
    }

    protected override void CalculateCurrentOrder()
    {
      if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation == null || (double) this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2.DistanceSquared(this.formation.QuerySystem.AveragePosition) > 2500.0)
      {
        this.CurrentOrder = this.UncapturedFlagMoveOrder();
      }
      else
      {
        FlagCapturePoint flagCapturePoint = (FlagCapturePoint) null;
        if (this._flagpositions.Any<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != this.formation.Team && !fp.IsContested)))
          flagCapturePoint = this._flagpositions.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (fp => this._flagDominationGameMode.GetFlagOwnerTeam(fp) != this.formation.Team && !fp.IsContested)).MinBy<FlagCapturePoint, float>((Func<FlagCapturePoint, float>) (fp => this.formation.QuerySystem.AveragePosition.DistanceSquared(fp.Position.AsVec2)));
        if ((!this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.IsRangedFormation ? 0 : ((double) this.formation.QuerySystem.FormationPower / (double) this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.FormationPower / (double) this.formation.Team.QuerySystem.OverallPowerRatio > 0.699999988079071 ? 1 : 0)) == 0 && flagCapturePoint != null)
          this.CurrentOrder = MovementOrder.MovementOrderMove(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, flagCapturePoint.Position, false));
        else
          this.CurrentOrder = MovementOrder.MovementOrderChargeToTarget(this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.Formation);
      }
    }

    protected internal override void TickOccasionally()
    {
      this._flagpositions.RemoveAll((Predicate<FlagCapturePoint>) (fp => fp.IsDeactivated));
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

    protected override float GetAiWeight() => this.formation.QuerySystem.IsCavalryFormation ? 1.2f : 0.0f;
  }
}
