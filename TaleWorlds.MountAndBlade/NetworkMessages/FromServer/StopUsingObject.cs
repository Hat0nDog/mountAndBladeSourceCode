// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.StopUsingObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class StopUsingObject : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public bool IsSuccessful { get; private set; }

    public StopUsingObject(Agent agent, bool isSuccessful)
    {
      this.Agent = agent;
      this.IsSuccessful = isSuccessful;
    }

    public StopUsingObject()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.IsSuccessful = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteBoolToPacket(this.IsSuccessful);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.AgentsDetailed | MultiplayerMessageFilter.MissionObjectsDetailed;

    protected override string OnGetLogFormat() => "Stop using Object on Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
