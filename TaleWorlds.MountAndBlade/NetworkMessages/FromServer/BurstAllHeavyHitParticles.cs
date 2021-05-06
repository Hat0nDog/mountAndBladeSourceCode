// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.BurstAllHeavyHitParticles
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class BurstAllHeavyHitParticles : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public BurstAllHeavyHitParticles(MissionObject missionObject) => this.MissionObject = missionObject;

    public BurstAllHeavyHitParticles()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjectsDetailed | MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Bursting all heavy-hit particles for the DestructableComponent of MissionObject with Id: " + (object) this.MissionObject.Id + " and name: " + this.MissionObject.GameEntity.Name;
  }
}
