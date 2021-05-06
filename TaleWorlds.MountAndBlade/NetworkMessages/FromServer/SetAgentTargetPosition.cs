// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetAgentTargetPosition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetAgentTargetPosition : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public Vec2 Position { get; private set; }

    public SetAgentTargetPosition(Agent agent, ref Vec2 position)
    {
      this.Agent = agent;
      this.Position = position;
    }

    public SetAgentTargetPosition()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.Position = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteVec2ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "Set Target Position: " + (object) this.Position + " on Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
