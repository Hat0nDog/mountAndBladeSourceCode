// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.PlayerMessageTeam
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class PlayerMessageTeam : GameNetworkMessage
  {
    public string Message { get; private set; }

    public PlayerMessageTeam(string message) => this.Message = message;

    public PlayerMessageTeam()
    {
    }

    protected override void OnWrite() => GameNetworkMessage.WriteStringToPacket(this.Message);

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Message = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Messaging;

    protected override string OnGetLogFormat() => "Receiving Player message to team: " + this.Message;
  }
}
