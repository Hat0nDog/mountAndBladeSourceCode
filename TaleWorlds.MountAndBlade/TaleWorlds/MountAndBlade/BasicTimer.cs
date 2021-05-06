// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BasicTimer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class BasicTimer
  {
    private float _startTime;
    private readonly MBCommon.TimeType _timeType;

    public float ElapsedTime => MBCommon.GetTime(this._timeType) - this._startTime;

    public BasicTimer(MBCommon.TimeType timeType)
    {
      this._timeType = timeType;
      this._startTime = MBCommon.GetTime(timeType);
    }

    public void Reset() => this._startTime = MBCommon.GetTime(this._timeType);
  }
}
