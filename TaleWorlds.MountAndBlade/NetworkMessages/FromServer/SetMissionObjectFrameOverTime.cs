// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetMissionObjectFrameOverTime
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class SetMissionObjectFrameOverTime : GameNetworkMessage
  {
    public MissionObject MissionObject { get; private set; }

    public MatrixFrame Frame { get; private set; }

    public float Duration { get; private set; }

    public SetMissionObjectFrameOverTime(
      MissionObject missionObject,
      ref MatrixFrame frame,
      float duration)
    {
      this.MissionObject = missionObject;
      this.Frame = frame;
      this.Duration = duration;
    }

    public SetMissionObjectFrameOverTime()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.MissionObject = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid);
      this.Frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref bufferReadValid);
      this.Duration = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagCapturePointDurationCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteMissionObjectReferenceToPacket(this.MissionObject);
      GameNetworkMessage.WriteMatrixFrameToPacket(this.Frame);
      GameNetworkMessage.WriteFloatToPacket(this.Duration, CompressionMission.FlagCapturePointDurationCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.MissionObjectsDetailed;

    protected override string OnGetLogFormat() => "Set Move-to-frame on MissionObject with ID: " + (object) this.MissionObject.Id + " and with name: " + this.MissionObject.GameEntity.Name + " over a period of " + (object) this.Duration + " seconds.";
  }
}
