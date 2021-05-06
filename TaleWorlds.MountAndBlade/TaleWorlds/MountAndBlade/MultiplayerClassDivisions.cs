// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerClassDivisions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerClassDivisions
  {
    public static IEnumerable<BasicCultureObject> AvailableCultures;

    public static List<MultiplayerClassDivisions.MPHeroClassGroup> MultiplayerHeroClassGroups { get; private set; }

    public static IEnumerable<MultiplayerClassDivisions.MPHeroClass> GetMPHeroClasses(
      BasicCultureObject culture)
    {
      return MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>().Where<MultiplayerClassDivisions.MPHeroClass>((Func<MultiplayerClassDivisions.MPHeroClass, bool>) (x => x.Culture == culture));
    }

    public static MBReadOnlyList<MultiplayerClassDivisions.MPHeroClass> GetMPHeroClasses() => MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>();

    public static MultiplayerClassDivisions.MPHeroClass GetMPHeroClassForCharacter(
      BasicCharacterObject character)
    {
      return MBObjectManager.Instance.GetObjectTypeList<MultiplayerClassDivisions.MPHeroClass>().FirstOrDefault<MultiplayerClassDivisions.MPHeroClass>((Func<MultiplayerClassDivisions.MPHeroClass, bool>) (x => x.HeroCharacter == character || x.TroopCharacter == character));
    }

    public static MultiplayerClassDivisions.MPHeroClass GetMPHeroClassForPeer(
      MissionPeer peer)
    {
      Team team = peer.Team;
      bool flag1 = team == Mission.Current.AttackerTeam;
      bool flag2 = team == Mission.Current.DefenderTeam;
      if (team == null || !flag1 && !flag2 || peer.SelectedTroopIndex < 0 && peer.ControlledAgent == null)
        return (MultiplayerClassDivisions.MPHeroClass) null;
      if (peer.ControlledAgent != null)
        return MultiplayerClassDivisions.GetMPHeroClassForCharacter(peer.ControlledAgent.Character);
      return peer.SelectedTroopIndex >= 0 ? MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(flag1 ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue() : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue())).ToList<MultiplayerClassDivisions.MPHeroClass>()[peer.SelectedTroopIndex] : (MultiplayerClassDivisions.MPHeroClass) null;
    }

    public static TargetIconType GetMPHeroClassForFormation(Formation formation)
    {
      switch (formation.PrimaryClass)
      {
        case FormationClass.Infantry:
          return TargetIconType.Infantry_Light;
        case FormationClass.Ranged:
          return TargetIconType.Archer_Light;
        case FormationClass.Cavalry:
          return TargetIconType.Cavalry_Light;
        default:
          return TargetIconType.HorseArcher_Light;
      }
    }

    public static List<List<IReadOnlyPerkObject>> GetAvailablePerksForPeer(
      MissionPeer missionPeer)
    {
      List<List<IReadOnlyPerkObject>> readOnlyPerkObjectListList = new List<List<IReadOnlyPerkObject>>();
      if (missionPeer?.Team == null)
        return readOnlyPerkObjectListList;
      MultiplayerClassDivisions.MPHeroClass heroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(missionPeer);
      for (int index = 0; index < 3; ++index)
        readOnlyPerkObjectListList.Add(heroClassForPeer.GetAllAvailablePerksForListIndex(index).ToList<IReadOnlyPerkObject>());
      return readOnlyPerkObjectListList;
    }

    public static void Initialize()
    {
      MultiplayerClassDivisions.MultiplayerHeroClassGroups = new List<MultiplayerClassDivisions.MPHeroClassGroup>()
      {
        new MultiplayerClassDivisions.MPHeroClassGroup("Infantry"),
        new MultiplayerClassDivisions.MPHeroClassGroup("Ranged"),
        new MultiplayerClassDivisions.MPHeroClassGroup("Cavalry"),
        new MultiplayerClassDivisions.MPHeroClassGroup("HorseArcher")
      };
      MultiplayerClassDivisions.AvailableCultures = ((IEnumerable<BasicCultureObject>) MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>().ToArray<BasicCultureObject>()).Where<BasicCultureObject>((Func<BasicCultureObject, bool>) (x => x.IsMainCulture));
    }

    private static BasicCharacterObject GetMPCharacter(string stringId) => MBObjectManager.Instance.GetObject<BasicCharacterObject>(stringId);

    public static int GetMinimumTroopCost(BasicCultureObject culture = null)
    {
      MBReadOnlyList<MultiplayerClassDivisions.MPHeroClass> mpHeroClasses = MultiplayerClassDivisions.GetMPHeroClasses();
      return culture != null ? mpHeroClasses.Where<MultiplayerClassDivisions.MPHeroClass>((Func<MultiplayerClassDivisions.MPHeroClass, bool>) (c => c.Culture == culture)).Min<MultiplayerClassDivisions.MPHeroClass>((Func<MultiplayerClassDivisions.MPHeroClass, int>) (troop => troop.TroopCost)) : mpHeroClasses.Min<MultiplayerClassDivisions.MPHeroClass>((Func<MultiplayerClassDivisions.MPHeroClass, int>) (troop => troop.TroopCost));
    }

    public class MPHeroClass : MBObjectBase
    {
      private List<IReadOnlyPerkObject> _perks;

      public BasicCharacterObject HeroCharacter { get; private set; }

      public BasicCharacterObject TroopCharacter { get; private set; }

      public BasicCharacterObject BannerBearerCharacter { get; private set; }

      public BasicCultureObject Culture { get; private set; }

      public MultiplayerClassDivisions.MPHeroClassGroup ClassGroup { get; private set; }

      public string HeroIdleAnim { get; private set; }

      public string HeroMountIdleAnim { get; private set; }

      public string TroopIdleAnim { get; private set; }

      public string TroopMountIdleAnim { get; private set; }

      public int ArmorValue { get; private set; }

      public int Health { get; private set; }

      public float CombatMovementSpeedMultiplier { get; private set; }

      public float MovementSpeedMultiplier { get; private set; }

      public float TopSpeedReachDuration { get; private set; }

      public float TroopMultiplier { get; private set; }

      public int TroopCost { get; private set; }

      public int MeleeAI { get; private set; }

      public int RangedAI { get; private set; }

      public TextObject HeroInformation { get; private set; }

      public TextObject TroopInformation { get; private set; }

      public TargetIconType IconType { get; private set; }

      public TextObject HeroName => this.HeroCharacter.Name;

      public TextObject TroopName => this.TroopCharacter.Name;

      public override bool Equals(object obj) => obj is MultiplayerClassDivisions.MPHeroClass && ((MBObjectBase) obj).StringId.Equals(this.StringId);

      public override int GetHashCode() => base.GetHashCode();

      public List<IReadOnlyPerkObject> GetAllAvailablePerksForListIndex(
        int index)
      {
        string strValue = MultiplayerOptions.OptionType.GameType.GetStrValue();
        List<IReadOnlyPerkObject> readOnlyPerkObjectList = new List<IReadOnlyPerkObject>();
        foreach (IReadOnlyPerkObject perk in this._perks)
        {
          foreach (string gameMode in perk.GameModes)
          {
            if ((gameMode.Equals(strValue, StringComparison.InvariantCultureIgnoreCase) || gameMode.Equals("all", StringComparison.InvariantCultureIgnoreCase)) && perk.PerkListIndex == index)
            {
              readOnlyPerkObjectList.Add(perk);
              break;
            }
          }
        }
        return readOnlyPerkObjectList;
      }

      public override void Deserialize(MBObjectManager objectManager, XmlNode node)
      {
        base.Deserialize(objectManager, node);
        this.HeroCharacter = MultiplayerClassDivisions.GetMPCharacter(node.Attributes["hero"].Value);
        this.TroopCharacter = MultiplayerClassDivisions.GetMPCharacter(node.Attributes["troop"].Value);
        string stringId = node.Attributes["banner_bearer"]?.Value;
        if (stringId != null)
          this.BannerBearerCharacter = MultiplayerClassDivisions.GetMPCharacter(stringId);
        this.HeroIdleAnim = node.Attributes["hero_idle_anim"]?.Value;
        this.HeroMountIdleAnim = node.Attributes["hero_mount_idle_anim"]?.Value;
        this.TroopIdleAnim = node.Attributes["troop_idle_anim"]?.Value;
        this.TroopMountIdleAnim = node.Attributes["troop_mount_idle_anim"]?.Value;
        this.Culture = this.HeroCharacter.Culture;
        this.ClassGroup = new MultiplayerClassDivisions.MPHeroClassGroup(this.HeroCharacter.DefaultFormationClass.GetName());
        this.TroopMultiplier = (float) Convert.ToDouble(node.Attributes["multiplier"].Value);
        this.TroopCost = Convert.ToInt32(node.Attributes["cost"].Value);
        this.ArmorValue = Convert.ToInt32(node.Attributes["armor"].Value);
        this.Health = 100;
        this.MeleeAI = 50;
        this.RangedAI = 50;
        XmlNode attribute = (XmlNode) node.Attributes["hitpoints"];
        if (attribute != null)
          this.Health = Convert.ToInt32(attribute.Value);
        this.MovementSpeedMultiplier = (float) Convert.ToDouble(node.Attributes["movement_speed"].Value);
        this.CombatMovementSpeedMultiplier = (float) Convert.ToDouble(node.Attributes["combat_movement_speed"].Value);
        this.TopSpeedReachDuration = (float) Convert.ToDouble(node.Attributes["acceleration"].Value);
        this.MeleeAI = Convert.ToInt32(node.Attributes["melee_ai"].Value);
        this.RangedAI = Convert.ToInt32(node.Attributes["ranged_ai"].Value);
        TargetIconType result;
        if (Enum.TryParse<TargetIconType>(node.Attributes["icon"].Value, true, out result))
          this.IconType = result;
        foreach (XmlNode childNode1 in node.ChildNodes)
        {
          if (childNode1.NodeType != XmlNodeType.Comment && childNode1.Name == "Perks")
          {
            this._perks = new List<IReadOnlyPerkObject>();
            foreach (XmlNode childNode2 in childNode1.ChildNodes)
            {
              if (childNode2.NodeType != XmlNodeType.Comment)
                this._perks.Add(MPPerkObject.Deserialize(childNode2));
            }
          }
        }
      }
    }

    public class MPHeroClassGroup
    {
      public readonly string StringId;
      public readonly TextObject Name;

      public MPHeroClassGroup(string stringId)
      {
        this.StringId = stringId;
        this.Name = GameTexts.FindText("str_troop_type_name", this.StringId);
      }

      public override bool Equals(object obj) => obj is MultiplayerClassDivisions.MPHeroClassGroup && ((MultiplayerClassDivisions.MPHeroClassGroup) obj).StringId.Equals(this.StringId);

      public override int GetHashCode() => base.GetHashCode();
    }
  }
}
