// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FadeOutAgentComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class FadeOutAgentComponent : AgentComponent
  {
    private readonly Timer timer;

    public FadeOutAgentComponent(Agent agent)
      : base(agent)
    {
      this.timer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), (float) (0.5 + (double) MBRandom.RandomFloat * 0.100000001490116));
    }

    public void CheckFadeOut()
    {
      if (!this.timer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)) || this.Agent.IsFadingOut())
        return;
      Vec3 position = this.Agent.Position;
      WorldPosition retreatPos = this.Agent.GetRetreatPos();
      if ((!retreatPos.AsVec2.IsValid || (double) retreatPos.AsVec2.DistanceSquared(position.AsVec2) >= 25.0 || (double) retreatPos.GetGroundVec3().DistanceSquared(position) >= 25.0) && (this.Agent.Mission.IsPositionInsideBoundaries(position.AsVec2) && (double) position.DistanceSquared(this.Agent.Mission.GetClosestBoundaryPosition(position.AsVec2).ToVec3()) >= 25.0))
        return;
      this.Agent.StartFadingOut();
    }
  }
}
