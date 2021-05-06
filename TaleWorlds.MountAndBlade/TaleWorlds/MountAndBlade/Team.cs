// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Team
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class Team : IMissionTeam
  {
    public readonly MBTeam MBTeam;
    private List<Formation> _formations;
    private List<List<Formation>> _recentlySplitFormationBatches;
    private Banner _banner;
    private List<OrderController> _orderControllers;
    private List<Agent> _activeAgents;
    private List<Agent> _teamAgents;
    private List<(float, WorldPosition, int, Vec2, Vec2, bool)> _cachedEnemyDataForFleeing;
    private static Team _invalid;

    public event Action<Team, Formation> OnFormationsChanged;

    public event OnOrderIssuedDelegate OnOrderIssued;

    public event Action<Formation> OnFormationAIActiveBehaviourChanged;

    public BattleSideEnum Side { get; }

    public IEnumerable<Formation> Formations => this._formations.Take<Formation>(8).Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0));

    public IEnumerable<Formation> FormationsIncludingEmpty => this._formations.Take<Formation>(8);

    public IEnumerable<Formation> SpecialFormations => this._formations.Skip<Formation>(8).Take<Formation>(2);

    public IEnumerable<Formation> FormationsIncludingSpecial => this._formations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0));

    public List<Formation> FormationsIncludingSpecialAndEmpty => this._formations;

    public TeamAIComponent TeamAI { get; private set; }

    public bool IsPlayerTeam => Mission.Current.PlayerTeam == this;

    public bool IsPlayerAlly => Mission.Current.PlayerTeam != null && Mission.Current.PlayerTeam.Side == this.Side;

    public bool IsDefender => this.Side == BattleSideEnum.Defender;

    public bool IsAttacker => this.Side == BattleSideEnum.Attacker;

    public uint Color { get; private set; }

    public uint Color2 { get; private set; }

    public Banner Banner => this._banner;

    public OrderController MasterOrderController => this._orderControllers[0];

    public OrderController PlayerOrderController => this._orderControllers[1];

    public TeamQuerySystem QuerySystem { get; private set; }

    internal DetachmentManager DetachmentManager { get; private set; }

    public bool IsPlayerGeneral { get; private set; }

    public bool IsPlayerSergeant { get; private set; }

    public MBReadOnlyList<Agent> ActiveAgents { get; private set; }

    public MBReadOnlyList<Agent> TeamAgents { get; private set; }

    public MBReadOnlyList<(float, WorldPosition, int, Vec2, Vec2, bool)> CachedEnemyDataForFleeing { get; private set; }

    public int TeamIndex => this.MBTeam.Index;

    public Team(MBTeam mbTeam, BattleSideEnum side, uint color = 4294967295, uint color2 = 4294967295, Banner banner = null)
    {
      this.MBTeam = mbTeam;
      this.Side = side;
      this.Color = color;
      this.Color2 = color2;
      this._banner = banner;
      this.IsPlayerGeneral = true;
      this.IsPlayerSergeant = false;
      if (this != Team._invalid)
        this.Initialize();
      this.MoraleChangeFactor = 1f;
    }

    public void UpdateCachedEnemyDataForFleeing()
    {
      if (!this._cachedEnemyDataForFleeing.IsEmpty<(float, WorldPosition, int, Vec2, Vec2, bool)>())
        return;
      foreach (Formation formation in this.QuerySystem.EnemyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (t => t.Team.FormationsIncludingSpecial)))
      {
        int countOfUnits = formation.CountOfUnits;
        if (countOfUnits > 0)
        {
          WorldPosition medianPosition = formation.QuerySystem.MedianPosition;
          float movementSpeedMaximum = formation.QuerySystem.MovementSpeedMaximum;
          bool flag = (formation.QuerySystem.IsCavalryFormation || formation.QuerySystem.IsRangedCavalryFormation) && formation.HasAnyMountedUnit;
          if (countOfUnits == 1)
          {
            Vec2 asVec2 = medianPosition.AsVec2;
            this._cachedEnemyDataForFleeing.Add((movementSpeedMaximum, medianPosition, countOfUnits, asVec2, asVec2, flag));
          }
          else
          {
            Vec2 vec2_1 = formation.QuerySystem.EstimatedDirection.LeftVec();
            float num = formation.Width / 2f;
            Vec2 vec2_2 = medianPosition.AsVec2 - vec2_1 * num;
            Vec2 vec2_3 = medianPosition.AsVec2 + vec2_1 * num;
            this._cachedEnemyDataForFleeing.Add((movementSpeedMaximum, medianPosition, countOfUnits, vec2_2, vec2_3, flag));
          }
        }
      }
    }

    public float MoraleChangeFactor { get; private set; }

    private void Initialize()
    {
      this._activeAgents = new List<Agent>();
      this.ActiveAgents = this._activeAgents.GetReadOnlyList<Agent>();
      this._teamAgents = new List<Agent>();
      this.TeamAgents = this._teamAgents.GetReadOnlyList<Agent>();
      this._cachedEnemyDataForFleeing = new List<(float, WorldPosition, int, Vec2, Vec2, bool)>();
      this.CachedEnemyDataForFleeing = this._cachedEnemyDataForFleeing.GetReadOnlyList<(float, WorldPosition, int, Vec2, Vec2, bool)>();
      if (GameNetwork.IsReplay)
        return;
      this._formations = new List<Formation>();
      for (int index = 0; index < 10; ++index)
      {
        Formation formation = new Formation(this, index);
        this._formations.Add(formation);
        formation.AI.OnActiveBehaviorChanged += new Action<Formation>(this.FormationAI_OnActiveBehaviourChanged);
      }
      this._orderControllers = new List<OrderController>();
      OrderController orderController1 = new OrderController(Mission.Current, this, (Agent) null);
      this._orderControllers.Add(orderController1);
      orderController1.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
      OrderController orderController2 = new OrderController(Mission.Current, this, (Agent) null);
      this._orderControllers.Add(orderController2);
      orderController2.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
      this.QuerySystem = new TeamQuerySystem(this);
      this.DetachmentManager = new DetachmentManager(this);
      this._recentlySplitFormationBatches = new List<List<Formation>>();
    }

    internal void Reset()
    {
      if (!GameNetwork.IsReplay)
      {
        foreach (Formation formation in this._formations)
          formation.Reset();
        if (this._orderControllers.Count > 2)
        {
          for (int index = this._orderControllers.Count - 1; index >= 2; --index)
          {
            this._orderControllers[index].OnOrderIssued -= new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
            this._orderControllers.RemoveAt(index);
          }
        }
        this.QuerySystem = new TeamQuerySystem(this);
      }
      this._teamAgents.Clear();
      this._activeAgents.Clear();
    }

    internal void Clear()
    {
      if (!GameNetwork.IsReplay)
      {
        foreach (Formation formation in this._formations)
          formation.AI.OnActiveBehaviorChanged -= new Action<Formation>(this.FormationAI_OnActiveBehaviourChanged);
      }
      this.Reset();
    }

    private void OrderController_OnOrderIssued(
      OrderType orderType,
      IEnumerable<Formation> appliedFormations,
      params object[] delegateParams)
    {
      OnOrderIssuedDelegate onOrderIssued = this.OnOrderIssued;
      if (onOrderIssued == null)
        return;
      onOrderIssued(orderType, appliedFormations, delegateParams);
    }

    internal static bool DoesFirstFormationClassContainSecond(FormationClass f1, FormationClass f2) => (f1 & f2) == f2;

    internal static FormationClass GetFormationFormationClass(Formation f)
    {
      if (f.QuerySystem.IsRangedCavalryFormation)
        return FormationClass.HorseArcher;
      if (f.QuerySystem.IsCavalryFormation)
        return FormationClass.Cavalry;
      return !f.QuerySystem.IsRangedFormation ? FormationClass.Infantry : FormationClass.Ranged;
    }

    internal static FormationClass GetPlayerTeamFormationClass()
    {
      Agent mainAgent = Mission.Current.MainAgent;
      if (mainAgent.IsRangedCached && mainAgent.HasMount)
        return FormationClass.HorseArcher;
      if (mainAgent.IsRangedCached)
        return FormationClass.Ranged;
      return mainAgent.HasMount ? FormationClass.Cavalry : FormationClass.Infantry;
    }

    public void AssignPlayerAsSergeantOfFormation(MissionPeer peer, FormationClass formationClass)
    {
      Formation formation = this.GetFormation(formationClass);
      formation.PlayerOwner = peer.ControlledAgent;
      formation.BannerCode = peer.Peer.BannerCode;
      if (peer.IsMine)
        this.PlayerOrderController.Owner = peer.ControlledAgent;
      else
        this.GetOrderControllerOf(peer.ControlledAgent).Owner = peer.ControlledAgent;
      formation.IsAIControlled = false;
      foreach (MissionBehaviour missionBehaviour in Mission.Current.MissionBehaviours)
        missionBehaviour.OnAssignPlayerAsSergeantOfFormation(peer.ControlledAgent);
      if (peer.IsMine)
        this.PlayerOrderController.SelectAllFormations();
      peer.ControlledFormation = formation;
      if (!GameNetwork.IsServer)
        return;
      peer.ControlledAgent.UpdateCachedAndFormationValues(false, false);
      if (peer.IsMine)
        return;
      GameNetwork.BeginModuleEventAsServer(peer.GetNetworkPeer());
      GameNetwork.WriteMessage((GameNetworkMessage) new AssignFormationToPlayer(peer.GetNetworkPeer(), formationClass));
      GameNetwork.EndModuleEventAsServer();
    }

    private void FormationAI_OnActiveBehaviourChanged(Formation formation)
    {
      if (formation.CountOfUnits <= 0)
        return;
      Action<Formation> behaviourChanged = this.OnFormationAIActiveBehaviourChanged;
      if (behaviourChanged == null)
        return;
      behaviourChanged(formation);
    }

    public void AddTacticOption(TacticComponent tacticOption)
    {
      if (!this.HasTeamAi)
        return;
      this.TeamAI.AddTacticOption(tacticOption);
    }

    public void RemoveTacticOption(System.Type tacticType)
    {
      if (!this.HasTeamAi)
        return;
      this.TeamAI.RemoveTacticOption(tacticType);
    }

    public void ClearTacticOptions()
    {
      if (!this.HasTeamAi)
        return;
      this.TeamAI.ClearTacticOptions();
    }

    public void ResetTactic()
    {
      if (!this.HasTeamAi)
        return;
      this.TeamAI.ResetTactic();
    }

    public void AddTeamAI(TeamAIComponent teamAI, bool forceNotAIControlled = false)
    {
      if (teamAI == null)
        return;
      this.TeamAI = teamAI;
      foreach (Formation formation in this._formations)
        formation.IsAIControlled = !forceNotAIControlled && (this != Mission.Current.PlayerTeam || !this.IsPlayerGeneral);
      this.TeamAI.InitializeDetachments(Mission.Current);
      this.TeamAI.CreateMissionSpecificBehaviours();
      this.TeamAI.ResetTactic();
      foreach (Formation formation in this.FormationsIncludingSpecial)
        formation.AI.Tick();
      this.TeamAI.TickOccasionally();
    }

    internal void DelegateCommandToAI()
    {
      foreach (Formation formation in this.FormationsIncludingEmpty)
        formation.IsAIControlled = true;
    }

    internal void ClearRecentlySplitFormations(Formation formation) => this._recentlySplitFormationBatches.RemoveAll((Predicate<List<Formation>>) (rsfb => rsfb.Any<Formation>((Func<Formation, bool>) (rsf => rsf == formation))));

    internal void ClearRecentlySplitFormations() => this._recentlySplitFormationBatches.Clear();

    internal void RegisterRecentlySplitFormations(List<Formation> recentlySplitFormations)
    {
      foreach (Formation recentlySplitFormation in recentlySplitFormations)
        recentlySplitFormation.IsAITickedAfterSplit = false;
      this._recentlySplitFormationBatches.Add(recentlySplitFormations);
    }

    private void RearrangeRecentlySplitFormations(List<Formation> recentlySplitFormations)
    {
      // ISSUE: unable to decompile the method.
    }

    internal Formation GeneralsFormation { get; set; }

    internal Formation BodyGuardFormation { get; set; }

    public Agent GeneralAgent { get; internal set; }

    internal void OnFormationUnitsSpawned()
    {
      foreach (MissionBehaviour missionBehaviour in Mission.Current.MissionBehaviours)
        missionBehaviour.OnFormationUnitsSpawned(this);
    }

    public void Tick(float dt)
    {
      if (!this._cachedEnemyDataForFleeing.IsEmpty<(float, WorldPosition, int, Vec2, Vec2, bool)>())
        this._cachedEnemyDataForFleeing.Clear();
      Mission current = Mission.Current;
      if (current.AllowAiTicking)
      {
        if (current.RetreatSide != BattleSideEnum.None && this.Side == current.RetreatSide)
        {
          foreach (Formation formation in this.FormationsIncludingSpecialAndEmpty)
          {
            if (formation.CountOfUnits > 0)
              formation.MovementOrder = MovementOrder.MovementOrderRetreat;
          }
        }
        else if (this.TeamAI != null && this.HasBots)
          this.TeamAI.Tick(dt);
      }
      if (!GameNetwork.IsReplay)
      {
        this.DetachmentManager.TickDetachments();
        foreach (Formation formation in this.FormationsIncludingSpecialAndEmpty)
        {
          if (formation.CountOfUnits > 0)
            formation.Tick(dt);
        }
      }
      if (GameNetwork.IsClientOrReplay || this._recentlySplitFormationBatches.Count <= 0)
        return;
      foreach (List<Formation> formationList in this._recentlySplitFormationBatches.ToList<List<Formation>>())
      {
        if (formationList.All<Formation>((Func<Formation, bool>) (rsf => rsf.IsAITickedAfterSplit)))
        {
          this.RearrangeRecentlySplitFormations(formationList);
          this._recentlySplitFormationBatches.Remove(formationList);
        }
      }
    }

    public Formation GetFormation(FormationClass formationClass) => this._formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.FormationIndex == formationClass));

    public void SetIsEnemyOf(Team otherTeam, bool isEnemyOf)
    {
      this.MBTeam.SetIsEnemyOf(otherTeam.MBTeam, isEnemyOf);
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new TeamSetIsEnemyOf(this, otherTeam, isEnemyOf));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    public bool IsEnemyOf(Team otherTeam) => this.MBTeam.IsEnemyOf(otherTeam.MBTeam);

    public bool IsFriendOf(Team otherTeam) => !this.MBTeam.IsEnemyOf(otherTeam.MBTeam);

    internal IEnumerable<Agent> Heroes
    {
      get
      {
        Team team = this;
        Agent main = Agent.Main;
        if (main != null && main.Team == team)
          yield return main;
      }
    }

    internal void AddAgentToTeam(Agent unit)
    {
      this._teamAgents.Add(unit);
      this._activeAgents.Add(unit);
    }

    internal void RemoveAgentFromTeam(Agent unit)
    {
      this._teamAgents.Remove(unit);
      this._activeAgents.Remove(unit);
    }

    internal void DeactivateAgent(Agent agent) => this._activeAgents.Remove(agent);

    public bool HasBots
    {
      get
      {
        bool flag = false;
        for (int index = 0; index < this.ActiveAgents.Count; ++index)
        {
          if (!this.ActiveAgents[index].IsMount && !this.ActiveAgents[index].IsPlayerControlled)
          {
            flag = true;
            break;
          }
        }
        return flag;
      }
    }

    public Agent Leader
    {
      get
      {
        if (Agent.Main != null && Agent.Main.Team == this)
          return Agent.Main;
        Agent agent = (Agent) null;
        for (int index = 0; index < this.ActiveAgents.Count; ++index)
        {
          if (agent == null || this.ActiveAgents[index].IsHero)
          {
            agent = this.ActiveAgents[index];
            if (agent.IsHero)
              break;
          }
        }
        return agent;
      }
    }

    public static Team Invalid
    {
      get
      {
        if (Team._invalid == null)
          Team._invalid = new Team(MBTeam.InvalidTeam, BattleSideEnum.None);
        return Team._invalid;
      }
      internal set => Team._invalid = value;
    }

    public bool IsValid => this.MBTeam.IsValid;

    public override string ToString() => this.MBTeam.ToString();

    public bool HasTeamAi => this.TeamAI != null;

    internal void OnMissionEnded()
    {
      if (!this.HasTeamAi)
        return;
      this.TeamAI.OnMissionEnded();
    }

    internal void TriggerOnFormationsChanged(Formation formation)
    {
      if (this.OnFormationsChanged == null)
        return;
      this.OnFormationsChanged(this, formation);
    }

    public OrderController GetOrderControllerOf(Agent agent)
    {
      OrderController orderController = this._orderControllers.FirstOrDefault<OrderController>((Func<OrderController, bool>) (oc => oc.Owner == agent));
      if (orderController == null)
      {
        orderController = new OrderController(Mission.Current, this, agent);
        this._orderControllers.Add(orderController);
        orderController.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
      }
      return orderController;
    }

    public void ExpireAIQuerySystem() => this.QuerySystem.Expire();

    public void SetPlayerRole(bool isPlayerGeneral, bool isPlayerSergeant)
    {
      this.IsPlayerGeneral = isPlayerGeneral;
      this.IsPlayerSergeant = isPlayerSergeant;
      foreach (Formation formation in this._formations)
        formation.IsAIControlled = this != Mission.Current.PlayerTeam || !this.IsPlayerGeneral;
    }

    public bool HasAnyEnemyTeamsWithAgents(bool ignoreMountedAgents)
    {
      Mission current = Mission.Current;
      for (int index1 = 0; index1 < current.Teams.Count; ++index1)
      {
        Team team = current.Teams[index1];
        if (team != this && team.IsEnemyOf(this) && team.ActiveAgents.Count > 0)
        {
          if (!ignoreMountedAgents)
            return true;
          for (int index2 = 0; index2 < team.ActiveAgents.Count; ++index2)
          {
            if (!team.ActiveAgents[index2].HasMount)
              return true;
          }
        }
      }
      return false;
    }

    [Conditional("DEBUG")]
    private void TickStandingPointDebug()
    {
      foreach (IDetachment detachment in this.DetachmentManager.Detachments)
      {
        if (detachment is UsableMachine)
        {
          foreach (StandingPoint standingPoint in (detachment as UsableMachine).StandingPoints)
          {
            if (standingPoint.HasAIMovingTo)
            {
              foreach (KeyValuePair<Agent, UsableMissionObject.MoveInfo> movingAgent in standingPoint.MovingAgents)
              {
                Vec3 position = movingAgent.Key.Position;
                Vec3 globalPosition = standingPoint.GameEntity.GlobalPosition;
              }
            }
          }
        }
        else
        {
          foreach (Agent agent in detachment.Agents)
          {
            Vec3 position = agent.Position;
            WorldFrame? agentFrame = detachment.GetAgentFrame(agent);
            ref WorldFrame? local = ref agentFrame;
            int num = (local.HasValue ? new WorldPosition?(local.GetValueOrDefault().Origin) : new WorldPosition?()).HasValue ? 1 : 0;
          }
        }
      }
      foreach (StrategicArea strategicArea in Mission.Current.MissionObjects.OfType<StrategicArea>())
        ;
      foreach (DestructableComponent destructableComponent in Mission.Current.MissionObjects.OfType<DestructableComponent>())
      {
        if ((NativeObject) destructableComponent.GameEntity.Parent != (NativeObject) null && destructableComponent.GameEntity.Parent.HasScriptOfType<WallSegment>())
          destructableComponent.GameEntity.Parent.GetGlobalFrame().TransformToParent((destructableComponent.GameEntity.GetBoundingBoxMax() + destructableComponent.GameEntity.GetBoundingBoxMin()) * 0.5f);
      }
    }
  }
}
