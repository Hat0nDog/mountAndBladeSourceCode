// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.SetFollowedAgent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class SetFollowedAgent : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public SetFollowedAgent(Agent agent) => this.Agent = agent;

    public SetFollowedAgent()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid, true);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionDetailed;

    protected override string OnGetLogFormat() => "Peer switched spectating an agent";
  }
}
