// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.OrderController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class OrderController
  {
    private static readonly ActionIndexCache act_command = ActionIndexCache.Create(nameof (act_command));
    private static readonly ActionIndexCache act_command_leftstance = ActionIndexCache.Create(nameof (act_command_leftstance));
    private static readonly ActionIndexCache act_command_unarmed = ActionIndexCache.Create(nameof (act_command_unarmed));
    private static readonly ActionIndexCache act_command_unarmed_leftstance = ActionIndexCache.Create(nameof (act_command_unarmed_leftstance));
    private static readonly ActionIndexCache act_command_2h = ActionIndexCache.Create(nameof (act_command_2h));
    private static readonly ActionIndexCache act_command_2h_leftstance = ActionIndexCache.Create(nameof (act_command_2h_leftstance));
    private static readonly ActionIndexCache act_command_bow = ActionIndexCache.Create(nameof (act_command_bow));
    private static readonly ActionIndexCache act_command_follow = ActionIndexCache.Create(nameof (act_command_follow));
    private static readonly ActionIndexCache act_command_follow_leftstance = ActionIndexCache.Create(nameof (act_command_follow_leftstance));
    private static readonly ActionIndexCache act_command_follow_unarmed = ActionIndexCache.Create(nameof (act_command_follow_unarmed));
    private static readonly ActionIndexCache act_command_follow_unarmed_leftstance = ActionIndexCache.Create(nameof (act_command_follow_unarmed_leftstance));
    private static readonly ActionIndexCache act_command_follow_2h = ActionIndexCache.Create(nameof (act_command_follow_2h));
    private static readonly ActionIndexCache act_command_follow_2h_leftstance = ActionIndexCache.Create(nameof (act_command_follow_2h_leftstance));
    private static readonly ActionIndexCache act_command_follow_bow = ActionIndexCache.Create(nameof (act_command_follow_bow));
    private static readonly ActionIndexCache act_horse_command = ActionIndexCache.Create(nameof (act_horse_command));
    private static readonly ActionIndexCache act_horse_command_unarmed = ActionIndexCache.Create(nameof (act_horse_command_unarmed));
    private static readonly ActionIndexCache act_horse_command_2h = ActionIndexCache.Create(nameof (act_horse_command_2h));
    private static readonly ActionIndexCache act_horse_command_bow = ActionIndexCache.Create(nameof (act_horse_command_bow));
    private static readonly ActionIndexCache act_horse_command_follow = ActionIndexCache.Create(nameof (act_horse_command_follow));
    private static readonly ActionIndexCache act_horse_command_follow_unarmed = ActionIndexCache.Create(nameof (act_horse_command_follow_unarmed));
    private static readonly ActionIndexCache act_horse_command_follow_2h = ActionIndexCache.Create(nameof (act_horse_command_follow_2h));
    private static readonly ActionIndexCache act_horse_command_follow_bow = ActionIndexCache.Create(nameof (act_horse_command_follow_bow));
    public const float FormationGapInLine = 1.5f;
    private readonly Mission _mission;
    private readonly Team _team;
    public Agent Owner;
    private readonly List<Formation> _selectedFormations;
    private Dictionary<Formation, float> actualWidths;
    private Dictionary<Formation, int> actualUnitSpacings;
    private List<Func<Formation, MovementOrder, MovementOrder>> orderOverrides;
    private List<(Formation, OrderType)> overridenOrders;

    public SiegeWeaponController SiegeWeaponController { get; private set; }

    public MBReadOnlyList<Formation> SelectedFormations { get; private set; }

    public event OnOrderIssuedDelegate OnOrderIssued;

    public event Action OnSelectedFormationsChanged;

    public Dictionary<Formation, Formation> simulationFormations { get; private set; }

    public OrderController(Mission mission, Team team, Agent owner)
    {
      this._mission = mission;
      this._team = team;
      this.Owner = owner;
      this._selectedFormations = new List<Formation>();
      this.SelectedFormations = new MBReadOnlyList<Formation>(this._selectedFormations);
      this.SiegeWeaponController = new SiegeWeaponController(mission, this._team);
      this.simulationFormations = new Dictionary<Formation, Formation>();
      this.actualWidths = new Dictionary<Formation, float>();
      this.actualUnitSpacings = new Dictionary<Formation, int>();
      foreach (Formation formation in this._team.FormationsIncludingEmpty)
      {
        formation.OnWidthChanged += new Action<Formation>(this.Formation_OnWidthChanged);
        formation.OnUnitSpacingChanged += new Action<Formation>(this.Formation_OnUnitSpacingChanged);
      }
      if (this._team.IsPlayerGeneral)
      {
        foreach (Formation formation in this._team.FormationsIncludingEmpty)
          formation.PlayerOwner = owner;
      }
      this.CreateDefaultOrderOverrides();
    }

    private void Formation_OnUnitSpacingChanged(Formation formation) => this.actualUnitSpacings.Remove(formation);

    private void Formation_OnWidthChanged(Formation formation) => this.actualWidths.Remove(formation);

    private void OnSelectedFormationsCollectionChanged()
    {
      Action formationsChanged = this.OnSelectedFormationsChanged;
      if (formationsChanged != null)
        formationsChanged();
      foreach (Formation key in this.SelectedFormations.Except<Formation>((IEnumerable<Formation>) this.simulationFormations.Keys))
        this.simulationFormations[key] = new Formation((Team) null, -1);
    }

    private static void SelectFormationMakeVoice(Formation formation, Agent agent)
    {
      if (!Mission.Current.IsOrderShoutingAllowed())
        return;
      switch (formation.InitialClass)
      {
        case FormationClass.Infantry:
        case FormationClass.HeavyInfantry:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Infantry, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case FormationClass.Ranged:
        case FormationClass.NumberOfDefaultFormations:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Archers, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case FormationClass.Cavalry:
        case FormationClass.LightCavalry:
        case FormationClass.HeavyCavalry:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Cavalry, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case FormationClass.HorseArcher:
          agent.MakeVoice(SkinVoiceManager.VoiceType.HorseArchers, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
      }
    }

    private void SelectFormation(Formation formation, Agent selectorAgent)
    {
      if (this._selectedFormations.Contains(formation) || !this.IsFormationSelectable(formation, selectorAgent))
        return;
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.SelectFormation(formation.Index));
        GameNetwork.EndModuleEventAsClient();
      }
      if (!GameNetwork.IsClientOrReplay && selectorAgent != null)
        OrderController.SelectFormationMakeVoice(formation, selectorAgent);
      MBDebug.Print(formation?.InitialClass.ToString() + " added to selected formations.");
      this._selectedFormations.Add(formation);
      this.OnSelectedFormationsCollectionChanged();
    }

    public void SelectFormation(Formation formation) => this.SelectFormation(formation, this.Owner);

    public void DeselectFormation(Formation formation)
    {
      if (!this._selectedFormations.Contains(formation))
        return;
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new UnselectFormation(formation.Index));
        GameNetwork.EndModuleEventAsClient();
      }
      MBDebug.Print(formation?.InitialClass.ToString() + " is removed from selected formations.");
      this._selectedFormations.Remove(formation);
      this.OnSelectedFormationsCollectionChanged();
    }

    public bool IsFormationListening(Formation formation) => this.SelectedFormations.Contains(formation);

    public bool IsFormationSelectable(Formation formation) => this.IsFormationSelectable(formation, this.Owner);

    private bool IsFormationSelectable(Formation formation, Agent selectorAgent) => (selectorAgent == null || formation.PlayerOwner == selectorAgent) && formation.CountOfUnits > 0;

    private void SelectAllFormations(Agent selectorAgent, bool uiFeedback)
    {
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.SelectAllFormations());
        GameNetwork.EndModuleEventAsClient();
      }
      if (uiFeedback && !GameNetwork.IsClientOrReplay && (selectorAgent != null && Mission.Current.IsOrderShoutingAllowed()))
        selectorAgent.MakeVoice(SkinVoiceManager.VoiceType.Everyone, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
      MBDebug.Print("Selected formations being cleared. Select all formations:");
      this._selectedFormations.Clear();
      foreach (Formation formation in this._team.Formations.Where<Formation>((Func<Formation, bool>) (f => this.IsFormationSelectable(f, selectorAgent))))
      {
        MBDebug.Print(formation?.InitialClass.ToString() + " added to selected formations.");
        this._selectedFormations.Add(formation);
      }
      this.OnSelectedFormationsCollectionChanged();
    }

    public void SelectAllFormations(bool uiFeedback = false) => this.SelectAllFormations(this.Owner, uiFeedback);

    public void ClearSelectedFormations()
    {
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.ClearSelectedFormations());
        GameNetwork.EndModuleEventAsClient();
      }
      MBDebug.Print("Selected formations being cleared.");
      this._selectedFormations.Clear();
      this.OnSelectedFormationsCollectionChanged();
    }

    public void ReleaseFormationsFromAI()
    {
      foreach (Formation selectedFormation in this.SelectedFormations)
        selectedFormation.ReleaseFormationFromAI();
    }

    private MovementOrder GetChargeOrderSubstituteForSiege(Formation formation)
    {
      TeamAISiegeComponent teamAi = formation.Team?.TeamAI as TeamAISiegeComponent;
      if (this._mission.IsTeleportingAgents || teamAi == null || teamAi.IsAnyLaneThroughWallsOpen())
        return MovementOrder.MovementOrderCharge;
      if (formation.Team.IsAttacker)
      {
        if (teamAi.Ladders.Count > 0)
        {
          SiegeLadder siegeLadder = teamAi.Ladders.MinBy<SiegeLadder, float>((Func<SiegeLadder, float>) (l => l.WaitFrame.origin.DistanceSquared(formation.QuerySystem.MedianPosition.GetNavMeshVec3())));
          if (!formation.IsUsingMachine((UsableMachine) siegeLadder))
            formation.StartUsingMachine((UsableMachine) siegeLadder, true);
          return MovementOrder.MovementOrderMove(siegeLadder.WaitFrame.origin.ToWorldPosition());
        }
        CastleGate castleGate = !teamAi.OuterGate.IsGateOpen ? teamAi.OuterGate : teamAi.InnerGate;
        return castleGate != null ? MovementOrder.MovementOrderAttackEntity(castleGate.GameEntity, false) : MovementOrder.MovementOrderCharge;
      }
      CastleGate castleGate1 = !teamAi.InnerGate.IsGateOpen ? teamAi.InnerGate : teamAi.OuterGate;
      if (castleGate1 != null && !formation.IsUsingMachine((UsableMachine) castleGate1))
        formation.StartUsingMachine((UsableMachine) castleGate1, true);
      return MovementOrder.MovementOrderCharge;
    }

    public void SetOrder(OrderType orderType)
    {
      MBDebug.Print("SetOrder " + (object) orderType + "on team");
      this.BeforeSetOrder(orderType);
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrder(orderType));
        GameNetwork.EndModuleEventAsClient();
      }
      switch (orderType)
      {
        case OrderType.Charge:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              current.MovementOrder = this._mission.MissionTeamAIType != Mission.MissionTeamAITypeEnum.Siege || this._mission.IsTeleportingAgents ? MovementOrder.MovementOrderCharge : this.GetChargeOrderSubstituteForSiege(current);
            }
            break;
          }
        case OrderType.StandYourGround:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderStop;
            break;
          }
        case OrderType.Retreat:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderRetreat;
            break;
          }
        case OrderType.AdvanceTenPaces:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              if (current.MovementOrder.OrderEnum == MovementOrder.MovementOrderEnum.Move)
                current.MovementOrder.Advance(current, 7f);
            }
            break;
          }
        case OrderType.FallBackTenPaces:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              if (current.MovementOrder.OrderEnum == MovementOrder.MovementOrderEnum.Move)
                current.MovementOrder.FallBack(current, 7f);
            }
            break;
          }
        case OrderType.Advance:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderAdvance;
            break;
          }
        case OrderType.FallBack:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderFallBack;
            break;
          }
        case OrderType.LookAtEnemy:
          FacingOrder orderLookAtEnemy = FacingOrder.FacingOrderLookAtEnemy;
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.FacingOrder = orderLookAtEnemy;
            }
            break;
          }
        case OrderType.ArrangementLine:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
            }
            break;
          }
        case OrderType.ArrangementCloseOrder:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
            }
            break;
          }
        case OrderType.ArrangementLoose:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.ArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
            }
            break;
          }
        case OrderType.ArrangementCircular:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.ArrangementOrder = ArrangementOrder.ArrangementOrderCircle;
            }
            break;
          }
        case OrderType.ArrangementSchiltron:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.ArrangementOrder = ArrangementOrder.ArrangementOrderSquare;
            }
            break;
          }
        case OrderType.ArrangementVee:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.ArrangementOrder = ArrangementOrder.ArrangementOrderSkein;
            }
            break;
          }
        case OrderType.ArrangementColumn:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.ArrangementOrder = ArrangementOrder.ArrangementOrderColumn;
            }
            break;
          }
        case OrderType.ArrangementScatter:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.ArrangementOrder = ArrangementOrder.ArrangementOrderScatter;
            }
            break;
          }
        case OrderType.FormDeep:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.FormOrder = FormOrder.FormOrderDeep;
            }
            break;
          }
        case OrderType.FormWide:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.FormOrder = FormOrder.FormOrderWide;
            }
            break;
          }
        case OrderType.FormWider:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              OrderController.TryCancelStopOrder(current);
              current.FormOrder = FormOrder.FormOrderWider;
            }
            break;
          }
        case OrderType.HoldFire:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.FiringOrder = FiringOrder.FiringOrderHoldYourFire;
            break;
          }
        case OrderType.FireAtWill:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.FiringOrder = FiringOrder.FiringOrderFireAtWill;
            break;
          }
        case OrderType.Mount:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              if (current.IsMounted() || current.HasAnyMountedUnit)
                OrderController.TryCancelStopOrder(current);
              current.RidingOrder = RidingOrder.RidingOrderMount;
            }
            break;
          }
        case OrderType.Dismount:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Formation current = enumerator.Current;
              if (current.IsMounted() || current.HasAnyMountedUnit)
                OrderController.TryCancelStopOrder(current);
              current.RidingOrder = RidingOrder.RidingOrderDismount;
            }
            break;
          }
        case OrderType.UseAnyWeapon:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
            break;
          }
        case OrderType.UseBluntWeaponsOnly:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseOnlyBlunt;
            break;
          }
        case OrderType.AIControlOn:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.IsAIControlled = true;
            break;
          }
        case OrderType.AIControlOff:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.IsAIControlled = false;
            break;
          }
      }
      this.AfterSetOrder(orderType);
      if (this.OnOrderIssued == null)
        return;
      this.OnOrderIssued(orderType, (IEnumerable<Formation>) this.SelectedFormations, Array.Empty<object>());
    }

    private static void AfterSetOrderMakeVoice(OrderType orderType, Agent agent)
    {
      if (!Mission.Current.IsOrderShoutingAllowed())
        return;
      switch (orderType)
      {
        case OrderType.Move:
        case OrderType.MoveToLineSegment:
        case OrderType.MoveToLineSegmentWithHorizontalLayout:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Move, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.Charge:
        case OrderType.ChargeWithTarget:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Charge, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.StandYourGround:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Stop, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.FollowMe:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Follow, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.Retreat:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Retreat, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.AdvanceTenPaces:
        case OrderType.Advance:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Advance, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.FallBackTenPaces:
        case OrderType.FallBack:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FallBack, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.LookAtEnemy:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FaceEnemy, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.LookAtDirection:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FaceDirection, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.ArrangementLine:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FormLine, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.ArrangementCloseOrder:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FormShieldWall, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.ArrangementLoose:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FormLoose, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.ArrangementCircular:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FormCircle, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.ArrangementSchiltron:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FormSquare, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.ArrangementVee:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FormSkein, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.ArrangementColumn:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FormColumn, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.ArrangementScatter:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FormScatter, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.HoldFire:
          agent.MakeVoice(SkinVoiceManager.VoiceType.HoldFire, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.FireAtWill:
          agent.MakeVoice(SkinVoiceManager.VoiceType.FireAtWill, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.Mount:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Mount, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.Dismount:
          agent.MakeVoice(SkinVoiceManager.VoiceType.Dismount, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.AIControlOn:
          agent.MakeVoice(SkinVoiceManager.VoiceType.CommandDelegate, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
        case OrderType.AIControlOff:
          agent.MakeVoice(SkinVoiceManager.VoiceType.CommandUndelegate, SkinVoiceManager.CombatVoiceNetworkPredictionType.NoPrediction);
          break;
      }
    }

    private void AfterSetOrder(OrderType orderType)
    {
      MBDebug.Print("After set order called, number of selected formations: " + (object) this.SelectedFormations.Count);
      foreach (Formation selectedFormation in this.SelectedFormations)
      {
        MBDebug.Print(selectedFormation?.FormationIndex.ToString() + " formation being processed.");
        selectedFormation.ApplyActionOnEachUnit((Action<Agent>) (agent => agent.UpdateCachedAndFormationValues(false, false)));
        MBDebug.Print("Update cached and formation values on each agent complete, number of selected formations: " + (object) this.SelectedFormations.Count);
        this._mission.SetRandomDecideTimeOfAgentsWithIndices(selectedFormation.CollectUnitIndices());
        MBDebug.Print("Set random decide time of agents with indices complete, number of selected formations: " + (object) this.SelectedFormations.Count);
      }
      MBDebug.Print("After set order loop complete, number of selected formations: " + (object) this.SelectedFormations.Count);
      if (GameNetwork.IsClientOrReplay || this.Owner == null)
        return;
      OrderController.AfterSetOrderMakeVoice(orderType, this.Owner);
      if (this._selectedFormations.Count <= 0 || this.Owner == null || this.Owner.Controller == Agent.ControllerType.AI)
        return;
      MissionWeapon wieldedWeapon = this.Owner.WieldedWeapon;
      switch (wieldedWeapon.IsEmpty ? 0 : (int) wieldedWeapon.Item.PrimaryWeapon.WeaponClass)
      {
        case 0:
        case 17:
          if (this.Owner.MountAgent == null)
          {
            this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? (this.Owner.GetIsLeftStance() ? OrderController.act_command_follow_unarmed_leftstance : OrderController.act_command_follow_unarmed) : (this.Owner.GetIsLeftStance() ? OrderController.act_command_unarmed_leftstance : OrderController.act_command_unarmed));
            break;
          }
          this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? OrderController.act_horse_command_follow_unarmed : OrderController.act_horse_command_unarmed);
          break;
        case 1:
        case 2:
        case 4:
        case 6:
        case 7:
        case 9:
        case 19:
        case 20:
          if (this.Owner.MountAgent == null)
          {
            this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? (this.Owner.GetIsLeftStance() ? OrderController.act_command_follow_leftstance : OrderController.act_command_follow) : (this.Owner.GetIsLeftStance() ? OrderController.act_command_leftstance : OrderController.act_command));
            break;
          }
          this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? OrderController.act_horse_command_follow : OrderController.act_horse_command);
          break;
        case 3:
        case 5:
        case 8:
        case 10:
        case 11:
        case 16:
        case 21:
        case 22:
        case 23:
          if (this.Owner.MountAgent == null)
          {
            this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? (this.Owner.GetIsLeftStance() ? OrderController.act_command_follow_2h_leftstance : OrderController.act_command_follow_2h) : (this.Owner.GetIsLeftStance() ? OrderController.act_command_2h_leftstance : OrderController.act_command_2h));
            break;
          }
          this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? OrderController.act_horse_command_follow_2h : OrderController.act_horse_command_2h);
          break;
        case 15:
          if (this.Owner.MountAgent == null)
          {
            this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? OrderController.act_command_follow_bow : OrderController.act_command_bow);
            break;
          }
          this.Owner.SetActionChannel(1, orderType == OrderType.FollowMe ? OrderController.act_horse_command_follow_bow : OrderController.act_horse_command_bow);
          break;
      }
    }

    private void BeforeSetOrder(OrderType orderType)
    {
      foreach (Formation formation in this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => !this.IsFormationSelectable(f, this.Owner))).ToList<Formation>())
        this.DeselectFormation(formation);
      if (GameNetwork.IsClientOrReplay || orderType == OrderType.AIControlOff || orderType == OrderType.AIControlOn)
        return;
      this.ReleaseFormationsFromAI();
    }

    public void SetOrderWithAgent(OrderType orderType, Agent agent)
    {
      MBDebug.Print("SetOrderWithAgent " + (object) orderType + " " + agent.Name + "on team");
      this.BeforeSetOrder(orderType);
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithAgent(orderType, agent));
        GameNetwork.EndModuleEventAsClient();
      }
      switch (orderType)
      {
        case OrderType.FollowMe:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderFollow(agent);
            break;
          }
        case OrderType.GuardMe:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderGuard(agent);
            break;
          }
      }
      this.AfterSetOrder(orderType);
      if (this.OnOrderIssued == null)
        return;
      this.OnOrderIssued(orderType, (IEnumerable<Formation>) this.SelectedFormations, new object[1]
      {
        (object) agent
      });
    }

    public void SetOrderWithPosition(OrderType orderType, WorldPosition orderPosition)
    {
      MBDebug.Print("SetOrderWithPosition " + (object) orderType + " " + (object) orderPosition + "on team");
      this.BeforeSetOrder(orderType);
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithPosition(orderType, orderPosition.GetGroundVec3()));
        GameNetwork.EndModuleEventAsClient();
      }
      switch (orderType)
      {
        case OrderType.Move:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderMove(orderPosition);
            break;
          }
        case OrderType.LookAtDirection:
          FacingOrder facingOrder = FacingOrder.FacingOrderLookAtDirection(OrderController.GetOrderLookAtDirection((IEnumerable<Formation>) this.SelectedFormations, orderPosition.AsVec2));
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.FacingOrder = facingOrder;
            break;
          }
        case OrderType.FormCustom:
          float orderFormCustomWidth = OrderController.GetOrderFormCustomWidth((IEnumerable<Formation>) this.SelectedFormations, orderPosition.GetGroundVec3());
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.FormOrder = FormOrder.FormOrderCustom(orderFormCustomWidth);
            break;
          }
      }
      this.AfterSetOrder(orderType);
      if (this.OnOrderIssued == null)
        return;
      this.OnOrderIssued(orderType, (IEnumerable<Formation>) this.SelectedFormations, new object[1]
      {
        (object) orderPosition
      });
    }

    public void SetOrderWithFormation(OrderType orderType, Formation orderFormation)
    {
      MBDebug.Print("SetOrderWithFormation " + (object) orderType + " " + (object) orderFormation + "on team");
      this.BeforeSetOrder(orderType);
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithFormation(orderType, orderFormation.Index));
        GameNetwork.EndModuleEventAsClient();
      }
      if (orderType == OrderType.ChargeWithTarget)
      {
        foreach (Formation selectedFormation in this.SelectedFormations)
          selectedFormation.MovementOrder = MovementOrder.MovementOrderChargeToTarget(orderFormation);
      }
      this.AfterSetOrder(orderType);
      if (this.OnOrderIssued == null)
        return;
      this.OnOrderIssued(orderType, (IEnumerable<Formation>) this.SelectedFormations, new object[1]
      {
        (object) orderFormation
      });
    }

    public void SetOrderWithFormationAndPercentage(
      OrderType orderType,
      Formation orderFormation,
      float percentage)
    {
      int percentage1 = MBMath.ClampInt((int) ((double) percentage * 100.0), 0, 100);
      this.BeforeSetOrder(orderType);
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithFormationAndPercentage(orderType, orderFormation.Index, percentage1));
        GameNetwork.EndModuleEventAsClient();
      }
      this.AfterSetOrder(orderType);
      if (this.OnOrderIssued == null)
        return;
      this.OnOrderIssued(orderType, (IEnumerable<Formation>) this.SelectedFormations, new object[2]
      {
        (object) orderFormation,
        (object) percentage
      });
    }

    public void SetOrderWithFormationAndNumber(
      OrderType orderType,
      Formation orderFormation,
      int number)
    {
      this.BeforeSetOrder(orderType);
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithFormationAndNumber(orderType, orderFormation.Index, number));
        GameNetwork.EndModuleEventAsClient();
      }
      List<int> intList = (List<int>) null;
      switch (orderType)
      {
        case OrderType.Attach:
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderAttach(orderFormation, (MovementOrder.Side) number);
            break;
          }
        case OrderType.Transfer:
          int num1 = this.SelectedFormations.Sum<Formation>((Func<Formation, int>) (f => f.CountOfUnits));
          int num2 = number;
          int num3 = 0;
          if (this.SelectedFormations.Count<Formation>() > 1)
            intList = new List<int>();
          foreach (Formation selectedFormation in this.SelectedFormations)
          {
            int countOfUnits = selectedFormation.CountOfUnits;
            int unitCount = num2 * countOfUnits / num1;
            if (!GameNetwork.IsClientOrReplay)
            {
              selectedFormation.TransferUnitsAux(orderFormation, unitCount, true);
              selectedFormation.QuerySystem.Expire();
            }
            intList?.Add(unitCount);
            num2 -= unitCount;
            num1 -= countOfUnits;
            num3 += unitCount;
          }
          if (!GameNetwork.IsClientOrReplay)
          {
            orderFormation.QuerySystem.Expire();
            break;
          }
          break;
      }
      this.AfterSetOrder(orderType);
      if (this.OnOrderIssued == null)
        return;
      if (intList != null)
      {
        object[] objArray = new object[intList.Count + 1];
        objArray[0] = (object) number;
        for (int index = 0; index < intList.Count; ++index)
          objArray[index + 1] = (object) intList[index];
        this.OnOrderIssued(orderType, (IEnumerable<Formation>) this.SelectedFormations, new object[2]
        {
          (object) orderFormation,
          (object) objArray
        });
      }
      else
        this.OnOrderIssued(orderType, (IEnumerable<Formation>) this.SelectedFormations, new object[2]
        {
          (object) orderFormation,
          (object) number
        });
    }

    public void SetOrderWithTwoPositions(
      OrderType orderType,
      WorldPosition position1,
      WorldPosition position2)
    {
      MBDebug.Print("SetOrderWithTwoPositions " + (object) orderType + " " + (object) position1 + " " + (object) position2 + "on team");
      this.BeforeSetOrder(orderType);
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithTwoPositions(orderType, position1.GetGroundVec3(), position2.GetGroundVec3()));
        GameNetwork.EndModuleEventAsClient();
      }
      switch (orderType)
      {
        case OrderType.MoveToLineSegment:
        case OrderType.MoveToLineSegmentWithHorizontalLayout:
          bool isFormationLayoutVertical = orderType == OrderType.MoveToLineSegment;
          IEnumerable<Formation> formations = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
          if (formations.Any<Formation>())
          {
            OrderController.MoveToLineSegment(formations, this.simulationFormations, position1, position2, this.OnOrderIssued, this.actualWidths, this.actualUnitSpacings, isFormationLayoutVertical);
            break;
          }
          break;
      }
      this.AfterSetOrder(orderType);
      if (this.OnOrderIssued == null)
        return;
      this.OnOrderIssued(orderType, (IEnumerable<Formation>) this.SelectedFormations, new object[2]
      {
        (object) position1,
        (object) position2
      });
    }

    public void SetOrderWithOrderableObject(IOrderable target)
    {
      BattleSideEnum side = this.SelectedFormations.First<Formation>().Team.Side;
      OrderType order = target.GetOrder(side);
      this.BeforeSetOrder(order);
      MissionObject missionObject = target as MissionObject;
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new ApplyOrderWithMissionObject(missionObject));
        GameNetwork.EndModuleEventAsClient();
      }
      switch (order)
      {
        case OrderType.Move:
          WorldPosition position = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, missionObject.GameEntity.GlobalPosition, false);
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderMove(position);
            break;
          }
        case OrderType.MoveToLineSegment:
          IPointDefendable pointDefendable1 = target as IPointDefendable;
          Vec3 globalPosition1 = pointDefendable1.DefencePoints.Last<DefencePoint>().GameEntity.GlobalPosition;
          Vec3 globalPosition2 = pointDefendable1.DefencePoints.First<DefencePoint>().GameEntity.GlobalPosition;
          IEnumerable<Formation> formations1 = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
          if (formations1.Any<Formation>())
          {
            WorldPosition TargetLineSegmentBegin = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition1, false);
            WorldPosition TargetLineSegmentEnd = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition2, false);
            OrderController.MoveToLineSegment(formations1, this.simulationFormations, TargetLineSegmentBegin, TargetLineSegmentEnd, this.OnOrderIssued, this.actualWidths, this.actualUnitSpacings);
            break;
          }
          break;
        case OrderType.MoveToLineSegmentWithHorizontalLayout:
          IPointDefendable pointDefendable2 = target as IPointDefendable;
          Vec3 globalPosition3 = pointDefendable2.DefencePoints.Last<DefencePoint>().GameEntity.GlobalPosition;
          Vec3 globalPosition4 = pointDefendable2.DefencePoints.First<DefencePoint>().GameEntity.GlobalPosition;
          IEnumerable<Formation> formations2 = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
          if (formations2.Any<Formation>())
          {
            WorldPosition TargetLineSegmentBegin = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition3, false);
            WorldPosition TargetLineSegmentEnd = new WorldPosition(this._mission.Scene, UIntPtr.Zero, globalPosition4, false);
            OrderController.MoveToLineSegment(formations2, this.simulationFormations, TargetLineSegmentBegin, TargetLineSegmentEnd, this.OnOrderIssued, this.actualWidths, this.actualUnitSpacings, false);
            break;
          }
          break;
        case OrderType.FollowEntity:
          GameEntity waitEntity = (target as UsableMachine).WaitEntity;
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderFollowEntity(waitEntity);
            break;
          }
        case OrderType.Use:
          this.ToggleSideOrderUse((IEnumerable<Formation>) this.SelectedFormations, target as UsableMachine);
          break;
        case OrderType.AttackEntity:
          GameEntity gameEntity = missionObject.GameEntity;
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderAttackEntity(gameEntity, !(missionObject is CastleGate));
            break;
          }
        case OrderType.PointDefence:
          IPointDefendable pointDefendable3 = target as IPointDefendable;
          using (List<Formation>.Enumerator enumerator = this.SelectedFormations.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.MovementOrder = MovementOrder.MovementOrderMove(pointDefendable3.MiddleFrame.Origin);
            break;
          }
      }
      this.AfterSetOrder(order);
      OnOrderIssuedDelegate onOrderIssued = this.OnOrderIssued;
      if (onOrderIssued == null)
        return;
      onOrderIssued(order, (IEnumerable<Formation>) this.SelectedFormations, new object[1]
      {
        (object) target
      });
    }

    public static OrderType GetActiveMovementOrderOf(Formation formation)
    {
      switch (formation.MovementOrder.MovementState)
      {
        case MovementOrder.MovementStateEnum.Charge:
          return formation.MovementOrder.OrderType == OrderType.GuardMe ? OrderType.GuardMe : OrderType.Charge;
        case MovementOrder.MovementStateEnum.Hold:
          switch (formation.MovementOrder.OrderType)
          {
            case OrderType.ChargeWithTarget:
              return OrderType.Charge;
            case OrderType.FollowMe:
              return OrderType.FollowMe;
            case OrderType.Attach:
              return OrderType.Attach;
            case OrderType.Advance:
              return OrderType.Advance;
            case OrderType.FallBack:
              return OrderType.FallBack;
            default:
              return OrderType.Move;
          }
        case MovementOrder.MovementStateEnum.Retreat:
          return OrderType.Retreat;
        case MovementOrder.MovementStateEnum.StandGround:
          return OrderType.StandYourGround;
        default:
          return OrderType.Move;
      }
    }

    public static OrderType GetActiveFacingOrderOf(Formation formation)
    {
      FacingOrder facingOrder = formation.FacingOrder;
      if (facingOrder.OrderType == OrderType.LookAtDirection)
        return OrderType.LookAtDirection;
      int orderType = (int) facingOrder.OrderType;
      return OrderType.LookAtEnemy;
    }

    public static OrderType GetActiveRidingOrderOf(Formation formation)
    {
      OrderType orderType = formation.RidingOrder.OrderType;
      return orderType == OrderType.RideFree ? OrderType.Mount : orderType;
    }

    public static OrderType GetActiveArrangementOrderOf(Formation formation) => formation.ArrangementOrder.OrderType;

    public static OrderType GetActiveFormOrderOf(Formation formation) => formation.FormOrder.OrderType;

    public static OrderType GetActiveWeaponUsageOrderOf(Formation formation) => formation.WeaponUsageOrder.OrderType;

    public static OrderType GetActiveFiringOrderOf(Formation formation) => formation.FiringOrder.OrderType;

    public static OrderType GetActiveAIControlOrderOf(Formation formation) => formation.IsAIControlled ? OrderType.AIControlOn : OrderType.AIControlOff;

    public void SimulateNewOrderWithPositionAndDirection(
      WorldPosition formationLineBegin,
      WorldPosition formationLineEnd,
      out List<(Agent, WorldFrame)> simulationAgentFrames,
      out List<(Formation, int, float, Vec2, WorldPosition)> formationChanges,
      out List<WorldFrame> simulationUnavailablePositionFrames,
      bool isFormationLayoutVertical)
    {
      IEnumerable<Formation> formations = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
      if (formations.Any<Formation>())
      {
        OrderController.SimulateNewOrderWithPositionAndDirection(formations, this.simulationFormations, formationLineBegin, formationLineEnd, out simulationAgentFrames, out formationChanges, out simulationUnavailablePositionFrames, isFormationLayoutVertical);
      }
      else
      {
        simulationAgentFrames = new List<(Agent, WorldFrame)>();
        formationChanges = new List<(Formation, int, float, Vec2, WorldPosition)>();
        simulationUnavailablePositionFrames = new List<WorldFrame>();
      }
    }

    public void SimulateNewOrderWithPositionAndDirection(
      WorldPosition formationLineBegin,
      WorldPosition formationLineEnd,
      out List<(Agent, WorldFrame)> simulationAgentFrames,
      bool isFormationLayoutVertical)
    {
      IEnumerable<Formation> formations = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
      if (formations.Any<Formation>())
        OrderController.SimulateNewOrderWithPositionAndDirection(formations, this.simulationFormations, formationLineBegin, formationLineEnd, out simulationAgentFrames, isFormationLayoutVertical);
      else
        simulationAgentFrames = new List<(Agent, WorldFrame)>();
    }

    public void SimulateNewFacingOrder(
      Vec2 direction,
      out List<(Agent, WorldFrame)> simulationAgentFrames)
    {
      IEnumerable<Formation> formations = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
      if (formations.Any<Formation>())
        OrderController.SimulateNewFacingOrder(formations, this.simulationFormations, direction, out simulationAgentFrames);
      else
        simulationAgentFrames = new List<(Agent, WorldFrame)>();
    }

    public void SimulateNewCustomWidthOrder(
      float width,
      out List<(Agent, WorldFrame)> simulationAgentFrames)
    {
      IEnumerable<Formation> formations = this.SelectedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0));
      if (formations.Any<Formation>())
        OrderController.SimulateNewCustomWidthOrder(formations, this.simulationFormations, width, out simulationAgentFrames);
      else
        simulationAgentFrames = new List<(Agent, WorldFrame)>();
    }

    private static void SimulateNewOrderWithPositionAndDirectionAux(
      IEnumerable<Formation> formations,
      Dictionary<Formation, Formation> simulationFormations,
      WorldPosition formationLineBegin,
      WorldPosition formationLineEnd,
      bool isSimulatingAgentFrames,
      out List<(Agent, WorldFrame)> simulationAgentFrames,
      bool isSimulatingFormationChanges,
      out List<(Formation, int, float, Vec2, WorldPosition)> simulationFormationChanges,
      bool isSimulatingUnavailablePositionFrames,
      out List<WorldFrame> simulationUnavailablePositionFrames,
      out bool isLineShort,
      bool isFormationLayoutVertical = true)
    {
      float length = (formationLineEnd.AsVec2 - formationLineBegin.AsVec2).Length;
      isLineShort = false;
      if ((double) length < (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius))
      {
        isLineShort = true;
      }
      else
      {
        float num = !isFormationLayoutVertical ? formations.Max<Formation>((Func<Formation, float>) (f => f.Width)) : formations.Sum<Formation>((Func<Formation, float>) (f => f.MinimumWidth)) + (float) (formations.Count<Formation>() - 1) * 1.5f;
        if ((double) length < (double) num)
          isLineShort = true;
      }
      if (isLineShort)
      {
        float num1 = !isFormationLayoutVertical ? formations.Max<Formation>((Func<Formation, float>) (f => f.Width)) : formations.Sum<Formation>((Func<Formation, float>) (f => f.Width)) + (float) (formations.Count<Formation>() - 1) * 1.5f;
        Vec2 direction = formations.MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes)).Direction;
        direction.RotateCCW(-1.570796f);
        double num2 = (double) direction.Normalize();
        formationLineEnd = Mission.Current.GetStraightPathToTarget(formationLineBegin.AsVec2 + num1 / 2f * direction, formationLineBegin);
        formationLineBegin = Mission.Current.GetStraightPathToTarget(formationLineBegin.AsVec2 - num1 / 2f * direction, formationLineBegin);
      }
      else
        formationLineEnd = Mission.Current.GetStraightPathToTarget(formationLineEnd.AsVec2, formationLineBegin);
      if (isFormationLayoutVertical)
        OrderController.SimulateNewOrderWithVerticalLayout(formations, simulationFormations, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, out simulationAgentFrames, isSimulatingFormationChanges, out simulationFormationChanges, isSimulatingUnavailablePositionFrames, out simulationUnavailablePositionFrames);
      else
        OrderController.SimulateNewOrderWithHorizontalLayout(formations, simulationFormations, formationLineBegin, formationLineEnd, isSimulatingAgentFrames, out simulationAgentFrames, isSimulatingFormationChanges, out simulationFormationChanges, isSimulatingUnavailablePositionFrames, out simulationUnavailablePositionFrames);
    }

    private static Formation GetSimulationFormation(
      Formation formation,
      Dictionary<Formation, Formation> simulationFormations)
    {
      return simulationFormations?[formation];
    }

    private static void SimulateNewFacingOrder(
      IEnumerable<Formation> formations,
      Dictionary<Formation, Formation> simulationFormations,
      Vec2 direction,
      out List<(Agent, WorldFrame)> simulationAgentFrames)
    {
      simulationAgentFrames = new List<(Agent, WorldFrame)>();
      Mat3 identity = Mat3.Identity;
      identity.f = direction.ToVec3();
      identity.Orthonormalize();
      foreach (Formation formation in formations)
      {
        float width = formation.Width;
        WorldFrame formationPositionMatrix = new WorldFrame(identity, formation.OrderPosition);
        int unitSpacingReduction = 0;
        OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, OrderController.GetSimulationFormation(formation, simulationFormations), ref formationPositionMatrix, ref width, ref unitSpacingReduction);
        OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, (List<(Formation, int, float, Vec2, WorldPosition)>) null, (List<WorldFrame>) null, ref formationPositionMatrix, ref identity, width, unitSpacingReduction, false, out float _);
      }
    }

    private static void SimulateNewCustomWidthOrder(
      IEnumerable<Formation> formations,
      Dictionary<Formation, Formation> simulationFormations,
      float width,
      out List<(Agent, WorldFrame)> simulationAgentFrames)
    {
      simulationAgentFrames = new List<(Agent, WorldFrame)>();
      foreach (Formation formation in formations)
      {
        float formationWidth = width;
        Mat3 identity = Mat3.Identity;
        identity.f = formation.Direction.ToVec3();
        identity.Orthonormalize();
        WorldFrame formationPositionMatrix = new WorldFrame(identity, formation.OrderPosition);
        int unitSpacingReduction = 0;
        OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, OrderController.GetSimulationFormation(formation, simulationFormations), ref formationPositionMatrix, ref formationWidth, ref unitSpacingReduction);
        int count = simulationAgentFrames.Count;
        float simulatedFormationDepth;
        OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, (List<(Formation, int, float, Vec2, WorldPosition)>) null, (List<WorldFrame>) null, ref formationPositionMatrix, ref identity, formationWidth, unitSpacingReduction, false, out simulatedFormationDepth);
        float lesserThanActualWidth = Formation.GetLastSimulatedFormationsOccupationWidthIfLesserThanActualWidth(OrderController.GetSimulationFormation(formation, simulationFormations));
        if ((double) lesserThanActualWidth > 0.0)
        {
          simulationAgentFrames.RemoveRange(count, simulationAgentFrames.Count - count);
          OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, (List<(Formation, int, float, Vec2, WorldPosition)>) null, (List<WorldFrame>) null, ref formationPositionMatrix, ref identity, lesserThanActualWidth, unitSpacingReduction, false, out simulatedFormationDepth);
        }
      }
    }

    public static void SimulateNewOrderWithPositionAndDirection(
      IEnumerable<Formation> formations,
      Dictionary<Formation, Formation> simulationFormations,
      WorldPosition formationLineBegin,
      WorldPosition formationLineEnd,
      out List<(Agent, WorldFrame)> simulationAgentFrames,
      out List<(Formation, int, float, Vec2, WorldPosition)> formationChanges,
      out List<WorldFrame> simulationUnavailablePositionFrames,
      bool isFormationLayoutVertical = true)
    {
      OrderController.SimulateNewOrderWithPositionAndDirectionAux(formations, simulationFormations, formationLineBegin, formationLineEnd, true, out simulationAgentFrames, true, out formationChanges, true, out simulationUnavailablePositionFrames, out bool _, isFormationLayoutVertical);
    }

    public static void SimulateNewOrderWithPositionAndDirection(
      IEnumerable<Formation> formations,
      Dictionary<Formation, Formation> simulationFormations,
      WorldPosition formationLineBegin,
      WorldPosition formationLineEnd,
      out List<(Agent, WorldFrame)> simulationAgentFrames,
      bool isFormationLayoutVertical = true)
    {
      List<(Formation, int, float, Vec2, WorldPosition)> simulationFormationChanges = (List<(Formation, int, float, Vec2, WorldPosition)>) null;
      List<WorldFrame> simulationUnavailablePositionFrames = (List<WorldFrame>) null;
      OrderController.SimulateNewOrderWithPositionAndDirectionAux(formations, simulationFormations, formationLineBegin, formationLineEnd, true, out simulationAgentFrames, false, out simulationFormationChanges, false, out simulationUnavailablePositionFrames, out bool _, isFormationLayoutVertical);
    }

    public static void SimulateNewOrderWithPositionAndDirection(
      IEnumerable<Formation> formations,
      Dictionary<Formation, Formation> simulationFormations,
      WorldPosition formationLineBegin,
      WorldPosition formationLineEnd,
      out List<(Formation, int, float, Vec2, WorldPosition)> formationChanges,
      out bool isLineShort,
      bool isFormationLayoutVertical = true)
    {
      List<(Agent, WorldFrame)> simulationAgentFrames = (List<(Agent, WorldFrame)>) null;
      List<WorldFrame> simulationUnavailablePositionFrames = (List<WorldFrame>) null;
      OrderController.SimulateNewOrderWithPositionAndDirectionAux(formations, simulationFormations, formationLineBegin, formationLineEnd, false, out simulationAgentFrames, true, out formationChanges, false, out simulationUnavailablePositionFrames, out isLineShort, isFormationLayoutVertical);
    }

    private static void SimulateNewOrderWithVerticalLayout(
      IEnumerable<Formation> formations,
      Dictionary<Formation, Formation> simulationFormations,
      WorldPosition formationLineBegin,
      WorldPosition formationLineEnd,
      bool isSimulatingAgentFrames,
      out List<(Agent, WorldFrame)> simulationAgentFrames,
      bool isSimulatingFormationChanges,
      out List<(Formation, int, float, Vec2, WorldPosition)> simulationFormationChanges,
      bool isSimulatingUnavailablePositionFrames,
      out List<WorldFrame> simulationUnavailablePositionFrames)
    {
      simulationAgentFrames = !isSimulatingAgentFrames ? (List<(Agent, WorldFrame)>) null : new List<(Agent, WorldFrame)>();
      simulationFormationChanges = !isSimulatingFormationChanges ? (List<(Formation, int, float, Vec2, WorldPosition)>) null : new List<(Formation, int, float, Vec2, WorldPosition)>();
      simulationUnavailablePositionFrames = !isSimulatingUnavailablePositionFrames ? (List<WorldFrame>) null : new List<WorldFrame>();
      Vec2 vec2 = formationLineEnd.AsVec2 - formationLineBegin.AsVec2;
      float length = vec2.Length;
      double num1 = (double) vec2.Normalize();
      float f1 = Math.Max(0.0f, length - (float) (formations.Count<Formation>() - 1) * 1.5f);
      float comparedValue = formations.Sum<Formation>((Func<Formation, float>) (f => f.Width));
      bool flag = f1.ApproximatelyEqualsTo(comparedValue, 0.1f);
      float num2 = formations.Sum<Formation>((Func<Formation, float>) (f => f.MinimumWidth));
      float num3 = num2 + (float) (formations.Count<Formation>() - 1) * 1.5f;
      if ((double) length < (double) num3)
        f1 = num2;
      Mat3 identity = Mat3.Identity;
      identity.f.x = vec2.x;
      identity.f.y = vec2.y;
      identity.f.z = 0.0f;
      identity.f.RotateAboutZ(1.570796f);
      identity.Orthonormalize();
      float num4 = 0.0f;
      foreach (Formation formation in formations)
      {
        float formationWidth = flag ? formation.Width : Math.Min((double) f1 < (double) comparedValue ? formation.Width : float.MaxValue, f1 * (formation.MinimumWidth / num2));
        WorldPosition origin = formationLineBegin;
        origin.SetVec2(origin.AsVec2 + vec2 * (formationWidth * 0.5f + num4));
        WorldFrame formationPositionMatrix = new WorldFrame(identity, origin);
        int unitSpacingReduction = 0;
        OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, OrderController.GetSimulationFormation(formation, simulationFormations), ref formationPositionMatrix, ref formationWidth, ref unitSpacingReduction);
        OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, simulationUnavailablePositionFrames, ref formationPositionMatrix, ref identity, formationWidth, unitSpacingReduction, false, out float _);
        num4 += formationWidth + 1.5f;
      }
    }

    private static void DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(
      Formation formation,
      Formation simulationFormation,
      ref WorldFrame formationPositionMatrix,
      ref float formationWidth,
      ref int unitSpacingReduction)
    {
      if (simulationFormation.UnitSpacing != formation.UnitSpacing)
        simulationFormation = new Formation((Team) null, -1);
      int unitIndex = formation.CountOfUnitsWithoutDetachedOnes - 1;
      float actualWidth = formationWidth;
      while (!formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, ref formationPositionMatrix, formationWidth, formation.UnitSpacing - unitSpacingReduction, out actualWidth).HasValue)
      {
        ++unitSpacingReduction;
        if (formation.UnitSpacing - unitSpacingReduction < 0)
          break;
      }
      unitSpacingReduction = Math.Min(unitSpacingReduction, formation.UnitSpacing);
      if (unitSpacingReduction <= 0)
        return;
      formationWidth = actualWidth;
    }

    private static float GetGapBetweenLinesOfFormation(Formation f, float unitSpacing)
    {
      float num1 = 0.0f;
      float num2 = 0.2f;
      if (f.HasAnyMountedUnit && !(f.RidingOrder == RidingOrder.RidingOrderDismount))
      {
        num1 = 2f;
        num2 = 0.6f;
      }
      return num1 + unitSpacing * num2;
    }

    private static void SimulateNewOrderWithHorizontalLayout(
      IEnumerable<Formation> formations,
      Dictionary<Formation, Formation> simulationFormations,
      WorldPosition formationLineBegin,
      WorldPosition formationLineEnd,
      bool isSimulatingAgentFrames,
      out List<(Agent, WorldFrame)> simulationAgentFrames,
      bool isSimulatingFormationChanges,
      out List<(Formation, int, float, Vec2, WorldPosition)> simulationFormationChanges,
      bool isSimulatingUnavailablePositionFrames,
      out List<WorldFrame> simulationUnavailablePositionFrames)
    {
      simulationAgentFrames = !isSimulatingAgentFrames ? (List<(Agent, WorldFrame)>) null : new List<(Agent, WorldFrame)>();
      simulationFormationChanges = !isSimulatingFormationChanges ? (List<(Formation, int, float, Vec2, WorldPosition)>) null : new List<(Formation, int, float, Vec2, WorldPosition)>();
      simulationUnavailablePositionFrames = !isSimulatingUnavailablePositionFrames ? (List<WorldFrame>) null : new List<WorldFrame>();
      Vec2 vec2 = formationLineEnd.AsVec2 - formationLineBegin.AsVec2;
      float num1 = vec2.Normalize();
      float num2 = formations.Max<Formation>((Func<Formation, float>) (f => f.MinimumWidth));
      if ((double) num1 < (double) num2)
        num1 = num2;
      Mat3 identity = Mat3.Identity;
      identity.f.x = vec2.x;
      identity.f.y = vec2.y;
      identity.f.RotateAboutZ(1.570796f);
      identity.Orthonormalize();
      float num3 = 0.0f;
      foreach (Formation formation in formations)
      {
        float formationWidth = num1;
        WorldPosition origin = formationLineBegin;
        origin.SetVec2((formationLineEnd.AsVec2 + formationLineBegin.AsVec2) * 0.5f - identity.f.AsVec2 * num3);
        WorldFrame formationPositionMatrix = new WorldFrame(identity, origin);
        int unitSpacingReduction = 0;
        OrderController.DecreaseUnitSpacingAndWidthIfNotAllUnitsFit(formation, OrderController.GetSimulationFormation(formation, simulationFormations), ref formationPositionMatrix, ref formationWidth, ref unitSpacingReduction);
        float simulatedFormationDepth;
        OrderController.SimulateNewOrderWithFrameAndWidth(formation, OrderController.GetSimulationFormation(formation, simulationFormations), simulationAgentFrames, simulationFormationChanges, simulationUnavailablePositionFrames, ref formationPositionMatrix, ref identity, formationWidth, unitSpacingReduction, true, out simulatedFormationDepth);
        num3 += simulatedFormationDepth + OrderController.GetGapBetweenLinesOfFormation(formation, (float) (formation.UnitSpacing - unitSpacingReduction));
      }
    }

    private static void SimulateNewOrderWithFrameAndWidth(
      Formation formation,
      Formation simulationFormation,
      List<(Agent, WorldFrame)> simulationAgentFrames,
      List<(Formation, int, float, Vec2, WorldPosition)> simulationFormationChanges,
      List<WorldFrame> simulationUnavailablePositionFrames,
      ref WorldFrame formationPositionMatrix,
      ref Mat3 formationRotation,
      float formationWidth,
      int unitSpacingReduction,
      bool simulateFormationDepth,
      out float simulatedFormationDepth)
    {
      int unitIndex = 0;
      float num = simulateFormationDepth ? 0.0f : float.NaN;
      foreach (Agent agent in (IEnumerable<Agent>) formation.GetUnitsWithoutDetachedOnes().OrderBy<Agent, int>((Func<Agent, int>) (u => MBCommon.Hash(u.Index, (object) u))))
      {
        WorldFrame? accordingToNewOrder = formation.GetUnitPositionWithIndexAccordingToNewOrder(simulationFormation, unitIndex, ref formationPositionMatrix, formationWidth, formation.UnitSpacing - unitSpacingReduction);
        if (accordingToNewOrder.HasValue)
        {
          simulationAgentFrames?.Add((agent, accordingToNewOrder.Value));
          if (simulateFormationDepth)
          {
            float line = Vec2.DistanceToLine(formationPositionMatrix.Origin.AsVec2, formationPositionMatrix.Origin.AsVec2 + formationRotation.f.AsVec2.RightVec(), accordingToNewOrder.Value.Origin.AsVec2);
            if ((double) line > (double) num)
              num = line;
          }
        }
        ++unitIndex;
      }
      simulationUnavailablePositionFrames?.AddRange(formation.GetUnavailableUnitPositionsAccordingToNewOrder(simulationFormation, formationPositionMatrix, formationWidth, formation.UnitSpacing - unitSpacingReduction));
      simulationFormationChanges?.Add(ValueTuple.Create<Formation, int, float, Vec2, WorldPosition>(formation, unitSpacingReduction, formationWidth, formationRotation.f.AsVec2, formationPositionMatrix.Origin));
      simulatedFormationDepth = num + formation.UnitDiameter;
    }

    public void SimulateDestinationFrames(
      out List<(Agent, WorldFrame)> simulationAgentFrames,
      float minDistance = 3f)
    {
      MBReadOnlyList<Formation> selectedFormations = this.SelectedFormations;
      simulationAgentFrames = new List<(Agent, WorldFrame)>(100);
      float minDistanceSq = minDistance * minDistance;
      foreach (Formation formation in selectedFormations)
        formation.ApplyActionOnEachUnit((Action<Agent, List<(Agent, WorldFrame)>>) ((agent, localSimulationAgentFrames) =>
        {
          WorldPosition orderPositionOfUnit = agent.Formation.GetOrderPositionOfUnit(agent);
          if (!orderPositionOfUnit.IsValid || (double) agent.Position.AsVec2.DistanceSquared(orderPositionOfUnit.AsVec2) < (double) minDistanceSq)
            return;
          Vec2 directionOfUnit = agent.Formation.GetDirectionOfUnit(agent);
          Mat3 identity = Mat3.Identity;
          identity.f = directionOfUnit.ToVec3();
          identity.Orthonormalize();
          WorldFrame worldFrame = new WorldFrame(identity, orderPositionOfUnit);
          localSimulationAgentFrames.Add((agent, worldFrame));
        }), simulationAgentFrames);
    }

    private void ToggleSideOrderUse(IEnumerable<Formation> formations, UsableMachine usable)
    {
      IEnumerable<Formation> source = formations.Where<Formation>((Func<Formation, bool>) (f => f.IsUsingMachine(usable)));
      if (source.IsEmpty<Formation>())
      {
        foreach (Formation formation in formations)
          formation.StartUsingMachine(usable, true);
        if (!usable.HasWaitFrame)
          return;
        foreach (Formation formation in formations)
          formation.MovementOrder = MovementOrder.MovementOrderFollowEntity(usable.WaitEntity);
      }
      else
      {
        foreach (Formation formation in source)
          formation.StopUsingMachine(usable, true);
      }
    }

    private static int GetLineOrderByClass(FormationClass formationClass) => Array.IndexOf<FormationClass>(new FormationClass[8]
    {
      FormationClass.HeavyInfantry,
      FormationClass.Infantry,
      FormationClass.HeavyCavalry,
      FormationClass.Cavalry,
      FormationClass.LightCavalry,
      FormationClass.NumberOfDefaultFormations,
      FormationClass.Ranged,
      FormationClass.HorseArcher
    }, formationClass);

    internal static IEnumerable<Formation> SortFormationsForHorizontalLayout(
      IEnumerable<Formation> formations)
    {
      return (IEnumerable<Formation>) formations.OrderBy<Formation, int>((Func<Formation, int>) (f => OrderController.GetLineOrderByClass(f.FormationIndex)));
    }

    private static IEnumerable<Formation> GetSortedFormations(
      IEnumerable<Formation> formations,
      bool isFormationLayoutVertical)
    {
      return isFormationLayoutVertical ? formations : OrderController.SortFormationsForHorizontalLayout(formations);
    }

    private static void MoveToLineSegment(
      IEnumerable<Formation> formations,
      Dictionary<Formation, Formation> simulationFormations,
      WorldPosition TargetLineSegmentBegin,
      WorldPosition TargetLineSegmentEnd,
      OnOrderIssuedDelegate OnOrderIssued,
      Dictionary<Formation, float> actualWidths,
      Dictionary<Formation, int> actualUnitSpacings,
      bool isFormationLayoutVertical = true)
    {
      foreach (Formation formation in formations)
      {
        int num;
        if (actualUnitSpacings.TryGetValue(formation, out num))
          formation.SetPositioning(unitSpacing: new int?(num));
        float customWidth;
        if (actualWidths.TryGetValue(formation, out customWidth))
          formation.FormOrder = FormOrder.FormOrderCustom(customWidth);
      }
      formations = OrderController.GetSortedFormations(formations, isFormationLayoutVertical);
      List<(Formation, int, float, Vec2, WorldPosition)> formationChanges;
      bool isLineShort;
      OrderController.SimulateNewOrderWithPositionAndDirection(formations, simulationFormations, TargetLineSegmentBegin, TargetLineSegmentEnd, out formationChanges, out isLineShort, isFormationLayoutVertical);
      foreach ((Formation formation1, int num1, float customWidth1, Vec2 direction, WorldPosition position) in formationChanges)
      {
        int local_14 = formation1.UnitSpacing;
        float local_15 = formation1.Width;
        if (num1 > 0)
        {
          int local_16 = Math.Max(formation1.UnitSpacing - num1, 0);
          formation1.SetPositioning(unitSpacing: new int?(local_16));
          if (formation1.UnitSpacing != local_14)
            actualUnitSpacings[formation1] = local_14;
        }
        if ((double) formation1.Width != (double) customWidth1)
        {
          formation1.FormOrder = FormOrder.FormOrderCustom(customWidth1);
          if (isLineShort)
            actualWidths[formation1] = local_15;
        }
        if (!isLineShort)
        {
          formation1.MovementOrder = MovementOrder.MovementOrderMove(position);
          formation1.FacingOrder = FacingOrder.FacingOrderLookAtDirection(direction);
          formation1.FormOrder = FormOrder.FormOrderCustom(customWidth1);
          if (OnOrderIssued != null)
          {
            IEnumerable<Formation> local_17 = Enumerable.Repeat<Formation>(formation1, 1);
            OnOrderIssued(OrderType.Move, local_17, new object[1]
            {
              (object) position
            });
            OnOrderIssued(OrderType.LookAtDirection, local_17, new object[1]
            {
              (object) direction
            });
            OnOrderIssued(OrderType.FormCustom, local_17, new object[1]
            {
              (object) customWidth1
            });
          }
        }
        else
        {
          Formation local_18 = formations.MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes));
          switch (OrderController.GetActiveFacingOrderOf(local_18))
          {
            case OrderType.LookAtEnemy:
              formation1.MovementOrder = MovementOrder.MovementOrderMove(position);
              if (OnOrderIssued != null)
              {
                IEnumerable<Formation> local_20 = Enumerable.Repeat<Formation>(formation1, 1);
                OnOrderIssued(OrderType.Move, local_20, new object[1]
                {
                  (object) position
                });
                OnOrderIssued(OrderType.LookAtEnemy, local_20, Array.Empty<object>());
                continue;
              }
              continue;
            case OrderType.LookAtDirection:
              formation1.MovementOrder = MovementOrder.MovementOrderMove(position);
              formation1.FacingOrder = FacingOrder.FacingOrderLookAtDirection(local_18.Direction);
              if (OnOrderIssued != null)
              {
                IEnumerable<Formation> local_21 = Enumerable.Repeat<Formation>(formation1, 1);
                OnOrderIssued(OrderType.Move, local_21, new object[1]
                {
                  (object) position
                });
                OnOrderIssued(OrderType.LookAtDirection, local_21, new object[1]
                {
                  (object) local_18.Direction
                });
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
    }

    public static Vec2 GetOrderLookAtDirection(IEnumerable<Formation> formations, Vec2 target)
    {
      if (!formations.Any<Formation>())
        return Vec2.One;
      Formation formation = formations.MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes));
      return (target - formation.OrderPosition.AsVec2).Normalized();
    }

    public static float GetOrderFormCustomWidth(
      IEnumerable<Formation> formations,
      Vec3 orderPosition)
    {
      return (Agent.Main.Position - orderPosition).Length;
    }

    internal void TransferUnits(Formation source, Formation target, int count)
    {
      source.TransferUnitsAux(target, count);
      if (this.OnOrderIssued == null)
        return;
      this.OnOrderIssued(OrderType.Transfer, Enumerable.Repeat<Formation>(source, 1), new object[2]
      {
        (object) target,
        (object) count
      });
    }

    internal IEnumerable<Formation> SplitFormation(
      Formation formation,
      int count = 2)
    {
      if (!formation.IsSplittableByAI || formation.CountOfUnitsWithoutDetachedOnes < count)
        return (IEnumerable<Formation>) new List<Formation>()
        {
          formation
        };
      MBDebug.Print((formation.Team.Side == BattleSideEnum.Attacker ? (object) "Attacker team" : (object) "Defender team").ToString() + " formation " + (object) (int) formation.FormationIndex + " split");
      List<Formation> formationList = new List<Formation>();
      formationList.Add(formation);
      for (; count > 1; --count)
      {
        int unitCount = formation.CountOfUnits / count;
        for (int index = 0; index < 8; ++index)
        {
          Formation formation1 = formation.Team.GetFormation((FormationClass) index);
          if (formation1.CountOfUnits == 0)
          {
            formation.TransferUnitsAux(formation1, unitCount);
            formationList.Add(formation1);
            OnOrderIssuedDelegate onOrderIssued = this.OnOrderIssued;
            if (onOrderIssued != null)
            {
              onOrderIssued(OrderType.Transfer, Enumerable.Repeat<Formation>(formation, 1), new object[2]
              {
                (object) formation1,
                (object) unitCount
              });
              break;
            }
            break;
          }
        }
      }
      return (IEnumerable<Formation>) formationList;
    }

    [Conditional("DEBUG")]
    public void TickDebug()
    {
    }

    public void AddOrderOverride(
      Func<Formation, MovementOrder, MovementOrder> orderOverride)
    {
      if (this.orderOverrides == null)
      {
        this.orderOverrides = new List<Func<Formation, MovementOrder, MovementOrder>>();
        this.overridenOrders = new List<(Formation, OrderType)>();
      }
      this.orderOverrides.Add(orderOverride);
    }

    public OrderType GetOverridenOrderType(Formation formation)
    {
      // ISSUE: unable to decompile the method.
    }

    private void CreateDefaultOrderOverrides() => this.AddOrderOverride((Func<Formation, MovementOrder, MovementOrder>) ((formation, order) =>
    {
      if (formation.ArrangementOrder.OrderType != OrderType.ArrangementCloseOrder || order.OrderType != OrderType.StandYourGround)
        return MovementOrder.MovementOrderStop;
      Vec2 averagePosition = formation.QuerySystem.AveragePosition;
      float movementSpeed = formation.QuerySystem.MovementSpeed;
      WorldPosition medianPosition = formation.QuerySystem.MedianPosition;
      medianPosition.SetVec2(averagePosition + formation.Direction * formation.Depth * (0.5f + movementSpeed));
      return MovementOrder.MovementOrderMove(medianPosition);
    }));

    private static void TryCancelStopOrder(Formation formation)
    {
      if (formation.MovementOrder.OrderEnum != MovementOrder.MovementOrderEnum.Stop)
        return;
      WorldPosition orderPosition = formation.OrderPosition;
      formation.MovementOrder = MovementOrder.MovementOrderMove(formation.OrderPosition);
    }
  }
}
