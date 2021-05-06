// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromClient.TeamChange
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromClient
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromClient)]
  internal sealed class TeamChange : GameNetworkMessage
  {
    public bool AutoAssign { get; private set; }

    public Team Team { get; private set; }

    public TeamChange(bool autoAssign, Team team)
    {
      this.AutoAssign = autoAssign;
      this.Team = team;
    }

    public TeamChange()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.AutoAssign = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      if (!this.AutoAssign)
        this.Team = GameNetworkMessage.ReadTeamReferenceFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteBoolToPacket(this.AutoAssign);
      if (this.AutoAssign)
        return;
      GameNetworkMessage.WriteTeamReferenceToPacket(this.Team);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

    protected override string OnGetLogFormat() => "Changed team to: " + (object) this.Team;
  }
}
