// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SiegeMoraleChangeMessage
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SiegeMoraleChangeMessage : GameNetworkMessage
  {
    public int AttackerMorale { get; private set; }

    public int DefenderMorale { get; private set; }

    public int[] CapturePointRemainingMoraleGains { get; private set; }

    public SiegeMoraleChangeMessage()
    {
    }

    public SiegeMoraleChangeMessage(
      int attackerMorale,
      int defenderMorale,
      int[] capturePointRemainingMoraleGains)
    {
      this.AttackerMorale = attackerMorale;
      this.DefenderMorale = defenderMorale;
      this.CapturePointRemainingMoraleGains = capturePointRemainingMoraleGains;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.AttackerMorale, CompressionMission.SiegeMoraleCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.DefenderMorale, CompressionMission.SiegeMoraleCompressionInfo);
      foreach (int remainingMoraleGain in this.CapturePointRemainingMoraleGains)
        GameNetworkMessage.WriteIntToPacket(remainingMoraleGain, CompressionMission.SiegeMoralePerFlagCompressionInfo);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.AttackerMorale = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeMoraleCompressionInfo, ref bufferReadValid);
      this.DefenderMorale = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeMoraleCompressionInfo, ref bufferReadValid);
      this.CapturePointRemainingMoraleGains = new int[7];
      for (int index = 0; index < this.CapturePointRemainingMoraleGains.Length; ++index)
        this.CapturePointRemainingMoraleGains[index] = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeMoralePerFlagCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.GameMode;

    protected override string OnGetLogFormat() => "Morale synched. A: " + (object) this.AttackerMorale + " D: " + (object) this.DefenderMorale;
  }
}
