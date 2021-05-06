// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.TDMGoldGain
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class TDMGoldGain : GameNetworkMessage
  {
    public List<KeyValuePair<ushort, int>> GoldChangeEventList { get; private set; }

    public TDMGoldGain(
      List<KeyValuePair<ushort, int>> goldChangeEventList)
    {
      this.GoldChangeEventList = goldChangeEventList;
    }

    public TDMGoldGain()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteIntToPacket(this.GoldChangeEventList.Count - 1, CompressionMission.TdmGoldGainTypeCompressionInfo);
      foreach (KeyValuePair<ushort, int> goldChangeEvent in this.GoldChangeEventList)
      {
        GameNetworkMessage.WriteIntToPacket((int) goldChangeEvent.Key, CompressionMission.TdmGoldGainTypeCompressionInfo);
        GameNetworkMessage.WriteIntToPacket(goldChangeEvent.Value, CompressionMission.TdmGoldChangeCompressionInfo);
      }
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.GoldChangeEventList = new List<KeyValuePair<ushort, int>>();
      int num = GameNetworkMessage.ReadIntFromPacket(CompressionMission.TdmGoldGainTypeCompressionInfo, ref bufferReadValid) + 1;
      for (int index = 0; index < num; ++index)
        this.GoldChangeEventList.Add(new KeyValuePair<ushort, int>((ushort) GameNetworkMessage.ReadIntFromPacket(CompressionMission.TdmGoldGainTypeCompressionInfo, ref bufferReadValid), GameNetworkMessage.ReadIntFromPacket(CompressionMission.TdmGoldChangeCompressionInfo, ref bufferReadValid)));
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.GameMode;

    protected override string OnGetLogFormat() => "Gold change events synced.";
  }
}
