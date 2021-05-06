// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorValues
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("behavior_values_struct")]
  public struct BehaviorValues
  {
    internal float y1;
    internal float x2;
    internal float y2;
    internal float x3;
    internal float y3;

    internal float GetValueAt(float x)
    {
      if ((double) x <= (double) this.x2)
        return (this.y2 - this.y1) * x / this.x2 + this.y1;
      return (double) x <= (double) this.x3 ? (float) (((double) this.y3 - (double) this.y2) * ((double) x - (double) this.x2) / ((double) this.x3 - (double) this.x2)) + this.y2 : this.y3;
    }
  }
}
