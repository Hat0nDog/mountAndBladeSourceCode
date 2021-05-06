// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CPUBenchmarkMissionSpawnHandler
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public class CPUBenchmarkMissionSpawnHandler : MissionLogic
  {
    private MissionAgentSpawnLogic _missionAgentSpawnLogic;
    private CustomBattleCombatant _defenderParty;
    private CustomBattleCombatant _attackerParty;

    public CPUBenchmarkMissionSpawnHandler()
    {
    }

    public CPUBenchmarkMissionSpawnHandler(
      CustomBattleCombatant defenderParty,
      CustomBattleCombatant attackerParty)
    {
      this._defenderParty = defenderParty;
      this._attackerParty = attackerParty;
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._missionAgentSpawnLogic = this.Mission.GetMissionBehaviour<MissionAgentSpawnLogic>();
    }

    public override void AfterStart()
    {
      int ofHealthyMembers1 = this._defenderParty.NumberOfHealthyMembers;
      int ofHealthyMembers2 = this._attackerParty.NumberOfHealthyMembers;
      this.Mission.PlayerTeam.GetFormation(FormationClass.Cavalry).ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.Mission.PlayerTeam.GetFormation(FormationClass.Infantry).ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.Mission.PlayerEnemyTeam.GetFormation(FormationClass.Cavalry).ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.Mission.PlayerEnemyTeam.GetFormation(FormationClass.Infantry).ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Defender, true);
      this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Attacker, true);
      this._missionAgentSpawnLogic.InitWithSinglePhase(ofHealthyMembers1, ofHealthyMembers2, ofHealthyMembers1, ofHealthyMembers2, true, true);
    }
  }
}
