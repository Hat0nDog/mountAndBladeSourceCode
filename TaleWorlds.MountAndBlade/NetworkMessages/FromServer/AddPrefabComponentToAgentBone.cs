// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.AddPrefabComponentToAgentBone
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class AddPrefabComponentToAgentBone : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public string PrefabName { get; private set; }

    public sbyte BoneIndex { get; private set; }

    public AddPrefabComponentToAgentBone(Agent agent, string prefabName, sbyte boneIndex)
    {
      this.Agent = agent;
      this.PrefabName = prefabName;
      this.BoneIndex = boneIndex;
    }

    public AddPrefabComponentToAgentBone()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.PrefabName = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      this.BoneIndex = (sbyte) GameNetworkMessage.ReadIntFromPacket(CompressionMission.BoneIndexCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteStringToPacket(this.PrefabName);
      GameNetworkMessage.WriteIntToPacket((int) this.BoneIndex, CompressionMission.BoneIndexCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "Add prefab component: " + this.PrefabName + " on bone with index: " + (object) this.BoneIndex + " on agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
