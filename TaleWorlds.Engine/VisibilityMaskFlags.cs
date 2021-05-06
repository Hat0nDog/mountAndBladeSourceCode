// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.VisibilityMaskFlags
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;

namespace TaleWorlds.Engine
{
  [Flags]
  public enum VisibilityMaskFlags : uint
  {
    Final = 1,
    ShadowStatic = 16, // 0x00000010
    ShadowDynamic = 32, // 0x00000020
    Contour = 64, // 0x00000040
    EditModeAtmosphere = 268435456, // 0x10000000
    EditModeLight = 536870912, // 0x20000000
    EditModeParticleSystem = 1073741824, // 0x40000000
    EditModeHelpers = 2147483648, // 0x80000000
    EditModeTerrain = 16777216, // 0x01000000
    EditModeGameEntity = 33554432, // 0x02000000
    EditModeFloraEntity = 67108864, // 0x04000000
    EditModeLayerFlora = 134217728, // 0x08000000
    EditModeShadows = 1048576, // 0x00100000
    EditModeBorders = 2097152, // 0x00200000
    EditModeEditingEntity = 4194304, // 0x00400000
    EditModeAnimations = 8388608, // 0x00800000
    EditModeAny = EditModeAnimations | EditModeEditingEntity | EditModeBorders | EditModeShadows | EditModeLayerFlora | EditModeFloraEntity | EditModeGameEntity | EditModeTerrain | EditModeHelpers | EditModeParticleSystem | EditModeLight | EditModeAtmosphere, // 0xFFF00000
    Default = Final, // 0x00000001
    DefaultStatic = Default | ShadowDynamic | ShadowStatic, // 0x00000031
    DefaultDynamic = Default | ShadowDynamic, // 0x00000021
    DefaultStaticWithoutDynamic = Default | ShadowStatic, // 0x00000011
  }
}
