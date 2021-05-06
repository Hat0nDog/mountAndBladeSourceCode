// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattleAgentOrigin
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class CustomBattleAgentOrigin : IAgentOriginBase
  {
    private readonly UniqueTroopDescriptor _descriptor;
    private readonly bool _isPlayerSide;
    private CustomBattleTroopSupplier _troopSupplier;
    private bool _isRemoved;

    public CustomBattleCombatant CustomBattleCombatant { get; private set; }

    IBattleCombatant IAgentOriginBase.BattleCombatant => (IBattleCombatant) this.CustomBattleCombatant;

    public BasicCharacterObject Troop { get; private set; }

    public int Rank { get; private set; }

    public Banner Banner => this.CustomBattleCombatant.Banner;

    public bool IsUnderPlayersCommand => this._isPlayerSide;

    public uint FactionColor => this.CustomBattleCombatant.BasicCulture.Color;

    public uint FactionColor2 => this.CustomBattleCombatant.BasicCulture.Color2;

    public int Seed => this.Troop.GetDefaultFaceSeed(this.Rank);

    public int UniqueSeed => this._descriptor.UniqueSeed;

    public CustomBattleAgentOrigin(
      CustomBattleCombatant customBattleCombatant,
      BasicCharacterObject characterObject,
      CustomBattleTroopSupplier troopSupplier,
      bool isPlayerSide,
      int rank = -1,
      UniqueTroopDescriptor uniqueNo = default (UniqueTroopDescriptor))
    {
      this.CustomBattleCombatant = customBattleCombatant;
      this.Troop = characterObject;
      this._descriptor = !uniqueNo.IsValid ? new UniqueTroopDescriptor(Game.Current.NextUniqueTroopSeed) : uniqueNo;
      this.Rank = rank == -1 ? MBRandom.RandomInt(10000) : rank;
      this._troopSupplier = troopSupplier;
      this._isPlayerSide = isPlayerSide;
    }

    public void SetWounded()
    {
      if (this._isRemoved)
        return;
      this._troopSupplier.OnTroopWounded();
      this._isRemoved = true;
    }

    public void SetKilled()
    {
      if (this._isRemoved)
        return;
      this._troopSupplier.OnTroopKilled();
      this._isRemoved = true;
    }

    public void SetRouted()
    {
      if (this._isRemoved)
        return;
      this._troopSupplier.OnTroopRouted();
      this._isRemoved = true;
    }

    public void OnAgentRemoved(float agentHealth)
    {
    }

    void IAgentOriginBase.OnScoreHit(
      BasicCharacterObject victim,
      BasicCharacterObject captain,
      int damage,
      bool isFatal,
      bool isTeamKill,
      WeaponComponentData attackerWeapon)
    {
    }

    public void SetBanner(Banner banner) => throw new NotImplementedException();
  }
}
