// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Missions.EquipmentTestMissionController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
  public class EquipmentTestMissionController : MissionLogic
  {
    public override void AfterStart()
    {
      base.AfterStart();
      GameEntity entityWithTag = this.Mission.Scene.FindEntityWithTag("spawnpoint_player");
      this.Mission.SpawnAgent(new AgentBuildData(Game.Current.PlayerTroop).Team(this.Mission.AttackerTeam).InitialFrameFromSpawnPointEntity(entityWithTag).CivilianEquipment(false).Controller(Agent.ControllerType.Player));
    }
  }
}
