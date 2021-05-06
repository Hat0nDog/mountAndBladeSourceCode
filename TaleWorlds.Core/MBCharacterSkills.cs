// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MBCharacterSkills
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Xml;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
  public class MBCharacterSkills : MBObjectBase
  {
    public CharacterSkills Skills { get; private set; }

    public MBCharacterSkills() => this.Skills = new CharacterSkills();

    public void Init(MBObjectManager objectManager, XmlNode node) => this.Skills.Deserialize(objectManager, node);

    public override void Deserialize(MBObjectManager objectManager, XmlNode node)
    {
      base.Deserialize(objectManager, node);
      this.Skills.Deserialize(objectManager, node);
    }
  }
}
