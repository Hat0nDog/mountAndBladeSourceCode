// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetMissionObjectDisabled
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetMissionObjectDisabled : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public SetMissionObjectDisabled(MissionObject missionObject) => this.MissionObject = missionObject;

    public SetMissionObjectDisabled()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjects;

    protected override string OnGetLogFormat() => "Mission Object with ID: " + (object) this.MissionObject.Id.Id + " and with name: " + this.MissionObject.GameEntity.Name + " has been disabled.";
  }
}
