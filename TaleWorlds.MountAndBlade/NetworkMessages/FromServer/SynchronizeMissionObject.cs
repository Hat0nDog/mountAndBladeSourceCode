// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SynchronizeMissionObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SynchronizeMissionObject : GameNetworkMessage
  {
    public SynchedMissionObject MissionObject { get; private set; }

    public SynchronizeMissionObject(SynchedMissionObject missionObject) => this.MissionObject = missionObject;

    public SynchronizeMissionObject()
    {
    }

    protected override void OnWrite() => this.WriteSynchedMissionObjectToPacket(this.MissionObject);

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadSynchedMissionObjectFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

    protected override string OnGetLogFormat() => "Synchronize MissionObject with Id: " + (object) this.MissionObject.Id.Id + " and name: " + ((NativeObject) this.MissionObject.GameEntity != (NativeObject) null ? (object) this.MissionObject.GameEntity.Name : (object) "null entity");
  }
}
