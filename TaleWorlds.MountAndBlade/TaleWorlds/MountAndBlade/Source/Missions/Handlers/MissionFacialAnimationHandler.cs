// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Missions.Handlers.MissionFacialAnimationHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Source.Missions.Handlers
{
  public class MissionFacialAnimationHandler : MissionLogic
  {
    private Timer _animRefreshTimer;

    public override void EarlyStart() => this._animRefreshTimer = new Timer(this.Mission.Time, 5f);

    public override void AfterStart()
    {
    }

    public override void OnMissionTick(float dt)
    {
    }

    private void SetDefaultFacialAnimationsForAllAgents()
    {
      foreach (Agent agent in (IEnumerable<Agent>) this.Mission.Agents)
      {
        if (agent.IsActive() && agent.IsHuman)
          agent.SetAgentFacialAnimation(Agent.FacialAnimChannel.Low, "idle_tired", true);
      }
    }
  }
}
