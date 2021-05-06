// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AI.SiegeWeaponDeploymentAI
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions;

namespace TaleWorlds.MountAndBlade.AI
{
  public class SiegeWeaponDeploymentAI
  {
    private SiegeWeaponCollection weapons;
    private List<DeploymentPoint> deploymentPoints;

    public SiegeWeaponDeploymentAI(
      List<DeploymentPoint> deploymentPoints,
      Dictionary<System.Type, int> availableWeapons)
    {
      this.deploymentPoints = deploymentPoints;
      this.weapons = new SiegeWeaponCollection(availableWeapons);
    }

    public SiegeWeaponDeploymentAI(
      List<DeploymentPoint> deploymentPoints,
      SiegeWeaponCollection availableWeapons)
    {
      this.deploymentPoints = deploymentPoints;
      this.weapons = availableWeapons;
    }

    public void DeployAll(Mission mission, BattleSideEnum side)
    {
      if (side == BattleSideEnum.Attacker)
      {
        this.DeployAllForAttackers();
      }
      else
      {
        if (side != BattleSideEnum.Defender)
          return;
        this.DeployAllForDefenders();
      }
    }

    private bool DeployWeaponFrom(DeploymentPoint dp)
    {
      IEnumerable<System.Type> source = dp.DeployableWeaponTypes.Where<System.Type>((Func<System.Type, bool>) (t => this.deploymentPoints.Count<DeploymentPoint>((Func<DeploymentPoint, bool>) (dep => dep.IsDeployed && SiegeWeaponCollection.GetWeaponType((ScriptComponentBehaviour) dep.DeployedWeapon) == t)) < this.weapons.GetMaxDeployableWeaponCount(t)));
      if (source.IsEmpty<System.Type>())
        return false;
      System.Type t1 = source.MaxBy<System.Type, float>((Func<System.Type, float>) (t => this.GetWeaponValue(t)));
      dp.Deploy(t1);
      return true;
    }

    private void DeployAllForAttackers()
    {
      List<DeploymentPoint> list = this.deploymentPoints.Where<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => !dp.IsDisabled && !dp.IsDeployed)).ToList<DeploymentPoint>();
      list.Shuffle<DeploymentPoint>();
      int num = this.deploymentPoints.Count<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => dp.GetDeploymentPointType() == DeploymentPoint.DeploymentPointType.Breach));
      bool flag = Mission.Current.AttackerTeam != Mission.Current.PlayerTeam && num >= 2;
      foreach (DeploymentPoint dp in list)
      {
        if (!flag || dp.GetDeploymentPointType() == DeploymentPoint.DeploymentPointType.Ranged)
          this.DeployWeaponFrom(dp);
      }
    }

    private void DeployAllForDefenders()
    {
      Mission current = Mission.Current;
      Scene scene = current.Scene;
      List<ICastleKeyPosition> list1 = current.ActiveMissionObjects.Select<MissionObject, GameEntity>((Func<MissionObject, GameEntity>) (amo => amo.GameEntity)).Select<GameEntity, UsableMachine>((Func<GameEntity, UsableMachine>) (e => e.GetFirstScriptOfType<UsableMachine>())).Where<UsableMachine>((Func<UsableMachine, bool>) (um => um is ICastleKeyPosition)).Cast<ICastleKeyPosition>().Where<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (x =>
      {
        IPrimarySiegeWeapon attackerSiegeWeapon = x.AttackerSiegeWeapon;
        return attackerSiegeWeapon == null || attackerSiegeWeapon.WeaponSide != FormationAI.BehaviorSide.BehaviorSideNotSet;
      })).ToList<ICastleKeyPosition>();
      List<DeploymentPoint> list2 = this.deploymentPoints.Where<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => !dp.IsDeployed)).ToList<DeploymentPoint>();
      while (!list2.IsEmpty<DeploymentPoint>())
      {
        Threat maxThreat = ThreatSeeker.GetMaxThreat(list1);
        Vec3 mostDangerousThreatPosition = maxThreat.Position;
        DeploymentPoint dp1 = list2.MinBy<DeploymentPoint, float>((Func<DeploymentPoint, float>) (dp => dp.GameEntity.GlobalPosition.DistanceSquared(mostDangerousThreatPosition)));
        if (this.DeployWeaponFrom(dp1))
          maxThreat.ThreatValue *= 0.5f;
        list2.Remove(dp1);
      }
    }

    protected virtual float GetWeaponValue(System.Type weaponType)
    {
      if (weaponType == typeof (BatteringRam) || weaponType == typeof (SiegeTower) || weaponType == typeof (SiegeLadder))
        return (float) (0.899999976158142 + (double) MBRandom.RandomFloat * 0.200000002980232);
      return typeof (RangedSiegeWeapon).IsAssignableFrom(weaponType) ? (float) (0.699999988079071 + (double) MBRandom.RandomFloat * 0.200000002980232) : 1f;
    }
  }
}
