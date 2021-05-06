// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CreateBodyguardMissionBehavior
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class CreateBodyguardMissionBehavior : MissionLogic
  {
    private readonly string _attackerGeneralName;
    private readonly string _defenderGeneralName;
    private readonly string _attackerAllyGeneralName;
    private readonly string _defenderAllyGeneralName;
    private readonly bool _createBodyguard;

    public CreateBodyguardMissionBehavior(
      string attackerGeneralName,
      string defenderGeneralName,
      string attackerAllyGeneralName = null,
      string defenderAllyGeneralName = null,
      bool createBodyguard = true)
    {
      this._attackerGeneralName = attackerGeneralName;
      this._defenderGeneralName = defenderGeneralName;
      this._attackerAllyGeneralName = attackerAllyGeneralName;
      this._defenderAllyGeneralName = defenderAllyGeneralName;
      this._createBodyguard = createBodyguard;
    }

    private void CreateBodyGuardFormation(Team team)
    {
      Agent generalAgent = (Agent) null;
      Formation formation1 = (Formation) null;
      Formation formation2 = (Formation) null;
      List<IFormationUnit> list1 = team.Formations.SelectMany<Formation, IFormationUnit>((Func<Formation, IEnumerable<IFormationUnit>>) (f => (IEnumerable<IFormationUnit>) f.UnitsWithoutLooseDetachedOnes)).ToList<IFormationUnit>();
      if (team.IsPlayerTeam && team.IsPlayerGeneral)
      {
        generalAgent = Mission.Current.MainAgent;
      }
      else
      {
        string generalName = team == this.Mission.AttackerTeam ? this._attackerGeneralName : (team == this.Mission.DefenderTeam ? this._defenderGeneralName : (team == this.Mission.AttackerAllyTeam ? this._attackerAllyGeneralName : (team == this.Mission.DefenderAllyTeam ? this._defenderAllyGeneralName : (string) null)));
        if (generalName != null && list1.Count<IFormationUnit>((Func<IFormationUnit, bool>) (ta => ((Agent) ta).Character != null && ((Agent) ta).Character.GetName().Equals(generalName))) == 1)
          generalAgent = (Agent) list1.First<IFormationUnit>((Func<IFormationUnit, bool>) (ta => ((Agent) ta).Character != null && ((Agent) ta).Character.GetName().Equals(generalName)));
        else if (list1.Any<IFormationUnit>((Func<IFormationUnit, bool>) (u => !((Agent) u).IsMainAgent && ((Agent) u).IsHero)))
          generalAgent = (Agent) list1.Where<IFormationUnit>((Func<IFormationUnit, bool>) (u => !((Agent) u).IsMainAgent && ((Agent) u).IsHero)).MaxBy<IFormationUnit, float>((Func<IFormationUnit, float>) (u => ((Agent) u).CharPowerCached));
      }
      team.GeneralAgent = generalAgent;
      if (generalAgent != null && team.QuerySystem.MemberCount >= 50)
      {
        formation1 = team.GetFormation(FormationClass.NumberOfRegularFormations);
        formation1.MovementOrder = MovementOrder.MovementOrderMove(generalAgent.GetWorldPosition());
        formation1.IsAIControlled = true;
        Formation formation3 = generalAgent.Formation;
        generalAgent.Formation = formation1;
        team.TriggerOnFormationsChanged(formation1);
        formation1.QuerySystem.Expire();
        team.GeneralsFormation = formation1;
        if (this._createBodyguard && generalAgent.IsAIControlled)
        {
          list1.Remove((IFormationUnit) generalAgent);
          List<IFormationUnit> list2 = list1.Where<IFormationUnit>((Func<IFormationUnit, bool>) (u =>
          {
            if (((Agent) u).Character != null && ((Agent) u).Character.IsHero)
              return false;
            return generalAgent.MountAgent == null ? !((Agent) u).HasMount : ((Agent) u).HasMount;
          })).ToList<IFormationUnit>();
          int count = Math.Min((int) ((double) list2.Count<IFormationUnit>() / 10.0), 20);
          if (count != 0)
          {
            formation2 = team.GetFormation(FormationClass.Bodyguard);
            formation2.MovementOrder = MovementOrder.MovementOrderMove(generalAgent.GetWorldPosition());
            formation2.IsAIControlled = true;
            List<IFormationUnit> list3 = list2.OrderByDescending<IFormationUnit, float>((Func<IFormationUnit, float>) (u => ((Agent) u).CharPowerCached)).Take<IFormationUnit>(count).ToList<IFormationUnit>();
            IEnumerable<Formation> formations = list3.Select<IFormationUnit, Formation>((Func<IFormationUnit, Formation>) (bu => ((Agent) bu).Formation)).Distinct<Formation>();
            foreach (IFormationUnit formationUnit1 in list3)
            {
              IFormationUnit formationUnit2;
              Formation formation4 = ((Agent) (formationUnit2 = formationUnit1)).Formation;
              ((Agent) formationUnit2).Formation = formation2;
            }
            foreach (Formation formation4 in formations)
            {
              team.TriggerOnFormationsChanged(formation4);
              formation4.QuerySystem.Expire();
            }
            team.TriggerOnFormationsChanged(formation2);
            formation2.QuerySystem.Expire();
          }
        }
        TacticComponent.SetDefaultBehaviorWeights(formation1);
        formation1.AI.SetBehaviorWeight<BehaviorGeneral>(1f);
        formation1.PlayerOwner = (Agent) null;
        if (formation2 != null)
        {
          TacticComponent.SetDefaultBehaviorWeights(formation2);
          formation2.AI.SetBehaviorWeight<BehaviorProtectGeneral>(1f);
          formation2.PlayerOwner = (Agent) null;
        }
      }
      team.GeneralAgent = generalAgent;
      team.GeneralsFormation = formation1;
      team.BodyGuardFormation = formation2;
    }

    public override void OnFormationUnitsSpawned(Team team)
    {
      base.OnFormationUnitsSpawned(team);
      this.CreateBodyGuardFormation(team);
    }
  }
}
