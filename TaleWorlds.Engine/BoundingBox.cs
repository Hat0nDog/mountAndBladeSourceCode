﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.BoundingBox
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineStruct("rglBounding_box")]
  public struct BoundingBox
  {
    public Vec3 min;
    public Vec3 max;
    public Vec3 center;
    public float radius;
  }
}