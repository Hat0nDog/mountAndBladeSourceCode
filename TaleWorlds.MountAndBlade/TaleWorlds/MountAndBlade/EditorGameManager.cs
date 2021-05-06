// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.EditorGameManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class EditorGameManager : MBGameManager
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
          Game.CreateGame((TaleWorlds.Core.GameType) new EditorGame(), (GameManagerBase) this).DoLoading();
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

    public override void OnLoadFinished() => base.OnLoadFinished();
  }
}
