// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.WeaponUsageDataManager
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;

namespace TaleWorlds.Core
{
  public class WeaponUsageDataManager
  {
    private static WeaponUsageDataManager _instance;
    private readonly WeaponUsageData _oneHandedSwordUsageData;
    private readonly WeaponUsageData _oneHandedBastardSwordUsageData;
    private readonly WeaponUsageData _twoHandedSwordUsageData;
    private readonly WeaponUsageData _daggerUsageData;
    private readonly WeaponUsageData _throwingKnifeUsageData;
    private readonly WeaponUsageData _oneHandedAxeUsageData;
    private readonly WeaponUsageData _oneHandedBastardAxeUsageData;
    private readonly WeaponUsageData _twoHandedAxeUsageData;
    private readonly WeaponUsageData _throwingAxeUsageData;
    private readonly WeaponUsageData _oneHandedPolearmUsageData;
    private readonly WeaponUsageData _oneHandedPolearmJavelinAlternativeUsageData;
    private readonly WeaponUsageData _twoHandedPolearmUsageData;
    private readonly WeaponUsageData _twoHandedPolearmCouchableUsageData;
    private readonly WeaponUsageData _twoHandedPolearmPikeUsageData;
    private readonly WeaponUsageData _twoHandedPolearmBracingUsageData;
    private readonly WeaponUsageData _javelinUsageData;
    private readonly WeaponUsageData _maceUsageData;
    private readonly WeaponUsageData _twoHandedMaceUsageData;

    public static WeaponUsageDataManager Instance => WeaponUsageDataManager._instance ?? (WeaponUsageDataManager._instance = new WeaponUsageDataManager());

    private WeaponUsageDataManager()
    {
      this._oneHandedSwordUsageData = new WeaponUsageData("OneHandedSword", WeaponClass.OneHandedSword, WeaponFlags.MeleeWeapon, "onehanded:block:shield:swing:thrust");
      this._oneHandedBastardSwordUsageData = new WeaponUsageData("OneHandedBastardSword", WeaponClass.OneHandedSword, WeaponFlags.MeleeWeapon, "onehanded:block:rshield:swing:thrust");
      this._twoHandedSwordUsageData = new WeaponUsageData("TwoHandedSword", WeaponClass.TwoHandedSword, WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand, "twohanded:block:swing:thrust");
      this._daggerUsageData = new WeaponUsageData("Dagger", WeaponClass.Dagger, WeaponFlags.MeleeWeapon, "onehanded:block:shield:swing:thrust");
      this._throwingKnifeUsageData = new WeaponUsageData("ThrowingKnife", WeaponClass.ThrowingKnife, WeaponFlags.RangedWeapon | WeaponFlags.Consumable | WeaponFlags.UnloadWhenSheathed | WeaponFlags.AutoReload | WeaponFlags.UseHandAsThrowBase | WeaponFlags.AmmoSticksWhenShot, "throwing:knife");
      this._oneHandedAxeUsageData = new WeaponUsageData("OneHandedAxe", WeaponClass.OneHandedAxe, WeaponFlags.MeleeWeapon | WeaponFlags.BonusAgainstShield, "onehanded:axe");
      this._oneHandedBastardAxeUsageData = new WeaponUsageData("OneHandedBastardAxe", WeaponClass.OneHandedAxe, WeaponFlags.MeleeWeapon | WeaponFlags.BonusAgainstShield, "onehanded:rshield:axe");
      this._twoHandedAxeUsageData = new WeaponUsageData("TwoHandedAxe", WeaponClass.TwoHandedAxe, WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.BonusAgainstShield | WeaponFlags.TwoHandIdleOnMount, "twohanded:widegrip:axe");
      this._throwingAxeUsageData = new WeaponUsageData("ThrowingAxe", WeaponClass.ThrowingAxe, WeaponFlags.RangedWeapon | WeaponFlags.Consumable | WeaponFlags.UnloadWhenSheathed | WeaponFlags.AutoReload | WeaponFlags.UseHandAsThrowBase | WeaponFlags.AmmoSticksWhenShot, "throwing:axe");
      this._oneHandedPolearmUsageData = new WeaponUsageData("OneHandedPolearm", WeaponClass.OneHandedPolearm, WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip, "onehanded_polearm:block:long:rshield:thrust");
      this._oneHandedPolearmJavelinAlternativeUsageData = new WeaponUsageData("OneHandedPolearm_JavelinAlternative", WeaponClass.OneHandedPolearm, WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip, "onehanded_polearm:block:shield:thrust");
      this._twoHandedPolearmUsageData = new WeaponUsageData("TwoHandedPolearm", WeaponClass.TwoHandedPolearm, WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.WideGrip | WeaponFlags.TwoHandIdleOnMount, "polearm:block:long:shield:swing:thrust");
      this._twoHandedPolearmCouchableUsageData = new WeaponUsageData("TwoHandedPolearm_Couchable", WeaponClass.TwoHandedPolearm, WeaponFlags.MeleeWeapon | WeaponFlags.WideGrip, "polearm:couch");
      this._twoHandedPolearmPikeUsageData = new WeaponUsageData("TwoHandedPolearm_Pike", WeaponClass.TwoHandedPolearm, WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.WideGrip | WeaponFlags.TwoHandIdleOnMount, "polearm:pike");
      this._twoHandedPolearmBracingUsageData = new WeaponUsageData("TwoHandedPolearm_Bracing", WeaponClass.TwoHandedPolearm, WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.WideGrip | WeaponFlags.TwoHandIdleOnMount, "polearm:bracing");
      this._javelinUsageData = new WeaponUsageData("Javelin", WeaponClass.Javelin, WeaponFlags.RangedWeapon | WeaponFlags.Consumable | WeaponFlags.UnloadWhenSheathed | WeaponFlags.AutoReload | WeaponFlags.UseHandAsThrowBase | WeaponFlags.AmmoSticksWhenShot, "throwing:javelin", true, true);
      this._maceUsageData = new WeaponUsageData("Mace", WeaponClass.Mace, WeaponFlags.MeleeWeapon, "onehanded:block:shield:tipdraw:swing:thrust");
      this._twoHandedMaceUsageData = new WeaponUsageData("TwoHandedMace", WeaponClass.TwoHandedMace, WeaponFlags.MeleeWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.TwoHandIdleOnMount, "twohanded:axe");
    }

    public static IEnumerable<WeaponUsageData> All
    {
      get
      {
        yield return WeaponUsageDataManager.OneHandedSwordUsageData;
        yield return WeaponUsageDataManager.OneHandedBastardSwordUsageData;
        yield return WeaponUsageDataManager.TwoHandedSwordUsageData;
        yield return WeaponUsageDataManager.DaggerUsageData;
        yield return WeaponUsageDataManager.ThrowingKnifeUsageData;
        yield return WeaponUsageDataManager.OneHandedAxeUsageData;
        yield return WeaponUsageDataManager.OneHandedBastardAxeUsageData;
        yield return WeaponUsageDataManager.TwoHandedAxeUsageData;
        yield return WeaponUsageDataManager.ThrowingAxeUsageData;
        yield return WeaponUsageDataManager.OneHandedPolearmUsageData;
        yield return WeaponUsageDataManager.OneHandedPolearmJavelinAlternativeUsageData;
        yield return WeaponUsageDataManager.TwoHandedPolearmUsageData;
        yield return WeaponUsageDataManager.TwoHandedPolearmCouchableUsageData;
        yield return WeaponUsageDataManager.TwoHandedPolearmPikeUsageData;
        yield return WeaponUsageDataManager.TwoHandedPolearmBracingUsageData;
        yield return WeaponUsageDataManager.JavelinUsageData;
        yield return WeaponUsageDataManager.MaceUsageData;
        yield return WeaponUsageDataManager.TwoHandedMaceUsageData;
      }
    }

    public static WeaponUsageData OneHandedSwordUsageData => WeaponUsageDataManager.Instance._oneHandedSwordUsageData;

    public static WeaponUsageData OneHandedBastardSwordUsageData => WeaponUsageDataManager.Instance._oneHandedBastardSwordUsageData;

    public static WeaponUsageData TwoHandedSwordUsageData => WeaponUsageDataManager.Instance._twoHandedSwordUsageData;

    public static WeaponUsageData DaggerUsageData => WeaponUsageDataManager.Instance._daggerUsageData;

    public static WeaponUsageData ThrowingKnifeUsageData => WeaponUsageDataManager.Instance._throwingKnifeUsageData;

    public static WeaponUsageData OneHandedAxeUsageData => WeaponUsageDataManager.Instance._oneHandedAxeUsageData;

    public static WeaponUsageData OneHandedBastardAxeUsageData => WeaponUsageDataManager.Instance._oneHandedBastardAxeUsageData;

    public static WeaponUsageData TwoHandedAxeUsageData => WeaponUsageDataManager.Instance._twoHandedAxeUsageData;

    public static WeaponUsageData ThrowingAxeUsageData => WeaponUsageDataManager.Instance._throwingAxeUsageData;

    public static WeaponUsageData OneHandedPolearmUsageData => WeaponUsageDataManager.Instance._oneHandedPolearmUsageData;

    public static WeaponUsageData OneHandedPolearmJavelinAlternativeUsageData => WeaponUsageDataManager.Instance._oneHandedPolearmJavelinAlternativeUsageData;

    public static WeaponUsageData TwoHandedPolearmUsageData => WeaponUsageDataManager.Instance._twoHandedPolearmUsageData;

    public static WeaponUsageData TwoHandedPolearmCouchableUsageData => WeaponUsageDataManager.Instance._twoHandedPolearmCouchableUsageData;

    public static WeaponUsageData TwoHandedPolearmPikeUsageData => WeaponUsageDataManager.Instance._twoHandedPolearmPikeUsageData;

    public static WeaponUsageData TwoHandedPolearmBracingUsageData => WeaponUsageDataManager.Instance._twoHandedPolearmBracingUsageData;

    public static WeaponUsageData JavelinUsageData => WeaponUsageDataManager.Instance._javelinUsageData;

    public static WeaponUsageData MaceUsageData => WeaponUsageDataManager.Instance._maceUsageData;

    public static WeaponUsageData TwoHandedMaceUsageData => WeaponUsageDataManager.Instance._twoHandedMaceUsageData;
  }
}
