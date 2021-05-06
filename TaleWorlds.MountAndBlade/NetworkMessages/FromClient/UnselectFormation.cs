// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.UnselectFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  public sealed class UnselectFormation : GameNetworkMessage
  {
    public int FormationIndex { get; private set; }

    public UnselectFormation(int formationIndex) => this.FormationIndex = formationIndex;

    public UnselectFormation()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.FormationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteIntToPacket(this.FormationIndex, CompressionOrder.FormationClassCompressionInfo);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Formations;

    protected override string OnGetLogFormat() => "Deselect Formation with index: " + (object) this.FormationIndex;
  }
}
