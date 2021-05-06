// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.IBattleObserver
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public interface IBattleObserver
  {
    void TroopNumberChanged(
      BattleSideEnum side,
      IBattleCombatant battleCombatant,
      BasicCharacterObject character,
      int number = 0,
      int numberKilled = 0,
      int numberWounded = 0,
      int numberRouted = 0,
      int killCount = 0,
      int numberReadyToUpgrade = 0);

    void HeroSkillIncreased(
      BattleSideEnum side,
      IBattleCombatant battleCombatant,
      BasicCharacterObject heroCharacter,
      SkillObject skill);

    void BattleResultsReady();
  }
}
