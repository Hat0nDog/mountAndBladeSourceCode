// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.VictoryComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class VictoryComponent : AgentComponent
  {
    private readonly RandomTimer _timer;

    public VictoryComponent(Agent agent, RandomTimer timer)
      : base(agent)
    {
      this._timer = timer;
    }

    public bool CheckTimer() => this._timer.Check(Mission.Current.Time);

    public void ChangeTimerDuration(float min, float max) => this._timer.ChangeDuration(min, max);
  }
}
