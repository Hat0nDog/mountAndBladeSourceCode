// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Music
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

namespace TaleWorlds.Engine
{
  public class Music
  {
    public static int GetFreeMusicChannelIndex() => EngineApplicationInterface.IMusic.GetFreeMusicChannelIndex();

    public static void LoadClip(int index, string pathToClip) => EngineApplicationInterface.IMusic.LoadClip(index, pathToClip);

    public static void UnloadClip(int index) => EngineApplicationInterface.IMusic.UnloadClip(index);

    public static bool IsClipLoaded(int index) => EngineApplicationInterface.IMusic.IsClipLoaded(index);

    public static void PlayMusic(int index) => EngineApplicationInterface.IMusic.PlayMusic(index);

    public static void PlayDelayed(int index, int deltaMilliseconds) => EngineApplicationInterface.IMusic.PlayDelayed(index, deltaMilliseconds);

    public static bool IsMusicPlaying(int index) => EngineApplicationInterface.IMusic.IsMusicPlaying(index);

    public static void PauseMusic(int index) => EngineApplicationInterface.IMusic.PauseMusic(index);

    public static void StopMusic(int index) => EngineApplicationInterface.IMusic.StopMusic(index);

    public static void SetVolume(int index, float volume) => EngineApplicationInterface.IMusic.SetVolume(index, volume);
  }
}
