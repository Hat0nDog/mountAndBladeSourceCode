// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.DefaultRidingModel
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public class DefaultRidingModel : RidingModel
  {
    public override float CalculateAcceleration(
      in EquipmentElement mountElement,
      in EquipmentElement harnessElement,
      int ridingSkill)
    {
      float num = (float) mountElement.GetModifiedMountManeuver(in harnessElement) * 0.008f;
      if (ridingSkill >= 0)
        num *= (float) (0.699999988079071 + 3.0 / 1000.0 * ((double) ridingSkill - 1.5 * (double) mountElement.Item.Difficulty));
      return MathF.Clamp(num, 0.15f, 0.7f);
    }
  }
}
