// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattleCombatant
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class CustomBattleCombatant : IBattleCombatant
  {
    private List<BasicCharacterObject> _characters;

    public TextObject Name { get; private set; }

    public BattleSideEnum Side { get; set; }

    public BasicCultureObject BasicCulture { get; private set; }

    public Tuple<uint, uint> PrimaryColorPair => new Tuple<uint, uint>(this.Banner.GetPrimaryColor(), this.Banner.GetPrimaryColor());

    public Tuple<uint, uint> AlternativeColorPair => new Tuple<uint, uint>(this.Banner.GetPrimaryColor(), this.Banner.GetPrimaryColor());

    public Banner Banner { get; private set; }

    public int GetTacticsSkillAmount() => this._characters.Any<BasicCharacterObject>() ? this._characters.Max<BasicCharacterObject>((Func<BasicCharacterObject, int>) (h => h.GetSkillValue(DefaultSkills.Tactics))) : 0;

    public IEnumerable<BasicCharacterObject> Characters => (IEnumerable<BasicCharacterObject>) this._characters.AsReadOnly();

    public int NumberOfAllMembers { get; private set; }

    public int NumberOfHealthyMembers => this._characters.Count;

    public CustomBattleCombatant(TextObject name, BasicCultureObject culture, Banner banner)
    {
      this.Name = name;
      this.BasicCulture = culture;
      this.Banner = banner;
      this._characters = new List<BasicCharacterObject>();
    }

    public void AddCharacter(BasicCharacterObject characterObject, int number)
    {
      for (int index = 0; index < number; ++index)
        this._characters.Add(characterObject);
      this.NumberOfAllMembers += number;
    }

    public void KillCharacter(BasicCharacterObject character) => this._characters.Remove(character);
  }
}
