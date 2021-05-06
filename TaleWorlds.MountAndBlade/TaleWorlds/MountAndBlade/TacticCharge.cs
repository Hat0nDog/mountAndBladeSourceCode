// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticCharge
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TacticCharge : TacticComponent
  {
    public TacticCharge(Team team)
      : base(team)
    {
    }

    protected internal override void TickOccasionally()
    {
      foreach (Formation formation in this.Formations)
      {
        formation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(formation);
        formation.AI.SetBehaviorWeight<BehaviorCharge>(10000f);
      }
      base.TickOccasionally();
    }

    protected internal override void OnApply()
    {
      if (!this.team.IsPlayerTeam || !this.team.FormationsIncludingSpecial.Any<Formation>() || (!this.team.QuerySystem.EnemyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (t => t.Team.FormationsIncludingSpecial)).Any<Formation>() || this.team.IsPlayerGeneral || !this.team.IsPlayerSergeant))
        return;
      this.SoundTacticalHorn(TacticComponent.AttackHornSoundIndex);
    }

    internal override float GetTacticWeight()
    {
      float num1 = this.team.QuerySystem.OverallPowerRatio / this.team.QuerySystem.PowerRatioIncludingCasualties;
      float num2 = Math.Max(this.team.QuerySystem.InfantryRatio, Math.Max(this.team.QuerySystem.RangedRatio, this.team.QuerySystem.CavalryRatio)) + (this.team.Side == BattleSideEnum.Defender ? 0.33f : 0.0f);
      float num3 = this.team.Side == BattleSideEnum.Defender ? 0.33f : 0.5f;
      double num4 = (double) this.team.Formations.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.FormationPower)) + (double) this.team.QuerySystem.EnemyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (t => t.Team.Formations)).Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.FormationPower));
      CasualtyHandler casualtyHandler = Mission.Current.GetMissionBehaviour<CasualtyHandler>();
      float num5 = (float) num4 / ((float) num4 + (this.team.Formations.Sum<Formation>((Func<Formation, float>) (f => casualtyHandler.GetCasualtyPowerLossOfFormation(f))) + this.team.QuerySystem.EnemyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (t => t.Team.Formations)).Sum<Formation>((Func<Formation, float>) (f => casualtyHandler.GetCasualtyPowerLossOfFormation(f)))));
      return Math.Max(this.team.Side != BattleSideEnum.Attacker || (double) num1 >= 0.5 ? MBMath.LinearExtrapolation(0.0f, 1.6f * num2, (float) ((1.0 - (double) num5) / (1.0 - (double) num3))) : 0.0f, MBMath.LinearExtrapolation(0.0f, 1.6f * num2, (float) ((double) this.team.QuerySystem.OverallPowerRatio * (double) num3 * 0.5)));
    }
  }
}
