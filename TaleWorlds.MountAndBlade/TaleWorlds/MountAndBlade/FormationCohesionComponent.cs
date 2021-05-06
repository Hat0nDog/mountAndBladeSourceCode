// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FormationCohesionComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class FormationCohesionComponent : AgentComponent
  {
    private bool _formationNeighborhoodDataIsDirty;
    private bool _shouldCatchUpWithFormation;
    public static bool FormationSpeedAdjustmentEnabled = true;
    private const int _numNeighbors = 4;
    private Agent[] _neighborAgents = new Agent[4];
    private int[] _neighborAgentIndices = new int[4];
    private Vec2[] _neighborAgentLocalDifferences = new Vec2[4];

    public bool ShouldCatchUpWithFormation
    {
      get => this._shouldCatchUpWithFormation;
      set
      {
        if (this._shouldCatchUpWithFormation == value)
          return;
        this._shouldCatchUpWithFormation = value;
        this.Agent.SetShouldCatchUpWithFormation(value);
      }
    }

    public FormationCohesionComponent(Agent agent)
      : base(agent)
    {
    }

    public void MarkAsDirty() => this._formationNeighborhoodDataIsDirty = true;

    public void Update()
    {
      if (!this._formationNeighborhoodDataIsDirty)
        return;
      this._neighborAgents[0] = this.Agent.Formation.GetNeighbourUnit((IFormationUnit) this.Agent, -1, 0) as Agent;
      this._neighborAgents[1] = this.Agent.Formation.GetNeighbourUnit((IFormationUnit) this.Agent, 1, 0) as Agent;
      this._neighborAgents[2] = this.Agent.Formation.GetNeighbourUnit((IFormationUnit) this.Agent, 0, -1) as Agent;
      this._neighborAgents[3] = this.Agent.Formation.GetNeighbourUnit((IFormationUnit) this.Agent, 0, 1) as Agent;
      Vec2 localPositionOfUnit = this.Agent.Formation.GetLocalPositionOfUnit(this.Agent);
      for (int index = 0; index < 4; ++index)
      {
        if (this._neighborAgents[index] != null)
        {
          this._neighborAgentIndices[index] = this._neighborAgents[index].Index;
          this._neighborAgentLocalDifferences[index] = this.Agent.Formation.GetLocalPositionOfUnit(this._neighborAgents[index]) - localPositionOfUnit;
        }
        else
          this._neighborAgentIndices[index] = -1;
      }
      this.Agent.SetFormationNeighborhoodData(this._neighborAgentIndices, this._neighborAgentLocalDifferences);
      this._formationNeighborhoodDataIsDirty = false;
    }

    public float GetDesiredSpeedInFormation(bool isCharging)
    {
      if (this.ShouldCatchUpWithFormation)
      {
        Agent mountAgent = this.Agent.MountAgent;
        float num1 = mountAgent != null ? mountAgent.MaximumForwardUnlimitedSpeed : this.Agent.MaximumForwardUnlimitedSpeed;
        bool flag = !isCharging;
        if (isCharging)
        {
          FormationQuerySystem closestEnemyFormation = this.Agent.Formation.QuerySystem.ClosestEnemyFormation;
          float num2 = float.MaxValue;
          if (closestEnemyFormation != null)
            num2 = this.Agent.Formation.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(closestEnemyFormation.MedianPosition.GetNavMeshVec3());
          flag = (double) num2 > 4.0 * (double) num1 * (double) num1;
        }
        if (flag)
        {
          Vec2 v = this.Agent.Formation.GetCurrentGlobalPositionOfUnit(this.Agent, true) - this.Agent.Position.AsVec2;
          float num2 = MathF.Clamp(-this.Agent.GetMovementDirection().AsVec2.DotProduct(v), 0.0f, 100f);
          float num3 = this.Agent.MountAgent != null ? 4f : 2f;
          float num4 = (isCharging ? this.Agent.Formation.QuerySystem.FormationIntegrityData.AverageMaxUnlimitedSpeedExcludeFarAgents : this.Agent.Formation.QuerySystem.MovementSpeed) / num1;
          return MathF.Clamp((float) (0.699999988079071 + 0.400000005960464 * (((double) num1 - (double) num2 * (double) num3) / ((double) num1 + (double) num2 * (double) num3))) * num4, 0.3f, 1f);
        }
      }
      return 1f;
    }
  }
}
