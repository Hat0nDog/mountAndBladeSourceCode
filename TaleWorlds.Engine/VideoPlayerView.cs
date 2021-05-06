// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.VideoPlayerView
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineClass("rglVideo_player_view")]
  public sealed class VideoPlayerView : View
  {
    internal VideoPlayerView(UIntPtr meshPointer)
      : base(meshPointer)
    {
    }

    public static VideoPlayerView CreateVideoPlayerView() => EngineApplicationInterface.IVideoPlayerView.CreateVideoPlayerView();

    public void PlayVideo(string videoFileName, string soundFileName, float framerate) => EngineApplicationInterface.IVideoPlayerView.PlayVideo(this.Pointer, videoFileName, soundFileName, framerate);

    public void StopVideo() => EngineApplicationInterface.IVideoPlayerView.StopVideo(this.Pointer);

    public bool IsVideoFinished() => EngineApplicationInterface.IVideoPlayerView.IsVideoFinished(this.Pointer);
  }
}
