// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.KillDeathCountChange
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class KillDeathCountChange : GameNetworkMessage
  {
    public NetworkCommunicator VictimPeer { get; private set; }

    public NetworkCommunicator AttackerPeer { get; private set; }

    public int KillCount { get; private set; }

    public int AssistCount { get; private set; }

    public int DeathCount { get; private set; }

    public int Score { get; private set; }

    public KillDeathCountChange(
      NetworkCommunicator peer,
      NetworkCommunicator killedPeer,
      int killCount,
      int assistCount,
      int deathCount,
      int score)
    {
      this.VictimPeer = peer;
      this.AttackerPeer = killedPeer;
      this.KillCount = killCount;
      this.AssistCount = assistCount;
      this.DeathCount = deathCount;
      this.Score = score;
    }

    public KillDeathCountChange()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.VictimPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.AttackerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.KillCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref bufferReadValid);
      this.AssistCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref bufferReadValid);
      this.DeathCount = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.KillDeathAssistCountCompressionInfo, ref bufferReadValid);
      this.Score = GameNetworkMessage.ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.VictimPeer);
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.AttackerPeer);
      GameNetworkMessage.WriteIntToPacket(this.KillCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.AssistCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.DeathCount, CompressionMatchmaker.KillDeathAssistCountCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.Score, CompressionMatchmaker.ScoreCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Administration;

    protected override string OnGetLogFormat() => "Kill-Death Count Changed. Peer: " + (this.VictimPeer?.UserName ?? "NULL") + " killed peer: " + (this.AttackerPeer?.UserName ?? "NULL") + " and now has " + (object) this.KillCount + " kills, " + (object) this.AssistCount + " assists, and " + (object) this.DeathCount + " deaths.";
  }
}
