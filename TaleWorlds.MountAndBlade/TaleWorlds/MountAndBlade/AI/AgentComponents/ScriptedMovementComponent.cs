// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AI.AgentComponents.ScriptedMovementComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.AI.AgentComponents
{
  public class ScriptedMovementComponent : AgentComponent
  {
    private bool _isInDialogueRange;
    private readonly bool _isCharacterToTalkTo;
    private readonly float _dialogueTriggerProximity = 10f;
    private readonly float _agentSpeedLimit;
    private Agent _targetAgent;

    public ScriptedMovementComponent(
      Agent agent,
      bool isCharacterToTalkTo = false,
      float dialogueProximityOffset = 0.0f)
      : base(agent)
    {
      this._isCharacterToTalkTo = isCharacterToTalkTo;
      this._agentSpeedLimit = this.Agent.GetMaximumSpeedLimit();
      if (this._isCharacterToTalkTo)
        return;
      this.Agent.SetMaximumSpeedLimit(0.3f, true);
      this._dialogueTriggerProximity += dialogueProximityOffset;
    }

    public void SetTargetAgent(Agent targetAgent) => this._targetAgent = targetAgent;

    protected internal override void OnTickAsAI(float dt)
    {
      if (this._isInDialogueRange || this._targetAgent == null)
        return;
      this._isInDialogueRange = (double) this._targetAgent.Position.DistanceSquared(this.Agent.Position) <= (double) this._dialogueTriggerProximity * (double) this._dialogueTriggerProximity;
      if (this._isInDialogueRange)
      {
        this.Agent.SetScriptedFlags(this.Agent.GetScriptedFlags() & ~Agent.AIScriptedFrameFlags.DoNotRun);
        this.Agent.DisableScriptedMovement();
        this.Agent.SetLookAgent(this._targetAgent);
        this.Agent.SetMaximumSpeedLimit(this._agentSpeedLimit, false);
      }
      else
      {
        WorldPosition worldPosition = this._targetAgent.Position.ToWorldPosition();
        this.Agent.SetScriptedPosition(ref worldPosition, false, Agent.AIScriptedFrameFlags.DoNotRun);
      }
    }

    public bool ShouldConversationStartWithAgent() => this._isInDialogueRange && this._isCharacterToTalkTo;
  }
}
