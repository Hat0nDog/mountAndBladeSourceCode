// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.SoundManager
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public static class SoundManager
  {
    private static bool _loaded;

    public static void SetListenerFrame(MatrixFrame frame) => EngineApplicationInterface.ISoundManager.SetListenerFrame(ref frame);

    public static MatrixFrame GetListenerFrame()
    {
      MatrixFrame result;
      EngineApplicationInterface.ISoundManager.GetListenerFrame(out result);
      return result;
    }

    public static void Reset() => EngineApplicationInterface.ISoundManager.Reset();

    public static bool StartOneShotEvent(
      string eventFullName,
      in Vec3 position,
      string paramName,
      float paramValue)
    {
      return EngineApplicationInterface.ISoundManager.StartOneShotEventWithParam(eventFullName, position, paramName, paramValue);
    }

    public static bool StartOneShotEvent(string eventFullName, in Vec3 position) => EngineApplicationInterface.ISoundManager.StartOneShotEvent(eventFullName, position);

    public static void SetState(string stateGroup, string state) => EngineApplicationInterface.ISoundManager.SetState(stateGroup, state);

    public static SoundEvent CreateEvent(string eventFullName, Scene scene) => SoundEvent.CreateEventFromString(eventFullName, scene);

    public static void LoadEventFileAux(string soundBank, bool decompressSamples)
    {
      if (SoundManager._loaded)
        return;
      EngineApplicationInterface.ISoundManager.LoadEventFileAux(soundBank, decompressSamples);
      SoundManager._loaded = true;
    }

    public static void SetGlobalParameter(string parameterName, float value) => EngineApplicationInterface.ISoundManager.SetGlobalParameter(parameterName, value);

    public static int GetEventGlobalIndex(string eventFullName) => string.IsNullOrEmpty(eventFullName) ? -1 : EngineApplicationInterface.ISoundManager.GetGlobalIndexOfEvent(eventFullName);
  }
}
