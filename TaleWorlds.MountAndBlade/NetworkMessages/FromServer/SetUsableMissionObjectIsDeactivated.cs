// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetUsableMissionObjectIsDeactivated
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetUsableMissionObjectIsDeactivated : GameNetworkMessage
  {
    public UsableMissionObject UsableGameObject { get; private set; }

    public bool IsDeactivated { get; private set; }

    public SetUsableMissionObjectIsDeactivated(
      UsableMissionObject usableGameObject,
      bool isDeactivated)
    {
      this.UsableGameObject = usableGameObject;
      this.IsDeactivated = isDeactivated;
    }

    public SetUsableMissionObjectIsDeactivated()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.UsableGameObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as UsableMissionObject;
      this.IsDeactivated = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.UsableGameObject);
      GameNetworkMessage.WriteBoolToPacket(this.IsDeactivated);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjects;

    protected override string OnGetLogFormat() => "Set IsDeactivated: " + (this.IsDeactivated ? (object) "True" : (object) "False") + " on UsableMissionObject with ID: " + (object) this.UsableGameObject.Id + " and with name: " + this.UsableGameObject.GameEntity.Name;
  }
}
