// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ScriptComponentFieldHolder
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Runtime.InteropServices;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineStruct("rglScript_component_field_holder")]
  internal struct ScriptComponentFieldHolder
  {
    public double d;
    public float f;
    public int b;
    public int i;
    public Vec3 v3;
    public UIntPtr entityPointer;
    public UIntPtr texturePointer;
    public UIntPtr meshPointer;
    public UIntPtr materialPointer;
    public Vec3 color;
    public MatrixFrame matrixFrame;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string s;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string enumValue;
  }
}
