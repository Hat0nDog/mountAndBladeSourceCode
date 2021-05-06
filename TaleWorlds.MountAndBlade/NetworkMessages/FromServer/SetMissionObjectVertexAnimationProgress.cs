// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetMissionObjectVertexAnimationProgress
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetMissionObjectVertexAnimationProgress : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public float Progress { get; private set; }

    public SetMissionObjectVertexAnimationProgress(MissionObject missionObject, float progress)
    {
      this.MissionObject = missionObject;
      this.Progress = progress;
    }

    public SetMissionObjectVertexAnimationProgress()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.Progress = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
      GameNetworkMessage.WriteFloatToPacket(this.Progress, CompressionBasic.UnitVectorCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjectsDetailed;

    protected override string OnGetLogFormat() => "Set progress of Vertex Animation on MissionObject with ID: " + (object) this.MissionObject.Id + " and with name: " + this.MissionObject.GameEntity.Name + " to: " + (object) this.Progress;
  }
}
