// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.EditorGame
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class EditorGame : TaleWorlds.Core.GameType
  {
    public static EditorGame Current => Game.Current.GameType as EditorGame;

    protected override void OnInitialize()
    {
      Game currentGame = this.CurrentGame;
      currentGame.FirstInitialize(false);
      this.InitializeGameTexts(currentGame.GameTextManager);
      IGameStarter gameStarter = (IGameStarter) new BasicGameStarter();
      this.InitializeGameModels(gameStarter);
      this.GameManager.OnGameStart(this.CurrentGame, gameStarter);
      MBObjectManager objectManager = currentGame.ObjectManager;
      currentGame.SecondInitialize(gameStarter.Models);
      currentGame.CreateGameManager();
      this.GameManager.BeginGameStart(this.CurrentGame);
      this.CurrentGame.ThirdInitialize();
      currentGame.CreateObjects();
      currentGame.InitializeDefaultGameObjects();
      currentGame.LoadBasicFiles(false);
      this.LoadCustomGameXmls();
      objectManager.ClearEmptyObjects();
      currentGame.SetDefaultEquipments((IReadOnlyDictionary<string, Equipment>) new Dictionary<string, Equipment>());
      currentGame.CreateLists();
      objectManager.ClearEmptyObjects();
      this.GameManager.OnCampaignStart(this.CurrentGame, (object) null);
      this.GameManager.OnAfterCampaignStart(this.CurrentGame);
      this.GameManager.OnGameInitializationFinished(this.CurrentGame);
    }

    private void InitializeGameModels(IGameStarter basicGameStarter)
    {
      basicGameStarter.AddModel((GameModel) new MultiplayerAgentDecideKilledOrUnconsciousModel());
      basicGameStarter.AddModel((GameModel) new CustomBattleAgentStatCalculateModel());
      basicGameStarter.AddModel((GameModel) new CustomBattleApplyWeatherEffectsModel());
      basicGameStarter.AddModel((GameModel) new MultiplayerAgentApplyDamageModel());
      basicGameStarter.AddModel((GameModel) new DefaultRidingModel());
      basicGameStarter.AddModel((GameModel) new DefaultStrikeMagnitudeModel());
      basicGameStarter.AddModel((GameModel) new EditorGameSkillList());
      basicGameStarter.AddModel((GameModel) new CustomBattleMoraleModel());
      basicGameStarter.AddModel((GameModel) new CustomBattleInitializationModel());
    }

    private void InitializeGameTexts(GameTextManager gameTextManager)
    {
      gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/multiplayer_strings.xml");
      gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/global_strings.xml");
      gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/module_strings.xml");
      gameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/native_strings.xml");
    }

    private void LoadCustomGameXmls()
    {
      this.ObjectManager.LoadXML("Items");
      this.ObjectManager.LoadXML("EquipmentRosters");
      this.ObjectManager.LoadXML("NPCCharacters");
      this.ObjectManager.LoadXML("SPCultures");
    }

    protected override void BeforeRegisterTypes(MBObjectManager objectManager)
    {
    }

    protected override void OnRegisterTypes(MBObjectManager objectManager)
    {
      objectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "NPCCharacters", 43U);
      objectManager.RegisterType<BasicCultureObject>("Culture", "SPCultures", 17U);
    }

    protected override void DoLoadingForGameType(
      GameTypeLoadingStates gameTypeLoadingState,
      out GameTypeLoadingStates nextState)
    {
      nextState = GameTypeLoadingStates.None;
      switch (gameTypeLoadingState)
      {
        case GameTypeLoadingStates.InitializeFirstStep:
          this.CurrentGame.Initialize();
          nextState = GameTypeLoadingStates.WaitSecondStep;
          break;
        case GameTypeLoadingStates.WaitSecondStep:
          nextState = GameTypeLoadingStates.LoadVisualsThirdState;
          break;
        case GameTypeLoadingStates.LoadVisualsThirdState:
          nextState = GameTypeLoadingStates.PostInitializeFourthState;
          break;
      }
    }

    public override void OnDestroy()
    {
    }

    public override void OnStateChanged(GameState oldState)
    {
    }
  }
}
