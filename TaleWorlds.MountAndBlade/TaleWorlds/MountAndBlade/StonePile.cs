// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.StonePile
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class StonePile : UsableMachine, IDetachment
  {
    private static readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create(nameof (act_pickup_boulder_begin));
    private static readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create(nameof (act_pickup_boulder_end));
    private const float EnemyInRangeTimerDuration = 0.5f;
    private const float EnemyWaitTimeLimit = 3f;
    public string ThrowingPointTag = "throwing";
    public int StartingAmmoCount = 12;
    public string GivenItemID;
    private ItemObject _givenItem;
    public bool IsWaiting;
    private List<StonePile.ThrowingPoint> _throwingPoints;
    private List<StonePile.VolumeBoxTimerPair> _volumeBoxTimerPairs;
    private Timer _tickOccasionallyTimer;

    public int AmmoCount { get; protected set; }

    public bool HasThrowingPointUsed
    {
      get
      {
        foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
        {
          if (throwingPoint.StandingPoint.HasUser || throwingPoint.StandingPoint.HasAIMovingTo)
            return true;
        }
        return false;
      }
    }

    public virtual BattleSideEnum Side => BattleSideEnum.Defender;

    public override int MaxUserCount => this._throwingPoints.Count;

    protected StonePile()
    {
    }

    protected void ConsumeAmmo()
    {
      --this.AmmoCount;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetStonePileAmmo(this, this.AmmoCount));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.UpdateAmmoMesh();
      this.CheckAmmo();
    }

    public void SetAmmo(int ammoLeft)
    {
      if (this.AmmoCount == ammoLeft)
        return;
      this.AmmoCount = ammoLeft;
      this.UpdateAmmoMesh();
      this.CheckAmmo();
    }

    protected virtual void CheckAmmo()
    {
      if (this.AmmoCount > 0)
        return;
      foreach (UsableMissionObject ammoPickUpPoint in this.AmmoPickUpPoints)
        ammoPickUpPoint.IsDeactivated = true;
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this._tickOccasionallyTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), (float) (0.5 + (double) MBRandom.RandomFloat * 0.5));
      this._givenItem = Game.Current.ObjectManager.GetObject<ItemObject>(this.GivenItemID);
      List<VolumeBox> source = this.GameEntity.CollectObjects<VolumeBox>();
      this._throwingPoints = new List<StonePile.ThrowingPoint>();
      this._volumeBoxTimerPairs = new List<StonePile.VolumeBoxTimerPair>();
      foreach (StandingPointWithWeaponRequirement weaponRequirement in this.StandingPoints.OfType<StandingPointWithWeaponRequirement>())
      {
        if (weaponRequirement.GameEntity.HasTag(this.AmmoPickUpTag))
        {
          weaponRequirement.InitGivenWeapon(this._givenItem);
          weaponRequirement.SetHasAlternative(true);
        }
        else if (weaponRequirement.GameEntity.HasTag(this.ThrowingPointTag))
        {
          weaponRequirement.InitRequiredWeapon(this._givenItem);
          StonePile.ThrowingPoint throwingPoint = new StonePile.ThrowingPoint();
          throwingPoint.StandingPoint = weaponRequirement as StandingPointWithVolumeBox;
          throwingPoint.AmmoPickUpPoint = (StandingPointWithWeaponRequirement) null;
          throwingPoint.AttackEntity = (GameEntity) null;
          bool flag = false;
          for (int index = 0; index < this._volumeBoxTimerPairs.Count && !flag; ++index)
          {
            if (this._volumeBoxTimerPairs[index].VolumeBox.GameEntity.HasTag(throwingPoint.StandingPoint.VolumeBoxTag))
            {
              throwingPoint.EnemyInRangeTimer = this._volumeBoxTimerPairs[index].Timer;
              flag = true;
            }
          }
          if (!flag)
          {
            VolumeBox volumeBox = source.FirstOrDefault<VolumeBox>((Func<VolumeBox, bool>) (vb => vb.GameEntity.HasTag(throwingPoint.StandingPoint.VolumeBoxTag)));
            StonePile.VolumeBoxTimerPair volumeBoxTimerPair = new StonePile.VolumeBoxTimerPair();
            volumeBoxTimerPair.VolumeBox = volumeBox;
            volumeBoxTimerPair.Timer = new Timer(-3.5f, 0.5f, false);
            throwingPoint.EnemyInRangeTimer = volumeBoxTimerPair.Timer;
            this._volumeBoxTimerPairs.Add(volumeBoxTimerPair);
          }
          this._throwingPoints.Add(throwingPoint);
        }
      }
      this.EnemyRangeToStopUsing = 5f;
      this.AmmoCount = this.StartingAmmoCount;
      this.UpdateAmmoMesh();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected internal override void OnMissionReset()
    {
      base.OnMissionReset();
      this.AmmoCount = this.StartingAmmoCount;
      this.UpdateAmmoMesh();
      foreach (UsableMissionObject ammoPickUpPoint in this.AmmoPickUpPoints)
        ammoPickUpPoint.IsDeactivated = false;
      foreach (StonePile.VolumeBoxTimerPair volumeBoxTimerPair in this._volumeBoxTimerPairs)
        volumeBoxTimerPair.Timer.Reset(-3.5f);
      foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
        throwingPoint.AmmoPickUpPoint = (StandingPointWithWeaponRequirement) null;
    }

    public override void AfterMissionStart()
    {
      if (this.AmmoPickUpPoints != null)
      {
        foreach (UsableMissionObject ammoPickUpPoint in this.AmmoPickUpPoints)
          ammoPickUpPoint.LockUserFrames = true;
      }
      if (this._throwingPoints == null)
        return;
      foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
      {
        throwingPoint.StandingPoint.IsDisabledForPlayers = true;
        throwingPoint.StandingPoint.LockUserFrames = false;
        throwingPoint.StandingPoint.LockUserPositions = true;
      }
    }

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      if (!usableGameObject.GameEntity.HasTag(this.AmmoPickUpTag))
        return TextObject.Empty;
      TextObject textObject = new TextObject("{=jfcceEoE}{PILE_TYPE} Pile");
      textObject.SetTextVariable("PILE_TYPE", new TextObject("{=1CPdu9K0}Stone"));
      return textObject;
    }

    public override string GetDescriptionText(GameEntity gameEntity = null)
    {
      if (!gameEntity.HasTag(this.AmmoPickUpTag))
        return string.Empty;
      TextObject textObject = new TextObject("{=bNYm3K6b}({KEY}) Pick Up");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject.ToString();
    }

    public override UsableMachineAIBase CreateAIBehaviourObject() => (UsableMachineAIBase) new StonePileAI(this);

    public override bool IsInRangeToCheckAlternativePoints(Agent agent)
    {
      float num = this.StandingPoints.Count > 0 ? agent.GetInteractionDistanceToUsable((IUsable) this.StandingPoints[0]) + 2f : 2f;
      return (double) this.GameEntity.GlobalPosition.DistanceSquared(agent.Position) < (double) num * (double) num;
    }

    public override StandingPoint GetBestPointAlternativeTo(
      StandingPoint standingPoint,
      Agent agent)
    {
      if (!this.AmmoPickUpPoints.Contains(standingPoint))
        return standingPoint;
      Vec3 globalPosition = standingPoint.GameEntity.GlobalPosition;
      float num1 = globalPosition.DistanceSquared(agent.Position);
      StandingPoint standingPoint1 = standingPoint;
      foreach (StandingPoint ammoPickUpPoint in this.AmmoPickUpPoints)
      {
        globalPosition = ammoPickUpPoint.GameEntity.GlobalPosition;
        float num2 = globalPosition.DistanceSquared(agent.Position);
        if ((double) num2 < (double) num1 && (!ammoPickUpPoint.HasUser && !ammoPickUpPoint.HasAIMovingTo || ammoPickUpPoint.IsInstantUse) && (!ammoPickUpPoint.IsDeactivated && !ammoPickUpPoint.IsDisabledForAgent(agent)))
        {
          num1 = num2;
          standingPoint1 = ammoPickUpPoint;
        }
      }
      return standingPoint1;
    }

    private List<Agent> GetPotentialUsersOfPile() => new List<Agent>();

    private void TickOccasionally()
    {
      if (GameNetwork.IsClientOrReplay)
        return;
      List<Formation> formationList = new List<Formation>();
      int num1 = 0;
      foreach (Team team in (ReadOnlyCollection<Team>) Mission.Current.Teams)
      {
        if (team.Side == this.Side)
        {
          foreach (Formation formation in team.FormationsIncludingSpecial)
          {
            if (formation.IsUsingMachine((UsableMachine) this))
            {
              formationList.Add(formation);
              num1 += formation.UnitsWithoutLooseDetachedOnes.Count<IFormationUnit>();
            }
          }
        }
      }
      if (this.IsDisabledForBattleSide(this.Side) || this.AmmoCount <= 0 && !this.HasThrowingPointUsed)
      {
        foreach (Formation formation in formationList)
          formation.StopUsingMachine((UsableMachine) this);
      }
      else if (formationList.Count == 0)
      {
        float minDistanceSquared = float.MaxValue;
        Formation bestFormation = (Formation) null;
        foreach (Team team in (ReadOnlyCollection<Team>) Mission.Current.Teams)
        {
          if (team.Side == this.Side)
          {
            foreach (Formation formation1 in team.Formations)
            {
              Formation formation = formation1;
              if (formation.CountOfUnitsWithoutLooseDetachedOnes >= this.MaxUserCount)
                formation.ApplyActionOnEachUnit((Action<Agent>) (agent =>
                {
                  float num2 = agent.Position.DistanceSquared(this.GameEntity.GlobalPosition);
                  if ((double) minDistanceSquared <= (double) num2)
                    return;
                  minDistanceSquared = num2;
                  bestFormation = formation;
                }));
            }
          }
        }
        Formation formation2 = bestFormation;
        if (formation2 == null)
          return;
        formation2.StartUsingMachine((UsableMachine) this);
      }
      else if (num1 == 0 && this.StandingPoints.Count<StandingPoint>((Func<StandingPoint, bool>) (sp => sp.HasUser || sp.HasAIMovingTo)) == 0)
      {
        foreach (Formation formation in formationList)
          formation.StopUsingMachine((UsableMachine) this);
      }
      else
      {
        List<GameEntity> gameEntityList = (List<GameEntity>) null;
        foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
        {
          if (throwingPoint.StandingPoint.HasAIUser)
          {
            if (gameEntityList == null)
            {
              gameEntityList = this.GetEnemySiegeWeapons();
              if (gameEntityList == null)
              {
                using (List<StonePile.ThrowingPoint>.Enumerator enumerator = this._throwingPoints.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                    enumerator.Current.AttackEntity = (GameEntity) null;
                  break;
                }
              }
            }
            Agent userAgent = throwingPoint.StandingPoint.UserAgent;
            GameEntity attackEntity = throwingPoint.AttackEntity;
            if ((NativeObject) attackEntity != (NativeObject) null && (!gameEntityList.Contains(attackEntity) || !this.CanShootAtEntity(userAgent, attackEntity)))
              throwingPoint.AttackEntity = (GameEntity) null;
            if ((NativeObject) throwingPoint.AttackEntity == (NativeObject) null && gameEntityList != null)
            {
              foreach (GameEntity entity in gameEntityList)
              {
                if ((NativeObject) attackEntity != (NativeObject) entity && this.CanShootAtEntity(userAgent, entity))
                {
                  throwingPoint.AttackEntity = entity;
                  break;
                }
              }
            }
          }
        }
        foreach (StonePile.VolumeBoxTimerPair volumeBoxTimerPair in this._volumeBoxTimerPairs)
        {
          if (volumeBoxTimerPair.VolumeBox.HasAgentsIn((Predicate<Agent>) (a => a.Team != null && a.Team.Side == BattleSideEnum.Attacker)))
          {
            if ((double) volumeBoxTimerPair.Timer.ElapsedTime() > 3.5)
              volumeBoxTimerPair.Timer.Reset(Mission.Current.Time);
            else
              volumeBoxTimerPair.Timer.Reset(Mission.Current.Time - 0.5f);
          }
        }
      }
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!GameNetwork.IsClientOrReplay && this._tickOccasionallyTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        this.TickOccasionally();
      if (GameNetwork.IsClientOrReplay)
        return;
      List<StandingPoint> standingPointList = (List<StandingPoint>) null;
      List<Agent> agentList = (List<Agent>) null;
      foreach (StandingPoint ammoPickUpPoint in this.AmmoPickUpPoints)
      {
        if (ammoPickUpPoint.HasUser)
        {
          ActionIndexCache currentAction = ammoPickUpPoint.UserAgent.GetCurrentAction(1);
          if (!(currentAction == StonePile.act_pickup_boulder_begin))
          {
            if (currentAction == StonePile.act_pickup_boulder_end)
            {
              MissionWeapon weapon = new MissionWeapon(this._givenItem, (ItemModifier) null, (Banner) null, (short) 1);
              Agent userAgent = ammoPickUpPoint.UserAgent;
              userAgent.EquipWeaponToExtraSlotAndWield(ref weapon);
              userAgent.StopUsingGameObject();
              this.ConsumeAmmo();
              if (userAgent.IsAIControlled)
              {
                if (agentList == null)
                  agentList = new List<Agent>();
                agentList.Add(userAgent);
              }
            }
            else if (!ammoPickUpPoint.UserAgent.SetActionChannel(1, StonePile.act_pickup_boulder_begin))
              ammoPickUpPoint.UserAgent.StopUsingGameObject();
          }
        }
        if (ammoPickUpPoint.HasAIUser || ammoPickUpPoint.HasAIMovingTo)
        {
          if (standingPointList == null)
            standingPointList = new List<StandingPoint>();
          standingPointList.Add(ammoPickUpPoint);
        }
      }
      List<StonePile.ThrowingPoint> throwingPointList = (List<StonePile.ThrowingPoint>) null;
      foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
      {
        throwingPoint.AmmoPickUpPoint = (StandingPointWithWeaponRequirement) null;
        if ((NativeObject) throwingPoint.AttackEntity != (NativeObject) null || throwingPoint.EnemyInRangeTimer.Check(Mission.Current.Time) && (double) throwingPoint.EnemyInRangeTimer.ElapsedTime() < 3.5)
        {
          throwingPoint.StandingPoint.IsDeactivated = false;
          MissionWeapon missionWeapon;
          if (throwingPoint.StandingPoint.HasAIMovingTo)
          {
            Dictionary<Agent, UsableMissionObject.MoveInfo>.Enumerator enumerator = throwingPoint.StandingPoint.MovingAgents.GetEnumerator();
            enumerator.MoveNext();
            Agent key = enumerator.Current.Key;
            EquipmentIndex wieldedItemIndex = key.GetWieldedItemIndex(Agent.HandIndex.MainHand);
            if (wieldedItemIndex != EquipmentIndex.None)
            {
              missionWeapon = key.Equipment[wieldedItemIndex];
              if (missionWeapon.Item == this._givenItem)
                continue;
            }
            key.StopUsingGameObject();
          }
          else if (throwingPoint.StandingPoint.HasUser)
          {
            Agent userAgent = throwingPoint.StandingPoint.UserAgent;
            EquipmentIndex wieldedItemIndex = userAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
            if (wieldedItemIndex != EquipmentIndex.None)
            {
              missionWeapon = userAgent.Equipment[wieldedItemIndex];
              if (missionWeapon.Item == this._givenItem)
              {
                if (userAgent.Controller == Agent.ControllerType.AI && (NativeObject) throwingPoint.AttackEntity != (NativeObject) null)
                {
                  userAgent.DisableScriptedCombatMovement();
                  userAgent.SetScriptedTargetEntityAndPosition(throwingPoint.AttackEntity, new WorldPosition(throwingPoint.AttackEntity.Scene, UIntPtr.Zero, throwingPoint.AttackEntity.GlobalPosition, false));
                  continue;
                }
                continue;
              }
            }
            userAgent.StopUsingGameObject();
            if (userAgent.Controller == Agent.ControllerType.AI)
              userAgent.DisableScriptedCombatMovement();
          }
          else
          {
            if (throwingPointList == null)
              throwingPointList = new List<StonePile.ThrowingPoint>();
            throwingPointList.Add(throwingPoint);
          }
        }
        else
          throwingPoint.StandingPoint.IsDeactivated = true;
      }
      if (standingPointList != null)
      {
        for (int index = 0; index < standingPointList.Count; ++index)
        {
          if (throwingPointList != null && throwingPointList.Count > index)
          {
            throwingPointList[index].AmmoPickUpPoint = standingPointList[index] as StandingPointWithWeaponRequirement;
          }
          else
          {
            standingPointList[index].UserAgent?.StopUsingGameObject();
            if (standingPointList[index].HasAIMovingTo)
            {
              foreach (KeyValuePair<Agent, UsableMissionObject.MoveInfo> keyValuePair in standingPointList[index].MovingAgents.ToList<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>())
                keyValuePair.Key.StopUsingGameObject();
              standingPointList[index].MovingAgents.Clear();
            }
          }
        }
      }
      if (agentList == null)
        return;
      foreach (Agent agent in agentList)
        this.AssignAgentToStandingPoint(this.GetSuitableStandingPointFor(this.Side, agent, (IEnumerable<Agent>) null, (IEnumerable<AgentValuePair<float>>) null), agent);
    }

    public override bool ReadFromNetwork()
    {
      bool bufferReadValid = base.ReadFromNetwork();
      int num = GameNetworkMessage.ReadIntFromPacket(CompressionMission.RangedSiegeWeaponAmmoCompressionInfo, ref bufferReadValid);
      if (bufferReadValid)
      {
        this.AmmoCount = num;
        this.CheckAmmo();
        this.UpdateAmmoMesh();
      }
      return bufferReadValid;
    }

    public override void WriteToNetwork()
    {
      base.WriteToNetwork();
      GameNetworkMessage.WriteIntToPacket(this.AmmoCount, CompressionMission.RangedSiegeWeaponAmmoCompressionInfo);
    }

    float? IDetachment.GetWeightOfAgentAtNextSlot(
      IEnumerable<AgentValuePair<float>> candidates,
      out Agent match)
    {
      BattleSideEnum side = candidates.First<AgentValuePair<float>>().Agent.Team.Side;
      StandingPoint standingPointFor = this.GetSuitableStandingPointFor(side, (Agent) null, (IEnumerable<Agent>) null, candidates);
      if (standingPointFor != null)
      {
        float? weightOfNextSlot = ((IDetachment) this).GetWeightOfNextSlot(side);
        match = StonePileAI.GetSuitableAgentForStandingPoint(this, standingPointFor, candidates, new List<Agent>(), weightOfNextSlot.Value);
        if (match == null)
          return new float?();
        float? nullable = weightOfNextSlot;
        float num = 1f;
        return !nullable.HasValue ? new float?() : new float?(nullable.GetValueOrDefault() * num);
      }
      match = (Agent) null;
      return new float?();
    }

    float? IDetachment.GetWeightOfAgentAtNextSlot(
      IEnumerable<Agent> candidates,
      out Agent match)
    {
      BattleSideEnum side = candidates.First<Agent>().Team.Side;
      StandingPoint standingPointFor = this.GetSuitableStandingPointFor(side, (Agent) null, candidates, (IEnumerable<AgentValuePair<float>>) null);
      if (standingPointFor != null)
      {
        match = StonePileAI.GetSuitableAgentForStandingPoint(this, standingPointFor, candidates, new List<Agent>());
        if (match == null)
          return new float?();
        float? weightOfNextSlot = ((IDetachment) this).GetWeightOfNextSlot(side);
        float num = 1f;
        return !weightOfNextSlot.HasValue ? new float?() : new float?(weightOfNextSlot.GetValueOrDefault() * num);
      }
      match = (Agent) null;
      return new float?();
    }

    protected override StandingPoint GetSuitableStandingPointFor(
      BattleSideEnum side,
      Agent agent = null,
      IEnumerable<Agent> agents = null,
      IEnumerable<AgentValuePair<float>> agentValuePairs = null)
    {
      List<Agent> agentList = new List<Agent>();
      if (agents == null)
      {
        if (agent != null)
          agentList.Add(agent);
        else if (agentValuePairs != null)
        {
          foreach (AgentValuePair<float> agentValuePair in agentValuePairs)
            agentList.Add(agentValuePair.Agent);
        }
      }
      else
        agentList = new List<Agent>(agents);
      bool flag1 = false;
      StandingPoint standingPoint1 = (StandingPoint) null;
      for (int index1 = 0; index1 < this._throwingPoints.Count && standingPoint1 == null; ++index1)
      {
        StonePile.ThrowingPoint throwingPoint = this._throwingPoints[index1];
        if (this.IsThrowingPointAssignable(throwingPoint))
        {
          bool flag2 = agentList == null;
          for (int index2 = 0; !flag2 && index2 < agentList.Count; ++index2)
            flag2 = !throwingPoint.StandingPoint.IsDisabledForAgent(agentList[index2]);
          if (flag2)
          {
            if (standingPoint1 == null)
              standingPoint1 = (StandingPoint) throwingPoint.StandingPoint;
          }
          else
            flag1 = true;
        }
      }
      for (int index1 = 0; index1 < this.StandingPoints.Count && standingPoint1 == null; ++index1)
      {
        StandingPoint standingPoint2 = this.StandingPoints[index1];
        if (!standingPoint2.IsDeactivated && (standingPoint2.IsInstantUse || !standingPoint2.HasUser && !standingPoint2.HasAIMovingTo) && (!standingPoint2.GameEntity.HasTag(this.ThrowingPointTag) && (flag1 || !standingPoint2.GameEntity.HasTag(this.AmmoPickUpTag))))
        {
          for (int index2 = 0; index2 < agentList.Count && standingPoint1 == null; ++index2)
          {
            if (!standingPoint2.IsDisabledForAgent(agentList[index2]))
              standingPoint1 = standingPoint2;
          }
          if (agentList.Count == 0)
            standingPoint1 = standingPoint2;
        }
      }
      return standingPoint1;
    }

    protected override float GetDetachmentWeightAux(BattleSideEnum side)
    {
      if (this.IsDisabledForBattleSideAI(side))
        return float.MinValue;
      this._usableStandingPoints.Clear();
      int num = 0;
      foreach (StonePile.ThrowingPoint throwingPoint in this._throwingPoints)
      {
        if (this.IsThrowingPointAssignable(throwingPoint))
          ++num;
      }
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < this.StandingPoints.Count; ++index)
      {
        StandingPoint standingPoint = this.StandingPoints[index];
        if (standingPoint.GameEntity.HasTag(this.AmmoPickUpTag) && num > 0)
        {
          --num;
          if (standingPoint.IsUsableBySide(side))
          {
            if (!standingPoint.HasAIMovingTo)
            {
              if (!flag2)
                this._usableStandingPoints.Clear();
              flag2 = true;
            }
            else if (flag2 || standingPoint.MovingAgents.FirstOrDefault<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>().Key.Formation.Team.Side != side)
              continue;
            flag1 = true;
            this._usableStandingPoints.Add((index, standingPoint));
          }
        }
      }
      this._areUsableStandingPointsVacant = flag2;
      if (!flag1)
        return float.MinValue;
      if (flag2)
        return 1f;
      return !this._isDetachmentRecentlyEvaluated ? 0.1f : 0.01f;
    }

    protected virtual void UpdateAmmoMesh()
    {
      int num = 20 - this.AmmoCount;
      if (!((NativeObject) this.GameEntity != (NativeObject) null))
        return;
      for (int metaMeshIndex = 0; metaMeshIndex < this.GameEntity.MultiMeshComponentCount; ++metaMeshIndex)
      {
        MetaMesh metaMesh = this.GameEntity.GetMetaMesh(metaMeshIndex);
        for (int meshIndex = 0; meshIndex < metaMesh.MeshCount; ++meshIndex)
          metaMesh.GetMeshAtIndex(meshIndex).SetVectorArgument(0.0f, (float) num, 0.0f, 0.0f);
      }
    }

    private bool CanShootAtEntity(Agent agent, GameEntity entity)
    {
      GameEntity collidedEntity;
      for (this.Scene.RayCastForClosestEntityOrTerrain(agent.Position + new Vec3(z: agent.GetEyeGlobalHeight()), entity.GlobalPosition, out float _, out collidedEntity); (NativeObject) collidedEntity != (NativeObject) null; collidedEntity = collidedEntity.Parent)
      {
        if ((NativeObject) collidedEntity == (NativeObject) entity)
          return true;
      }
      return false;
    }

    private List<GameEntity> GetEnemySiegeWeapons()
    {
      List<GameEntity> gameEntityList = (List<GameEntity>) null;
      if (Mission.Current.Teams.Attacker.TeamAI is TeamAISiegeComponent)
      {
        foreach (SiegeWeapon primarySiegeWeapon in (Mission.Current.Teams.Attacker.TeamAI as TeamAISiegeComponent).PrimarySiegeWeapons)
        {
          if (primarySiegeWeapon.GameEntity.GetFirstScriptOfType<DestructableComponent>() != null && primarySiegeWeapon.IsUsed)
          {
            if (gameEntityList == null)
              gameEntityList = new List<GameEntity>();
            gameEntityList.Add(primarySiegeWeapon.GameEntity);
          }
        }
      }
      return gameEntityList;
    }

    private bool IsThrowingPointAssignable(StonePile.ThrowingPoint throwingPoint, Agent agent = null)
    {
      if (throwingPoint == null || throwingPoint.StandingPoint.IsDeactivated || (throwingPoint.AmmoPickUpPoint != null || throwingPoint.StandingPoint.HasUser) || throwingPoint.StandingPoint.HasAIMovingTo)
        return false;
      if (agent == null)
        return true;
      return StonePileAI.IsAgentAssignable(agent) && !throwingPoint.StandingPoint.IsDisabledForAgent(agent);
    }

    private bool AssignAgentToStandingPoint(StandingPoint standingPoint, Agent agent)
    {
      if (standingPoint == null || agent == null || !StonePileAI.IsAgentAssignable(agent))
        return false;
      int slotIndex = this.StandingPoints.IndexOf(standingPoint);
      if (slotIndex >= 0)
      {
        ((IDetachment) this).AddAgent(agent, slotIndex);
        if (agent.Formation != null)
        {
          agent.Formation.DetachUnit(agent, this.IsLoose);
          agent.Detachment = (IDetachment) this;
          agent.DetachmentWeight = this.GetWeightOfStandingPoint(standingPoint);
          return true;
        }
      }
      return false;
    }

    private class ThrowingPoint
    {
      public StandingPointWithVolumeBox StandingPoint;
      public StandingPointWithWeaponRequirement AmmoPickUpPoint;
      public Timer EnemyInRangeTimer;
      public GameEntity AttackEntity;
    }

    private struct VolumeBoxTimerPair
    {
      public VolumeBox VolumeBox;
      public Timer Timer;
    }
  }
}
