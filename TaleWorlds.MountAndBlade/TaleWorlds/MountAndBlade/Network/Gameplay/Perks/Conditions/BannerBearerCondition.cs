// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.BannerBearerCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class BannerBearerCondition : MPPerkCondition
  {
    protected static string StringType = "BannerBearer";

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.AliveBotCountChange | MPPerkCondition.PerkEventFlags.BannerPickUp | MPPerkCondition.PerkEventFlags.BannerDrop | MPPerkCondition.PerkEventFlags.SpawnEnd;

    public override bool IsPeerCondition => true;

    protected BannerBearerCondition()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
    }

    public override bool Check(MissionPeer peer)
    {
      Formation controlledFormation = peer?.ControlledFormation;
      if (controlledFormation != null && MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0)
      {
        foreach (IFormationUnit allUnit in controlledFormation.arrangement.GetAllUnits())
        {
          if (allUnit is Agent agent2 && agent2.IsActive())
          {
            MissionWeapon missionWeapon = agent2.Equipment[EquipmentIndex.Weapon4];
            if (!missionWeapon.IsEmpty && missionWeapon.Item.ItemType == ItemObject.ItemTypeEnum.Banner && new Banner(controlledFormation.BannerCode, peer.Team.Color, peer.Team.Color2).Serialize() == missionWeapon.Banner.Serialize())
              return true;
          }
        }
      }
      return false;
    }

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      return this.Check(agent?.MissionPeer ?? agent?.OwningAgentMissionPeer);
    }
  }
}
