// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorSergeantMPLastFlagLastStand
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorSergeantMPLastFlagLastStand : BehaviorComponent
  {
    private List<FlagCapturePoint> _flagpositions;
    private bool _lastEffort;
    private MissionMultiplayerFlagDomination _flagDominationGameMode;

    public BehaviorSergeantMPLastFlagLastStand(Formation formation)
      : base(formation)
    {
      this._flagpositions = Mission.Current.ActiveMissionObjects.FindAllWithType<FlagCapturePoint>().ToList<FlagCapturePoint>();
      this._flagDominationGameMode = Mission.Current.GetMissionBehaviour<MissionMultiplayerFlagDomination>();
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      if (this._flagpositions.Any<FlagCapturePoint>())
        this.CurrentOrder = MovementOrder.MovementOrderMove(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, this._flagpositions.First<FlagCapturePoint>().Position, false));
      else
        this.CurrentOrder = MovementOrder.MovementOrderStop;
    }

    protected internal override void TickOccasionally()
    {
      this._flagpositions.RemoveAll((Predicate<FlagCapturePoint>) (fp => fp.IsDeactivated));
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this._flagpositions.RemoveAll((Predicate<FlagCapturePoint>) (fp => fp.IsDeactivated));
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected override float GetAiWeight()
    {
      if (this._lastEffort)
        return 10f;
      this._flagpositions.RemoveAll((Predicate<FlagCapturePoint>) (fp => fp.IsDeactivated));
      FlagCapturePoint flag = this._flagpositions.FirstOrDefault<FlagCapturePoint>();
      if (this._flagpositions.Count != 1 || this._flagDominationGameMode.GetFlagOwnerTeam(flag) == null || !this._flagDominationGameMode.GetFlagOwnerTeam(flag).IsEnemyOf(this.formation.Team))
        return 0.0f;
      float battleSideVictory = this._flagDominationGameMode.GetTimeUntilBattleSideVictory(this._flagDominationGameMode.GetFlagOwnerTeam(flag).Side);
      if ((double) battleSideVictory <= 60.0)
        return 10f;
      if ((double) this.formation.QuerySystem.AveragePosition.Distance(flag.Position.AsVec2) / (double) this.formation.QuerySystem.MovementSpeedMaximum * 8.0 <= (double) battleSideVictory)
        return 0.0f;
      this._lastEffort = true;
      return 10f;
    }
  }
}
