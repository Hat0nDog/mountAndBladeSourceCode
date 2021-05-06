// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ThreatSeeker
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class ThreatSeeker
  {
    private const float SingleUnitThreatValue = 3f;
    private const float InsideWallsThreatMultiplier = 3f;
    private Threat currentThreat;
    private Agent targetAgent;
    public RangedSiegeWeapon Weapon;
    private static Vec3 randomBoxLocation = Vec3.Zero;
    public List<Vec3> WeaponPositions;
    private List<ITargetable> _potentialTargetUsableMachines;
    private List<ICastleKeyPosition> _referencePositions;

    internal ThreatSeeker(RangedSiegeWeapon weapon)
    {
      this.Weapon = weapon;
      this.WeaponPositions = new List<Vec3>();
      this.WeaponPositions.Add(this.Weapon.GameEntity.GlobalPosition);
      this.targetAgent = (Agent) null;
      IEnumerable<UsableMachine> allWithType = Mission.Current.ActiveMissionObjects.FindAllWithType<UsableMachine>();
      this._potentialTargetUsableMachines = allWithType.Where<UsableMachine>((Func<UsableMachine, bool>) (um => um is ITargetable && (um as ITargetable).GetSide() != this.Weapon.Side && (NativeObject) (um as ITargetable).GetTargetEntity() != (NativeObject) null)).Select<UsableMachine, ITargetable>((Func<UsableMachine, ITargetable>) (um => um as ITargetable)).ToList<ITargetable>();
      this._referencePositions = allWithType.OfType<ICastleKeyPosition>().ToList<ICastleKeyPosition>();
    }

    internal Threat GetTarget()
    {
      IEnumerable<Threat> allThreats = this.GetAllThreats();
      foreach (Threat threat in allThreats)
        threat.ThreatValue *= (float) (0.899999976158142 + (double) MBRandom.RandomFloat * 0.200000002980232);
      if (this.currentThreat != null)
      {
        this.currentThreat = allThreats.SingleOrDefault<Threat>((Func<Threat, bool>) (t => t.Equals((object) this.currentThreat)));
        if (this.currentThreat != null)
          this.currentThreat.ThreatValue *= 2f;
      }
      IEnumerable<Threat> source = allThreats.Where<Threat>((Func<Threat, bool>) (t =>
      {
        if ((t.WeaponEntity != null || t.Agent != null) && this.Weapon.CanShootAtBox(t.BoundingBoxMin, t.BoundingBoxMax))
          return true;
        return t.Formation != null && t.Formation.GetCountOfUnitsWithCondition((Func<Agent, bool>) (agent =>
        {
          RangedSiegeWeapon weapon = this.Weapon;
          CapsuleData collisionCapsule = agent.CollisionCapsule;
          Vec3 boxMin = collisionCapsule.GetBoxMin();
          collisionCapsule = agent.CollisionCapsule;
          Vec3 boxMax = collisionCapsule.GetBoxMax();
          return weapon.CanShootAtBox(boxMin, boxMax);
        })) > 0;
      }));
      if (source.IsEmpty<Threat>())
        return (Threat) null;
      this.currentThreat = source.MaxBy<Threat, float>((Func<Threat, float>) (t => t.ThreatValue));
      if (this.currentThreat.WeaponEntity == null)
      {
        if (this.targetAgent != null && this.targetAgent.IsActive() && this.currentThreat.Formation.HasUnitsWithCondition((Func<Agent, bool>) (agent => agent == this.targetAgent)))
        {
          RangedSiegeWeapon weapon = this.Weapon;
          CapsuleData collisionCapsule = this.targetAgent.CollisionCapsule;
          Vec3 boxMin = collisionCapsule.GetBoxMin();
          collisionCapsule = this.targetAgent.CollisionCapsule;
          Vec3 boxMax = collisionCapsule.GetBoxMax();
          if (weapon.CanShootAtBox(boxMin, boxMax))
            goto label_15;
        }
        float selectedAgentScore = float.MaxValue;
        Agent selectedAgent = this.targetAgent;
        this.currentThreat.Formation.ApplyActionOnEachUnit((Action<Agent>) (agent =>
        {
          float num = agent.Position.DistanceSquared(this.Weapon.GameEntity.GlobalPosition) * (float) ((double) MBRandom.RandomFloat * 0.200000002980232 + 0.800000011920929);
          if (agent == this.targetAgent)
            num *= 0.5f;
          if ((double) selectedAgentScore <= (double) num || !this.Weapon.CanShootAtBox(agent.CollisionCapsule.GetBoxMin(), agent.CollisionCapsule.GetBoxMax()))
            return;
          selectedAgent = agent;
          selectedAgentScore = num;
        }));
        this.targetAgent = selectedAgent ?? this.currentThreat.Formation.GetUnitWithIndex(MBRandom.RandomInt(this.currentThreat.Formation.CountOfUnits));
        this.currentThreat.Agent = this.targetAgent;
      }
label_15:
      this.targetAgent = (Agent) null;
      return this.currentThreat.WeaponEntity == null && this.currentThreat.Agent == null ? (Threat) null : this.currentThreat;
    }

    internal void Release()
    {
      this.targetAgent = (Agent) null;
      this.currentThreat = (Threat) null;
    }

    public IEnumerable<Threat> GetAllThreats()
    {
      List<Threat> threatList = new List<Threat>();
      this._potentialTargetUsableMachines.RemoveAll((Predicate<ITargetable>) (ptum =>
      {
        if (!(ptum is UsableMachine))
          return false;
        return (ptum as UsableMachine).IsDestroyed || (NativeObject) (ptum as UsableMachine).GameEntity == (NativeObject) null;
      }));
      threatList.AddRange(this._potentialTargetUsableMachines.Select<ITargetable, Threat>((Func<ITargetable, Threat>) (um => new Threat()
      {
        WeaponEntity = um,
        ThreatValue = this.Weapon.ProcessTargetValue(um.GetTargetValue(this.WeaponPositions), um.GetTargetFlags())
      })));
      foreach (Formation unemployedEnemyFormation in this.GetUnemployedEnemyFormations())
        threatList.Add(new Threat()
        {
          Formation = unemployedEnemyFormation,
          ThreatValue = this.Weapon.ProcessTargetValue(ThreatSeeker.GetTargetValueOfFormation(unemployedEnemyFormation, (IEnumerable<ICastleKeyPosition>) this._referencePositions), ThreatSeeker.GetTargetFlagsOfFormation())
        });
      return (IEnumerable<Threat>) threatList;
    }

    private static float GetTargetValueOfFormation(
      Formation formation,
      IEnumerable<ICastleKeyPosition> referencePositions)
    {
      float num = (float) formation.CountOfUnits * 3f;
      if (TeamAISiegeComponent.IsFormationInsideCastle(formation, true))
        num *= 3f;
      return num * ThreatSeeker.GetPositionMultiplierOfFormation(formation, referencePositions) * (MBMath.ClampFloat(formation.QuerySystem.LocalAllyPower / (formation.QuerySystem.LocalEnemyPower + 0.01f), 0.0f, 5f) / 5f);
    }

    public static TargetFlags GetTargetFlagsOfFormation() => (TargetFlags) (0 | 1 | 2 | 16);

    private static float GetPositionMultiplierOfFormation(
      Formation formation,
      IEnumerable<ICastleKeyPosition> referencePositions)
    {
      ICastleKeyPosition closestCastlePosition;
      float betweenPositions = ThreatSeeker.GetMinimumDistanceBetweenPositions(formation.GetMedianAgent(false, false, formation.GetAveragePositionOfUnits(false, false)).Position, referencePositions, out closestCastlePosition);
      bool flag = closestCastlePosition.AttackerSiegeWeapon != null && closestCastlePosition.AttackerSiegeWeapon.HasCompletedAction();
      return formation.IsRanged() ? (float) (((double) betweenPositions >= 20.0 ? ((double) betweenPositions >= 35.0 ? 0.600000023841858 : 0.800000011920929) : 1.0) + (flag ? 0.200000002980232 : 0.0)) : (float) (((double) betweenPositions >= 15.0 ? ((double) betweenPositions >= 40.0 ? 0.119999997317791 : 0.150000005960464) : 0.200000002980232) * (flag ? 7.5 : 1.0));
    }

    private static float GetMinimumDistanceBetweenPositions(
      Vec3 position,
      IEnumerable<ICastleKeyPosition> referencePositions,
      out ICastleKeyPosition closestCastlePosition)
    {
      if (referencePositions != null && referencePositions.Count<ICastleKeyPosition>() != 0)
      {
        closestCastlePosition = referencePositions.MinBy<ICastleKeyPosition, float>((Func<ICastleKeyPosition, float>) (rp => rp.GetPosition().DistanceSquared(position)));
        return (float) Math.Sqrt((double) closestCastlePosition.GetPosition().DistanceSquared(position));
      }
      closestCastlePosition = (ICastleKeyPosition) null;
      return -1f;
    }

    public static Threat GetMaxThreat(List<ICastleKeyPosition> castleKeyPositions)
    {
      List<ITargetable> source1 = new List<ITargetable>();
      List<Threat> source2 = new List<Threat>();
      foreach (GameEntity gameEntity in Mission.Current.ActiveMissionObjects.Select<MissionObject, GameEntity>((Func<MissionObject, GameEntity>) (amo => amo.GameEntity)))
      {
        UsableMachine firstScriptOfType = gameEntity.GetFirstScriptOfType<UsableMachine>();
        if (firstScriptOfType is ITargetable)
          source1.Add((ITargetable) firstScriptOfType);
      }
      source1.RemoveAll((Predicate<ITargetable>) (um => um.GetSide() == BattleSideEnum.Defender));
      source2.AddRange(source1.Select<ITargetable, Threat>((Func<ITargetable, Threat>) (um => new Threat()
      {
        WeaponEntity = um,
        ThreatValue = um.GetTargetValue(castleKeyPositions.Select<ICastleKeyPosition, Vec3>((Func<ICastleKeyPosition, Vec3>) (c => c.GetPosition())).ToList<Vec3>())
      })));
      return source2.MaxBy<Threat, float>((Func<Threat, float>) (t => t.ThreatValue));
    }

    private IEnumerable<Formation> GetUnemployedEnemyFormations() => Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => t.Side.GetOppositeSide() == this.Weapon.Side)).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0));
  }
}
