// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.PlayerMessageAll
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class PlayerMessageAll : GameNetworkMessage
  {
    public NetworkCommunicator Player { get; private set; }

    public string Message { get; private set; }

    public PlayerMessageAll(NetworkCommunicator player, string message)
    {
      this.Player = player;
      this.Message = message;
    }

    public PlayerMessageAll()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Player);
      GameNetworkMessage.WriteStringToPacket(this.Message);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Player = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.Message = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Messaging;

    protected override string OnGetLogFormat() => "Receiving Player message to all: " + this.Message;
  }
}
