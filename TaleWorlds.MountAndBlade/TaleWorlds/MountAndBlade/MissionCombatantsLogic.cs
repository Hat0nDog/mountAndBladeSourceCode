// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionCombatantsLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class MissionCombatantsLogic : MissionLogic
  {
    private readonly IEnumerable<IBattleCombatant> _battleCombatants;
    private readonly IBattleCombatant _playerBattleCombatant;
    private readonly IBattleCombatant _defenderLeaderBattleCombatant;
    private readonly IBattleCombatant _attackerLeaderBattleCombatant;
    private readonly Mission.MissionTeamAITypeEnum _teamAIType;
    private readonly bool _isPlayerSergeant;

    public BattleSideEnum PlayerSide
    {
      get
      {
        if (this._playerBattleCombatant == null)
          return BattleSideEnum.None;
        return this._playerBattleCombatant != this._defenderLeaderBattleCombatant ? BattleSideEnum.Attacker : BattleSideEnum.Defender;
      }
    }

    public MissionCombatantsLogic(
      IEnumerable<IBattleCombatant> battleCombatants,
      IBattleCombatant playerBattleCombatant,
      IBattleCombatant defenderLeaderBattleCombatant,
      IBattleCombatant attackerLeaderBattleCombatant,
      Mission.MissionTeamAITypeEnum teamAIType,
      bool isPlayerSergeant)
    {
      if (battleCombatants == null)
        battleCombatants = (IEnumerable<IBattleCombatant>) new IBattleCombatant[2]
        {
          defenderLeaderBattleCombatant,
          attackerLeaderBattleCombatant
        };
      this._battleCombatants = battleCombatants;
      this._playerBattleCombatant = playerBattleCombatant;
      this._defenderLeaderBattleCombatant = defenderLeaderBattleCombatant;
      this._attackerLeaderBattleCombatant = attackerLeaderBattleCombatant;
      this._teamAIType = teamAIType;
      this._isPlayerSergeant = isPlayerSergeant;
    }

    public Banner GetBannerForSide(BattleSideEnum side) => side != BattleSideEnum.Defender ? this._attackerLeaderBattleCombatant.Banner : this._defenderLeaderBattleCombatant.Banner;

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      if (!this.Mission.Teams.IsEmpty<Team>())
        throw new MBIllegalValueException("Number of teams is not 0.");
      if (this._playerBattleCombatant.Side == BattleSideEnum.Defender && this._playerBattleCombatant != this._defenderLeaderBattleCombatant)
        this.Mission.Teams.Add(BattleSideEnum.Defender, this._playerBattleCombatant.PrimaryColorPair.Item1, this._playerBattleCombatant.PrimaryColorPair.Item2, this._playerBattleCombatant.Banner);
      else
        this.Mission.Teams.Add(BattleSideEnum.Defender, this._defenderLeaderBattleCombatant.PrimaryColorPair.Item1, this._defenderLeaderBattleCombatant.PrimaryColorPair.Item2, this._defenderLeaderBattleCombatant.Banner);
      if (this._playerBattleCombatant.Side == BattleSideEnum.Attacker && this._playerBattleCombatant != this._attackerLeaderBattleCombatant)
        this.Mission.Teams.Add(BattleSideEnum.Attacker, this._playerBattleCombatant.PrimaryColorPair.Item1, this._playerBattleCombatant.PrimaryColorPair.Item2, this._playerBattleCombatant.Banner);
      else
        this.Mission.Teams.Add(BattleSideEnum.Attacker, this._attackerLeaderBattleCombatant.PrimaryColorPair.Item1, this._attackerLeaderBattleCombatant.PrimaryColorPair.Item2, this._attackerLeaderBattleCombatant.Banner);
      switch (this._playerBattleCombatant.Side)
      {
        case BattleSideEnum.Defender:
          this.Mission.PlayerTeam = this.Mission.DefenderTeam;
          if (this._battleCombatants == null)
            break;
          using (IEnumerator<IBattleCombatant> enumerator = this._battleCombatants.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IBattleCombatant current = enumerator.Current;
              if (current != this._playerBattleCombatant && current.Side == BattleSideEnum.Defender && !this._isPlayerSergeant)
              {
                this.Mission.Teams.Add(BattleSideEnum.Defender, current.PrimaryColorPair.Item1, current.PrimaryColorPair.Item2, current.Banner);
                break;
              }
            }
            break;
          }
        case BattleSideEnum.Attacker:
          this.Mission.PlayerTeam = this.Mission.AttackerTeam;
          if (this._battleCombatants == null)
            break;
          using (IEnumerator<IBattleCombatant> enumerator = this._battleCombatants.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              IBattleCombatant current = enumerator.Current;
              if (current != this._playerBattleCombatant && current.Side == BattleSideEnum.Attacker && !this._isPlayerSergeant)
              {
                this.Mission.Teams.Add(BattleSideEnum.Attacker, current.PrimaryColorPair.Item1, current.PrimaryColorPair.Item2, current.Banner);
                break;
              }
            }
            break;
          }
      }
    }

    public override void EarlyStart()
    {
      Mission.Current.MissionTeamAIType = this._teamAIType;
      switch (this._teamAIType)
      {
        case Mission.MissionTeamAITypeEnum.FieldBattle:
          using (IEnumerator<Team> enumerator = Mission.Current.Teams.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Team current = enumerator.Current;
              current.AddTeamAI((TeamAIComponent) new TeamAIGeneral(this.Mission, current));
            }
            break;
          }
        case Mission.MissionTeamAITypeEnum.Siege:
          using (IEnumerator<Team> enumerator = Mission.Current.Teams.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Team current = enumerator.Current;
              if (current.Side == BattleSideEnum.Attacker)
                current.AddTeamAI((TeamAIComponent) new TeamAISiegeAttacker(this.Mission, current, 5f, 1f));
              if (current.Side == BattleSideEnum.Defender)
                current.AddTeamAI((TeamAIComponent) new TeamAISiegeDefender(this.Mission, current, 5f, 1f));
            }
            break;
          }
        case Mission.MissionTeamAITypeEnum.SallyOut:
          using (IEnumerator<Team> enumerator = Mission.Current.Teams.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Team current = enumerator.Current;
              if (current.Side == BattleSideEnum.Attacker)
                current.AddTeamAI((TeamAIComponent) new TeamAISallyOutDefender(this.Mission, current, 5f, 1f));
              else
                current.AddTeamAI((TeamAIComponent) new TeamAISallyOutAttacker(this.Mission, current, 5f, 1f));
            }
            break;
          }
      }
      if (!Mission.Current.Teams.Any<Team>())
        return;
      switch (Mission.Current.MissionTeamAIType)
      {
        case Mission.MissionTeamAITypeEnum.NoTeamAI:
          using (IEnumerator<Team> enumerator = Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => t.HasTeamAi)).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Team current = enumerator.Current;
              current.AddTacticOption((TacticComponent) new TacticCharge(current));
            }
            break;
          }
        case Mission.MissionTeamAITypeEnum.FieldBattle:
          using (IEnumerator<Team> enumerator = Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => t.HasTeamAi)).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Team team = enumerator.Current;
              int num = this._battleCombatants.Where<IBattleCombatant>((Func<IBattleCombatant, bool>) (bc => bc.Side == team.Side)).Max<IBattleCombatant>((Func<IBattleCombatant, int>) (bcs => bcs.GetTacticsSkillAmount()));
              team.AddTacticOption((TacticComponent) new TacticCharge(team));
              if ((double) num >= 20.0)
              {
                team.AddTacticOption((TacticComponent) new TacticFullScaleAttack(team));
                if (team.Side == BattleSideEnum.Defender)
                {
                  team.AddTacticOption((TacticComponent) new TacticDefensiveEngagement(team));
                  team.AddTacticOption((TacticComponent) new TacticDefensiveLine(team));
                }
                if (team.Side == BattleSideEnum.Attacker)
                  team.AddTacticOption((TacticComponent) new TacticRangedHarrassmentOffensive(team));
              }
              if ((double) num >= 50.0)
              {
                team.AddTacticOption((TacticComponent) new TacticFrontalCavalryCharge(team));
                if (team.Side == BattleSideEnum.Defender)
                {
                  team.AddTacticOption((TacticComponent) new TacticDefensiveRing(team));
                  team.AddTacticOption((TacticComponent) new TacticHoldChokePoint(team));
                }
                if (team.Side == BattleSideEnum.Attacker)
                  team.AddTacticOption((TacticComponent) new TacticCoordinatedRetreat(team));
              }
            }
            break;
          }
        case Mission.MissionTeamAITypeEnum.Siege:
          using (IEnumerator<Team> enumerator = Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => t.HasTeamAi)).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Team current = enumerator.Current;
              if (current.Side == BattleSideEnum.Attacker)
                current.AddTacticOption((TacticComponent) new TacticBreachWalls(current));
              if (current.Side == BattleSideEnum.Defender)
                current.AddTacticOption((TacticComponent) new TacticDefendCastle(current));
            }
            break;
          }
        case Mission.MissionTeamAITypeEnum.SallyOut:
          using (IEnumerator<Team> enumerator = Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => t.HasTeamAi)).GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Team current = enumerator.Current;
              if (current.Side == BattleSideEnum.Defender)
                current.AddTacticOption((TacticComponent) new TacticSallyOutHitAndRun(current));
              if (current.Side == BattleSideEnum.Attacker)
                current.AddTacticOption((TacticComponent) new TacticSallyOutDefense(current));
              current.AddTacticOption((TacticComponent) new TacticCharge(current));
            }
            break;
          }
      }
      foreach (Team team in (ReadOnlyCollection<Team>) this.Mission.Teams)
      {
        team.QuerySystem.Expire();
        team.ResetTactic();
      }
    }

    public override void AfterStart() => this.Mission.SetMissionMode(MissionMode.Battle, true);

    public IEnumerable<IBattleCombatant> GetAllCombatants()
    {
      foreach (IBattleCombatant battleCombatant in this._battleCombatants)
        yield return battleCombatant;
    }
  }
}
