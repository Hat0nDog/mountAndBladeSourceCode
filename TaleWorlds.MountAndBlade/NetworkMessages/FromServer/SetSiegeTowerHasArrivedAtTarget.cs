// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetSiegeTowerHasArrivedAtTarget
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetSiegeTowerHasArrivedAtTarget : GameNetworkMessage
  {
    public SiegeTower SiegeTower { get; private set; }

    public SetSiegeTowerHasArrivedAtTarget(SiegeTower siegeTower) => this.SiegeTower = siegeTower;

    public SetSiegeTowerHasArrivedAtTarget()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.SiegeTower = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as SiegeTower;
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.SiegeTower);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeapons;

    protected override string OnGetLogFormat() => "SiegeTower with ID: " + (object) this.SiegeTower.Id + " and name: " + this.SiegeTower.GameEntity.Name + " has arrived at its target.";
  }
}
