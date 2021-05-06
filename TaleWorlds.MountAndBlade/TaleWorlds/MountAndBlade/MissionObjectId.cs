// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionObjectId
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public struct MissionObjectId
  {
    public readonly int Id;
    public readonly bool CreatedAtRuntime;

    public MissionObjectId(int id, bool createdAtRuntime = false)
    {
      this.Id = id;
      this.CreatedAtRuntime = createdAtRuntime;
    }

    public static bool operator ==(MissionObjectId a, MissionObjectId b) => a.Id == b.Id && a.CreatedAtRuntime == b.CreatedAtRuntime;

    public static bool operator !=(MissionObjectId a, MissionObjectId b) => a.Id != b.Id || a.CreatedAtRuntime != b.CreatedAtRuntime;

    public override bool Equals(object obj) => obj is MissionObjectId missionObjectId && missionObjectId.Id == this.Id && missionObjectId.CreatedAtRuntime == this.CreatedAtRuntime;

    public override int GetHashCode()
    {
      int id = this.Id;
      if (this.CreatedAtRuntime)
        id |= 1073741824;
      return id.GetHashCode();
    }

    public override string ToString() => this.Id.ToString() + " - " + this.CreatedAtRuntime.ToString();
  }
}
