// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.InitializeLobbyPeer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.PlayerServices;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class InitializeLobbyPeer : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public PlayerId ProvidedId { get; private set; }

    public string BannerCode { get; private set; }

    public BodyProperties BodyProperties { get; private set; }

    public int ChosenBadgeIndex { get; private set; }

    public int ForcedAvatarIndex { get; private set; }

    public InitializeLobbyPeer(
      NetworkCommunicator peer,
      PlayerId providedId,
      string bannerCode,
      BodyProperties bodyProperties,
      int chosenBadgeIndex,
      int forcedAvatarIndex)
    {
      this.Peer = peer;
      this.ProvidedId = providedId;
      this.BannerCode = bannerCode != null ? bannerCode : string.Empty;
      this.BodyProperties = bodyProperties;
      this.ChosenBadgeIndex = chosenBadgeIndex;
      this.ForcedAvatarIndex = forcedAvatarIndex;
    }

    public InitializeLobbyPeer()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      ulong part1 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      ulong part2 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      ulong part3 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      ulong part4 = GameNetworkMessage.ReadUlongFromPacket(CompressionBasic.DebugULongNonCompressionInfo, ref bufferReadValid);
      this.BannerCode = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      if (bufferReadValid)
        this.ProvidedId = new PlayerId(part1, part2, part3, part4);
      this.BodyProperties = GameNetworkMessage.ReadBodyPropertiesFromPacket(ref bufferReadValid);
      this.ChosenBadgeIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.PlayerChosenBadgeCompressionInfo, ref bufferReadValid);
      this.ForcedAvatarIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.ForcedAvatarIndexCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteUlongToPacket(this.ProvidedId.Part1, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(this.ProvidedId.Part2, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(this.ProvidedId.Part3, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteUlongToPacket(this.ProvidedId.Part4, CompressionBasic.DebugULongNonCompressionInfo);
      GameNetworkMessage.WriteStringToPacket(this.BannerCode);
      BodyProperties bodyProperties = this.BodyProperties;
      GameNetworkMessage.WriteBodyPropertiesToPacket(in bodyProperties);
      GameNetworkMessage.WriteIntToPacket(this.ChosenBadgeIndex, CompressionBasic.PlayerChosenBadgeCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.ForcedAvatarIndex, CompressionBasic.ForcedAvatarIndexCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Peers;

    protected override string OnGetLogFormat() => "Initialize LobbyPeer from Peer: " + this.Peer.UserName;
  }
}
