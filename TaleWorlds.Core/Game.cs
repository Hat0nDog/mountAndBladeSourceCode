// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.Game
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Library.EventSystem;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.SaveSystem.Save;

namespace TaleWorlds.Core
{
  [SaveableRootClass(5000)]
  public sealed class Game : IGameStateManagerOwner
  {
    private Game.State _currentState;
    private EntitySystem<GameHandler> _gameEntitySystem;
    private Monster _humanMonster;
    private Monster _horseMonster;
    private BasicGameModels _basicModels;
    private Dictionary<Type, GameModelsManager> _gameModelManagers;
    private static Game _current;
    private IBannerVisualCreator _bannerVisualCreator;
    [SaveableField(10)]
    private int _randomSeed;
    [SaveableField(11)]
    private int _nextUniqueTroopSeed = 1;
    private IReadOnlyDictionary<string, Equipment> _defaultEquipments;
    public Action<float> AfterTick;

    public Game.State CurrentState
    {
      get => this._currentState;
      private set => this._currentState = value;
    }

    public IMonsterMissionDataCreator MonsterMissionDataCreator { get; set; }

    public MBReadOnlyList<SkillObject> SkillList { get; set; }

    public MBReadOnlyList<ItemCategory> ItemCategoryList { get; set; }

    public Monster HumanMonster => this._humanMonster ?? (this._humanMonster = this.ObjectManager.GetObject<Monster>("human"));

    public Monster HorseMonster => this._horseMonster ?? (this._horseMonster = this.ObjectManager.GetObject<Monster>("horse"));

    [SaveableProperty(3)]
    public GameType GameType { get; private set; }

    [SaveableProperty(4)]
    public DefaultSiegeEngineTypes DefaultSiegeEngineTypes { get; private set; }

    [SaveableProperty(5)]
    public ItemObjectProperties ItemObjectProperties { get; private set; }

    [SaveableProperty(6)]
    public ObsoleteObjectManager ObsoleteObjectManager { get; private set; }

    public MBObjectManager ObjectManager { get; private set; }

    [SaveableProperty(7)]
    public BasicCharacterObject LastFaceEditedCharacter { get; set; }

    [SaveableProperty(8)]
    public BasicCharacterObject PlayerTroop { get; set; }

    public Random RandomGenerator { get; private set; }

    public BasicGameModels BasicModels => this._basicModels;

    public T AddGameModelsManager<T>(IEnumerable<GameModel> inputComponents) where T : GameModelsManager
    {
      T instance = (T) Activator.CreateInstance(typeof (T), (object) inputComponents);
      this._gameModelManagers.Add(typeof (T), (GameModelsManager) instance);
      return instance;
    }

    public GameManagerBase GameManager { get; private set; }

    public GameTextManager GameTextManager { get; private set; }

    public GameStateManager GameStateManager { get; private set; }

    internal Random DeterministicRandomGenerator { get; private set; }

    public bool CheatMode => this.GameManager.CheatMode;

    public bool IsDevelopmentMode => this.GameManager.IsDevelopmentMode;

    public bool IsEditModeOn => this.GameManager.IsEditModeOn;

    public bool DeterministicMode => this.GameManager.DeterministicMode;

    public float ApplicationTime => this.GameManager.ApplicationTime;

    public static Game Current
    {
      get => Game._current;
      internal set => Game._current = value;
    }

    public IBannerVisualCreator BannerVisualCreator
    {
      get => this._bannerVisualCreator;
      set => this._bannerVisualCreator = value;
    }

    public IBannerVisual CreateBannerVisual(Banner banner) => this.BannerVisualCreator == null ? (IBannerVisual) null : this.BannerVisualCreator.CreateBannerVisual(banner);

    public int NextUniqueTroopSeed => this._nextUniqueTroopSeed++;

    public CharacterAttributes CharacterAttributes { get; private set; }

    public DefaultSkills DefaultSkills { get; private set; }

    public DefaultItemCategories DefaultItemCategories { get; private set; }

    public DefaultItems DefaultItems { get; private set; }

    public EventManager EventManager { get; private set; }

    public Equipment GetDefaultEquipmentWithName(string equipmentName) => !this._defaultEquipments.ContainsKey(equipmentName) ? (Equipment) null : this._defaultEquipments[equipmentName].Clone();

    public void SetDefaultEquipments(
      IReadOnlyDictionary<string, Equipment> defaultEquipments)
    {
      if (this._defaultEquipments != null)
        return;
      this._defaultEquipments = defaultEquipments;
    }

    private Game(GameType gameType, GameManagerBase gameManager, MBObjectManager objectManager)
    {
      this.GameType = gameType;
      Game.Current = this;
      this.GameType.CurrentGame = this;
      this.GameManager = gameManager;
      this.GameManager.Game = this;
      this.EventManager = new EventManager();
      this.ObjectManager = objectManager;
      this.InitializeParameters();
    }

    public static Game CreateGame(GameType gameType, GameManagerBase gameManager)
    {
      MBObjectManager objectManager = MBObjectManager.Init();
      Game.RegisterTypes(gameType, objectManager);
      return new Game(gameType, gameManager, objectManager);
    }

    public static Game LoadSaveGame(LoadResult loadResult, GameManagerBase gameManager)
    {
      MBObjectManager objectManager = MBObjectManager.Init();
      Game root = (Game) loadResult.Root;
      Game.RegisterTypes(root.GameType, objectManager);
      ObsoleteObjectManager.Instance = root.ObsoleteObjectManager;
      loadResult.InitializeObjects();
      MBObjectManager.Instance.ReInitialize();
      GC.Collect();
      GC.WaitForPendingFinalizers();
      root.ObjectManager = objectManager;
      root.ObsoleteObjectManager = (ObsoleteObjectManager) null;
      ObsoleteObjectManager.Instance = (ObsoleteObjectManager) null;
      root.BeginLoading(gameManager);
      return root;
    }

    private void BeginLoading(GameManagerBase gameManager)
    {
      Game.Current = this;
      this.GameType.CurrentGame = this;
      this.GameManager = gameManager;
      this.GameManager.Game = this;
      this.EventManager = new EventManager();
      this.InitializeParameters();
    }

    private bool SaveAux(MetaData metaData, ISaveDriver driver)
    {
      foreach (GameHandler component in (IEnumerable<GameHandler>) this._gameEntitySystem.Components)
        component.OnBeforeSave();
      SaveOutput saveOutput = SaveManager.Save((object) this, metaData, driver);
      saveOutput.PrintStatus();
      foreach (GameHandler component in (IEnumerable<GameHandler>) this._gameEntitySystem.Components)
        component.OnAfterSave();
      return saveOutput.Successful;
    }

    public bool Save(MetaData metaData, ISaveDriver driver)
    {
      int num = this.SaveAux(metaData, driver) ? 1 : 0;
      Common.MemoryCleanup();
      return num != 0;
    }

    private void InitializeParameters()
    {
      ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Native", "managed_core_parameters"));
      this.GameType.InitializeParameters();
    }

    void IGameStateManagerOwner.OnStateStackEmpty() => this.Destroy();

    public void Destroy()
    {
      this.CurrentState = Game.State.Destroying;
      foreach (GameHandler component in (IEnumerable<GameHandler>) this._gameEntitySystem.Components)
        component.OnGameEnd();
      this.GameManager.OnGameEnd(this);
      this.GameType.OnDestroy();
      this.ObjectManager.Destroy();
      this.EventManager.Clear();
      this.EventManager = (EventManager) null;
      GameStateManager.Current = (GameStateManager) null;
      this.GameStateManager = (GameStateManager) null;
      Game.Current = (Game) null;
      this.CurrentState = Game.State.Destroyed;
      Common.MemoryCleanup();
    }

    public void CreateGameManager() => this.GameStateManager = new GameStateManager((IGameStateManagerOwner) this, GameStateManager.GameStateManagerType.Game);

    public void SetLoadingParameters(int randomSeed) => this._randomSeed = this.DeterministicMode ? 45153 : randomSeed;

    public void OnStateChanged(GameState oldState) => this.GameType.OnStateChanged(oldState);

    public T AddGameHandler<T>() where T : GameHandler, new() => this._gameEntitySystem.AddComponent<T>();

    public T GetGameHandler<T>() where T : GameHandler => this._gameEntitySystem.GetComponent<T>();

    public void RemoveGameHandler<T>() where T : GameHandler => this._gameEntitySystem.RemoveComponent<T>();

    public void FirstInitialize(bool isSavedGame) => this.ObjectManager.LoadWithValidation = !isSavedGame;

    public void ThirdInitialize()
    {
      this.RandomGenerator = new Random(this._randomSeed);
      this.DeterministicRandomGenerator = new Random(10000);
    }

    public void Initialize()
    {
      if (this._gameEntitySystem == null)
        this._gameEntitySystem = new EntitySystem<GameHandler>();
      this.GameTextManager = new GameTextManager();
      this._gameModelManagers = new Dictionary<Type, GameModelsManager>();
      GameTexts.Initialize(this.GameTextManager);
      this.GameType.OnInitialize();
    }

    public static void RegisterTypes(GameType gameType, MBObjectManager objectManager)
    {
      gameType?.BeforeRegisterTypes(objectManager);
      objectManager.RegisterNonSerializedType<Monster>("Monster", "Monsters", 2U);
      objectManager.RegisterNonSerializedType<SkeletonScale>("Scale", "Scales", 3U);
      objectManager.RegisterType<ItemObject>("Item", "Items", 4U);
      objectManager.RegisterType<ItemComponent>("ItemComponent", "ItemComponents", 5U);
      objectManager.RegisterType<ItemModifier>("ItemModifier", "ItemModifiers", 6U);
      objectManager.RegisterType<ItemModifierGroup>("ItemModifierGroup", "ItemModifierGroups", 7U);
      objectManager.RegisterType<CharacterAttribute>("CharacterAttribute", "CharacterAttributes", 8U);
      objectManager.RegisterType<SkillObject>("Skill", "Skills", 9U);
      objectManager.RegisterType<ItemCategory>("ItemCategory", "ItemCategories", 10U);
      objectManager.RegisterType<CraftingPiece>("CraftingPiece", "CraftingPieces", 11U);
      objectManager.RegisterType<CraftingTemplate>("CraftingTemplate", "CraftingTemplates", 12U);
      objectManager.RegisterType<SiegeEngineType>("SiegeEngineType", "SiegeEngineTypes", 13U);
      objectManager.RegisterType<MBBodyProperty>("BodyProperty", "BodyProperties", 50U);
      objectManager.RegisterNonSerializedType<MBEquipmentRoster>("EquipmentRoster", "EquipmentRosters", 51U);
      objectManager.RegisterNonSerializedType<MBCharacterSkills>("SkillSet", "SkillSets", 52U);
      gameType?.OnRegisterTypes(objectManager);
    }

    public void SecondInitialize(IEnumerable<GameModel> models) => this._basicModels = this.AddGameModelsManager<BasicGameModels>(models);

    internal void OnTick(float dt)
    {
      if (GameStateManager.Current == this.GameStateManager)
      {
        this.GameStateManager.OnTick(dt);
        if (this._gameEntitySystem != null)
        {
          foreach (GameHandler component in (IEnumerable<GameHandler>) this._gameEntitySystem.Components)
          {
            try
            {
              component.OnTick();
            }
            catch (Exception ex)
            {
              Debug.Print("Exception on gameHandler tick: " + (object) ex);
            }
          }
        }
      }
      Action<float> afterTick = this.AfterTick;
      if (afterTick == null)
        return;
      afterTick(dt);
    }

    internal void OnGameNetworkBegin()
    {
      foreach (GameHandler component in (IEnumerable<GameHandler>) this._gameEntitySystem.Components)
        component.OnGameNetworkBegin();
    }

    internal void OnGameNetworkEnd()
    {
      foreach (GameHandler component in (IEnumerable<GameHandler>) this._gameEntitySystem.Components)
        component.OnGameNetworkEnd();
    }

    internal void OnEarlyPlayerConnect(VirtualPlayer peer)
    {
      foreach (GameHandler component in (IEnumerable<GameHandler>) this._gameEntitySystem.Components)
        component.OnEarlyPlayerConnect(peer);
    }

    internal void OnPlayerConnect(VirtualPlayer peer)
    {
      foreach (GameHandler component in (IEnumerable<GameHandler>) this._gameEntitySystem.Components)
        component.OnPlayerConnect(peer);
    }

    internal void OnPlayerDisconnect(VirtualPlayer peer)
    {
      foreach (GameHandler component in (IEnumerable<GameHandler>) this._gameEntitySystem.Components)
        component.OnPlayerDisconnect(peer);
    }

    public void OnGameStart()
    {
      foreach (GameHandler component in (IEnumerable<GameHandler>) this._gameEntitySystem.Components)
        component.OnGameStart();
    }

    public bool DoLoading() => this.GameType.DoLoadingForGameType();

    public void ResetRandomGenerator(int randomSeed)
    {
      this._randomSeed = randomSeed;
      this.RandomGenerator = new Random(this._randomSeed);
    }

    public void OnMissionIsStarting(string missionName, MissionInitializerRecord rec) => this.GameType.OnMissionIsStarting(missionName, rec);

    public void OnFinalize()
    {
      this.CurrentState = Game.State.Destroying;
      GameStateManager.Current.CleanStates();
    }

    public void CreateLists()
    {
      this.SkillList = this.BasicModels.SkillList.GetSkillList().ToList<SkillObject>().GetReadOnlyList<SkillObject>();
      this.ItemCategoryList = this.ObjectManager.GetObjectTypeList<ItemCategory>();
    }

    public void CreateObjects() => this.ItemObjectProperties = new ItemObjectProperties(this);

    public void InitializeDefaultGameObjects()
    {
      this.CharacterAttributes = new CharacterAttributes();
      this.DefaultSkills = new DefaultSkills(this);
      this.DefaultItemCategories = new DefaultItemCategories(this);
      this.DefaultSiegeEngineTypes = new DefaultSiegeEngineTypes();
    }

    public void InitializeRegisteredXmlObjects() => this.ObjectManager.LoadXML("partyTemplates");

    public void LoadBasicFiles(bool isLoad)
    {
      this.ObjectManager.LoadXML("Monsters");
      this.ObjectManager.LoadXML("SkeletonScales");
      this.ObjectManager.LoadXML("ItemModifiers");
      this.ObjectManager.LoadXML("ItemModifierGroups");
      this.ObjectManager.LoadXML("CraftingPieces");
      this.ObjectManager.LoadXML("CraftingTemplates");
      this.ObjectManager.LoadXML("BodyProperties");
      this.ObjectManager.LoadXML("SkillSets");
    }

    public void LoadCampaignItems()
    {
      this.ObjectManager.LoadXML("Items");
      this.ObjectManager.LoadXML("EquipmentRosters");
    }

    public void InitializeDefaultItems() => this.DefaultItems = new DefaultItems(this);

    public enum State
    {
      Running,
      Destroying,
      Destroyed,
    }
  }
}
