// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.RetreatAgentComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class RetreatAgentComponent : AgentComponent
  {
    private Timer positionReevaluateTimer;

    public RetreatAgentComponent(Agent agent)
      : base(agent)
    {
      this.positionReevaluateTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), (float) (3.0 + (double) MBRandom.RandomFloat * 0.100000001490116));
    }

    public bool IsRetreating => this.Agent.IsRetreating();

    public void Retreat(WorldPosition? retreatPosition = null)
    {
      if (this.IsRetreating && !retreatPosition.HasValue)
        return;
      this.positionReevaluateTimer.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission));
      if (!retreatPosition.HasValue)
        retreatPosition = new WorldPosition?(this.Agent.Mission.GetClosestFleePositionForAgent(this.Agent));
      this.Agent.Retreat(retreatPosition.Value);
    }

    public void StopRetreating()
    {
      if (!this.Agent.IsRetreating())
        return;
      this.Agent.StopRetreating();
    }

    public void ReevaluatePosition() => this.positionReevaluateTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission));
  }
}
