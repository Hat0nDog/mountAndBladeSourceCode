// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.StandingPointWithTeamLimit
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class StandingPointWithTeamLimit : StandingPoint
  {
    public Team UsableTeam { get; set; }

    public override bool IsDisabledForAgent(Agent agent) => agent.Team != this.UsableTeam || base.IsDisabledForAgent(agent);

    internal override bool IsUsableBySide(BattleSideEnum side) => side == this.UsableTeam.Side && base.IsUsableBySide(side);
  }
}
