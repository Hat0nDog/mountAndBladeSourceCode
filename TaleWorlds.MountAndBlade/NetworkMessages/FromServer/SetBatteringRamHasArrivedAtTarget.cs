// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetBatteringRamHasArrivedAtTarget
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetBatteringRamHasArrivedAtTarget : GameNetworkMessage
  {
    public BatteringRam BatteringRam { get; private set; }

    public SetBatteringRamHasArrivedAtTarget(BatteringRam batteringRam) => this.BatteringRam = batteringRam;

    public SetBatteringRamHasArrivedAtTarget()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.BatteringRam = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as BatteringRam;
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.BatteringRam);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Battering Ram with ID: " + (object) this.BatteringRam.Id + " and name: " + this.BatteringRam.GameEntity.Name + " has arrived at its target.";
  }
}
