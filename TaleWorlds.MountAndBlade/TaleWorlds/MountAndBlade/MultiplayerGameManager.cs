// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerGameManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerGameManager : MBGameManager
  {
    protected override void DoLoadingForGameManager(
      GameManagerLoadingSteps gameManagerLoadingStep,
      out GameManagerLoadingSteps nextStep)
    {
      nextStep = GameManagerLoadingSteps.None;
      switch (gameManagerLoadingStep)
      {
        case GameManagerLoadingSteps.PreInitializeZerothStep:
          nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
          break;
        case GameManagerLoadingSteps.FirstInitializeFirstStep:
          MBGameManager.LoadModuleData(false);
          MBDebug.Print("Game creating...");
          MBGlobals.InitializeReferences();
          Game.CreateGame((TaleWorlds.Core.GameType) new MultiplayerGame(), (GameManagerBase) this).DoLoading();
          nextStep = GameManagerLoadingSteps.WaitSecondStep;
          break;
        case GameManagerLoadingSteps.WaitSecondStep:
          MBGameManager.StartNewGame();
          nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
          break;
        case GameManagerLoadingSteps.SecondInitializeThirdState:
          nextStep = Game.Current.DoLoading() ? GameManagerLoadingSteps.PostInitializeFourthState : GameManagerLoadingSteps.SecondInitializeThirdState;
          break;
        case GameManagerLoadingSteps.PostInitializeFourthState:
          bool flag = true;
          foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
            flag = flag && subModule.DoLoading(Game.Current);
          nextStep = flag ? GameManagerLoadingSteps.FinishLoadingFifthStep : GameManagerLoadingSteps.PostInitializeFourthState;
          break;
        case GameManagerLoadingSteps.FinishLoadingFifthStep:
          nextStep = GameManagerLoadingSteps.None;
          break;
      }
    }

    public override void OnLoadFinished()
    {
      base.OnLoadFinished();
      MBGlobals.InitializeReferences();
      GameState state;
      if (GameNetwork.IsDedicatedServer)
      {
        int dedicatedServerType = (int) Module.CurrentModule.StartupInfo.DedicatedServerType;
        state = (GameState) Game.Current.GameStateManager.CreateState<UnspecifiedDedicatedServerState>();
      }
      else
        state = (GameState) Game.Current.GameStateManager.CreateState<LobbyState>();
      Game.Current.GameStateManager.CleanAndPushState(state);
    }

    public override void OnAfterCampaignStart(Game game)
    {
      if (GameNetwork.IsDedicatedServer)
        NetworkMain.InitializeAsDedicatedServer();
      else
        NetworkMain.Initialize();
    }

    public override void OnCampaignStart(Game game, object starterObject)
    {
      foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
        subModule.OnMultiplayerGameStart(game, starterObject);
    }

    public override void OnSessionInvitationAccepted(SessionInvitationType sessionInvitationType)
    {
      if (sessionInvitationType == SessionInvitationType.Multiplayer)
        return;
      base.OnSessionInvitationAccepted(sessionInvitationType);
    }
  }
}
