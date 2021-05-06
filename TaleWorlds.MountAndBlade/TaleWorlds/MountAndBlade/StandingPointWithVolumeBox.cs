// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.StandingPointWithVolumeBox
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  public class StandingPointWithVolumeBox : StandingPointWithWeaponRequirement
  {
    private const float MaxUserAgentDistance = 10f;
    private const float MaxUserAgentElevation = 2f;
    public string VolumeBoxTag = "volumebox";

    public override Agent.AIScriptedFrameFlags DisableScriptedFrameFlags => Agent.AIScriptedFrameFlags.NoAttack;

    public override bool IsDisabledForAgent(Agent agent) => base.IsDisabledForAgent(agent) || (double) Math.Abs(agent.Position.z - this.GameEntity.GlobalPosition.z) > 2.0 || (double) agent.Position.DistanceSquared(this.GameEntity.GlobalPosition) > 100.0;

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      MBEditor.IsEntitySelected(this.GameEntity);
    }
  }
}
