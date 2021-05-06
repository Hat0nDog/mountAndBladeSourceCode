// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBParticleSystem
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public struct MBParticleSystem
  {
    private int index;

    internal MBParticleSystem(int i) => this.index = i;

    public bool Equals(MBParticleSystem a) => this.index == a.index;

    public override int GetHashCode() => this.index;
  }
}
