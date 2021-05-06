// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ISoundManager
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface ISoundManager
  {
    [EngineMethod("set_listener_frame", false)]
    void SetListenerFrame(ref MatrixFrame frame);

    [EngineMethod("get_listener_frame", false)]
    void GetListenerFrame(out MatrixFrame result);

    [EngineMethod("reset", false)]
    void Reset();

    [EngineMethod("start_one_shot_event_with_param", false)]
    bool StartOneShotEventWithParam(
      string eventFullName,
      Vec3 position,
      string paramName,
      float paramValue);

    [EngineMethod("start_one_shot_event", false)]
    bool StartOneShotEvent(string eventFullName, Vec3 position);

    [EngineMethod("set_state", false)]
    void SetState(string stateGroup, string state);

    [EngineMethod("load_event_file_aux", false)]
    void LoadEventFileAux(string soundBankName, bool decompressSamples);

    [EngineMethod("set_global_parameter", false)]
    void SetGlobalParameter(string parameterName, float value);

    [EngineMethod("get_global_index_of_event", false)]
    int GetGlobalIndexOfEvent(string eventFullName);
  }
}
