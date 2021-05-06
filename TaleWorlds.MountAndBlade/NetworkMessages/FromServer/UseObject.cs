// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.UseObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class UseObject : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public UsableMissionObject UsableGameObject { get; private set; }

    public UseObject(Agent agent, UsableMissionObject usableGameObject)
    {
      this.Agent = agent;
      this.UsableGameObject = usableGameObject;
    }

    public UseObject()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as UsableMissionObject;
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.UsableGameObject);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Agents | MultiplayerMessageFilter.MissionObjects;

    protected override string OnGetLogFormat() => "Use UsableMissionObject with ID: " + (object) this.UsableGameObject.Id + " and name: " + this.UsableGameObject.GameEntity.Name + " by Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
