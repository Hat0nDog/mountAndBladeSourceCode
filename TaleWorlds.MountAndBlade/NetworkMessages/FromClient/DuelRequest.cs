// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.DuelRequest
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  internal sealed class DuelRequest : GameNetworkMessage
  {
    public Agent RequestedAgent { get; private set; }

    public DuelRequest(Agent requestedAgent) => this.RequestedAgent = requestedAgent;

    public DuelRequest()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.RequestedAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteAgentReferenceToPacket(this.RequestedAgent);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.GameMode;

    protected override string OnGetLogFormat() => "Duel requested from agent with name: " + this.RequestedAgent.Name + " and index: " + (object) this.RequestedAgent.Index;
  }
}
