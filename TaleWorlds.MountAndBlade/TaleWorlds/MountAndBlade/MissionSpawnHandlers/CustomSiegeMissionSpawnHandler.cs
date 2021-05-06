// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionSpawnHandlers.CustomSiegeMissionSpawnHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
  public class CustomSiegeMissionSpawnHandler : MissionLogic
  {
    private MissionAgentSpawnLogic _missionAgentSpawnLogic;
    private CustomBattleCombatant[] _battleCombatants;

    public CustomSiegeMissionSpawnHandler(
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
      int defenderInitialSpawn = ofHealthyMembers1;
      int attackerInitialSpawn = ofHealthyMembers2;
      this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Defender, false);
      this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Attacker, false);
      this._missionAgentSpawnLogic.InitWithSinglePhase(ofHealthyMembers1, ofHealthyMembers2, defenderInitialSpawn, attackerInitialSpawn, false, false);
    }
  }
}
