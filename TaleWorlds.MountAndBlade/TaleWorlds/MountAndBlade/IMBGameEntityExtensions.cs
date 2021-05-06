// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBGameEntityExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBGameEntityExtensions
  {
    [EngineMethod("create_from_weapon", false)]
    GameEntity CreateFromWeapon(
      UIntPtr scenePointer,
      in WeaponData weaponData,
      WeaponStatsData[] weaponStatsData,
      int weaponStatsDataLength,
      in WeaponData ammoWeaponData,
      WeaponStatsData[] ammoWeaponStatsData,
      int ammoWeaponStatsDataLength,
      bool showHolsterWithWeapon);

    [EngineMethod("fade_out", false)]
    void FadeOut(UIntPtr entityPointer, float interval, bool isRemovingFromScene);

    [EngineMethod("fade_in", false)]
    void FadeIn(UIntPtr entityPointer, bool resetAlpha);

    [EngineMethod("hide_if_not_fading_out", false)]
    void HideIfNotFadingOut(UIntPtr entityPointer);
  }
}
