// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DeploymentHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class DeploymentHandler : MissionLogic
  {
    protected MissionMode previousMissionMode;
    protected readonly bool isPlayerAttacker;
    public const string DeploymentCastleBoundary = "deployment_castle_boundary";
    private const string BoundaryTagExpression = "deployment_castle_boundary(_\\d+)*";
    private bool areDeploymentPointsInitialized;

    public Team team => this.Mission.PlayerTeam;

    public bool IsPlayerAttacker => this.isPlayerAttacker;

    public DeploymentHandler(bool isPlayerAttacker) => this.isPlayerAttacker = isPlayerAttacker;

    public override void EarlyStart() => this.SetDeploymentBoundary(this.isPlayerAttacker ? BattleSideEnum.Attacker : BattleSideEnum.Defender);

    public override void AfterStart()
    {
      base.AfterStart();
      this.previousMissionMode = this.Mission.Mode;
      this.Mission.SetMissionMode(MissionMode.Deployment, true);
      this.team.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
    }

    private void OrderController_OnOrderIssued(
      OrderType orderType,
      IEnumerable<Formation> appliedFormations,
      params object[] delegateParams)
    {
      DeploymentHandler.OrderController_OnOrderIssued_Aux(orderType, appliedFormations, delegateParams);
    }

    internal static void OrderController_OnOrderIssued_Aux(
      OrderType orderType,
      IEnumerable<Formation> appliedFormations,
      params object[] delegateParams)
    {
      if (!appliedFormations.Any<Formation>())
        return;
      Team team = appliedFormations.First<Formation>().Team;
      bool flag = false;
      Action action1 = (Action) (() =>
      {
        foreach (Formation appliedFormation in appliedFormations)
          appliedFormation.ApplyActionOnEachUnit((Action<Agent>) (agent => agent.UpdateCachedAndFormationValues(true, false)));
      });
      Action action2 = (Action) (() =>
      {
        foreach (Formation appliedFormation in appliedFormations)
        {
          Vec2 direction = appliedFormation.FacingOrder.GetDirection(appliedFormation);
          appliedFormation.SetPositioning(new WorldPosition?(appliedFormation.MovementOrder.GetPosition(appliedFormation)), new Vec2?(direction));
        }
      });
      switch (orderType)
      {
        case OrderType.Move:
        case OrderType.MoveToLineSegment:
        case OrderType.MoveToLineSegmentWithHorizontalLayout:
        case OrderType.FollowMe:
        case OrderType.FollowEntity:
        case OrderType.Attach:
        case OrderType.Advance:
        case OrderType.FallBack:
          action2();
          action1();
          flag = true;
          break;
        case OrderType.Charge:
        case OrderType.ChargeWithTarget:
        case OrderType.GuardMe:
          action2();
          action1();
          flag = true;
          break;
        case OrderType.StandYourGround:
          action2();
          action1();
          flag = true;
          break;
        case OrderType.Retreat:
          action2();
          action1();
          flag = true;
          break;
        case OrderType.LookAtEnemy:
        case OrderType.LookAtDirection:
          action2();
          action1();
          flag = true;
          break;
        case OrderType.ArrangementLine:
        case OrderType.ArrangementCloseOrder:
        case OrderType.ArrangementLoose:
        case OrderType.ArrangementCircular:
        case OrderType.ArrangementSchiltron:
        case OrderType.ArrangementVee:
        case OrderType.ArrangementColumn:
        case OrderType.ArrangementScatter:
          action1();
          flag = true;
          break;
        case OrderType.FormCustom:
        case OrderType.FormDeep:
        case OrderType.FormWide:
        case OrderType.FormWider:
          action1();
          flag = true;
          break;
        case OrderType.Mount:
        case OrderType.Dismount:
          action1();
          flag = true;
          break;
        case OrderType.AIControlOn:
        case OrderType.AIControlOff:
          action2();
          action1();
          flag = true;
          break;
        case OrderType.Transfer:
        case OrderType.Use:
        case OrderType.AttackEntity:
          action1();
          flag = true;
          break;
      }
      if (!flag)
        return;
      IEnumerable<Formation> formations = team.FormationsIncludingSpecial.Where<Formation>((Func<Formation, bool>) (f => f.MovementOrder.OrderEnum == MovementOrder.MovementOrderEnum.Attach && appliedFormations.Contains<Formation>(f.MovementOrder.TargetFormation)));
      if (!formations.Any<Formation>())
        return;
      DeploymentHandler.OrderController_OnOrderIssued_Aux(OrderType.Attach, formations);
    }

    public void ForceUpdateAllUnits() => this.OrderController_OnOrderIssued(OrderType.Move, this.team.FormationsIncludingSpecial);

    public virtual void FinishDeployment()
    {
    }

    public override void OnRemoveBehaviour()
    {
      if (this.team != null)
        this.team.OnOrderIssued -= new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
      this.Mission.SetMissionMode(this.previousMissionMode, false);
      base.OnRemoveBehaviour();
    }

    public void SetDeploymentBoundary(BattleSideEnum side)
    {
      IEnumerable<GameEntity> withTagExpression = this.Mission.Scene.FindEntitiesWithTagExpression("deployment_castle_boundary(_\\d+)*");
      Regex regex = new Regex("deployment_castle_boundary(_\\d+)*");
      Func<GameEntity, string> getExpressedTag = (Func<GameEntity, string>) (e =>
      {
        foreach (string tag in e.Tags)
        {
          Match match = regex.Match(tag);
          if (match.Success)
            return match.Value;
        }
        return (string) null;
      });
      Func<GameEntity, string> keySelector = (Func<GameEntity, string>) (e => getExpressedTag(e));
      foreach (IGrouping<string, GameEntity> source1 in withTagExpression.GroupBy<GameEntity, string>(keySelector))
      {
        if (source1.Any<GameEntity>((Func<GameEntity, bool>) (e => e.HasTag(side.ToString()))))
        {
          string name = getExpressedTag(source1.First<GameEntity>());
          bool isAllowanceInside = !source1.Any<GameEntity>((Func<GameEntity, bool>) (e => e.HasTag("out")));
          IEnumerable<Vec2> source2 = source1.Select<GameEntity, Vec2>((Func<GameEntity, Vec2>) (bp => bp.GlobalPosition.AsVec2));
          this.Mission.Boundaries.Add(name, (ICollection<Vec2>) source2.ToList<Vec2>(), isAllowanceInside);
        }
      }
    }

    public void RemoveAllBoundaries()
    {
      List<string> stringList = new List<string>();
      foreach (KeyValuePair<string, ICollection<Vec2>> boundary in this.Mission.Boundaries)
        stringList.Add(boundary.Key);
      foreach (string name in stringList)
        this.Mission.Boundaries.Remove(name);
    }

    public void InitializeDeploymentPoints()
    {
      if (this.areDeploymentPointsInitialized)
        return;
      foreach (DeploymentPoint deploymentPoint in this.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>())
        deploymentPoint.Hide();
      this.areDeploymentPointsInitialized = true;
    }
  }
}
