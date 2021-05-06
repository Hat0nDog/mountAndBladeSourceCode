// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerGame
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade.Diamond.MultiplayerBadges;
using TaleWorlds.MountAndBlade.Multiplayer;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerGame : TaleWorlds.Core.GameType
  {
    public override bool IsCoreOnlyGameMode => true;

    public static MultiplayerGame Current => Game.Current.GameType as MultiplayerGame;

    protected override void OnInitialize()
    {
      Game currentGame = this.CurrentGame;
      currentGame.FirstInitialize(false);
      if (!GameNetwork.IsDedicatedServer)
        this.AddGameTexts();
      IGameStarter gameStarter = (IGameStarter) new BasicGameStarter();
      this.AddGameModels(gameStarter);
      this.GameManager.OnGameStart(this.CurrentGame, gameStarter);
      currentGame.SecondInitialize(gameStarter.Models);
      currentGame.CreateGameManager();
      currentGame.ThirdInitialize();
      this.GameManager.BeginGameStart(this.CurrentGame);
      currentGame.CreateObjects();
      currentGame.InitializeDefaultGameObjects();
      currentGame.LoadBasicFiles(false);
      this.ObjectManager.LoadXML("Items");
      this.ObjectManager.LoadXML("MPCharacters");
      this.ObjectManager.LoadXML("BasicCultures");
      currentGame.CreateLists();
      this.ObjectManager.LoadXML("MPClassDivisions");
      this.ObjectManager.ClearEmptyObjects();
      MultiplayerClassDivisions.Initialize();
      BadgeManager.LoadFromXml(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/mpbadges.xml");
      this.GameManager.OnCampaignStart(this.CurrentGame, (object) null);
      this.GameManager.OnAfterCampaignStart(this.CurrentGame);
      this.GameManager.OnGameInitializationFinished(this.CurrentGame);
      this.CurrentGame.AddGameHandler<ChatBox>();
      if (!GameNetwork.IsDedicatedServer)
        return;
      this.CurrentGame.AddGameHandler<MultiplayerGameLogger>();
    }

    private void AddGameTexts()
    {
      this.CurrentGame.GameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/multiplayer_strings.xml");
      this.CurrentGame.GameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/global_strings.xml");
      this.CurrentGame.GameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/module_strings.xml");
      this.CurrentGame.GameTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/native_strings.xml");
    }

    private void AddGameModels(IGameStarter basicGameStarter)
    {
      basicGameStarter.AddModel((GameModel) new MultiplayerSkillList());
      basicGameStarter.AddModel((GameModel) new MultiplayerRidingModel());
      basicGameStarter.AddModel((GameModel) new MultiplayerStrikeMagnitudeModel());
      basicGameStarter.AddModel((GameModel) new MultiplayerAgentDecideKilledOrUnconsciousModel());
      basicGameStarter.AddModel((GameModel) new MultiplayerAgentStatCalculateModel());
      basicGameStarter.AddModel((GameModel) new MultiplayerAgentApplyDamageModel());
      basicGameStarter.AddModel((GameModel) new MultiplayerBattleMoraleModel());
      basicGameStarter.AddModel((GameModel) new MultiplayerBattleInitializationModel());
    }

    public static Dictionary<string, Equipment> ReadDefaultEquipments(
      string defaultEquipmentsPath)
    {
      Dictionary<string, Equipment> dictionary = new Dictionary<string, Equipment>();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(defaultEquipmentsPath);
      foreach (XmlNode childNode in xmlDocument.ChildNodes[0].ChildNodes)
      {
        if (childNode.NodeType == XmlNodeType.Element)
        {
          string key = childNode.Attributes["name"].Value;
          Equipment equipment = new Equipment(false);
          equipment.Deserialize((MBObjectManager) null, childNode);
          dictionary.Add(key, equipment);
        }
      }
      return dictionary;
    }

    protected override void BeforeRegisterTypes(MBObjectManager objectManager)
    {
    }

    protected override void OnRegisterTypes(MBObjectManager objectManager)
    {
      objectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "MPCharacters", 43U);
      objectManager.RegisterType<BasicCultureObject>("Culture", "BasicCultures", 17U);
      objectManager.RegisterType<MultiplayerClassDivisions.MPHeroClass>("MPClassDivision", "MPClassDivisions", 45U);
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
