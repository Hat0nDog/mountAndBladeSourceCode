﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.CharacterAttribute
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableClass(20506)]
  public class CharacterAttribute : PropertyObject
  {
    private SkillObject[] _skills;

    public CharacterAttributesEnum AttributeEnum { get; private set; }

    public IReadOnlyList<SkillObject> Skills => (IReadOnlyList<SkillObject>) this._skills;

    public TextObject Abbreviation { get; private set; }

    public CharacterAttribute(string stringId)
      : base(stringId)
    {
    }

    internal void AddSkill(SkillObject skillObject)
    {
      List<SkillObject> list = ((IEnumerable<SkillObject>) this._skills).ToList<SkillObject>();
      list.Add(skillObject);
      this._skills = list.ToArray();
    }

    public void Initialize(
      TextObject name,
      TextObject description,
      TextObject abbreviation,
      CharacterAttributesEnum attributeEnum)
    {
      this.Initialize(name, description);
      this.AttributeEnum = attributeEnum;
      this.Abbreviation = abbreviation;
      this._skills = new SkillObject[0];
      this.AfterInitialized();
    }

    internal static void AutoGeneratedStaticCollectObjectsCharacterAttribute(
      object o,
      List<object> collectedObjects)
    {
      ((MBObjectBase) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected override void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects) => base.AutoGeneratedInstanceCollectObjects(collectedObjects);
  }
}
