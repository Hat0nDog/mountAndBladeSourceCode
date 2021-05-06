// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.MakeAgentDead
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class MakeAgentDead : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public bool IsKilled { get; private set; }

    public ActionIndexCache ActionCodeIndex { get; private set; }

    public MakeAgentDead(Agent agent, bool isKilled, ActionIndexCache actionCodeIndex)
    {
      this.Agent = agent;
      this.IsKilled = isKilled;
      this.ActionCodeIndex = actionCodeIndex;
    }

    public MakeAgentDead()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.IsKilled = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      this.ActionCodeIndex = new ActionIndexCache(GameNetworkMessage.ReadIntFromPacket(CompressionBasic.ActionCodeCompressionInfo, ref bufferReadValid));
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteBoolToPacket(this.IsKilled);
      GameNetworkMessage.WriteIntToPacket(this.ActionCodeIndex.Index, CompressionBasic.ActionCodeCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.EquipmentDetailed;

    protected override string OnGetLogFormat() => "Make Agent Dead on Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
