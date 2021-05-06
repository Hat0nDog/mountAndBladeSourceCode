// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetMissionObjectAnimationPaused
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetMissionObjectAnimationPaused : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public bool IsPaused { get; private set; }

    public SetMissionObjectAnimationPaused(MissionObject missionObject, bool isPaused)
    {
      this.MissionObject = missionObject;
      this.IsPaused = isPaused;
    }

    public SetMissionObjectAnimationPaused()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.IsPaused = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
      GameNetworkMessage.WriteBoolToPacket(this.IsPaused);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjectsDetailed;

    protected override string OnGetLogFormat() => "Set animation to be: " + (this.IsPaused ? (object) "Paused" : (object) "Not paused") + " on MissionObject with ID: " + (object) this.MissionObject.Id + " and with name: " + this.MissionObject.GameEntity.Name;
  }
}
