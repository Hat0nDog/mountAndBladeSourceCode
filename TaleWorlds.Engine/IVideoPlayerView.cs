// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IVideoPlayerView
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IVideoPlayerView
  {
    [EngineMethod("create_video_player_view", false)]
    VideoPlayerView CreateVideoPlayerView();

    [EngineMethod("play_video", false)]
    void PlayVideo(UIntPtr pointer, string videoFileName, string soundFileName, float framerate);

    [EngineMethod("stop_video", false)]
    void StopVideo(UIntPtr pointer);

    [EngineMethod("is_video_finished", false)]
    bool IsVideoFinished(UIntPtr pointer);
  }
}
