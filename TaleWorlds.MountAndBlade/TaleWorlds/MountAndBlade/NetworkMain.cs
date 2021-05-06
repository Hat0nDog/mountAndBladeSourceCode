// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.NetworkMain
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Threading.Tasks;
using TaleWorlds.Diamond.ClientApplication;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.Network;

namespace TaleWorlds.MountAndBlade
{
  public static class NetworkMain
  {
    private static ClientApplicationConfiguration _lobbyClientApplicationConfiguration;
    private static long _lastInternetConnectionCheck;
    private static bool _checkingConnection;
    private const long InternetConnectionCheckInterval = 300000000;
    private static DiamondClientApplication _diamondClientApplication;

    public static bool InternetConnectionAvailable { get; private set; }

    static NetworkMain()
    {
      NetworkMain.IsInitialized = false;
      NetworkMain._lobbyClientApplicationConfiguration = new ClientApplicationConfiguration();
      NetworkMain._lobbyClientApplicationConfiguration.FillFrom("LobbyClient");
    }

    public static void Initialize()
    {
      Debug.Print("Initializing NetworkMain");
      MBCommon.CurrentGameType = MBCommon.GameType.Single;
      GameNetwork.InitializeCompressionInfos();
      if (!NetworkMain.IsInitialized)
      {
        NetworkMain.IsInitialized = true;
        GameNetwork.Initialize((IGameNetworkHandler) new GameNetworkHandler());
        if (!GameNetwork.IsDedicatedServer)
        {
          NetworkMain._diamondClientApplication = new DiamondClientApplication(ApplicationVersion.FromParametersFile(ApplicationVersionGameType.Multiplayer));
          NetworkMain._diamondClientApplication.Initialize(NetworkMain._lobbyClientApplicationConfiguration);
          NetworkMain.GameClient = NetworkMain._diamondClientApplication.GetClient<LobbyClient>("LobbyClient");
          MachineId.Initialize();
        }
      }
      Debug.Print("NetworkMain Initialized");
    }

    private static async void CheckInternetConnection()
    {
      string str1 = "LobbyClient";
      string outValue1;
      NetworkMain._lobbyClientApplicationConfiguration.Parameters.TryGetParameter(str1 + ".Address", out outValue1);
      ushort outValue2;
      NetworkMain._lobbyClientApplicationConfiguration.Parameters.TryGetParameterAsUInt16(str1 + ".Port", out outValue2);
      bool outValue3;
      NetworkMain._lobbyClientApplicationConfiguration.Parameters.TryGetParameterAsBool(str1 + ".IsSecure", out outValue3);
      string outValue4;
      IHttpDriver httpDriver = !NetworkMain._lobbyClientApplicationConfiguration.Parameters.TryGetParameter(str1 + ".HttpDriver", out outValue4) ? HttpDriverManager.GetDefaultHttpDriver() : HttpDriverManager.GetHttpDriver(outValue4);
      if (NetworkMain._lobbyClientApplicationConfiguration.SessionProviderType == SessionProviderType.Rest || NetworkMain._lobbyClientApplicationConfiguration.SessionProviderType == SessionProviderType.ThreadedRest)
      {
        string url = string.Format("{0}://{1}:{2}/data/ping", outValue3 ? (object) "https" : (object) "http", (object) outValue1, (object) outValue2);
        try
        {
          string str2 = await httpDriver.HttpGetString(url, false);
          NetworkMain.InternetConnectionAvailable = true;
        }
        catch
        {
          NetworkMain.InternetConnectionAvailable = false;
        }
        NetworkMain._lastInternetConnectionCheck = DateTime.Now.Ticks;
      }
      else
      {
        NetworkMain.InternetConnectionAvailable = true;
        NetworkMain._lastInternetConnectionCheck = long.MaxValue;
      }
      NetworkMain._checkingConnection = false;
    }

    public static void InitializeAsDedicatedServer()
    {
      MBCommon.CurrentGameType = MBCommon.GameType.MultiServer;
      GameNetwork.InitializeCompressionInfos();
      if (NetworkMain.IsInitialized)
        return;
      NetworkMain.IsInitialized = true;
      GameNetwork.Initialize((IGameNetworkHandler) new GameNetworkHandler());
      GameStartupInfo startupInfo = Module.CurrentModule.StartupInfo;
    }

    internal static void Tick(float dt)
    {
      if (NetworkMain.IsInitialized)
      {
        if (NetworkMain.GameClient != null)
          NetworkMain.GameClient.Update();
        if (NetworkMain._diamondClientApplication != null)
          NetworkMain._diamondClientApplication.Update();
        GameNetwork.Tick(dt);
      }
      if (Module.CurrentModule.StartupInfo.StartupType == GameStartupType.Singleplayer || NetworkMain._checkingConnection || DateTime.Now.Ticks - NetworkMain._lastInternetConnectionCheck <= 300000000L)
        return;
      NetworkMain._checkingConnection = true;
      Task.Run((Action) (() => NetworkMain.CheckInternetConnection()));
    }

    public static bool IsInitialized { get; private set; }

    public static LobbyClient GameClient { get; private set; }

    public static MissionLobbyComponent.MultiplayerGameType[] GetAvailableRankedGameModes() => new MissionLobbyComponent.MultiplayerGameType[2]
    {
      MissionLobbyComponent.MultiplayerGameType.Captain,
      MissionLobbyComponent.MultiplayerGameType.Skirmish
    };

    public static MissionLobbyComponent.MultiplayerGameType[] GetAvailableCustomGameModes() => new MissionLobbyComponent.MultiplayerGameType[2]
    {
      MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch,
      MissionLobbyComponent.MultiplayerGameType.Siege
    };

    public static MissionLobbyComponent.MultiplayerGameType[] GetAvailableQuickPlayGameModes() => new MissionLobbyComponent.MultiplayerGameType[2]
    {
      MissionLobbyComponent.MultiplayerGameType.Captain,
      MissionLobbyComponent.MultiplayerGameType.Skirmish
    };

    public static string[] GetAvailableMatchmakerRegions() => new string[4]
    {
      "USE",
      "USW",
      "EU",
      "CN"
    };

    public static string GetUserDefaultRegion() => "None";

    public static string GetUserCurrentRegion()
    {
      LobbyClient gameClient = NetworkMain.GameClient;
      return (gameClient != null ? (gameClient.LoggedIn ? 1 : 0) : 0) != 0 ? NetworkMain.GameClient.PlayerData.LastRegion : NetworkMain.GetUserDefaultRegion();
    }

    public static string[] GetUserSelectedGameTypes()
    {
      LobbyClient gameClient = NetworkMain.GameClient;
      return (gameClient != null ? (gameClient.LoggedIn ? 1 : 0) : 0) != 0 ? NetworkMain.GameClient.PlayerData.LastGameTypes : new string[0];
    }
  }
}
