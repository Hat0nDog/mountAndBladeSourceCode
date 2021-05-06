// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.CultureVoteServer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class CultureVoteServer : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public BasicCultureObject VotedCulture { get; private set; }

    public CultureVoteTypes VotedType { get; private set; }

    public CultureVoteServer()
    {
    }

    public CultureVoteServer(
      NetworkCommunicator peer,
      CultureVoteTypes type,
      BasicCultureObject culture)
    {
      this.Peer = peer;
      this.VotedType = type;
      this.VotedCulture = culture;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteIntToPacket((int) this.VotedType, CompressionMission.TeamSideCompressionInfo);
      MBReadOnlyList<BasicCultureObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>();
      GameNetworkMessage.WriteIntToPacket(this.VotedCulture == null ? -1 : objectTypeList.IndexOf(this.VotedCulture), CompressionBasic.CultureIndexCompressionInfo);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.VotedType = (CultureVoteTypes) GameNetworkMessage.ReadIntFromPacket(CompressionMission.TeamSideCompressionInfo, ref bufferReadValid);
      int index = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.CultureIndexCompressionInfo, ref bufferReadValid);
      if (bufferReadValid)
      {
        MBReadOnlyList<BasicCultureObject> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>();
        this.VotedCulture = index < 0 ? (BasicCultureObject) null : objectTypeList[index];
      }
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

    protected override string OnGetLogFormat() => "Culture " + (object) this.VotedCulture.Name + " has been " + this.VotedType.ToString().ToLower() + (this.VotedType == CultureVoteTypes.Ban ? (object) "ned." : (object) "ed.");
  }
}
