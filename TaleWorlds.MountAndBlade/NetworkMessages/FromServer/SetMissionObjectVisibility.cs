// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetMissionObjectVisibility
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetMissionObjectVisibility : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public bool Visible { get; private set; }

    public SetMissionObjectVisibility(MissionObject missionObject, bool visible)
    {
      this.MissionObject = missionObject;
      this.Visible = visible;
    }

    public SetMissionObjectVisibility()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.Visible = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
      GameNetworkMessage.WriteBoolToPacket(this.Visible);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjects;

    protected override string OnGetLogFormat() => "Set Visibility of MissionObject with ID: " + (object) this.MissionObject.Id.Id + " and with name: " + this.MissionObject.GameEntity.Name + " to: " + (this.Visible ? (object) "True" : (object) "False");
  }
}
