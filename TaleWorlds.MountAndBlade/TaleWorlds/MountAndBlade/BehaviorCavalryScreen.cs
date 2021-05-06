// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorCavalryScreen
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorCavalryScreen : BehaviorComponent
  {
    private Formation _mainFormation;
    private Formation _flankingEnemyCavalryFormation;
    private float _threatFormationCacheTime;
    private const float _threatFormationCacheExpireTime = 5f;

    public BehaviorCavalryScreen(Formation formation)
      : base(formation)
    {
      this.behaviorSide = formation.AI.Side;
      this._mainFormation = formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
      this.CalculateCurrentOrder();
    }

    internal override void OnValidBehaviorSideSet()
    {
      base.OnValidBehaviorSideSet();
      this._mainFormation = this.formation.Team.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation));
    }

    protected override void CalculateCurrentOrder()
    {
      if (this._mainFormation == null || this.formation.AI.IsMainFormation || this.formation.AI.Side != FormationAI.BehaviorSide.Left && this.formation.AI.Side != FormationAI.BehaviorSide.Right)
      {
        this._flankingEnemyCavalryFormation = (Formation) null;
        WorldPosition medianPosition = this.formation.QuerySystem.MedianPosition;
        medianPosition.SetVec2(this.formation.QuerySystem.AveragePosition);
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition);
      }
      else
      {
        float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
        if ((double) this._threatFormationCacheTime + 5.0 < (double) time)
        {
          this._threatFormationCacheTime = time;
          Vec2 vec2_1;
          Vec2 vec2_2;
          if (this.formation.AI.Side != FormationAI.BehaviorSide.Left)
          {
            vec2_1 = this._mainFormation.Direction;
            vec2_2 = vec2_1.RightVec();
          }
          else
          {
            vec2_1 = this._mainFormation.Direction;
            vec2_2 = vec2_1.LeftVec();
          }
          vec2_1 = vec2_2;
          Vec2 vec2_3 = vec2_1.Normalized();
          vec2_1 = this._mainFormation.Direction;
          Vec2 vec2_4 = vec2_1.Normalized();
          Vec2 threatDirection = vec2_3 - vec2_4;
          IEnumerable<Formation> source = Mission.Current.Teams.GetEnemiesOf(this.formation.Team).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).Where<Formation>((Func<Formation, bool>) (f =>
          {
            WorldPosition medianPosition = f.QuerySystem.MedianPosition;
            Vec2 asVec2_1 = medianPosition.AsVec2;
            medianPosition = this._mainFormation.QuerySystem.MedianPosition;
            Vec2 asVec2_2 = medianPosition.AsVec2;
            Vec2 vec2 = asVec2_1 - asVec2_2;
            return (double) threatDirection.Normalized().DotProduct(vec2.Normalized()) > Math.Cos(Math.PI / 8.0);
          }));
          this._flankingEnemyCavalryFormation = !source.Any<Formation>() ? (Formation) null : source.MaxBy<Formation, float>((Func<Formation, float>) (tef => tef.QuerySystem.FormationPower));
        }
        WorldPosition medianPosition1;
        if (this._flankingEnemyCavalryFormation == null)
        {
          medianPosition1 = this.formation.QuerySystem.MedianPosition;
          medianPosition1.SetVec2(this.formation.QuerySystem.AveragePosition);
        }
        else
        {
          Vec2 vec2 = this._flankingEnemyCavalryFormation.QuerySystem.MedianPosition.AsVec2 - this._mainFormation.QuerySystem.MedianPosition.AsVec2;
          float num = vec2.Normalize() * 0.5f;
          medianPosition1 = this._mainFormation.QuerySystem.MedianPosition;
          medianPosition1.SetVec2(medianPosition1.AsVec2 + num * vec2);
        }
        this.CurrentOrder = MovementOrder.MovementOrderMove(medianPosition1);
      }
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
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderSkein;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
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
      return this._flankingEnemyCavalryFormation == null ? 0.0f : 1.2f;
    }
  }
}
