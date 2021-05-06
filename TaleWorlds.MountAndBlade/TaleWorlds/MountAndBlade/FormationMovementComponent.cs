// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FormationMovementComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class FormationMovementComponent : AgentComponent
  {
    private FormationCohesionComponent _cohesionComponent;
    public Agent LastFollowedUnitForMovement;
    public Agent FollowedAgent;

    private FormationCohesionComponent CohesionComponent
    {
      get
      {
        if (this._cohesionComponent == null)
          this._cohesionComponent = this.Agent.GetComponent<FormationCohesionComponent>();
        return this._cohesionComponent;
      }
    }

    public FormationMovementComponent(Agent agent)
      : base(agent)
    {
    }

    protected internal override void Initialize() => base.Initialize();

    public bool GetFormationFrame(
      out WorldPosition formationPosition,
      out Vec2 formationDirection,
      out float speedLimit,
      out bool isSettingDestinationSpeed,
      out bool limitIsMultiplier,
      bool finalDestination = false)
    {
      Formation formation = this.Agent.Formation;
      isSettingDestinationSpeed = false;
      limitIsMultiplier = false;
      bool flag = false;
      if (formation != null)
      {
        formationPosition = formation.GetOrderPositionOfUnit(this.Agent);
        formationDirection = formation.GetDirectionOfUnit(this.Agent);
      }
      else
      {
        formationPosition = WorldPosition.Invalid;
        formationDirection = Vec2.Invalid;
      }
      if (FormationCohesionComponent.FormationSpeedAdjustmentEnabled && this.Agent.IsMount)
      {
        formationPosition = WorldPosition.Invalid;
        formationDirection = Vec2.Invalid;
        if (this.Agent.RiderAgent == null || this.Agent.RiderAgent != null && (!this.Agent.RiderAgent.IsActive() || this.Agent.RiderAgent.Formation == null))
        {
          speedLimit = -1f;
        }
        else
        {
          limitIsMultiplier = true;
          speedLimit = this.Agent.RiderAgent.GetComponent<FormationMovementComponent>().CohesionComponent.GetDesiredSpeedInFormation(formation.MovementOrder.MovementState == MovementOrder.MovementStateEnum.Charge);
        }
      }
      else if (formation == null)
        speedLimit = -1f;
      else if (formation.IsUnitDetached(this.Agent))
      {
        speedLimit = -1f;
        WorldFrame? nullable = new WorldFrame?();
        if (formation.MovementOrder.MovementState != MovementOrder.MovementStateEnum.Charge || this.Agent.Detachment != null && !this.Agent.Detachment.IsLoose)
          nullable = formation.GetDetachmentFrame(this.Agent);
        if (nullable.HasValue)
        {
          formationDirection = nullable.Value.Rotation.f.AsVec2.Normalized();
          flag = true;
        }
        else
          formationDirection = Vec2.Invalid;
      }
      else
      {
        switch (formation.MovementOrder.MovementState)
        {
          case MovementOrder.MovementStateEnum.Charge:
            limitIsMultiplier = true;
            speedLimit = FormationCohesionComponent.FormationSpeedAdjustmentEnabled ? this.CohesionComponent.GetDesiredSpeedInFormation(true) : -1f;
            break;
          case MovementOrder.MovementStateEnum.Hold:
            isSettingDestinationSpeed = true;
            if (FormationCohesionComponent.FormationSpeedAdjustmentEnabled && this.CohesionComponent.ShouldCatchUpWithFormation)
            {
              limitIsMultiplier = true;
              speedLimit = this.CohesionComponent.GetDesiredSpeedInFormation(false);
            }
            else
              speedLimit = -1f;
            flag = true;
            break;
          case MovementOrder.MovementStateEnum.Retreat:
            speedLimit = -1f;
            break;
          case MovementOrder.MovementStateEnum.StandGround:
            formationDirection = this.Agent.Frame.rotation.f.AsVec2;
            speedLimit = -1f;
            flag = true;
            break;
          default:
            speedLimit = -1f;
            break;
        }
      }
      return flag;
    }

    public void AdjustSpeedLimit(Agent agent, float desiredSpeed, bool limitIsMultiplier)
    {
      if (agent.MissionPeer != null)
        desiredSpeed = -1f;
      this.Agent.SetMaximumSpeedLimit(desiredSpeed, limitIsMultiplier);
      agent.MountAgent?.SetMaximumSpeedLimit(desiredSpeed, limitIsMultiplier);
    }

    public void Update()
    {
      WorldPosition formationPosition;
      Vec2 formationDirection;
      float speedLimit;
      bool limitIsMultiplier;
      int num1 = this.GetFormationFrame(out formationPosition, out formationDirection, out speedLimit, out bool _, out limitIsMultiplier) ? 1 : 0;
      this.AdjustSpeedLimit(this.Agent, speedLimit, limitIsMultiplier);
      if (this.Agent.Controller == Agent.ControllerType.AI && this.Agent.Formation != null && (this.Agent.Formation.MovementOrder.OrderEnum != MovementOrder.MovementOrderEnum.Stop && this.Agent.Formation.MovementOrder.OrderEnum != MovementOrder.MovementOrderEnum.Retreat) && !this.Agent.IsRetreating())
      {
        FormationQuerySystem.FormationIntegrityDataGroup formationIntegrityData = this.Agent.Formation.QuerySystem.FormationIntegrityData;
        float num2 = formationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents * 3f;
        if ((double) formationIntegrityData.DeviationOfPositionsExcludeFarAgents > (double) num2)
        {
          this.CohesionComponent.ShouldCatchUpWithFormation = false;
          this.Agent.SetFormationIntegrityData(Vec2.Zero, Vec2.Zero, Vec2.Zero, 0.0f, 0.0f);
        }
        else
        {
          Vec2 globalPositionOfUnit = this.Agent.Formation.GetCurrentGlobalPositionOfUnit(this.Agent, true);
          this.CohesionComponent.ShouldCatchUpWithFormation = (double) this.Agent.Position.AsVec2.Distance(globalPositionOfUnit) < (double) num2 * 2.0;
          this.Agent.SetFormationIntegrityData(this.CohesionComponent.ShouldCatchUpWithFormation ? globalPositionOfUnit : Vec2.Zero, this.Agent.Formation.CurrentDirection, formationIntegrityData.AverageVelocityExcludeFarAgents, formationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents, formationIntegrityData.DeviationOfPositionsExcludeFarAgents);
        }
      }
      else
        this.CohesionComponent.ShouldCatchUpWithFormation = false;
      if (this.Agent.Formation != null)
        this.CohesionComponent.Update();
      if (num1 == 0 || !formationPosition.IsValid)
      {
        this.Agent.SetFormationFrameDisabled();
      }
      else
      {
        this.Agent.SetFormationFrameEnabled(formationPosition, formationDirection, this.Agent.Formation.CalculateFormationDirectionEnforcingFactorForRank(((IFormationUnit) this.Agent).FormationRankIndex));
        if (this.Agent.Formation.MovementOrder.MovementState == MovementOrder.MovementStateEnum.StandGround)
          this.Agent.CurrentDiscipline = 1f;
        else
          this.Agent.CurrentDiscipline = (float) (1.0 - (double) this.Agent.Formation.UnitSpacing * 0.5);
        float num2 = 7f;
        if (this.Agent.Formation.UnitSpacing == 0)
          num2 = 0.2f;
        else if (this.Agent.Formation.UnitSpacing == 1)
          num2 = 7f;
        else if (this.Agent.Formation.UnitSpacing == 2)
          num2 = 15f;
        if (this.Agent.MountAgent != null)
        {
          float num3 = num2 + 5f;
        }
        float tendency = 1f;
        if (this.Agent.Formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.ShieldWall && !this.Agent.Formation.IsUnitDetached(this.Agent))
          tendency = this.Agent.Formation.arrangement.GetDirectionChangeTendencyOfUnit((IFormationUnit) this.Agent);
        this.Agent.SetDirectionChangeTendency(tendency);
      }
    }

    protected internal override void OnRetreating()
    {
      base.OnRetreating();
      this.AdjustSpeedLimit(this.Agent, -1f, false);
    }

    protected internal override void OnDismount(Agent mount)
    {
      base.OnDismount(mount);
      mount.SetMaximumSpeedLimit(-1f, false);
    }
  }
}
