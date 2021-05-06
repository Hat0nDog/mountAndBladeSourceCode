// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetMissionObjectAnimationAtChannel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetMissionObjectAnimationAtChannel : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public int ChannelNo { get; private set; }

    public int AnimationIndex { get; private set; }

    public float AnimationSpeed { get; private set; }

    public SetMissionObjectAnimationAtChannel(
      MissionObject missionObject,
      int channelNo,
      int animationIndex,
      float animationSpeed)
    {
      this.MissionObject = missionObject;
      this.ChannelNo = channelNo;
      this.AnimationIndex = animationIndex;
      this.AnimationSpeed = animationSpeed;
    }

    public SetMissionObjectAnimationAtChannel()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.ChannelNo = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid) ? 1 : 0;
      this.AnimationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationIndexCompressionInfo, ref bufferReadValid);
      this.AnimationSpeed = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid) ? GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationSpeedCompressionInfo, ref bufferReadValid) : 1f;
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
      GameNetworkMessage.WriteBoolToPacket(this.ChannelNo == 1);
      GameNetworkMessage.WriteIntToPacket(this.AnimationIndex, CompressionBasic.AnimationIndexCompressionInfo);
      GameNetworkMessage.WriteBoolToPacket((double) this.AnimationSpeed != 1.0);
      if ((double) this.AnimationSpeed == 1.0)
        return;
      GameNetworkMessage.WriteFloatToPacket(this.AnimationSpeed, CompressionBasic.AnimationSpeedCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjectsDetailed;

    protected override string OnGetLogFormat() => "Set animation: " + (object) this.AnimationIndex + " on channel: " + (object) this.ChannelNo + " of MissionObject with ID: " + (object) this.MissionObject.Id.Id + (this.MissionObject.Id.CreatedAtRuntime ? (object) " (Created at runtime)" : (object) "") + " and with name: " + this.MissionObject.GameEntity.Name;
  }
}
