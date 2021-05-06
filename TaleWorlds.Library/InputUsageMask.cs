// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.InputUsageMask
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  [Flags]
  public enum InputUsageMask
  {
    Invalid = 0,
    MouseButtons = 1,
    MouseWheels = 2,
    Keyboardkeys = 4,
    BlockEverythingWithoutHitTest = 8,
    Mouse = MouseWheels | MouseButtons, // 0x00000003
    All = Mouse | Keyboardkeys, // 0x00000007
  }
}
