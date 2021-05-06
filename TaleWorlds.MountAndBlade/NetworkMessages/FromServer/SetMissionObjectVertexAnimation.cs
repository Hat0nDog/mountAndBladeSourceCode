// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetMissionObjectVertexAnimation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetMissionObjectVertexAnimation : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public int BeginKey { get; private set; }

    public int EndKey { get; private set; }

    public float Speed { get; private set; }

    public SetMissionObjectVertexAnimation(
      MissionObject missionObject,
      int beginKey,
      int endKey,
      float speed)
    {
      this.MissionObject = missionObject;
      this.BeginKey = beginKey;
      this.EndKey = endKey;
      this.Speed = speed;
    }

    public SetMissionObjectVertexAnimation()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.BeginKey = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref bufferReadValid);
      this.EndKey = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref bufferReadValid);
      this.Speed = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.VertexAnimationSpeedCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
      GameNetworkMessage.WriteIntToPacket(this.BeginKey, CompressionBasic.AnimationKeyCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.EndKey, CompressionBasic.AnimationKeyCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.Speed, CompressionBasic.VertexAnimationSpeedCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjectsDetailed;

    protected override string OnGetLogFormat() => "Set Vertex Animation on MissionObject with ID: " + (object) this.MissionObject.Id + " and with name: " + this.MissionObject.GameEntity.Name;
  }
}
