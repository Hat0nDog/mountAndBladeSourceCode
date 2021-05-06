// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattleViews
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.LegacyGUI.Missions;
using TaleWorlds.MountAndBlade.Missions.Handlers;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  [ViewCreatorModule]
  public class CustomBattleViews
  {
    [ViewMethod("CustomBattle")]
    public static MissionView[] OpenCustomBattleMission(Mission mission)
    {
      List<MissionView> missionViewList = new List<MissionView>();
      missionViewList.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu());
      missionViewList.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
      missionViewList.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardVM) new CustomBattleScoreboardVM()));
      missionViewList.Add(ViewCreator.CreateOptionsUIHandler());
      missionViewList.Add(ViewCreator.CreateMissionOrderUIHandler((Mission) null));
      missionViewList.Add((MissionView) new OrderTroopPlacer());
      missionViewList.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
      missionViewList.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
      missionViewList.Add(ViewCreator.CreateMissionMainAgentCheerControllerView(mission));
      missionViewList.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
      missionViewList.Add((MissionView) new MusicBattleMissionView(false));
      missionViewList.Add(ViewCreator.CreateMissionBoundaryCrossingView());
      missionViewList.Add((MissionView) new MissionBoundaryWallView());
      missionViewList.Add(ViewCreator.CreateMissionFormationMarkerUIHandler(mission));
      missionViewList.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
      missionViewList.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
      missionViewList.Add(ViewCreator.CreatePhotoModeView());
      missionViewList.Add((MissionView) new MissionAgentContourControllerView());
      missionViewList.Add((MissionView) new MissionCustomBattlePreloadView());
      return missionViewList.ToArray();
    }

    [ViewMethod("CustomSiegeBattle")]
    public static MissionView[] OpenCustomSiegeBattleMission(Mission mission)
    {
      List<MissionView> missionViewList = new List<MissionView>();
      mission.GetMissionBehaviour<SiegeDeploymentHandler>();
      missionViewList.Add(ViewCreator.CreateMissionSingleplayerEscapeMenu());
      missionViewList.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
      missionViewList.Add(ViewCreator.CreateMissionBattleScoreUIHandler(mission, (ScoreboardVM) new CustomBattleScoreboardVM()));
      missionViewList.Add(ViewCreator.CreateOptionsUIHandler());
      MissionView missionOrderUiHandler = ViewCreator.CreateMissionOrderUIHandler((Mission) null);
      missionViewList.Add(missionOrderUiHandler);
      missionViewList.Add((MissionView) new OrderTroopPlacer());
      missionViewList.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
      missionViewList.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
      missionViewList.Add(ViewCreator.CreateMissionMainAgentCheerControllerView(mission));
      missionViewList.Add(ViewCreator.CreateMissionAgentLockVisualizerView(mission));
      missionViewList.Add((MissionView) new MusicBattleMissionView(true));
      missionViewList.Add((MissionView) new SiegeMissionView());
      ISiegeDeploymentView isiegeDeploymentView = missionOrderUiHandler as ISiegeDeploymentView;
      missionViewList.Add((MissionView) new MissionEntitySelectionUIHandler(new Action<GameEntity>(isiegeDeploymentView.OnEntitySelection), new Action<GameEntity>(isiegeDeploymentView.OnEntityHover)));
      missionViewList.Add(ViewCreator.CreateMissionBoundaryCrossingView());
      missionViewList.Add(ViewCreator.CreateSingleplayerMissionKillNotificationUIHandler());
      missionViewList.Add((MissionView) new MissionBoundaryMarker((IEntityFactory) new FlagFactory("swallowtail_banner"), 2f));
      missionViewList.Add(ViewCreator.CreateMissionFormationMarkerUIHandler(mission));
      missionViewList.Add(ViewCreator.CreateMissionSpectatorControlView(mission));
      missionViewList.Add(ViewCreator.CreatePhotoModeView());
      missionViewList.Add((MissionView) new SiegeDeploymentVisualizationMissionView());
      missionViewList.Add((MissionView) new MissionAgentContourControllerView());
      missionViewList.Add((MissionView) new MissionCustomBattlePreloadView());
      return missionViewList.ToArray();
    }
  }
}
