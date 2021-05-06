// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Missions.SimpleMountedPlayerMissionController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
  public class SimpleMountedPlayerMissionController : MissionLogic
  {
    private Game _game;

    public SimpleMountedPlayerMissionController() => this._game = Game.Current;

    public override void AfterStart()
    {
      BasicCharacterObject characterObject = this._game.ObjectManager.GetObject<BasicCharacterObject>("aserai_tribal_horseman");
      GameEntity entityWithTag = Mission.Current.Scene.FindEntityWithTag("sp_play");
      MatrixFrame frame = entityWithTag != null ? entityWithTag.GetGlobalFrame() : MatrixFrame.Identity;
      AgentBuildData agentBuildData = new AgentBuildData(characterObject);
      agentBuildData.InitialFrame(frame).Controller(Agent.ControllerType.Player);
      this.Mission.SpawnAgent(agentBuildData).WieldInitialWeapons();
    }

    public override bool MissionEnded(ref MissionResult missionResult) => this.Mission.InputManager.IsGameKeyPressed(4);
  }
}
