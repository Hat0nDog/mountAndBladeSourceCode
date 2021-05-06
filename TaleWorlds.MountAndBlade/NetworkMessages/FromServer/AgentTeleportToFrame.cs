// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.AgentTeleportToFrame
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class AgentTeleportToFrame : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public Vec3 Position { get; private set; }

    public Vec2 Direction { get; private set; }

    public AgentTeleportToFrame(Agent agent, Vec3 position, Vec2 direction)
    {
      this.Agent = agent;
      this.Position = position;
      this.Direction = direction.Normalized();
    }

    public AgentTeleportToFrame()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.Position = GameNetworkMessage.ReadVec3FromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
      this.Direction = GameNetworkMessage.ReadVec2FromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteVec3ToPacket(this.Position, CompressionBasic.PositionCompressionInfo);
      GameNetworkMessage.WriteVec2ToPacket(this.Direction, CompressionBasic.UnitVectorCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "Teleporting agent with name: " + this.Agent.Name + ", and index: " + (object) this.Agent.Index + " to frame with position: " + (object) this.Position + " and direction: " + (object) this.Direction;
  }
}
