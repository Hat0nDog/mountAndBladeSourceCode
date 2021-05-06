// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.UseObjectAgentComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class UseObjectAgentComponent : AgentComponent
  {
    public UsableMissionObject CurrentlyMovingGameObject { get; private set; }

    public UseObjectAgentComponent(Agent agent)
      : base(agent)
    {
    }

    public void MoveToUsableGameObject(
      UsableMissionObject usedObject,
      Agent.AIScriptedFrameFlags scriptedFrameFlags = Agent.AIScriptedFrameFlags.NoAttack)
    {
      this.Agent.AIStateFlags |= Agent.AIStateFlag.UseObjectMoving;
      this.CurrentlyMovingGameObject = usedObject;
      usedObject.OnAIMoveToUse(this.Agent);
      WorldFrame userFrameForAgent = usedObject.GetUserFrameForAgent(this.Agent);
      this.Agent.SetScriptedPositionAndDirection(ref userFrameForAgent.Origin, userFrameForAgent.Rotation.f.AsVec2.RotationInRadians, false, scriptedFrameFlags);
    }

    public void MoveToClear()
    {
      if (this.CurrentlyMovingGameObject != null)
        this.CurrentlyMovingGameObject.OnMoveToStopped(this.Agent);
      this.CurrentlyMovingGameObject = (UsableMissionObject) null;
      this.Agent.AIStateFlags &= ~Agent.AIStateFlag.UseObjectMoving;
    }

    public bool IsMovingTo => this.Agent.AIStateFlags.HasAnyFlag<Agent.AIStateFlag>(Agent.AIStateFlag.UseObjectMoving);

    public bool IsUsing
    {
      get => this.Agent.AIStateFlags.HasAnyFlag<Agent.AIStateFlag>(Agent.AIStateFlag.UseObjectUsing);
      set
      {
        if (value)
          this.Agent.AIStateFlags |= Agent.AIStateFlag.UseObjectUsing;
        else
          this.Agent.AIStateFlags &= ~Agent.AIStateFlag.UseObjectUsing;
      }
    }
  }
}
