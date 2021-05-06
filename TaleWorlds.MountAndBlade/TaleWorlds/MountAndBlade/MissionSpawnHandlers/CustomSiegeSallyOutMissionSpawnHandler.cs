// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionSpawnHandlers.CustomSiegeSallyOutMissionSpawnHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
  public class CustomSiegeSallyOutMissionSpawnHandler : MissionLogic
  {
    private const int MinNumberOfReinforcementTroops = 5;
    private const float BesiegerReinforcementTimerInSeconds = 10f;
    private const float BesiegerReinforcementNumberPercentage = 0.07f;
    private const float BesiegerInitialSpawnNumberPercentage = 0.1f;
    private MissionAgentSpawnLogic _missionAgentSpawnLogic;
    private CustomBattleCombatant[] _battleCombatants;
    private float _dtSum;

    public CustomSiegeSallyOutMissionSpawnHandler(
      IBattleCombatant defenderBattleCombatant,
      IBattleCombatant attackerBattleCombatant)
    {
      this._battleCombatants = new CustomBattleCombatant[2]
      {
        (CustomBattleCombatant) defenderBattleCombatant,
        (CustomBattleCombatant) attackerBattleCombatant
      };
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._missionAgentSpawnLogic = this.Mission.GetMissionBehaviour<MissionAgentSpawnLogic>();
    }

    public override void AfterStart()
    {
      int ofHealthyMembers1 = this._battleCombatants[0].NumberOfHealthyMembers;
      int ofHealthyMembers2 = this._battleCombatants[1].NumberOfHealthyMembers;
      int defenderInitialSpawn = MBMath.Floor((float) ofHealthyMembers1);
      int attackerInitialSpawn = MBMath.Floor((float) ofHealthyMembers2 * 0.1f);
      this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Defender, true);
      this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Attacker, true);
      this._missionAgentSpawnLogic.InitWithSinglePhase(ofHealthyMembers1, ofHealthyMembers2, defenderInitialSpawn, attackerInitialSpawn, false, false);
      this._missionAgentSpawnLogic.ReserveReinforcement(BattleSideEnum.Attacker, ofHealthyMembers2 - attackerInitialSpawn);
    }

    public override void OnMissionTick(float dt)
    {
      if (!this.CheckTimer(dt))
        return;
      this._missionAgentSpawnLogic.CheckReinforcement(Math.Max(MBMath.Floor((float) this._battleCombatants[1].NumberOfHealthyMembers * 0.07f), 5));
    }

    private bool CheckTimer(float dt)
    {
      this._dtSum += dt;
      if ((double) this._dtSum < 10.0)
        return false;
      this._dtSum = 0.0f;
      return true;
    }
  }
}
