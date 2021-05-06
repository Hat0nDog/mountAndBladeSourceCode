// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.PollProgress
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class PollProgress : GameNetworkMessage
  {
    public int VotesAccepted { get; private set; }

    public int VotesRejected { get; private set; }

    public PollProgress(int votesAccepted, int votesRejected)
    {
      this.VotesAccepted = votesAccepted;
      this.VotesRejected = votesRejected;
    }

    public PollProgress()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.VotesAccepted = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref bufferReadValid);
      this.VotesRejected = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.VotesAccepted, CompressionBasic.PlayerCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.VotesRejected, CompressionBasic.PlayerCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Administration;

    protected override string OnGetLogFormat() => "Update on the voting progress.";
  }
}
