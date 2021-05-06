// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BotData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class BotData
  {
    public int AliveCount;
    public int KillCount;
    public int DeathCount;
    public int AssistCount;

    public int Score => this.KillCount * 3 + this.AssistCount;

    public BotData()
    {
    }

    public BotData(int kill, int assist, int death, int alive)
    {
      this.KillCount = kill;
      this.DeathCount = death;
      this.AssistCount = assist;
      this.AliveCount = alive;
    }

    public bool IsAnyValid => this.KillCount != 0 || this.DeathCount != 0 || this.AssistCount != 0 || (uint) this.AliveCount > 0U;

    public void ResetKillDeathAssist()
    {
      this.KillCount = 0;
      this.DeathCount = 0;
      this.AssistCount = 0;
    }
  }
}
