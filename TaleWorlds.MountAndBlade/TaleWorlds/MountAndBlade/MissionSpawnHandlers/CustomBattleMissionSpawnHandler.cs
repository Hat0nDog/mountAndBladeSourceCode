// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionSpawnHandlers.CustomBattleMissionSpawnHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.MissionSpawnHandlers
{
  public class CustomBattleMissionSpawnHandler : MissionLogic
  {
    private MissionAgentSpawnLogic _missionAgentSpawnLogic;
    private CustomBattleCombatant _defenderParty;
    private CustomBattleCombatant _attackerParty;

    public CustomBattleMissionSpawnHandler()
    {
    }

    public CustomBattleMissionSpawnHandler(
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
      int defenderInitialSpawn = ofHealthyMembers1;
      int attackerInitialSpawn = ofHealthyMembers2;
      this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Defender, true);
      this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Attacker, true);
      this._missionAgentSpawnLogic.InitWithSinglePhase(ofHealthyMembers1, ofHealthyMembers2, defenderInitialSpawn, attackerInitialSpawn, true, true);
    }
  }
}
