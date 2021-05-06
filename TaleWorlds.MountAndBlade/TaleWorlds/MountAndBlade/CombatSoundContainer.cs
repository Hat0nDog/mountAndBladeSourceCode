// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CombatSoundContainer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public static class CombatSoundContainer
  {
    public static int SoundCodeMissionCombatBluntHigh { get; private set; }

    public static int SoundCodeMissionCombatBluntLow { get; private set; }

    public static int SoundCodeMissionCombatBluntMed { get; private set; }

    public static int SoundCodeMissionCombatBoulderHigh { get; private set; }

    public static int SoundCodeMissionCombatBoulderLow { get; private set; }

    public static int SoundCodeMissionCombatBoulderMed { get; private set; }

    public static int SoundCodeMissionCombatCutHigh { get; private set; }

    public static int SoundCodeMissionCombatCutLow { get; private set; }

    public static int SoundCodeMissionCombatCutMed { get; private set; }

    public static int SoundCodeMissionCombatMissileHigh { get; private set; }

    public static int SoundCodeMissionCombatMissileLow { get; private set; }

    public static int SoundCodeMissionCombatMissileMed { get; private set; }

    public static int SoundCodeMissionCombatPierceHigh { get; private set; }

    public static int SoundCodeMissionCombatPierceLow { get; private set; }

    public static int SoundCodeMissionCombatPierceMed { get; private set; }

    public static int SoundCodeMissionCombatPunchHigh { get; private set; }

    public static int SoundCodeMissionCombatPunchLow { get; private set; }

    public static int SoundCodeMissionCombatPunchMed { get; private set; }

    public static int SoundCodeMissionCombatThrowingAxeHigh { get; private set; }

    public static int SoundCodeMissionCombatThrowingAxeLow { get; private set; }

    public static int SoundCodeMissionCombatThrowingAxeMed { get; private set; }

    public static int SoundCodeMissionCombatThrowingDaggerHigh { get; private set; }

    public static int SoundCodeMissionCombatThrowingDaggerLow { get; private set; }

    public static int SoundCodeMissionCombatThrowingDaggerMed { get; private set; }

    public static int SoundCodeMissionCombatThrowingStoneHigh { get; private set; }

    public static int SoundCodeMissionCombatThrowingStoneLow { get; private set; }

    public static int SoundCodeMissionCombatThrowingStoneMed { get; private set; }

    public static int SoundCodeMissionCombatChargeDamage { get; private set; }

    public static int SoundCodeMissionCombatKick { get; private set; }

    public static int SoundCodeMissionCombatPlayerhit { get; private set; }

    public static int SoundCodeMissionCombatWoodShieldBash { get; private set; }

    public static int SoundCodeMissionCombatMetalShieldBash { get; private set; }

    static CombatSoundContainer() => CombatSoundContainer.UpdateMissionCombatSoundCodes();

    private static void UpdateMissionCombatSoundCodes()
    {
      CombatSoundContainer.SoundCodeMissionCombatBluntHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/blunt/high");
      CombatSoundContainer.SoundCodeMissionCombatBluntLow = SoundEvent.GetEventIdFromString("event:/mission/combat/blunt/low");
      CombatSoundContainer.SoundCodeMissionCombatBluntMed = SoundEvent.GetEventIdFromString("event:/mission/combat/blunt/med");
      CombatSoundContainer.SoundCodeMissionCombatBoulderHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/boulder/high");
      CombatSoundContainer.SoundCodeMissionCombatBoulderLow = SoundEvent.GetEventIdFromString("event:/mission/combat/boulder/low");
      CombatSoundContainer.SoundCodeMissionCombatBoulderMed = SoundEvent.GetEventIdFromString("event:/mission/combat/boulder/med");
      CombatSoundContainer.SoundCodeMissionCombatCutHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/cut/high");
      CombatSoundContainer.SoundCodeMissionCombatCutLow = SoundEvent.GetEventIdFromString("event:/mission/combat/cut/low");
      CombatSoundContainer.SoundCodeMissionCombatCutMed = SoundEvent.GetEventIdFromString("event:/mission/combat/cut/med");
      CombatSoundContainer.SoundCodeMissionCombatMissileHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/missile/high");
      CombatSoundContainer.SoundCodeMissionCombatMissileLow = SoundEvent.GetEventIdFromString("event:/mission/combat/missile/low");
      CombatSoundContainer.SoundCodeMissionCombatMissileMed = SoundEvent.GetEventIdFromString("event:/mission/combat/missile/med");
      CombatSoundContainer.SoundCodeMissionCombatPierceHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/pierce/high");
      CombatSoundContainer.SoundCodeMissionCombatPierceLow = SoundEvent.GetEventIdFromString("event:/mission/combat/pierce/low");
      CombatSoundContainer.SoundCodeMissionCombatPierceMed = SoundEvent.GetEventIdFromString("event:/mission/combat/pierce/med");
      CombatSoundContainer.SoundCodeMissionCombatPunchHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/punch/high");
      CombatSoundContainer.SoundCodeMissionCombatPunchLow = SoundEvent.GetEventIdFromString("event:/mission/combat/punch/low");
      CombatSoundContainer.SoundCodeMissionCombatPunchMed = SoundEvent.GetEventIdFromString("event:/mission/combat/punch/med");
      CombatSoundContainer.SoundCodeMissionCombatThrowingAxeHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/high");
      CombatSoundContainer.SoundCodeMissionCombatThrowingAxeLow = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/low");
      CombatSoundContainer.SoundCodeMissionCombatThrowingAxeMed = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/med");
      CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/high");
      CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerLow = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/low");
      CombatSoundContainer.SoundCodeMissionCombatThrowingDaggerMed = SoundEvent.GetEventIdFromString("event:/mission/combat/throwing/med");
      CombatSoundContainer.SoundCodeMissionCombatThrowingStoneHigh = SoundEvent.GetEventIdFromString("event:/mission/combat/throwingstone/high");
      CombatSoundContainer.SoundCodeMissionCombatThrowingStoneLow = SoundEvent.GetEventIdFromString("event:/mission/combat/throwingstone/low");
      CombatSoundContainer.SoundCodeMissionCombatThrowingStoneMed = SoundEvent.GetEventIdFromString("event:/mission/combat/throwingstone/med");
      CombatSoundContainer.SoundCodeMissionCombatChargeDamage = SoundEvent.GetEventIdFromString("event:/mission/combat/charge/damage");
      CombatSoundContainer.SoundCodeMissionCombatKick = SoundEvent.GetEventIdFromString("event:/mission/combat/kick");
      CombatSoundContainer.SoundCodeMissionCombatPlayerhit = SoundEvent.GetEventIdFromString("event:/mission/combat/playerHit");
      CombatSoundContainer.SoundCodeMissionCombatWoodShieldBash = SoundEvent.GetEventIdFromString("event:/mission/combat/shield/bash");
      CombatSoundContainer.SoundCodeMissionCombatMetalShieldBash = SoundEvent.GetEventIdFromString("event:/mission/combat/shield/metal_bash");
    }
  }
}
