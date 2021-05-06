// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.RejectPollStart
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class RejectPollStart : GameNetworkMessage
  {
    public string Reason { get; private set; }

    public RejectPollStart(string reason) => this.Reason = reason;

    public RejectPollStart()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Reason = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteStringToPacket(this.Reason);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Administration;

    protected override string OnGetLogFormat() => "Reject start of poll.";
  }
}
