// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetAgentActionSet
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SetAgentActionSet : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public MBActionSet ActionSet { get; private set; }

    public int NumPaces { get; private set; }

    public int MonsterUsageSetIndex { get; private set; }

    public float WalkingSpeedLimit { get; private set; }

    public float CrouchWalkingSpeedLimit { get; private set; }

    public float StepSize { get; private set; }

    public SetAgentActionSet(Agent agent, AnimationSystemData animationSystemData)
    {
      this.Agent = agent;
      this.ActionSet = animationSystemData.ActionSet;
      this.NumPaces = animationSystemData.NumPaces;
      this.MonsterUsageSetIndex = animationSystemData.MonsterUsageSetIndex;
      this.WalkingSpeedLimit = animationSystemData.WalkingSpeedLimit;
      this.CrouchWalkingSpeedLimit = animationSystemData.CrouchWalkingSpeedLimit;
      this.StepSize = animationSystemData.StepSize;
    }

    public SetAgentActionSet()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.ActionSet = GameNetworkMessage.ReadActionSetReferenceFromPacket(CompressionMission.ActionSetCompressionInfo, ref bufferReadValid);
      this.NumPaces = GameNetworkMessage.ReadIntFromPacket(CompressionMission.NumberOfPacesCompressionInfo, ref bufferReadValid);
      this.MonsterUsageSetIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.MonsterUsageSetCompressionInfo, ref bufferReadValid);
      this.WalkingSpeedLimit = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.WalkingSpeedLimitCompressionInfo, ref bufferReadValid);
      this.CrouchWalkingSpeedLimit = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.WalkingSpeedLimitCompressionInfo, ref bufferReadValid);
      this.StepSize = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.StepSizeCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteActionSetReferenceToPacket(this.ActionSet, CompressionMission.ActionSetCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.NumPaces, CompressionMission.NumberOfPacesCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.MonsterUsageSetIndex, CompressionMission.MonsterUsageSetCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.WalkingSpeedLimit, CompressionMission.WalkingSpeedLimitCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.CrouchWalkingSpeedLimit, CompressionMission.WalkingSpeedLimitCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.StepSize, CompressionMission.StepSizeCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.AgentAnimations;

    protected override string OnGetLogFormat() => "Set ActionSet: " + (object) this.ActionSet + " on agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
