// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Module
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TaleWorlds.AchievementSystem;
using TaleWorlds.Core;
using TaleWorlds.Diamond.ClientApplication;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlatformService;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.MountAndBlade
{
  public sealed class Module : DotNetObject, IGameStateManagerOwner
  {
    private TestContext _testContext;
    private List<MissionInfo> _missionInfos;
    private Dictionary<string, System.Type> _loadedSubmoduleTypes;
    private List<MBSubModuleBase> _submodules;
    private SingleThreadedSynchronizationContext _synchronizationContext;
    private bool _splashScreenPlayed;
    private List<InitialStateOption> _initialStateOptions;
    private IEditorMissionTester _editorMissionTester;
    private Dictionary<string, MultiplayerGameMode> _multiplayerGameModesWithNames;
    private List<MultiplayerGameTypeInfo> _multiplayerGameTypes = new List<MultiplayerGameTypeInfo>();
    private bool _isShuttingDown;

    public GameTextManager GlobalTextManager { get; private set; }

    public JobManager JobManager { get; private set; }

    public IEnumerable<MBSubModuleBase> SubModules => (IEnumerable<MBSubModuleBase>) this._submodules.AsReadOnly();

    public GameStateManager GlobalGameStateManager { get; private set; }

    public bool ReturnToEditorState { get; private set; }

    public bool LoadingFinished { get; private set; }

    public GameStartupInfo StartupInfo { get; private set; }

    private Module()
    {
      MBDebug.Print("Creating module...");
      this.StartupInfo = new GameStartupInfo();
      this._testContext = new TestContext();
      this._loadedSubmoduleTypes = new Dictionary<string, System.Type>();
      this._submodules = new List<MBSubModuleBase>();
      this.GlobalGameStateManager = new GameStateManager((IGameStateManagerOwner) this, GameStateManager.GameStateManagerType.Global);
      GameStateManager.Current = this.GlobalGameStateManager;
      this.GlobalTextManager = new GameTextManager();
      this.JobManager = new JobManager();
    }

    public static Module CurrentModule { get; private set; }

    internal static void CreateModule()
    {
      Module.CurrentModule = new Module();
      string[] modulesNames = Utilities.GetModulesNames();
      List<string> stringList = new List<string>();
      foreach (string moduleId in modulesNames)
      {
        ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo(moduleId);
        stringList.Add(moduleInfo.GetFolderPath());
      }
      LocalizedTextManager.LoadLocalizationXmls(stringList.ToArray());
    }

    private void AddSubModule(Assembly subModuleAssembly, string name)
    {
      System.Type type = subModuleAssembly.GetType(name);
      this._loadedSubmoduleTypes.Add(name, type);
      Managed.AddTypes(this.CollectModuleAssemblyTypes(subModuleAssembly));
    }

    private Dictionary<string, System.Type> CollectModuleAssemblyTypes(
      Assembly moduleAssembly)
    {
      try
      {
        Dictionary<string, System.Type> dictionary = new Dictionary<string, System.Type>();
        foreach (System.Type type in moduleAssembly.GetTypes())
        {
          if (typeof (ManagedObject).IsAssignableFrom(type) || typeof (DotNetObject).IsAssignableFrom(type))
            dictionary.Add(type.Name, type);
        }
        return dictionary;
      }
      catch (Exception ex)
      {
        MBDebug.Print("Error while getting types and loading" + ex.Message);
        if (ex is ReflectionTypeLoadException typeLoadException1)
        {
          string customString = "";
          foreach (Exception loaderException in typeLoadException1.LoaderExceptions)
          {
            MBDebug.Print("Loader Exceptions: " + loaderException.Message);
            customString = customString + loaderException.Message + Environment.NewLine;
          }
          Debug.SetCrashReportCustomString(customString);
          foreach (System.Type type in typeLoadException1.Types)
          {
            if (type != (System.Type) null)
              MBDebug.Print("Loaded Types: " + type.FullName);
          }
        }
        if (ex.InnerException != null)
          MBDebug.Print("Inner excetion: " + ex.StackTrace);
        throw;
      }
    }

    private void InitializeSubModules()
    {
      foreach (System.Type type in this._loadedSubmoduleTypes.Values)
      {
        MBSubModuleBase mbSubModuleBase = (MBSubModuleBase) type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder) null, new System.Type[0], (ParameterModifier[]) null).Invoke(new object[0]);
        this._submodules.Add(mbSubModuleBase);
        mbSubModuleBase.OnSubModuleLoad();
      }
    }

    private void FinalizeSubModules()
    {
      foreach (MBSubModuleBase submodule in this._submodules)
        submodule.OnSubModuleUnloaded();
    }

    public System.Type GetSubModule(string name) => this._loadedSubmoduleTypes[name];

    [MBCallback]
    internal void Initialize()
    {
      MBDebug.Print("Module Initialize begin...");
      MBSaveLoad.SetSaveDriver((ISaveDriver) new MBAsyncSaveDriver());
      this.ProcessApplicationArguments();
      this.SetWindowTitle();
      this._initialStateOptions = new List<InitialStateOption>();
      this.FillMultiplayerGameTypes();
      if (!GameNetwork.IsDedicatedServer && !MBDebug.TestModeEnabled)
        this.LoadPlatformServices();
      this.GlobalTextManager.LoadGameTexts(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/global_strings.xml");
      this.LoadSubModules();
      MBDebug.Print("Adding trace listener...");
      MBDebug.Print("MBModuleBase Initialize begin...");
      MBDebug.Print("MBModuleBase Initialize end...");
      GameNetwork.FindGameNetworkMessages();
      HasTableauCache.CollectTableauCacheTypes();
      MBDebug.Print("Module Initialize end...");
      MBDebug.TestModeEnabled = Utilities.CommandLineArgumentExists("/runTest");
      this.FindMissions();
      BannerlordConfig.Initialize();
      EngineManaged.ConfigChange += new Action(this.OnConfigChanged);
      SaveManager.InitializeGlobalDefinitionContext();
      this.EnsureAsyncJobsAreFinished();
    }

    private void EnsureAsyncJobsAreFinished()
    {
      if (!GameNetwork.IsDedicatedServer)
      {
        while (!MBMusicManager.isCreationCompleted())
          Thread.Sleep(1);
      }
      if (GameNetwork.IsDedicatedServer || MBDebug.TestModeEnabled)
        return;
      while (!AchievementManager.AchievementService.IsInitializationCompleted())
        Thread.Sleep(1);
    }

    private void SetWindowTitle()
    {
      string applicationName = Utilities.GetApplicationName();
      string title;
      if (this.StartupInfo.StartupType == GameStartupType.Singleplayer)
        title = applicationName + " - Singleplayer";
      else if (this.StartupInfo.StartupType == GameStartupType.Multiplayer)
        title = applicationName + " - Multiplayer";
      else if (this.StartupInfo.StartupType == GameStartupType.GameServer)
        title = "[" + (object) Utilities.GetCurrentProcessID() + "] " + applicationName + " Dedicated Server Port:" + (object) this.StartupInfo.ServerPort;
      else
        title = applicationName;
      Utilities.SetWindowTitle(Utilities.ProcessWindowTitle(title));
    }

    private void ProcessApplicationArguments()
    {
      this.StartupInfo.StartupType = GameStartupType.None;
      string[] strArray = Utilities.GetFullCommandLineString().Split(' ');
      for (int index = 0; index < strArray.Length; ++index)
      {
        string lowerInvariant = strArray[index].ToLowerInvariant();
        if (lowerInvariant == "/dedicatedmatchmakingserver".ToLower())
        {
          int int32 = Convert.ToInt32(strArray[index + 1]);
          string str1 = strArray[index + 2];
          sbyte num = Convert.ToSByte(strArray[index + 3]);
          string str2 = strArray[index + 4];
          index += 4;
          this.StartupInfo.StartupType = GameStartupType.GameServer;
          this.StartupInfo.DedicatedServerType = DedicatedServerType.Matchmaker;
          this.StartupInfo.ServerPort = int32;
          this.StartupInfo.ServerRegion = str1;
          this.StartupInfo.ServerPriority = num;
          this.StartupInfo.ServerGameMode = str2;
        }
        else if (lowerInvariant == "/dedicatedcustomserver".ToLower())
        {
          int int32_1 = Convert.ToInt32(strArray[index + 1]);
          string str = strArray[index + 2];
          int int32_2 = Convert.ToInt32(strArray[index + 3]);
          index += 3;
          this.StartupInfo.StartupType = GameStartupType.GameServer;
          this.StartupInfo.DedicatedServerType = DedicatedServerType.Custom;
          this.StartupInfo.ServerPort = int32_1;
          this.StartupInfo.ServerRegion = str;
          this.StartupInfo.Permission = int32_2;
        }
        else if (lowerInvariant == "/dedicatedcustomserverconfigfile".ToLower())
        {
          string str = strArray[index + 1];
          ++index;
          this.StartupInfo.CustomGameServerConfigFile = str;
        }
        else if (lowerInvariant == "/singleplayer".ToLower())
          this.StartupInfo.StartupType = GameStartupType.Singleplayer;
        else if (lowerInvariant == "/multiplayer".ToLower())
          this.StartupInfo.StartupType = GameStartupType.Multiplayer;
        else if (lowerInvariant == "/anticheat".ToLower())
          this.StartupInfo.IsStartedWithAntiCheat = true;
        else if (lowerInvariant == "/clientConfigurationCategory".ToLower())
        {
          ClientApplicationConfiguration.SetDefualtConfigurationCategory(strArray[index + 1]);
          ++index;
        }
        else if (lowerInvariant == "/overridenusername".ToLower())
        {
          this.StartupInfo.OverridenUserName = strArray[index + 1];
          ++index;
        }
        else if (lowerInvariant.StartsWith("-AUTH_PASSWORD".ToLowerInvariant()))
          this.StartupInfo.EpicExchangeCode = lowerInvariant.Split('=')[1];
      }
    }

    internal void OnApplicationTick(float dt)
    {
      if (this._synchronizationContext == null)
      {
        this._synchronizationContext = new SingleThreadedSynchronizationContext();
        SynchronizationContext.SetSynchronizationContext((SynchronizationContext) this._synchronizationContext);
      }
      this._testContext.OnApplicationTick(dt);
      if (!GameNetwork.MultiplayerDisabled)
        this.OnNetworkTick(dt);
      if (GameStateManager.Current == null)
        GameStateManager.Current = this.GlobalGameStateManager;
      if (GameStateManager.Current == this.GlobalGameStateManager)
      {
        if (this.LoadingFinished && this.GlobalGameStateManager.ActiveState == null)
        {
          if (this.ReturnToEditorState)
          {
            this.ReturnToEditorState = false;
            this.SetEditorScreenAsRootScreen();
          }
          else
            this.SetInitialModuleScreenAsRootScreen();
        }
        this.GlobalGameStateManager.OnTick(dt);
      }
      Utilities.RunJobs();
      PlatformServices.Instance?.Tick(dt);
      this._synchronizationContext.Tick();
      if (GameManagerBase.Current != null)
        GameManagerBase.Current.OnTick(dt);
      foreach (MBSubModuleBase subModule in this.SubModules)
        subModule.OnApplicationTick(dt);
      this.JobManager.OnTick(dt);
    }

    private void OnNetworkTick(float dt) => NetworkMain.Tick(dt);

    [MBCallback]
    internal void RunTest(string commandLine)
    {
      MBDebug.Print(" TEST MODE ENABLED. Command line string: " + commandLine);
      this._testContext.RunTestAux(commandLine);
    }

    [MBCallback]
    internal void TickTest(float dt) => this._testContext.TickTest(dt);

    [MBCallback]
    internal void OnDumpCreated()
    {
      if (TestCommonBase.BaseInstance == null)
        return;
      TestCommonBase.BaseInstance.ToggleTimeoutTimer();
      TestCommonBase.BaseInstance.StartTimeoutTimer();
    }

    [MBCallback]
    internal void OnDumpCreationStarted()
    {
      if (TestCommonBase.BaseInstance == null)
        return;
      TestCommonBase.BaseInstance.ToggleTimeoutTimer();
    }

    public static void GetMetaMeshPackageMapping(Dictionary<string, string> metaMeshPackageMappings)
    {
      foreach (ItemObject objectType in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
      {
        if (objectType.HasArmorComponent)
        {
          string str = (objectType.Culture != null ? objectType.Culture.StringId : "shared") + "_armor";
          metaMeshPackageMappings[objectType.MultiMeshName] = str;
          metaMeshPackageMappings[objectType.MultiMeshName + "_converted"] = str;
          metaMeshPackageMappings[objectType.MultiMeshName + "_converted_slim"] = str;
          metaMeshPackageMappings[objectType.MultiMeshName + "_slim"] = str;
        }
        if (objectType.WeaponComponent != null)
        {
          string str = (objectType.Culture != null ? objectType.Culture.StringId : "shared") + "_weapon";
          metaMeshPackageMappings[objectType.MultiMeshName] = str;
          if (objectType.HolsterMeshName != null)
            metaMeshPackageMappings[objectType.HolsterMeshName] = str;
          if (objectType.HolsterWithWeaponMeshName != null)
            metaMeshPackageMappings[objectType.HolsterWithWeaponMeshName] = str;
        }
        if (objectType.HasHorseComponent)
        {
          string str = "horses";
          metaMeshPackageMappings[objectType.MultiMeshName] = str;
        }
        if (objectType.IsFood)
        {
          string str = "food";
          metaMeshPackageMappings[objectType.MultiMeshName] = str;
        }
      }
      foreach (CraftingPiece objectType in Game.Current.ObjectManager.GetObjectTypeList<CraftingPiece>())
      {
        string str = (objectType.Culture != null ? objectType.Culture.StringId : "shared") + "_crafting";
        metaMeshPackageMappings[objectType.MeshName] = str;
      }
    }

    public static void GetItemMeshNames(HashSet<string> itemMeshNames)
    {
      foreach (ItemObject objectType in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
      {
        if (!objectType.IsCraftedWeapon)
          itemMeshNames.Add(objectType.MultiMeshName);
        if (objectType.PrimaryWeapon != null)
        {
          if (objectType.FlyingMeshName != null && !objectType.FlyingMeshName.IsEmpty<char>())
            itemMeshNames.Add(objectType.FlyingMeshName);
          if (objectType.HolsterMeshName != null && !objectType.HolsterMeshName.IsEmpty<char>())
            itemMeshNames.Add(objectType.HolsterMeshName);
          if (objectType.HolsterWithWeaponMeshName != null && !objectType.HolsterWithWeaponMeshName.IsEmpty<char>())
            itemMeshNames.Add(objectType.HolsterWithWeaponMeshName);
        }
        if (objectType.HasHorseComponent)
        {
          foreach (KeyValuePair<string, bool> additionalMeshesName in objectType.HorseComponent.AdditionalMeshesNameList)
          {
            if (additionalMeshesName.Key != null && !additionalMeshesName.Key.IsEmpty<char>())
              itemMeshNames.Add(additionalMeshesName.Key);
          }
        }
      }
    }

    [MBCallback]
    internal string GetMetaMeshPackageMapping()
    {
      Dictionary<string, string> metaMeshPackageMappings = new Dictionary<string, string>();
      Module.GetMetaMeshPackageMapping(metaMeshPackageMappings);
      string str = "";
      foreach (string key in metaMeshPackageMappings.Keys)
        str = str + key + "|" + metaMeshPackageMappings[key] + ",";
      return str;
    }

    [MBCallback]
    internal string GetItemMeshNames()
    {
      HashSet<string> itemMeshNames = new HashSet<string>();
      Module.GetItemMeshNames(itemMeshNames);
      foreach (CraftingPiece objectType in MBObjectManager.Instance.GetObjectTypeList<CraftingPiece>())
      {
        itemMeshNames.Add(objectType.MeshName);
        if (objectType.BladeData != null)
          itemMeshNames.Add(objectType.BladeData.HolsterMeshName);
      }
      foreach (BannerIconGroup bannerIconGroup in BannerManager.Instance.BannerIconGroups)
      {
        foreach (KeyValuePair<int, BannerIconData> allIcon in bannerIconGroup.AllIcons)
        {
          if (allIcon.Value.MaterialName != "")
            itemMeshNames.Add(allIcon.Value.MaterialName + (object) allIcon.Value.TextureIndex);
        }
      }
      string str = "";
      foreach (string source in itemMeshNames)
      {
        if (source != null && !source.IsEmpty<char>())
          str = str + source + "#";
      }
      return str;
    }

    [MBCallback]
    internal string GetHorseMaterialNames()
    {
      HashSet<string> stringSet = new HashSet<string>();
      string str = "";
      foreach (ItemObject objectType in Game.Current.ObjectManager.GetObjectTypeList<ItemObject>())
      {
        if (objectType.HasHorseComponent && objectType.HorseComponent.HorseMaterialNames != null && objectType.HorseComponent.HorseMaterialNames.Count > 0)
        {
          foreach (HorseComponent.MaterialProperty horseMaterialName in objectType.HorseComponent.HorseMaterialNames)
            stringSet.Add(horseMaterialName.Name);
        }
      }
      foreach (string source in stringSet)
      {
        if (source != null && !source.IsEmpty<char>())
          str = str + source + "#";
      }
      return str;
    }

    public void SetInitialModuleScreenAsRootScreen()
    {
      if (GameStateManager.Current != this.GlobalGameStateManager)
        GameStateManager.Current = this.GlobalGameStateManager;
      foreach (MBSubModuleBase subModule in this.SubModules)
        subModule.OnBeforeInitialModuleScreenSetAsRoot();
      if (GameNetwork.IsDedicatedServer)
      {
        MBGameManager.StartNewGame((MBGameManager) new MultiplayerGameManager());
      }
      else
      {
        string str1 = ModuleHelper.GetModuleFullPath("Native") + "Videos/TWLogo_and_Partners.ivf";
        string str2 = ModuleHelper.GetModuleFullPath("Native") + "Videos/TWLogo_and_Partners.ogg";
        if (!this._splashScreenPlayed && File.Exists(str1) && (str2 == "" || File.Exists(str2)))
        {
          VideoPlaybackState state = this.GlobalGameStateManager.CreateState<VideoPlaybackState>();
          state.SetStartingParameters(str1, str2, string.Empty, 30f, true);
          state.SetOnVideoFinisedDelegate(new Action(this.OnSplashScreenFinished));
          this.GlobalGameStateManager.CleanAndPushState((GameState) state);
          this._splashScreenPlayed = true;
        }
        else
          this.OnSplashScreenFinished();
      }
    }

    private void OnSplashScreenFinished()
    {
      Utilities.EnableGlobalLoadingWindow();
      LoadingWindow.EnableGlobalLoadingWindow();
      if (this.StartupInfo.StartupType == GameStartupType.Multiplayer || PlatformServices.SessionInvitationType == SessionInvitationType.Multiplayer)
        MBGameManager.StartNewGame((MBGameManager) new MultiplayerGameManager());
      else
        this.GlobalGameStateManager.CleanAndPushState((GameState) this.GlobalGameStateManager.CreateState<InitialState>());
    }

    [MBCallback]
    internal bool SetEditorScreenAsRootScreen()
    {
      if (GameStateManager.Current != this.GlobalGameStateManager)
        GameStateManager.Current = this.GlobalGameStateManager;
      if (this.GlobalGameStateManager.ActiveState is EditorState)
        return false;
      this.GlobalGameStateManager.CleanAndPushState((GameState) GameStateManager.Current.CreateState<EditorState>());
      return true;
    }

    private bool CheckAssemblyForMissionMethods(Assembly assembly)
    {
      Assembly assembly1 = Assembly.GetAssembly(typeof (MissionMethod));
      if (assembly == assembly1)
        return true;
      foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
      {
        if (referencedAssembly.FullName == assembly1.FullName)
          return true;
      }
      return false;
    }

    private void FindMissions()
    {
      MBDebug.Print("Searching Mission Methods");
      this._missionInfos = new List<MissionInfo>();
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      List<System.Type> typeList = new List<System.Type>();
      foreach (Assembly assembly in assemblies)
      {
        if (this.CheckAssemblyForMissionMethods(assembly))
        {
          foreach (System.Type type in assembly.GetTypes())
          {
            object[] customAttributes = type.GetCustomAttributes(typeof (MissionManager), true);
            if (customAttributes != null && customAttributes.Length != 0)
              typeList.Add(type);
          }
        }
      }
      MBDebug.Print("Found " + (object) typeList.Count + " mission managers");
      foreach (System.Type type in typeList)
      {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
          object[] customAttributes = method.GetCustomAttributes(typeof (MissionMethod), true);
          if (customAttributes != null && customAttributes.Length != 0)
          {
            MissionMethod missionMethod = customAttributes[0] as MissionMethod;
            MissionInfo missionInfo1 = new MissionInfo();
            missionInfo1.Creator = method;
            missionInfo1.Manager = type;
            missionInfo1.UsableByEditor = missionMethod.UsableByEditor;
            missionInfo1.Name = method.Name;
            if (missionInfo1.Name.StartsWith("Open"))
              missionInfo1.Name = missionInfo1.Name.Substring(4);
            if (missionInfo1.Name.EndsWith("Mission"))
              missionInfo1.Name = missionInfo1.Name.Substring(0, missionInfo1.Name.Length - 7);
            MissionInfo missionInfo2 = missionInfo1;
            missionInfo2.Name = missionInfo2.Name + "[" + type.Name + "]";
            this._missionInfos.Add(missionInfo1);
          }
        }
      }
      MBDebug.Print("Found " + (object) this._missionInfos.Count + " missions");
    }

    [MBCallback]
    internal string GetMissionControllerClassNames()
    {
      string str = "";
      for (int index = 0; index < this._missionInfos.Count; ++index)
      {
        MissionInfo missionInfo = this._missionInfos[index];
        if (missionInfo.UsableByEditor)
        {
          str += missionInfo.Name;
          if (index + 1 != this._missionInfos.Count)
            str += " ";
        }
      }
      return str;
    }

    private void LoadPlatformServices()
    {
      IPlatformServices platformServices = (IPlatformServices) null;
      Assembly assembly = (Assembly) null;
      PlatformInitParams platformInitParams = new PlatformInitParams();
      switch (ApplicationPlatform.CurrentPlatform)
      {
        case Platform.WindowsSteam:
          assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.PlatformService.Steam.dll");
          platformInitParams.Add("AppId", (object) Utilities.GetSteamAppId().ToString());
          break;
        case Platform.WindowsEpic:
          assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.PlatformService.Epic.dll");
          platformInitParams.Add("ExchangeCode", (object) this.StartupInfo.EpicExchangeCode);
          break;
        case Platform.Orbis:
          assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.PlatformService.PS.dll");
          break;
        case Platform.Durango:
        case Platform.GDKDesktop:
          assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.PlatformService.GDK.dll");
          break;
        case Platform.WindowsNoPlatform:
          string userName = "TestUser" + (object) (DateTime.Now.Ticks % 10000L);
          if (!string.IsNullOrEmpty(this.StartupInfo.OverridenUserName))
            userName = this.StartupInfo.OverridenUserName;
          platformServices = (IPlatformServices) new TestPlatformServices(userName);
          break;
        case Platform.WindowsGOG:
          assembly = AssemblyLoader.LoadFrom(ManagedDllFolder.Name + "TaleWorlds.PlatformService.GOG.dll");
          break;
      }
      if (assembly != (Assembly) null)
      {
        System.Type[] types = assembly.GetTypes();
        System.Type type1 = (System.Type) null;
        foreach (System.Type type2 in types)
        {
          if (((IEnumerable<System.Type>) type2.GetInterfaces()).Contains<System.Type>(typeof (IPlatformServices)))
          {
            type1 = type2;
            break;
          }
        }
        platformServices = (IPlatformServices) type1.GetConstructor(new System.Type[1]
        {
          typeof (PlatformInitParams)
        }).Invoke(new object[1]
        {
          (object) platformInitParams
        });
      }
      if (platformServices == null)
        return;
      PlatformServices.Setup(platformServices);
      PlatformServices.OnSessionInvitationAccepted += new Action<SessionInvitationType>(this.OnSessionInvitationAccepted);
      PlatformServices.Initialize(new IFriendListService[2]
      {
        (IFriendListService) new BannerlordFriendListService(),
        (IFriendListService) new RecentPlayersFriendListService()
      });
      AchievementManager.AchievementService = platformServices.GetAchievementService();
    }

    private void OnSessionInvitationAccepted(SessionInvitationType targetGameType)
    {
      if (targetGameType == SessionInvitationType.Multiplayer)
      {
        this.JobManager.AddJob((Job) new Module.OnSessionInvitationAcceptedJob(targetGameType));
      }
      else
      {
        if (targetGameType != SessionInvitationType.ConnectionString)
          return;
        this.JobManager.AddJob((Job) new Module.OnConnectionStringSessionInvitationAcceptedJob());
      }
    }

    private void LoadSubModules()
    {
      MBDebug.Print("Loading submodules...");
      List<ModuleInfo> moduleInfoList = new List<ModuleInfo>();
      string[] modulesNames = Utilities.GetModulesNames();
      for (int index = 0; index < modulesNames.Length; ++index)
      {
        ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo(modulesNames[index]);
        moduleInfoList.Add(moduleInfo);
        XmlResource.GetMbprojxmls(modulesNames[index]);
        XmlResource.GetXmlListAndApply(modulesNames[index]);
      }
      string configName = Common.ConfigName;
      foreach (ModuleInfo moduleInfo in moduleInfoList)
      {
        foreach (SubModuleInfo subModule in moduleInfo.SubModules)
        {
          if (this.CheckIfSubmoduleCanBeLoadable(subModule) && !this._loadedSubmoduleTypes.ContainsKey(subModule.SubModuleClassType))
          {
            string path1 = System.IO.Path.Combine(moduleInfo.GetFolderPath(), "bin", configName);
            string str1 = System.IO.Path.Combine(path1, subModule.DLLName);
            string str2 = ManagedDllFolder.Name + subModule.DLLName;
            foreach (string assembly in subModule.Assemblies)
            {
              string str3 = System.IO.Path.Combine(path1, assembly);
              string assemblyFile = ManagedDllFolder.Name + assembly;
              if (File.Exists(str3))
                AssemblyLoader.LoadFrom(str3);
              else
                AssemblyLoader.LoadFrom(assemblyFile);
            }
            if (File.Exists(str1))
              this.AddSubModule(AssemblyLoader.LoadFrom(str1), subModule.SubModuleClassType);
            else if (File.Exists(str2))
              this.AddSubModule(AssemblyLoader.LoadFrom(str2), subModule.SubModuleClassType);
            else
              Debug.ShowMessageBox("Cannot find: " + str1, "Error", 4U);
          }
        }
      }
      this.InitializeSubModules();
    }

    public bool CheckIfSubmoduleCanBeLoadable(SubModuleInfo subModuleInfo)
    {
      if (subModuleInfo.Tags.Count > 0)
      {
        foreach (Tuple<SubModuleInfo.SubModuleTags, string> tag in subModuleInfo.Tags)
        {
          if (!this.GetSubModuleValiditiy(tag.Item1, tag.Item2))
            return false;
        }
      }
      return true;
    }

    private bool GetSubModuleValiditiy(SubModuleInfo.SubModuleTags tag, string value)
    {
      switch (tag)
      {
        case SubModuleInfo.SubModuleTags.RejectedPlatform:
          Platform result1;
          if (Enum.TryParse<Platform>(value, out result1))
            return ApplicationPlatform.CurrentPlatform != result1;
          break;
        case SubModuleInfo.SubModuleTags.ExclusivePlatform:
          Platform result2;
          if (Enum.TryParse<Platform>(value, out result2))
            return ApplicationPlatform.CurrentPlatform == result2;
          break;
        case SubModuleInfo.SubModuleTags.DedicatedServerType:
          string lower = value.ToLower();
          if (lower == "none")
            return this.StartupInfo.DedicatedServerType == DedicatedServerType.None;
          if (lower == "both")
            return this.StartupInfo.DedicatedServerType != DedicatedServerType.None;
          if (lower == "custom")
            return this.StartupInfo.DedicatedServerType == DedicatedServerType.Custom;
          if (lower == "matchmaker" && this.StartupInfo.DedicatedServerType != DedicatedServerType.Matchmaker)
            return false;
          break;
        case SubModuleInfo.SubModuleTags.IsNoRenderModeElement:
          return value.Equals("false");
        case SubModuleInfo.SubModuleTags.DependantRuntimeLibrary:
          Runtime result3;
          if (Enum.TryParse<Runtime>(value, out result3))
            return ApplicationPlatform.CurrentRuntimeLibrary == result3;
          break;
      }
      return true;
    }

    [MBCallback]
    internal static void MBThrowException()
    {
    }

    [MBCallback]
    internal void OnEnterEditMode(bool isFirstTime)
    {
      int num = isFirstTime ? 1 : 0;
    }

    [MBCallback]
    internal static Module GetInstance() => Module.CurrentModule;

    private void FinalizeModule()
    {
      if (Game.Current != null)
        Game.Current.OnFinalize();
      if (TestCommonBase.BaseInstance != null)
        TestCommonBase.BaseInstance.OnFinalize();
      this._testContext.FinalizeContext();
      InformationManager.Clear();
      ScreenManager.OnFinalize();
      BannerlordConfig.Save();
      this.FinalizeSubModules();
      Common.MemoryCleanup();
    }

    internal static void FinalizeCurrentModule()
    {
      Module.CurrentModule.FinalizeModule();
      Module.CurrentModule = (Module) null;
    }

    [MBCallback]
    internal void SetLoadingFinished() => this.LoadingFinished = true;

    [MBCallback]
    internal void OnCloseSceneEditorPresentation() => GameStateManager.Current.PopState();

    [MBCallback]
    internal void OnSceneEditorModeOver() => GameStateManager.Current.PopState();

    private void OnConfigChanged()
    {
      foreach (MBSubModuleBase subModule in this.SubModules)
        subModule.OnConfigChanged();
    }

    [MBCallback]
    internal void OnSkinsXMLHasChanged()
    {
      if (this.SkinsXMLHasChanged == null)
        return;
      this.SkinsXMLHasChanged();
    }

    public event Action SkinsXMLHasChanged;

    [MBCallback]
    internal void OnImguiProfilerTick()
    {
      if (this.ImguiProfilerTick == null)
        return;
      this.ImguiProfilerTick();
    }

    [MBCallback]
    internal static string CreateProcessedSkinsXMLForNative(out string baseSkinsXmlPath)
    {
      List<string> usedPaths;
      XmlDocument mergedXmlForNative = MBObjectManager.GetMergedXmlForNative("soln_skins", out usedPaths);
      System.IO.StringWriter stringWriter = new System.IO.StringWriter();
      XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) stringWriter);
      mergedXmlForNative.WriteTo((XmlWriter) xmlTextWriter);
      baseSkinsXmlPath = usedPaths.First<string>();
      return stringWriter.ToString();
    }

    [MBCallback]
    internal static string CreateProcessedActionSetsXMLForNative()
    {
      XDocument xdocument = MBObjectManager.ToXDocument(MBObjectManager.GetMergedXmlForNative("soln_action_sets", out List<string> _));
      for (int index1 = 0; index1 < xdocument.Descendants((XName) "action_set").Count<XElement>(); ++index1)
      {
        for (int index2 = index1 + 1; index2 < xdocument.Descendants((XName) "action_set").Count<XElement>(); ++index2)
        {
          if (xdocument.Descendants((XName) "action_set").ElementAt<XElement>(index1).FirstAttribute.ToString() == xdocument.Descendants((XName) "action_set").ElementAt<XElement>(index2).FirstAttribute.ToString())
          {
            xdocument.Descendants((XName) "action_set").ElementAt<XElement>(index1).Add((object) xdocument.Descendants((XName) "action_set").ElementAt<XElement>(index2).Descendants());
            xdocument.Descendants((XName) "action_set").ElementAt<XElement>(index2).Remove();
            --index2;
          }
        }
      }
      XmlDocument xmlDocument = MBObjectManager.ToXmlDocument(xdocument);
      System.IO.StringWriter stringWriter = new System.IO.StringWriter();
      XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) stringWriter);
      xmlDocument.WriteTo((XmlWriter) xmlTextWriter);
      return stringWriter.ToString();
    }

    [MBCallback]
    internal static string CreateProcessedActionTypesXMLForNative()
    {
      XmlDocument mergedXmlForNative = MBObjectManager.GetMergedXmlForNative("soln_action_types", out List<string> _);
      System.IO.StringWriter stringWriter = new System.IO.StringWriter();
      XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) stringWriter);
      mergedXmlForNative.WriteTo((XmlWriter) xmlTextWriter);
      return stringWriter.ToString();
    }

    [MBCallback]
    internal static string CreateProcessedAnimationsXMLForNative(out string animationsXmlPaths)
    {
      List<string> usedPaths;
      XmlDocument mergedXmlForNative = MBObjectManager.GetMergedXmlForNative("soln_animations", out usedPaths);
      System.IO.StringWriter stringWriter = new System.IO.StringWriter();
      XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) stringWriter);
      mergedXmlForNative.WriteTo((XmlWriter) xmlTextWriter);
      animationsXmlPaths = "";
      for (int index = 0; index < usedPaths.Count; ++index)
      {
        animationsXmlPaths += usedPaths[index];
        if (index != usedPaths.Count - 1)
          animationsXmlPaths += "\n";
      }
      return stringWriter.ToString();
    }

    [MBCallback]
    internal static string CreateProcessedModuleDataXMLForNative(string xmlType)
    {
      XmlDocument xmlDocument = MBObjectManager.GetMergedXmlForNative("soln_" + xmlType, out List<string> _);
      if (xmlType == "full_movement_sets")
      {
        XDocument xdocument = MBObjectManager.ToXDocument(xmlDocument);
        for (int index1 = 0; index1 < xdocument.Descendants((XName) "full_movement_set").Count<XElement>(); ++index1)
        {
          for (int index2 = index1 + 1; index2 < xdocument.Descendants((XName) "full_movement_set").Count<XElement>(); ++index2)
          {
            if (xdocument.Descendants((XName) "full_movement_set").ElementAt<XElement>(index1).FirstAttribute.ToString() == xdocument.Descendants((XName) "full_movement_set").ElementAt<XElement>(index2).FirstAttribute.ToString())
            {
              xdocument.Descendants((XName) "full_movement_set").ElementAt<XElement>(index1).Add((object) xdocument.Descendants((XName) "full_movement_set").ElementAt<XElement>(index2).Descendants());
              xdocument.Descendants((XName) "full_movement_set").ElementAt<XElement>(index2).Remove();
              --index2;
            }
          }
        }
        xmlDocument = MBObjectManager.ToXmlDocument(xdocument);
      }
      System.IO.StringWriter stringWriter = new System.IO.StringWriter();
      XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) stringWriter);
      xmlDocument.WriteTo((XmlWriter) xmlTextWriter);
      return stringWriter.ToString();
    }

    public event Action ImguiProfilerTick;

    public void ClearStateOptions() => this._initialStateOptions.Clear();

    public void AddInitialStateOption(InitialStateOption initialStateOption) => this._initialStateOptions.Add(initialStateOption);

    public IEnumerable<InitialStateOption> GetInitialStateOptions() => (IEnumerable<InitialStateOption>) this._initialStateOptions.OrderBy<InitialStateOption, int>((Func<InitialStateOption, int>) (s => s.OrderIndex));

    public InitialStateOption GetInitialStateOptionWithId(string id)
    {
      foreach (InitialStateOption initialStateOption in this._initialStateOptions)
      {
        if (initialStateOption.Id == id)
          return initialStateOption;
      }
      return (InitialStateOption) null;
    }

    public void ExecuteInitialStateOptionWithId(string id) => this.GetInitialStateOptionWithId(id)?.DoAction();

    void IGameStateManagerOwner.OnStateStackEmpty()
    {
    }

    void IGameStateManagerOwner.OnStateChanged(GameState oldState)
    {
    }

    public void SetEditorMissionTester(IEditorMissionTester editorMissionTester) => this._editorMissionTester = editorMissionTester;

    [MBCallback]
    internal void StartMissionForEditor(string missionName, string sceneName, string levels)
    {
      if (this._editorMissionTester == null)
        return;
      this._editorMissionTester.StartMissionForEditor(missionName, sceneName, levels);
    }

    [MBCallback]
    internal void StartMissionForReplayEditor(
      string missionName,
      string sceneName,
      string levels,
      string fileName,
      bool record,
      float startTime,
      float endTime)
    {
      if (this._editorMissionTester == null)
        return;
      this._editorMissionTester.StartMissionForReplayEditor(missionName, sceneName, levels, fileName, record, startTime, endTime);
    }

    public void StartMissionForEditorAux(
      string missionName,
      string sceneName,
      string levels,
      bool forReplay,
      string replayFileName,
      bool isRecord)
    {
      GameStateManager.Current = Game.Current.GameStateManager;
      this.ReturnToEditorState = true;
      MissionInfo missionInfo = this._missionInfos.Find((Predicate<MissionInfo>) (mi => mi.Name == missionName)) ?? this._missionInfos.Find((Predicate<MissionInfo>) (mi => mi.Name.Contains(missionName)));
      if (forReplay)
        missionInfo.Creator.Invoke((object) null, new object[2]
        {
          (object) replayFileName,
          (object) isRecord
        });
      else
        missionInfo.Creator.Invoke((object) null, new object[2]
        {
          (object) sceneName,
          (object) levels
        });
    }

    private void FillMultiplayerGameTypes()
    {
      this._multiplayerGameModesWithNames = new Dictionary<string, MultiplayerGameMode>();
      this._multiplayerGameTypes = new List<MultiplayerGameTypeInfo>();
      this.AddMultiplayerGameMode((MultiplayerGameMode) new MissionBasedMultiplayerGameMode("FreeForAll"));
      this.AddMultiplayerGameMode((MultiplayerGameMode) new MissionBasedMultiplayerGameMode("TeamDeathmatch"));
      this.AddMultiplayerGameMode((MultiplayerGameMode) new MissionBasedMultiplayerGameMode("Duel"));
      this.AddMultiplayerGameMode((MultiplayerGameMode) new MissionBasedMultiplayerGameMode("Siege"));
      this.AddMultiplayerGameMode((MultiplayerGameMode) new MissionBasedMultiplayerGameMode("Captain"));
      this.AddMultiplayerGameMode((MultiplayerGameMode) new MissionBasedMultiplayerGameMode("Skirmish"));
    }

    public MultiplayerGameMode GetMultiplayerGameMode(string gameType)
    {
      MultiplayerGameMode multiplayerGameMode;
      return this._multiplayerGameModesWithNames.TryGetValue(gameType, out multiplayerGameMode) ? multiplayerGameMode : (MultiplayerGameMode) null;
    }

    public void AddMultiplayerGameMode(MultiplayerGameMode multiplayerGameMode)
    {
      this._multiplayerGameModesWithNames.Add(multiplayerGameMode.Name, multiplayerGameMode);
      this._multiplayerGameTypes.Add(new MultiplayerGameTypeInfo("Native", multiplayerGameMode.Name));
    }

    public List<MultiplayerGameTypeInfo> GetMultiplayerGameTypes() => this._multiplayerGameTypes;

    public bool StartMultiplayerGame(string multiplayerGameType, string scene)
    {
      MultiplayerGameMode multiplayerGameMode;
      if (!this._multiplayerGameModesWithNames.TryGetValue(multiplayerGameType, out multiplayerGameMode))
        return false;
      multiplayerGameMode.StartMultiplayerGame(scene);
      return true;
    }

    public async void ShutDownWithDelay(string reason, int seconds)
    {
      if (this._isShuttingDown)
        return;
      this._isShuttingDown = true;
      for (int i = 0; i < seconds; ++i)
      {
        string message = "Shutting down in " + (object) (seconds - i) + " seconds with reason '" + reason + "'";
        Debug.Print(message);
        Console.WriteLine(message);
        await Task.Delay(1000);
      }
      if (Game.Current != null)
      {
        Debug.Print("Active game exist during ShutDownWithDelay");
        MBGameManager.EndGame();
      }
      Utilities.QuitGame();
    }

    public enum XmlInformationType
    {
      Parameters,
      MbObjectType,
    }

    private class OnSessionInvitationAcceptedJob : Job
    {
      private readonly SessionInvitationType _sessionInvitationType;

      public OnSessionInvitationAcceptedJob(SessionInvitationType sessionInvitationType) => this._sessionInvitationType = sessionInvitationType;

      public override void DoJob(float dt)
      {
        base.DoJob(dt);
        if (MBGameManager.Current != null)
          MBGameManager.Current.OnSessionInvitationAccepted(this._sessionInvitationType);
        else if (GameStateManager.Current != null)
        {
          if (GameStateManager.Current.ActiveState is InitialState)
          {
            if (this._sessionInvitationType == SessionInvitationType.Multiplayer)
              MBGameManager.StartNewGame((MBGameManager) new MultiplayerGameManager());
          }
          else if (GameStateManager.Current.ActiveState != null)
            GameStateManager.Current.CleanStates();
        }
        this.Finished = true;
      }
    }

    private class OnConnectionStringSessionInvitationAcceptedJob : Job
    {
      private bool _loaded;

      public OnConnectionStringSessionInvitationAcceptedJob() => this._loaded = GameStateManager.Current.ActiveState != null;

      public override void DoJob(float dt)
      {
        base.DoJob(dt);
        if (!this._loaded && (GameStateManager.Current == null || GameStateManager.Current.ActiveState == null || !(GameStateManager.Current.ActiveState is InitialState)))
          return;
        string[] strArray = PlatformServices.SessionInvitationData["value"].Split(':');
        if (strArray[0] == "CustomGame")
        {
          Guid result;
          if (strArray.Length != 2 || !Guid.TryParse(strArray[1], out result))
            return;
          if (MBGameManager.Current != null)
          {
            if (MBGameManager.Current is MultiplayerGameManager)
            {
              MultiplayerGameManager current = (MultiplayerGameManager) MBGameManager.Current;
              if (current.Game != null)
              {
                if (current.Game.CurrentState != Game.State.Running)
                  return;
                if (NetworkMain.GameClient.CurrentMatchId == result.ToString())
                {
                  this.Finished = true;
                  return;
                }
                MBGameManager.EndGame();
                return;
              }
            }
          }
          else
          {
            if (GameStateManager.Current == null)
              return;
            if (GameStateManager.Current.ActiveState is InitialState)
              MBGameManager.StartNewGame((MBGameManager) new MultiplayerGameManager());
            else if (GameStateManager.Current.ActiveState != null)
              GameStateManager.Current.CleanStates();
          }
        }
        this.Finished = true;
      }
    }

    private enum StartupType
    {
      None,
      TestMode,
      GameServer,
      Singleplayer,
      Multiplayer,
      Count,
    }
  }
}
