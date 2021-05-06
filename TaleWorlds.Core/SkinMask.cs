// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.SkinMask
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;

namespace TaleWorlds.Core
{
  [Flags]
  public enum SkinMask
  {
    NoneVisible = 0,
    HeadVisible = 1,
    BodyVisible = 32, // 0x00000020
    UnderwearVisible = 64, // 0x00000040
    HandsVisible = 128, // 0x00000080
    LegsVisible = 256, // 0x00000100
    AllVisible = LegsVisible | HandsVisible | UnderwearVisible | BodyVisible | HeadVisible, // 0x000001E1
  }
}
