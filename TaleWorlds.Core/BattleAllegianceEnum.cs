// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.BattleAllegianceEnum
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableEnum(20702)]
  public enum BattleAllegianceEnum
  {
    Neutral = -1, // 0xFFFFFFFF
    Ally = 0,
    Enemy = 1,
    NumSides = 2,
  }
}
