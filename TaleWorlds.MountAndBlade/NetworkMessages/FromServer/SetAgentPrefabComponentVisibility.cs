// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetAgentPrefabComponentVisibility
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SetAgentPrefabComponentVisibility : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public int ComponentIndex { get; private set; }

    public bool Visibility { get; private set; }

    public SetAgentPrefabComponentVisibility(Agent agent, int componentIndex, bool visibility)
    {
      this.Agent = agent;
      this.ComponentIndex = componentIndex;
      this.Visibility = visibility;
    }

    public SetAgentPrefabComponentVisibility()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.ComponentIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentPrefabComponentIndexCompressionInfo, ref bufferReadValid);
      this.Visibility = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket(this.ComponentIndex, CompressionMission.AgentPrefabComponentIndexCompressionInfo);
      GameNetworkMessage.WriteBoolToPacket(this.Visibility);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "Set Component with index: " + (object) this.ComponentIndex + " to be " + (this.Visibility ? (object) "visible" : (object) "invisible") + " on Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
