// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SpawnAttachedWeaponOnCorpse
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SpawnAttachedWeaponOnCorpse : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public int AttachedIndex { get; private set; }

    public int ForcedIndex { get; private set; }

    public SpawnAttachedWeaponOnCorpse(Agent agent, int attachedIndex, int forcedIndex)
    {
      this.Agent = agent;
      this.AttachedIndex = attachedIndex;
      this.ForcedIndex = forcedIndex;
    }

    public SpawnAttachedWeaponOnCorpse()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.AttachedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.WeaponAttachmentIndexCompressionInfo, ref bufferReadValid);
      this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket(this.AttachedIndex, CompressionMission.WeaponAttachmentIndexCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Items;

    protected override string OnGetLogFormat() => "SpawnAttachedWeaponOnCorpse with index: " + (object) this.Agent.Index + ", and with ID: " + (object) this.ForcedIndex;
  }
}
