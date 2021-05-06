// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.DebugAgentScaleOnNetworkTest
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.DebugFromServer)]
  internal sealed class DebugAgentScaleOnNetworkTest : GameNetworkMessage
  {
    internal Agent AgentToTest { get; private set; }

    internal float ScaleToTest { get; private set; }

    public DebugAgentScaleOnNetworkTest()
    {
    }

    internal DebugAgentScaleOnNetworkTest(Agent toTest, float scale)
    {
      this.AgentToTest = toTest;
      this.ScaleToTest = scale;
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.AgentToTest = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid, true);
      this.ScaleToTest = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.DebugScaleValueCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.AgentToTest);
      GameNetworkMessage.WriteFloatToPacket(this.ScaleToTest, CompressionMission.DebugScaleValueCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => nameof (DebugAgentScaleOnNetworkTest);
  }
}
