// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomGame
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public class CustomGame : TaleWorlds.Core.GameType
  {
    private List<CustomBattleSceneData> _customBattleScenes;
    private const TerrainType DefaultTerrain = TerrainType.Plain;
    private const ForestDensity DefaultForestDensity = ForestDensity.None;

    public IEnumerable<CustomBattleSceneData> CustomBattleScenes => (IEnumerable<CustomBattleSceneData>) this._customBattleScenes;

    public override bool IsCoreOnlyGameMode => true;

    public static CustomGame Current => Game.Current.GameType as CustomGame;

    public CustomGame() => this._customBattleScenes = new List<CustomBattleSceneData>();

    protected override void OnInitialize()
    {
      this.InitializeScenes();
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
      basicGameStarter.AddModel((GameModel) new CustomBattleAutoBlockModel());
      basicGameStarter.AddModel((GameModel) new MultiplayerAgentApplyDamageModel());
      basicGameStarter.AddModel((GameModel) new DefaultRidingModel());
      basicGameStarter.AddModel((GameModel) new DefaultStrikeMagnitudeModel());
      basicGameStarter.AddModel((GameModel) new CustomBattleSkillList());
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

    private void InitializeScenes() => this.LoadCustomBattleScenes(ModuleHelper.GetModuleFullPath("CustomBattle") + "ModuleData/custom_battle_scenes.xml");

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

    public void LoadCustomBattleScenes(string path) => this.LoadCustomBattleScenes(this.LoadXmlFile(path));

    private XmlDocument LoadXmlFile(string path)
    {
      Debug.Print("opening " + path);
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(new StreamReader(path).ReadToEnd());
      return xmlDocument;
    }

    private void LoadCustomBattleScenes(XmlDocument doc)
    {
      Debug.Print("loading custom_battle_scenes.xml");
      if (doc.ChildNodes.Count <= 1)
        throw new TWXmlLoadException("Incorrect XML document format. XML document must have at least 2 child nodes.");
      XmlNode childNode1 = doc.ChildNodes[1];
      if (childNode1.Name != "CustomBattleScenes")
        throw new TWXmlLoadException("Incorrect XML document format. Root node's name must be CustomBattleScenes.");
      if (!(childNode1.Name == "CustomBattleScenes"))
        return;
      foreach (XmlNode childNode2 in childNode1.ChildNodes)
      {
        if (childNode2.NodeType != XmlNodeType.Comment)
        {
          string sceneID = (string) null;
          TextObject name = (TextObject) null;
          TerrainType result1 = TerrainType.Plain;
          ForestDensity result2 = ForestDensity.None;
          bool isSiegeMap = false;
          bool isVillageMap = false;
          for (int i = 0; i < childNode2.Attributes.Count; ++i)
          {
            if (childNode2.Attributes[i].Name == "id")
              sceneID = childNode2.Attributes[i].InnerText;
            else if (childNode2.Attributes[i].Name == "name")
              name = new TextObject(childNode2.Attributes[i].InnerText);
            else if (childNode2.Attributes[i].Name == "terrain")
            {
              if (!Enum.TryParse<TerrainType>(childNode2.Attributes[i].InnerText, out result1))
                result1 = TerrainType.Plain;
            }
            else if (childNode2.Attributes[i].Name == "forest_density")
            {
              char[] charArray = childNode2.Attributes[i].InnerText.ToLower().ToCharArray();
              charArray[0] = char.ToUpper(charArray[0]);
              if (!Enum.TryParse<ForestDensity>(new string(charArray), out result2))
                result2 = ForestDensity.None;
            }
            else if (childNode2.Attributes[i].Name == "is_siege_map")
            {
              bool result3 = false;
              if (bool.TryParse(childNode2.Attributes[i].InnerText, out result3))
                isSiegeMap = result3;
            }
            else if (childNode2.Attributes[i].Name == "is_village_map")
            {
              bool result3 = false;
              if (bool.TryParse(childNode2.Attributes[i].InnerText, out result3))
                isVillageMap = result3;
            }
          }
          XmlNodeList childNodes = childNode2.ChildNodes;
          List<TerrainType> terrainTypes = new List<TerrainType>();
          foreach (XmlNode xmlNode in childNodes)
          {
            if (xmlNode.NodeType != XmlNodeType.Comment && xmlNode.Name == "flags")
            {
              foreach (XmlNode childNode3 in xmlNode.ChildNodes)
              {
                TerrainType result3;
                if (childNode3.NodeType != XmlNodeType.Comment && childNode3.Attributes["name"].InnerText == "TerrainType" && (Enum.TryParse<TerrainType>(childNode3.Attributes["value"].InnerText, out result3) && !terrainTypes.Contains(result3)))
                  terrainTypes.Add(result3);
              }
            }
          }
          this._customBattleScenes.Add(new CustomBattleSceneData(sceneID, name, result1, terrainTypes, result2, isSiegeMap, isVillageMap));
        }
      }
    }

    public override void OnStateChanged(GameState oldState)
    {
    }
  }
}
