// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.SoundEventParameter
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System.Runtime.InteropServices;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineStruct("Managed_sound_event_parameter")]
  public struct SoundEventParameter
  {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    internal string ParamName;
    internal float Value;

    public SoundEventParameter(string paramName, float value)
    {
      this.ParamName = paramName;
      this.Value = value;
    }
  }
}
