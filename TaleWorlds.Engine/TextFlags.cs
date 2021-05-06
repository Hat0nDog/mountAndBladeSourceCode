// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.TextFlags
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;

namespace TaleWorlds.Engine
{
  [Flags]
  public enum TextFlags
  {
    RglTfHAlignLeft = 1,
    RglTfHAlignRight = 2,
    RglTfHAlignCenter = RglTfHAlignRight | RglTfHAlignLeft, // 0x00000003
    RglTfVAlignTop = 4,
    RglTfVAlignDown = 8,
    RglTfVAlignCenter = RglTfVAlignDown | RglTfVAlignTop, // 0x0000000C
    RglTfSingleLine = 16, // 0x00000010
    RglTfMultiline = 32, // 0x00000020
    RglTfItalic = 64, // 0x00000040
    RglTfCutTextFromLeft = 128, // 0x00000080
    RglTfDoubleSpace = 256, // 0x00000100
    RglTfWithOutline = 512, // 0x00000200
    RglTfHalfSpace = 1024, // 0x00000400
  }
}
