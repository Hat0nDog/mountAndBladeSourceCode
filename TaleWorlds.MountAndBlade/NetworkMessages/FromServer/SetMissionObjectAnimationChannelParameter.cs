// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetMissionObjectAnimationChannelParameter
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetMissionObjectAnimationChannelParameter : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public int ChannelNo { get; private set; }

    public float Parameter { get; private set; }

    public SetMissionObjectAnimationChannelParameter(
      MissionObject missionObject,
      int channelNo,
      float parameter)
    {
      this.MissionObject = missionObject;
      this.ChannelNo = channelNo;
      this.Parameter = parameter;
    }

    public SetMissionObjectAnimationChannelParameter()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      bool flag = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      if (bufferReadValid)
        this.ChannelNo = flag ? 1 : 0;
      this.Parameter = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
      GameNetworkMessage.WriteBoolToPacket(this.ChannelNo == 1);
      GameNetworkMessage.WriteFloatToPacket(this.Parameter, CompressionBasic.UnitVectorCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjectsDetailed;

    protected override string OnGetLogFormat() => "Set animation parameter: " + (object) this.Parameter + " on channel: " + (object) this.ChannelNo + " of MissionObject with ID: " + (object) this.MissionObject.Id + " and with name: " + this.MissionObject.GameEntity.Name;
  }
}
