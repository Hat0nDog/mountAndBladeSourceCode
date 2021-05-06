// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.CreateBanner
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class CreateBanner : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public string BannerCode { get; private set; }

    public CreateBanner(NetworkCommunicator peer, string bannerCode)
    {
      this.Peer = peer;
      this.BannerCode = bannerCode;
    }

    public CreateBanner()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteStringToPacket(this.BannerCode);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Peers | MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "Create banner for peer: " + this.Peer.UserName + ", with index: " + (object) this.Peer.Index;
  }
}
