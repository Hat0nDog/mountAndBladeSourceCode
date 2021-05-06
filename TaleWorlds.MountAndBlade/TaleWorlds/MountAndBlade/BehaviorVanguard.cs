// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorVanguard
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorVanguard : BehaviorComponent
  {
    private Formation _mainFormation;
    public FormationAI.BehaviorSide FlankSide = FormationAI.BehaviorSide.Middle;

    public BehaviorVanguard(Formation formation)
      : base(formation)
    {
      this.behaviorSide = formation.AI.Side;
      this._mainFormation = formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      this.CalculateCurrentOrder();
    }

    protected override void CalculateCurrentOrder()
    {
      Vec2 direction;
      WorldPosition medianPosition;
      if (this._mainFormation != null)
      {
        direction = this._mainFormation.Direction;
        Vec2 vec2 = (this.formation.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2).Normalized();
        medianPosition = this._mainFormation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this._mainFormation.CurrentPosition + vec2 * (float) (((double) this._mainFormation.Depth + (double) this.formation.Depth) * 0.5 + 10.0));
      }
      else
      {
        direction = this.formation.Direction;
        medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
      }
      this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
      this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
    }

    internal override void OnValidBehaviorSideSet()
    {
      base.OnValidBehaviorSideSet();
      this._mainFormation = this.formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
    }

    protected internal override void TickOccasionally()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation != null && (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.formation.QuerySystem.ClosestSignificantlyLargeEnemyFormation.MedianPosition.AsVec2) > 1600.0 && (double) this.formation.QuerySystem.UnderRangedAttackRatio > 0.200000002980232 - (this.formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Loose ? 0.100000001490116 : 0.0))
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
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
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    public override TextObject GetBehaviorString()
    {
      MBTextManager.SetTextVariable("SIDE_STRING", GameTexts.FindText("str_formation_ai_side_strings", this.formation.AI.Side.ToString()), false);
      string text;
      if (this._mainFormation != null)
        text = string.Join(" ", (object) GameTexts.FindText("str_formation_ai_side_strings", this._mainFormation.AI.Side.ToString()), (object) GameTexts.FindText("str_formation_class_string", this._mainFormation.PrimaryClass.GetName()));
      else
        text = "";
      MBTextManager.SetTextVariable("TARGET_FORMATION", text, false);
      return base.GetBehaviorString();
    }

    protected override float GetAiWeight()
    {
      if (this._mainFormation == null || !this._mainFormation.AI.IsMainFormation)
        this._mainFormation = this.formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      return this._mainFormation == null || this.formation.AI.IsMainFormation ? 0.0f : 1.2f;
    }
  }
}
