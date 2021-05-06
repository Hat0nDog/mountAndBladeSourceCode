// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBGameManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ObjectSystem;
using TaleWorlds.PlatformService;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MBGameManager : GameManagerBase
  {
    private readonly object _lockObject = new object();
    private bool _isEnding;

    public static MBGameManager Current => (MBGameManager) GameManagerBase.Current;

    public bool IsLoaded { get; private set; }

    protected MBGameManager() => this._isEnding = false;

    protected static void StartNewGame() => MBAPI.IMBGame.StartNew();

    protected static void LoadModuleData(bool isLoadGame) => MBAPI.IMBGame.LoadModuleData(isLoadGame);

    public static void StartNewGame(MBGameManager gameLoader)
    {
      GameLoadingState state = GameStateManager.Current.CreateState<GameLoadingState>();
      state.SetLoadingParameters(gameLoader);
      GameStateManager.Current.CleanAndPushState((GameState) state);
    }

    public override void BeginGameStart(Game game)
    {
      foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
        subModule.BeginGameStart(game);
    }

    public override void OnCampaignEarlyStart(Game game, object starterObject)
    {
    }

    public override void OnCampaignStart(Game game, object starterObject)
    {
      foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
        subModule.OnCampaignStart(game, starterObject);
    }

    public override void OnGameInitializationFinished(Game game)
    {
      foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
        subModule.OnGameInitializationFinished(game);
      foreach (SkeletonScale objectType in Game.Current.ObjectManager.GetObjectTypeList<SkeletonScale>())
      {
        sbyte[] boneIndices = new sbyte[objectType.BoneNames.Count];
        for (int index = 0; index < boneIndices.Length; ++index)
          boneIndices[index] = Skeleton.GetBoneIndexFromName(objectType.SkeletonModel, objectType.BoneNames[index]);
        objectType.SetBoneIndices(boneIndices);
      }
    }

    public override void OnGameLoaded(Game game, object initializerObject)
    {
      NetworkMain.Initialize();
      foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
        subModule.OnGameLoaded(game, initializerObject);
    }

    public override void OnGameCreated(Game game, object initializerObject)
    {
      foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
        subModule.OnNewGameCreated(game, initializerObject);
    }

    public override void OnGameStart(Game game, IGameStarter gameStarter)
    {
      Game.Current.MonsterMissionDataCreator = (IMonsterMissionDataCreator) new MonsterMissionDataCreator();
      foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
        subModule.OnGameStart(game, gameStarter);
      Game.Current.AddGameModelsManager<MissionGameModels>(gameStarter.Models);
      Monster.GetBoneIndexWithId = new Func<string, string, sbyte>(MBActionSet.GetBoneIndexWithId);
    }

    public override void OnGameEnd(Game game)
    {
      foreach (MBSubModuleBase subModule in Module.CurrentModule.SubModules)
        subModule.OnGameEnd(game);
      MissionGameModels.Clear();
      base.OnGameEnd(game);
    }

    public static async void EndGame()
    {
      while (true)
      {
        MBGameManager current = MBGameManager.Current;
        if ((current != null ? (!current.IsLoaded ? 1 : 0) : 0) != 0)
          await Task.Delay(100);
        else
          break;
      }
      MBGameManager current1 = MBGameManager.Current;
      if ((current1 != null ? (!current1.CheckAndSetEnding() ? 1 : 0) : 0) != 0 || Game.Current.GameStateManager == null)
        return;
      while (Mission.Current != null && !(Game.Current.GameStateManager.ActiveState is MissionState))
        Game.Current.GameStateManager.PopState();
      if (Game.Current.GameStateManager.ActiveState is MissionState)
        ((MissionState) Game.Current.GameStateManager.ActiveState).CurrentMission.EndMission();
      while (Mission.Current != null)
        await Task.Delay(1);
      Game.Current.GameStateManager.CleanStates();
    }

    public override void OnAfterCampaignStart(Game game) => NetworkMain.Initialize();

    public override void OnLoadFinished()
    {
      base.OnLoadFinished();
      this.IsLoaded = true;
    }

    public bool CheckAndSetEnding()
    {
      lock (this._lockObject)
      {
        if (this._isEnding)
          return false;
        this._isEnding = true;
        return true;
      }
    }

    public virtual void OnSessionInvitationAccepted(SessionInvitationType targetGameType)
    {
      if (targetGameType == SessionInvitationType.None)
        return;
      MBGameManager.EndGame();
    }

    protected List<MbObjectXmlInformation> GetXmlInformationFromModule() => XmlResource.XmlInformationList;

    public override float ApplicationTime => MBCommon.GetTime(MBCommon.TimeType.Application);

    public override bool CheatMode => NativeConfig.CheatMode;

    public override bool IsDevelopmentMode => NativeConfig.IsDevelopmentMode;

    public override bool IsEditModeOn => MBEditor.IsEditModeOn;

    public override bool DeterministicMode => NativeConfig.DeterministicMode;
  }
}
