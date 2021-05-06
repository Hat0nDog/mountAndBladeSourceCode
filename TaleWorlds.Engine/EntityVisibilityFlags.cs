// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.EntityVisibilityFlags
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;

namespace TaleWorlds.Engine
{
  [Flags]
  public enum EntityVisibilityFlags
  {
    None = 0,
    VisibleOnlyWhenEditing = 2,
    NoShadow = 4,
    VisibleOnlyForEnvmap = 8,
    NotVisibleForEnvmap = 16, // 0x00000010
  }
}
