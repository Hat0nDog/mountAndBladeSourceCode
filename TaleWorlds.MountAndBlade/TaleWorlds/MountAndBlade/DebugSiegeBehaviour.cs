// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DebugSiegeBehaviour
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.InputSystem;

namespace TaleWorlds.MountAndBlade
{
  public static class DebugSiegeBehaviour
  {
    public static bool ToggleTargetDebug;
    public static DebugSiegeBehaviour.DebugStateAttacker DebugAttackState;
    public static DebugSiegeBehaviour.DebugStateDefender DebugDefendState;

    public static void SiegeDebug(UsableMachine usableMachine)
    {
      if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyAimAtRam"))
        DebugSiegeBehaviour.DebugDefendState = DebugSiegeBehaviour.DebugStateDefender.DebugDefendersToRam;
      else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyAimAtSt"))
        DebugSiegeBehaviour.DebugDefendState = DebugSiegeBehaviour.DebugStateDefender.DebugDefendersToTower;
      else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyAimAtBallistas2"))
        DebugSiegeBehaviour.DebugDefendState = DebugSiegeBehaviour.DebugStateDefender.DebugDefendersToBallistae;
      else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyAimAtMangonels2"))
        DebugSiegeBehaviour.DebugDefendState = DebugSiegeBehaviour.DebugStateDefender.DebugDefendersToMangonels;
      else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyAimAtNone2"))
        DebugSiegeBehaviour.DebugDefendState = DebugSiegeBehaviour.DebugStateDefender.None;
      else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyAimAtBallistas"))
        DebugSiegeBehaviour.DebugAttackState = DebugSiegeBehaviour.DebugStateAttacker.DebugAttackersToBallistae;
      else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyAimAtMangonels"))
        DebugSiegeBehaviour.DebugAttackState = DebugSiegeBehaviour.DebugStateAttacker.DebugAttackersToMangonels;
      else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyAimAtBattlements"))
        DebugSiegeBehaviour.DebugAttackState = DebugSiegeBehaviour.DebugStateAttacker.DebugAttackersToBattlements;
      else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyAimAtNone"))
        DebugSiegeBehaviour.DebugAttackState = DebugSiegeBehaviour.DebugStateAttacker.None;
      else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyTargetDebugActive"))
        DebugSiegeBehaviour.ToggleTargetDebug = true;
      else if (Input.DebugInput.IsHotKeyPressed("DebugSiegeBehaviourHotkeyTargetDebugDisactive"))
        DebugSiegeBehaviour.ToggleTargetDebug = false;
      int num = DebugSiegeBehaviour.ToggleTargetDebug ? 1 : 0;
    }

    public enum DebugStateAttacker
    {
      None,
      DebugAttackersToBallistae,
      DebugAttackersToMangonels,
      DebugAttackersToBattlements,
    }

    public enum DebugStateDefender
    {
      None,
      DebugDefendersToBallistae,
      DebugDefendersToMangonels,
      DebugDefendersToRam,
      DebugDefendersToTower,
    }
  }
}
