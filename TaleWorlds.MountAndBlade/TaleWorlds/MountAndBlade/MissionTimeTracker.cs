// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionTimeTracker
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  internal class MissionTimeTracker
  {
    private long _lastSyncDifference;

    internal long NumberOfTicks { get; private set; }

    internal long DeltaTimeInTicks { get; private set; }

    internal MissionTimeTracker(MissionTime intialMapTime) => this.NumberOfTicks = intialMapTime.NumberOfTicks;

    internal MissionTimeTracker() => this.NumberOfTicks = 0L;

    internal void Tick(float seconds)
    {
      this.DeltaTimeInTicks = (long) ((double) seconds * 10000000.0);
      this.NumberOfTicks += this.DeltaTimeInTicks;
    }

    internal void UpdateSync(float newValue) => this._lastSyncDifference = (long) ((double) newValue * 10000000.0) - this.NumberOfTicks;

    public float GetLastSyncDifference() => (float) this._lastSyncDifference / 1E+07f;
  }
}
