// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.FlagRaisingStatus
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class FlagRaisingStatus : GameNetworkMessage
  {
    public float Progress { get; private set; }

    public CaptureTheFlagFlagDirection Direction { get; private set; }

    public float Speed { get; private set; }

    public FlagRaisingStatus()
    {
    }

    public FlagRaisingStatus(
      float currProgress,
      CaptureTheFlagFlagDirection direction,
      float speed)
    {
      this.Progress = currProgress;
      this.Direction = direction;
      this.Speed = speed;
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Progress = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagClassicProgressCompressionInfo, ref bufferReadValid);
      this.Direction = (CaptureTheFlagFlagDirection) GameNetworkMessage.ReadIntFromPacket(CompressionMission.FlagDirectionEnumCompressionInfo, ref bufferReadValid);
      if (bufferReadValid && this.Direction != CaptureTheFlagFlagDirection.None && this.Direction != CaptureTheFlagFlagDirection.Static)
        this.Speed = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagSpeedCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteFloatToPacket(this.Progress, CompressionMission.FlagClassicProgressCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) this.Direction, CompressionMission.FlagDirectionEnumCompressionInfo);
      if (this.Direction == CaptureTheFlagFlagDirection.None || this.Direction == CaptureTheFlagFlagDirection.Static)
        return;
      GameNetworkMessage.WriteFloatToPacket(this.Speed, CompressionMission.FlagSpeedCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.GameMode;

    protected override string OnGetLogFormat() => "Updating flag movement: Progress: " + (object) this.Progress + ", Direction: " + (object) this.Direction + ", Speed: " + (object) this.Speed;
  }
}
