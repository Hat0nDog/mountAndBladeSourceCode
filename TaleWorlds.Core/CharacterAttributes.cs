// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.CharacterAttributes
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableClass(20507)]
  public class CharacterAttributes
  {
    private CharacterAttribute[] _allAttributes;
    private CharacterAttribute _control;
    private CharacterAttribute _vigor;
    private CharacterAttribute _endurance;
    private CharacterAttribute _cunning;
    private CharacterAttribute _social;
    private CharacterAttribute _intelligence;
    public const int Count = 6;

    public static CharacterAttribute GetCharacterAttribute(
      CharacterAttributesEnum attributeType)
    {
      switch (attributeType)
      {
        case CharacterAttributesEnum.Vigor:
          return CharacterAttributes.Vigor;
        case CharacterAttributesEnum.Control:
          return CharacterAttributes.Control;
        case CharacterAttributesEnum.Endurance:
          return CharacterAttributes.Endurance;
        case CharacterAttributesEnum.Cunning:
          return CharacterAttributes.Cunning;
        case CharacterAttributesEnum.Social:
          return CharacterAttributes.Social;
        case CharacterAttributesEnum.Intelligence:
          return CharacterAttributes.Intelligence;
        default:
          return CharacterAttributes.Vigor;
      }
    }

    public static CharacterAttribute Vigor => Game.Current.CharacterAttributes._vigor;

    public static CharacterAttribute Control => Game.Current.CharacterAttributes._control;

    public static CharacterAttribute Endurance => Game.Current.CharacterAttributes._endurance;

    public static CharacterAttribute Cunning => Game.Current.CharacterAttributes._cunning;

    public static CharacterAttribute Social => Game.Current.CharacterAttributes._social;

    public static CharacterAttribute Intelligence => Game.Current.CharacterAttributes._intelligence;

    public static IEnumerable<CharacterAttribute> All => (IEnumerable<CharacterAttribute>) Game.Current.CharacterAttributes._allAttributes;

    internal CharacterAttributes() => this.RegisterAll();

    private void RegisterAll()
    {
      this._vigor = MBObjectManager.Instance.RegisterPresumedObject<CharacterAttribute>(new CharacterAttribute("vigor"));
      this._control = MBObjectManager.Instance.RegisterPresumedObject<CharacterAttribute>(new CharacterAttribute("control"));
      this._endurance = MBObjectManager.Instance.RegisterPresumedObject<CharacterAttribute>(new CharacterAttribute("endurance"));
      this._cunning = MBObjectManager.Instance.RegisterPresumedObject<CharacterAttribute>(new CharacterAttribute("cunning"));
      this._social = MBObjectManager.Instance.RegisterPresumedObject<CharacterAttribute>(new CharacterAttribute("social"));
      this._intelligence = MBObjectManager.Instance.RegisterPresumedObject<CharacterAttribute>(new CharacterAttribute("intelligence"));
      this.InitializeAll();
      this._allAttributes = new CharacterAttribute[6]
      {
        this._vigor,
        this._control,
        this._endurance,
        this._cunning,
        this._social,
        this._intelligence
      };
    }

    private void InitializeAll()
    {
      this._vigor.Initialize(new TextObject("{=YWkdD7Ki}Vigor"), new TextObject("{=jJ9sLOLb}Vigor represents the ability to move with speed and force. It's important for melee combat."), new TextObject("{=Ve8xoa3i}VIG"), CharacterAttributesEnum.Vigor);
      this._control.Initialize(new TextObject("{=controlskill}Control"), new TextObject("{=vx0OCvaj}Control represents the ability to use strength without sacrificing precision. It's necessary for using ranged weapons."), new TextObject("{=HuXafdmR}CTR"), CharacterAttributesEnum.Control);
      this._endurance.Initialize(new TextObject("{=kvOavzcs}Endurance"), new TextObject("{=K8rCOQUZ}Endurance is the ability to perform taxing physical activity for a long time."), new TextObject("{=d2ApwXJr}END"), CharacterAttributesEnum.Endurance);
      this._cunning.Initialize(new TextObject("{=JZM1mQvb}Cunning"), new TextObject("{=YO5LUfiO}Cunning is the ability to predict what other people will do, and to outwit their plans."), new TextObject("{=tH6Ooj0P}CNG"), CharacterAttributesEnum.Cunning);
      this._social.Initialize(new TextObject("{=socialskill}Social"), new TextObject("{=XMDTt96y}Social is the ability to understand people's motivations and to sway them."), new TextObject("{=PHoxdReD}SOC"), CharacterAttributesEnum.Social);
      this._intelligence.Initialize(new TextObject("{=sOrJoxiC}Intelligence"), new TextObject("{=TeUtEGV0}Intelligence represents aptitude for reading and theoretical learning."), new TextObject("{=Bn7IsMpu}INT"), CharacterAttributesEnum.Intelligence);
    }

    internal static void AutoGeneratedStaticCollectObjectsCharacterAttributes(
      object o,
      List<object> collectedObjects)
    {
      ((CharacterAttributes) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
    {
    }
  }
}
