// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.ITelemetry
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
  [LibraryInterfaceBase]
  internal interface ITelemetry
  {
    [EngineMethod("get_telemetry_level_mask", false)]
    uint GetTelemetryLevelMask();
  }
}
