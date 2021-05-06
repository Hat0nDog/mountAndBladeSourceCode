// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.BotData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class BotData : GameNetworkMessage
  {
    public BattleSideEnum Side { get; private set; }

    public int KillCount { get; private set; }

    public int AssistCount { get; private set; }

    public int DeathCount { get; private set; }

    public int AliveBotCount { get; private set; }

    public BotData(BattleSideEnum side, int kill, int assist, int death, int alive)
    {
      this.Side = side;
      this.KillCount = kill;
      this.AssistCount = assist;
      this.DeathCount = death;
      this.AliveBotCount = alive;
    }

    public BotData()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Side = (BattleSideEnum) GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref bufferReadValid);
      this.KillCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref bufferReadValid);
      this.AssistCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref bufferReadValid);
      this.DeathCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref bufferReadValid);
      this.AliveBotCount = GameNetworkMessage.ReadIntFromPacket(CompressionMission.AgentCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket((int) this.Side, CompressionMission.TeamSideCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.KillCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.AssistCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.DeathCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.AliveBotCount, CompressionMission.AgentCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.General;

    protected override string OnGetLogFormat() => "BOTS for side: " + (object) this.Side + ", Kill: " + (object) this.KillCount + " Death: " + (object) this.DeathCount + " Assist: " + (object) this.AssistCount + ", Alive: " + (object) this.AliveBotCount;
  }
}
