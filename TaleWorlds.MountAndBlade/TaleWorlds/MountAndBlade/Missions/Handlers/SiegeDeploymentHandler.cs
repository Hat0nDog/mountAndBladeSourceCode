// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Missions.Handlers.SiegeDeploymentHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.AI;

namespace TaleWorlds.MountAndBlade.Missions.Handlers
{
  public class SiegeDeploymentHandler : DeploymentHandler
  {
    private readonly SiegeWeaponCollection _weapons;

    public event Action OnSiegeDeploymentReady;

    public IEnumerable<DeploymentPoint> DeploymentPoints { get; private set; }

    private void OnDeploymentStateChange(DeploymentPoint deploymentPoint)
    {
      if (deploymentPoint.IsDeployed || !this.team.DetachmentManager.Detachments.Contains(deploymentPoint.DisbandedWeapon as IDetachment))
        return;
      this.team.DetachmentManager.DestroyDetachment(deploymentPoint.DisbandedWeapon as IDetachment);
    }

    public SiegeDeploymentHandler(bool isPlayerAttacker, Dictionary<System.Type, int> weaponTypeAndCounts)
      : base(isPlayerAttacker)
    {
      this._weapons = new SiegeWeaponCollection(weaponTypeAndCounts);
    }

    public override void OnFormationUnitsSpawned(Team team)
    {
      if (!team.IsPlayerTeam)
        return;
      Action siegeDeploymentReady = this.OnSiegeDeploymentReady;
      if (siegeDeploymentReady == null)
        return;
      siegeDeploymentReady();
    }

    public override void AfterStart()
    {
      base.AfterStart();
      this.DeploymentPoints = Mission.Current.ActiveMissionObjects.FindAllWithType<DeploymentPoint>().Where<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => dp.Side == this.team.Side));
      foreach (DeploymentPoint deploymentPoint in this.DeploymentPoints)
        deploymentPoint.OnDeploymentStateChanged += new Action<DeploymentPoint>(this.OnDeploymentStateChange);
      this.Mission.IsFormationUnitPositionAvailable_AdditionalCondition += new Func<WorldPosition, Team, bool>(this.Mission_IsFormationUnitPositionAvailable_AdditionalCondition);
    }

    private bool Mission_IsFormationUnitPositionAvailable_AdditionalCondition(
      WorldPosition position,
      Team team)
    {
      if (team == null || !team.IsPlayerTeam || team.Side != BattleSideEnum.Defender)
        return true;
      Scene scene = this.Mission.Scene;
      Vec3 globalPosition = scene.FindEntityWithTag("defender_infantry").GlobalPosition;
      WorldPosition position1 = new WorldPosition(scene, UIntPtr.Zero, globalPosition, false);
      return scene.DoesPathExistBetweenPositions(position1, position);
    }

    public override void FinishDeployment()
    {
      base.FinishDeployment();
      foreach (DeploymentPoint deploymentPoint in this.DeploymentPoints)
        deploymentPoint.OnDeploymentStateChanged -= new Action<DeploymentPoint>(this.OnDeploymentStateChange);
      Mission mission = this.Mission;
      mission.GetMissionBehaviour<SiegeMissionController>().OnPlayerDeploymentFinish();
      mission.IsTeleportingAgents = false;
    }

    public void DeployAllSiegeWeaponsOfPlayer()
    {
      BattleSideEnum side = this.Mission.PlayerTeam.Side;
      new SiegeWeaponDeploymentAI(this.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>().Where<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => dp.Side == side)).ToList<DeploymentPoint>(), this._weapons).DeployAll(this.Mission, side);
    }

    public int GetDeployableWeaponCount(System.Type weapon) => this._weapons.GetMaxDeployableWeaponCount(weapon) - this.DeploymentPoints.Count<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => dp.IsDeployed && SiegeWeaponCollection.GetWeaponType((ScriptComponentBehaviour) dp.DeployedWeapon) == weapon));

    public Vec2 GetEstimatedAverageDefenderPosition() => this.Mission.GetFormationSpawnFrame(BattleSideEnum.Defender, FormationClass.Infantry, false).Origin.AsVec2;

    [Conditional("DEBUG")]
    private void AssertSiegeWeapons(IEnumerable<DeploymentPoint> allDeploymentPoints)
    {
      HashSet<SynchedMissionObject> synchedMissionObjectSet = new HashSet<SynchedMissionObject>();
      foreach (SynchedMissionObject synchedMissionObject in allDeploymentPoints.SelectMany<DeploymentPoint, SynchedMissionObject>((Func<DeploymentPoint, IEnumerable<SynchedMissionObject>>) (amo => amo.DeployableWeapons)))
      {
        if (!synchedMissionObjectSet.Add(synchedMissionObject))
          break;
      }
    }

    public override void OnRemoveBehaviour()
    {
      base.OnRemoveBehaviour();
      this.Mission.IsFormationUnitPositionAvailable_AdditionalCondition -= new Func<WorldPosition, Team, bool>(this.Mission_IsFormationUnitPositionAvailable_AdditionalCondition);
    }
  }
}
