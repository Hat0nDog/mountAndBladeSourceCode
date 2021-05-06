// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ISoundEvent
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface ISoundEvent
  {
    [EngineMethod("create_event_from_string", false)]
    int CreateEventFromString(string eventName, UIntPtr scene);

    [EngineMethod("get_event_id_from_string", false)]
    int GetEventIdFromString(string eventName);

    [EngineMethod("play_sound_2d", false)]
    bool PlaySound2D(int fmodEventIndex);

    [EngineMethod("get_total_event_count", false)]
    int GetTotalEventCount();

    [EngineMethod("set_event_min_max_distance", false)]
    void SetEventMinMaxDistance(int fmodEventIndex, Vec3 radius);

    [EngineMethod("create_event", false)]
    int CreateEvent(int fmodEventIndex, UIntPtr scene);

    [EngineMethod("release_event", false)]
    void ReleaseEvent(int eventId);

    [EngineMethod("set_event_parameter_from_string", false)]
    void SetEventParameterFromString(int eventId, string name, float value);

    [EngineMethod("get_event_min_max_distance", false)]
    Vec3 GetEventMinMaxDistance(int eventId);

    [EngineMethod("set_event_position", false)]
    void SetEventPosition(int eventId, ref Vec3 position);

    [EngineMethod("set_event_velocity", false)]
    void SetEventVelocity(int eventId, ref Vec3 velocity);

    [EngineMethod("start_event", false)]
    bool StartEvent(int eventId);

    [EngineMethod("start_event_in_position", false)]
    bool StartEventInPosition(int eventId, ref Vec3 position);

    [EngineMethod("stop_event", false)]
    void StopEvent(int eventId);

    [EngineMethod("pause_event", false)]
    void PauseEvent(int eventId);

    [EngineMethod("resume_event", false)]
    void ResumeEvent(int eventId);

    [EngineMethod("play_extra_event", false)]
    void PlayExtraEvent(int soundId, string eventName);

    [EngineMethod("set_switch", false)]
    void SetSwitch(int soundId, string switchGroupName, string newSwitchStateName);

    [EngineMethod("trigger_cue", false)]
    void TriggerCue(int eventId);

    [EngineMethod("set_event_parameter_at_index", false)]
    void SetEventParameterAtIndex(int soundId, int parameterIndex, float value);

    [EngineMethod("is_playing", false)]
    bool IsPlaying(int eventId);

    [EngineMethod("is_paused", false)]
    bool IsPaused(int eventId);

    [EngineMethod("is_valid", false)]
    bool IsValid(int eventId);

    [EngineMethod("create_event_from_external_file", false)]
    int CreateEventFromExternalFile(
      string programmerSoundEventName,
      string filePath,
      UIntPtr scene);

    [EngineMethod("create_event_from_sound_buffer", false)]
    int CreateEventFromSoundBuffer(
      string programmerSoundEventName,
      byte[] soundBuffer,
      UIntPtr scene);
  }
}
