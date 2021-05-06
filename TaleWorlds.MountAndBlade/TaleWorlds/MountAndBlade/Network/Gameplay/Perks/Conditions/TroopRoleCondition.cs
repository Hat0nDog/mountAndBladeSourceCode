// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.TroopRoleCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class TroopRoleCondition : MPPerkCondition
  {
    protected static string StringType = "TroopRole";
    private TroopRoleCondition.Role _role;

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.PeerControlledAgentChange;

    protected TroopRoleCondition()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string str = node?.Attributes?["role"]?.Value;
      this._role = TroopRoleCondition.Role.Sergeant;
      if (str == null || Enum.TryParse<TroopRoleCondition.Role>(str, true, out this._role))
        return;
      this._role = TroopRoleCondition.Role.Sergeant;
    }

    public override bool Check(MissionPeer peer) => this.Check(peer?.ControlledAgent);

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      if (agent != null && MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0)
      {
        switch (this._role)
        {
          case TroopRoleCondition.Role.Sergeant:
            return this.IsAgentSergeant(agent);
          case TroopRoleCondition.Role.Troop:
            return !this.IsAgentBannerBearer(agent) && !this.IsAgentSergeant(agent);
          case TroopRoleCondition.Role.BannerBearer:
            return this.IsAgentBannerBearer(agent) && !this.IsAgentSergeant(agent);
        }
      }
      return false;
    }

    private bool IsAgentSergeant(Agent agent) => agent.Character == MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character).HeroCharacter;

    private bool IsAgentBannerBearer(Agent agent)
    {
      MissionPeer missionPeer = agent?.MissionPeer ?? agent?.OwningAgentMissionPeer;
      Formation controlledFormation = missionPeer?.ControlledFormation;
      if (controlledFormation != null)
      {
        MissionWeapon missionWeapon = agent.Equipment[EquipmentIndex.Weapon4];
        if (!missionWeapon.IsEmpty && missionWeapon.Item.ItemType == ItemObject.ItemTypeEnum.Banner && new Banner(controlledFormation.BannerCode, missionPeer.Team.Color, missionPeer.Team.Color2).Serialize() == missionWeapon.Banner.Serialize())
          return true;
      }
      return false;
    }

    private enum Role
    {
      Sergeant,
      Troop,
      BannerBearer,
    }
  }
}
