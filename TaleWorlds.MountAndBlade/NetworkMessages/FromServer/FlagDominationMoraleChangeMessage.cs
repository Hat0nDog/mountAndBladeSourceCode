// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.FlagDominationMoraleChangeMessage
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class FlagDominationMoraleChangeMessage : GameNetworkMessage
  {
    public float Morale { get; private set; }

    public FlagDominationMoraleChangeMessage()
    {
    }

    public FlagDominationMoraleChangeMessage(float morale) => this.Morale = morale;

    protected override void OnWrite() => GameNetworkMessage.WriteFloatToPacket(this.Morale, CompressionMission.FlagDominationMoraleCompressionInfo);

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Morale = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagDominationMoraleCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.GameMode;

    protected override string OnGetLogFormat() => "Morale synched: " + (object) this.Morale;
  }
}
