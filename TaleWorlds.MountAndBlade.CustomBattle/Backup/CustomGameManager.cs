// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomGameManager
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public class CustomGameManager : MBGameManager
  {
    protected override void DoLoadingForGameManager(
      GameManagerLoadingSteps gameManagerLoadingStep,
      out GameManagerLoadingSteps nextStep)
    {
      nextStep = GameManagerLoadingSteps.None;
      switch (gameManagerLoadingStep)
      {
        case GameManagerLoadingSteps.PreInitializeZerothStep:
          MBGameManager.LoadModuleData(false);
          MBGlobals.InitializeReferences();
          Game.CreateGame((TaleWorlds.Core.GameType) new CustomGame(), (GameManagerBase) this).DoLoading();
          nextStep = GameManagerLoadingSteps.FirstInitializeFirstStep;
          break;
        case GameManagerLoadingSteps.FirstInitializeFirstStep:
          bool flag = true;
          foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
            flag = flag && subModule.DoLoading(Game.Current);
          nextStep = flag ? GameManagerLoadingSteps.WaitSecondStep : GameManagerLoadingSteps.FirstInitializeFirstStep;
          break;
        case GameManagerLoadingSteps.WaitSecondStep:
          MBGameManager.StartNewGame();
          nextStep = GameManagerLoadingSteps.SecondInitializeThirdState;
          break;
        case GameManagerLoadingSteps.SecondInitializeThirdState:
          nextStep = Game.Current.DoLoading() ? GameManagerLoadingSteps.PostInitializeFourthState : GameManagerLoadingSteps.SecondInitializeThirdState;
          break;
        case GameManagerLoadingSteps.PostInitializeFourthState:
          nextStep = GameManagerLoadingSteps.FinishLoadingFifthStep;
          break;
        case GameManagerLoadingSteps.FinishLoadingFifthStep:
          nextStep = GameManagerLoadingSteps.None;
          break;
      }
    }

    public override void OnLoadFinished()
    {
      base.OnLoadFinished();
      Game.Current.GameStateManager.CleanAndPushState((GameState) Game.Current.GameStateManager.CreateState<CustomBattleState>());
    }
  }
}
