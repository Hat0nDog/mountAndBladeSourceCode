// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.IAgentOriginBase
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public interface IAgentOriginBase
  {
    bool IsUnderPlayersCommand { get; }

    uint FactionColor { get; }

    uint FactionColor2 { get; }

    IBattleCombatant BattleCombatant { get; }

    int UniqueSeed { get; }

    int Seed { get; }

    Banner Banner { get; }

    BasicCharacterObject Troop { get; }

    void SetWounded();

    void SetKilled();

    void SetRouted();

    void OnAgentRemoved(float agentHealth);

    void OnScoreHit(
      BasicCharacterObject victim,
      BasicCharacterObject formationCaptain,
      int damage,
      bool isFatal,
      bool isTeamKill,
      WeaponComponentData attackerWeapon);

    void SetBanner(Banner banner);
  }
}
