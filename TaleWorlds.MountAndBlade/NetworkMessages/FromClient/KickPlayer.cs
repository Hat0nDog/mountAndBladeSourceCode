// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.KickPlayer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class KickPlayer : GameNetworkMessage
  {
    public NetworkCommunicator PlayerPeer { get; private set; }

    public bool BanPlayer { get; private set; }

    public KickPlayer(NetworkCommunicator playerPeer, bool banPlayer)
    {
      this.PlayerPeer = playerPeer;
      this.BanPlayer = banPlayer;
    }

    public KickPlayer()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.PlayerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.BanPlayer = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.PlayerPeer);
      GameNetworkMessage.WriteBoolToPacket(this.BanPlayer);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Administration;

    protected override string OnGetLogFormat() => "Requested to kick" + (this.BanPlayer ? " and ban" : "") + " player: " + this.PlayerPeer.UserName;
  }
}
