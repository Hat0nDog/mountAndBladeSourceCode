// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.PollResponse
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class PollResponse : GameNetworkMessage
  {
    public bool Accepted { get; private set; }

    public PollResponse(bool accepted) => this.Accepted = accepted;

    public PollResponse()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Accepted = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteBoolToPacket(this.Accepted);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Administration;

    protected override string OnGetLogFormat() => "Receiving poll response: " + (this.Accepted ? "Accepted." : "Not accepted.");
  }
}
