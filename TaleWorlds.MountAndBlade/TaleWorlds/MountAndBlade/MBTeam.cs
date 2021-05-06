// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBTeam
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public struct MBTeam
  {
    public readonly int Index;
    internal readonly Mission Mission;

    internal MBTeam(Mission mission, int index)
    {
      this.Mission = mission;
      this.Index = index;
    }

    public static MBTeam InvalidTeam => new MBTeam((Mission) null, -1);

    public override int GetHashCode() => this.Index;

    public override bool Equals(object obj) => ((MBTeam) obj).Index == this.Index;

    public static bool operator ==(MBTeam team1, MBTeam team2) => team1.Index == team2.Index;

    public static bool operator !=(MBTeam team1, MBTeam team2) => team1.Index != team2.Index;

    public bool IsValid => this.Index >= 0;

    public bool IsEnemyOf(MBTeam otherTeam) => MBAPI.IMBTeam.IsEnemy(this.Mission.Pointer, this.Index, otherTeam.Index);

    public void SetIsEnemyOf(MBTeam otherTeam, bool isEnemyOf) => MBAPI.IMBTeam.SetIsEnemy(this.Mission.Pointer, this.Index, otherTeam.Index, isEnemyOf);

    public override string ToString() => "Mission Team: " + (object) this.Index;
  }
}
