// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CircularSchiltronFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class CircularSchiltronFormation : CircularFormation
  {
    public CircularSchiltronFormation(IFormation owner)
      : base(owner)
    {
    }

    public override IFormationArrangement Clone(IFormation formation) => (IFormationArrangement) new CircularSchiltronFormation(formation);

    public void Form() => this.FormFromDepth(this.GetMaximumDepth(this.GetUnitCountWithOverride()));
  }
}
