// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBAPI
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public static class MBAPI
  {
    internal static IMBTestRun IMBTestRun;
    internal static IMBActionSet IMBActionSet;
    internal static IMBAgent IMBAgent;
    internal static IMBAgentVisuals IMBAgentVisuals;
    internal static IMBAnimation IMBAnimation;
    internal static IMBDelegate IMBDelegate;
    internal static IMBItem IMBItem;
    internal static IMBEditor IMBEditor;
    internal static IMBMission IMBMission;
    internal static IMBMultiplayerData IMBMultiplayerData;
    internal static IMouseManager IMouseManager;
    internal static IMBNetwork IMBNetwork;
    internal static IMBPeer IMBPeer;
    internal static IMBSkeletonExtensions IMBSkeletonExtensions;
    internal static IMBGameEntityExtensions IMBGameEntityExtensions;
    internal static IMBScreen IMBScreen;
    internal static IMBSoundEvent IMBSoundEvent;
    internal static IMBVoiceManager IMBVoiceManager;
    internal static IMBTeam IMBTeam;
    internal static IMBWorld IMBWorld;
    internal static IInput IInput;
    internal static IMBMessageManager IMBMessageManager;
    internal static IMBWindowManager IMBWindowManager;
    internal static IMBDebugExtensions IMBDebugExtensions;
    internal static IMBGame IMBGame;
    internal static IMBFaceGen IMBFaceGen;
    internal static IMBMapScene IMBMapScene;
    internal static IMBBannerlordChecker IMBBannerlordChecker;
    internal static IMBBannerlordTableauManager IMBBannerlordTableauManager;
    internal static IMBBannerlordConfig IMBBannerlordConfig;
    private static Dictionary<string, object> _objects;

    private static T GetObject<T>() where T : class
    {
      object obj;
      return MBAPI._objects.TryGetValue(typeof (T).FullName, out obj) ? obj as T : default (T);
    }

    internal static void SetObjects(Dictionary<string, object> objects)
    {
      MBAPI._objects = objects;
      MBAPI.IMBTestRun = MBAPI.GetObject<IMBTestRun>();
      MBAPI.IMBActionSet = MBAPI.GetObject<IMBActionSet>();
      MBAPI.IMBAgent = MBAPI.GetObject<IMBAgent>();
      MBAPI.IMBAnimation = MBAPI.GetObject<IMBAnimation>();
      MBAPI.IMBDelegate = MBAPI.GetObject<IMBDelegate>();
      MBAPI.IMBItem = MBAPI.GetObject<IMBItem>();
      MBAPI.IMBEditor = MBAPI.GetObject<IMBEditor>();
      MBAPI.IMBMission = MBAPI.GetObject<IMBMission>();
      MBAPI.IMBMultiplayerData = MBAPI.GetObject<IMBMultiplayerData>();
      MBAPI.IMouseManager = MBAPI.GetObject<IMouseManager>();
      MBAPI.IMBNetwork = MBAPI.GetObject<IMBNetwork>();
      MBAPI.IMBPeer = MBAPI.GetObject<IMBPeer>();
      MBAPI.IMBSkeletonExtensions = MBAPI.GetObject<IMBSkeletonExtensions>();
      MBAPI.IMBGameEntityExtensions = MBAPI.GetObject<IMBGameEntityExtensions>();
      MBAPI.IMBScreen = MBAPI.GetObject<IMBScreen>();
      MBAPI.IMBSoundEvent = MBAPI.GetObject<IMBSoundEvent>();
      MBAPI.IMBVoiceManager = MBAPI.GetObject<IMBVoiceManager>();
      MBAPI.IMBTeam = MBAPI.GetObject<IMBTeam>();
      MBAPI.IMBWorld = MBAPI.GetObject<IMBWorld>();
      MBAPI.IInput = MBAPI.GetObject<IInput>();
      MBAPI.IMBMessageManager = MBAPI.GetObject<IMBMessageManager>();
      MBAPI.IMBWindowManager = MBAPI.GetObject<IMBWindowManager>();
      MBAPI.IMBDebugExtensions = MBAPI.GetObject<IMBDebugExtensions>();
      MBAPI.IMBGame = MBAPI.GetObject<IMBGame>();
      MBAPI.IMBFaceGen = MBAPI.GetObject<IMBFaceGen>();
      MBAPI.IMBMapScene = MBAPI.GetObject<IMBMapScene>();
      MBAPI.IMBBannerlordChecker = MBAPI.GetObject<IMBBannerlordChecker>();
      MBAPI.IMBAgentVisuals = MBAPI.GetObject<IMBAgentVisuals>();
      MBAPI.IMBBannerlordTableauManager = MBAPI.GetObject<IMBBannerlordTableauManager>();
      MBAPI.IMBBannerlordConfig = MBAPI.GetObject<IMBBannerlordConfig>();
    }
  }
}
