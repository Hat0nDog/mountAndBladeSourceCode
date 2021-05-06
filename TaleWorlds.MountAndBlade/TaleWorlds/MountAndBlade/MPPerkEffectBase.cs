// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MPPerkEffectBase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MPPerkEffectBase
  {
    public virtual bool IsTickRequired => false;

    public virtual void OnUpdate(Agent agent, bool newState)
    {
    }

    public virtual void OnTick(MissionPeer peer, int tickCount)
    {
      if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0)
      {
        List<IFormationUnit> allUnits = peer?.ControlledFormation?.arrangement.GetAllUnits();
        if (allUnits == null)
          return;
        foreach (IFormationUnit formationUnit in allUnits)
        {
          if (formationUnit is Agent agent3 && agent3.IsActive())
            this.OnTick(agent3, tickCount);
        }
      }
      else
      {
        if (peer == null)
          return;
        bool? nullable = peer.ControlledAgent?.IsActive();
        bool flag = true;
        if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
          return;
        this.OnTick(peer.ControlledAgent, tickCount);
      }
    }

    public virtual void OnTick(Agent agent, int tickCount)
    {
    }

    public virtual float GetDamage(
      WeaponComponentData attackerWeapon,
      DamageTypes damageType,
      bool isAlternativeAttack)
    {
      return 0.0f;
    }

    public virtual float GetMountDamage(
      WeaponComponentData attackerWeapon,
      DamageTypes damageType,
      bool isAlternativeAttack)
    {
      return 0.0f;
    }

    public virtual float GetDamageTaken(WeaponComponentData attackerWeapon, DamageTypes damageType) => 0.0f;

    public virtual float GetMountDamageTaken(
      WeaponComponentData attackerWeapon,
      DamageTypes damageType)
    {
      return 0.0f;
    }

    public virtual float GetSpeedBonusEffectiveness(Agent attacker) => 0.0f;

    public virtual float GetShieldDamage(bool isCorrectSideBlock) => 0.0f;

    public virtual float GetShieldDamageTaken(bool isCorrectSideBlock) => 0.0f;

    public virtual float GetRangedAccuracy() => 0.0f;

    public virtual float GetThrowingWeaponSpeed(WeaponComponentData attackerWeapon) => 0.0f;

    public virtual float GetDamageInterruptionThreshold() => 0.0f;

    public virtual float GetMountManeuver() => 0.0f;

    public virtual float GetMountSpeed() => 0.0f;

    public virtual float GetRangedHeadShotDamage() => 0.0f;

    public virtual int GetGoldOnKill(float attackerValue, float victimValue) => 0;

    public virtual int GetGoldOnAssist() => 0;

    public virtual int GetRewardedGoldOnAssist() => 0;

    public virtual bool GetIsTeamRewardedOnDeath() => false;

    public virtual void CalculateRewardedGoldOnDeath(
      Agent agent,
      List<(MissionPeer, int)> teamMembers)
    {
    }

    public virtual float GetDrivenPropertyBonus(DrivenProperty drivenProperty, float baseValue) => 0.0f;

    public virtual float GetEncumbrance(bool isOnBody) => 0.0f;

    protected abstract void Deserialize(XmlNode node);
  }
}
