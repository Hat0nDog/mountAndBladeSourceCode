// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBMusicTrack
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public struct MBMusicTrack
  {
    private int index;

    public MBMusicTrack(MBMusicTrack obj) => this.index = obj.index;

    internal MBMusicTrack(int i) => this.index = i;

    private bool IsValid => this.index >= 0;

    public bool Equals(MBMusicTrack obj) => this.index == obj.index;

    public override int GetHashCode() => this.index;
  }
}
