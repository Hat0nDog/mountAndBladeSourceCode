// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetSpawnedFormationCount
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SetSpawnedFormationCount : GameNetworkMessage
  {
    public int NumOfFormationsTeamOne { get; private set; }

    public int NumOfFormationsTeamTwo { get; private set; }

    public SetSpawnedFormationCount(int numFormationsTeamOne, int numFormationsTeamTwo)
    {
      this.NumOfFormationsTeamOne = numFormationsTeamOne;
      this.NumOfFormationsTeamTwo = numFormationsTeamTwo;
    }

    public SetSpawnedFormationCount()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.NumOfFormationsTeamOne = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref bufferReadValid);
      this.NumOfFormationsTeamTwo = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.NumOfFormationsTeamOne, CompressionOrder.FormationClassCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.NumOfFormationsTeamTwo, CompressionOrder.FormationClassCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Peers;

    protected override string OnGetLogFormat() => "Syncing formation count";
  }
}
