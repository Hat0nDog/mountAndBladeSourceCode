// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SyncObjectHitpoints
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SyncObjectHitpoints : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public float Hitpoints { get; private set; }

    public SyncObjectHitpoints(MissionObject missionObject, float hitpoints)
    {
      this.MissionObject = missionObject;
      this.Hitpoints = hitpoints;
    }

    public SyncObjectHitpoints()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.Hitpoints = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.UsableGameObjectHealthCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
      GameNetworkMessage.WriteFloatToPacket(Math.Max(this.Hitpoints, 0.0f), CompressionMission.UsableGameObjectHealthCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Synchronize HitPoints: " + (object) this.Hitpoints + " of MissionObject with Id: " + (object) this.MissionObject.Id + " and name: " + this.MissionObject.GameEntity.Name;
  }
}
