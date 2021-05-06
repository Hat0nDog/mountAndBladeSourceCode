// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetSiegeLadderState
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetSiegeLadderState : GameNetworkMessage
  {
    public SiegeLadder SiegeLadder { get; private set; }

    public SiegeLadder.LadderState State { get; private set; }

    public SetSiegeLadderState(SiegeLadder siegeLadder, SiegeLadder.LadderState state)
    {
      this.SiegeLadder = siegeLadder;
      this.State = state;
    }

    public SetSiegeLadderState()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.SiegeLadder = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as SiegeLadder;
      this.State = (SiegeLadder.LadderState) GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeLadderStateCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.SiegeLadder);
      GameNetworkMessage.WriteIntToPacket((int) this.State, CompressionMission.SiegeLadderStateCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.SiegeWeaponsDetailed;

    protected override string OnGetLogFormat() => "Set SiegeLadder State to: " + (object) this.State + " on SiegeLadderState with ID: " + (object) this.SiegeLadder.Id + " and name: " + this.SiegeLadder.GameEntity.Name;
  }
}
