// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBCommon
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class MBCommon
  {
    private static MBCommon.GameType _currentGameType;

    public static MBCommon.GameType CurrentGameType
    {
      get => MBCommon._currentGameType;
      set
      {
        MBCommon._currentGameType = value;
        MBAPI.IMBWorld.SetGameType((int) value);
      }
    }

    public static void PauseGameEngine()
    {
      MBCommon.IsPaused = true;
      MBAPI.IMBWorld.PauseGame();
    }

    public static void UnPauseGameEngine()
    {
      MBCommon.IsPaused = false;
      MBAPI.IMBWorld.UnpauseGame();
    }

    public static float GetTime(MBCommon.TimeType timeType) => MBAPI.IMBWorld.GetTime(timeType);

    public static bool IsDebugMode => false;

    public static void FixSkeletons() => MBAPI.IMBWorld.FixSkeletons();

    public static bool IsPaused { get; private set; }

    public static void CheckResourceModifications() => MBAPI.IMBWorld.CheckResourceModifications();

    public static int Hash(int i, object o)
    {
      int num = i * 397 ^ o.GetHashCode();
      num = num.ToString().GetHashCode();
      return num;
    }

    public enum GameType
    {
      Single,
      MultiClient,
      MultiServer,
      MultiClientServer,
      SingleReplay,
      SingleRecord,
    }

    public enum TimeType
    {
      Application,
      Mission,
    }
  }
}
