// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBMusicManagerOld
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using psai.net;

namespace TaleWorlds.MountAndBlade
{
  public static class MBMusicManagerOld
  {
    public static void Initialize()
    {
    }

    public static void SetMood(
      MBMusicManagerOld.MusicMood moodType,
      float intensity,
      bool holdIntensity,
      bool immediately = false)
    {
    }

    public static MBMusicManagerOld.MusicMood GetCurrentMood()
    {
      if (PsaiCore.IsInstanceInitialized())
      {
        PsaiInfo psaiInfo = PsaiCore.Instance.GetPsaiInfo();
        if (psaiInfo.psaiState == PsaiState.playing)
          return (MBMusicManagerOld.MusicMood) psaiInfo.effectiveThemeId;
      }
      return MBMusicManagerOld.MusicMood.None;
    }

    public static void Update(float dt)
    {
      int num = (int) PsaiCore.Instance.Update();
    }

    public static void StopMusic(bool immediately)
    {
    }

    public static float GetCurrentIntensity() => PsaiCore.IsInstanceInitialized() ? PsaiCore.Instance.GetPsaiInfo().currentIntensity : 0.0f;

    public static void EnterMenuMode(int menuThemeID)
    {
    }

    public static void LeaveMenuMode()
    {
    }

    public enum MusicMood
    {
      None = -1, // 0xFFFFFFFF
      LocationStandardDay = 1,
      AseraiTeaser = 4,
      Arena = 6,
      CombatTurnsOutNegative = 9,
      CombatMediumSize = 10, // 0x0000000A
      CombatTurnsOutPositive = 11, // 0x0000000B
      CombatSmallSize = 12, // 0x0000000C
      CombatSiege = 13, // 0x0000000D
      CombatNegativeEvent = 14, // 0x0000000E
      CombatPositiveEvent = 15, // 0x0000000F
      BattaniaTeaser = 17, // 0x00000011
      SturgiaTeaser = 19, // 0x00000013
      KhuzaitTeaser = 21, // 0x00000015
      EmpireTeaser = 23, // 0x00000017
      VlandiaTeaser = 25, // 0x00000019
      BattleDefeated = 26, // 0x0000001A
      CombatPaganA = 27, // 0x0000001B
    }
  }
}
