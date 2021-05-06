// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.VideoPlaybackState
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class VideoPlaybackState : GameState
  {
    private Action _onVideoFinised;

    public string VideoPath { get; private set; }

    public string AudioPath { get; private set; }

    public float FrameRate { get; private set; }

    public string SubtitleFileBasePath { get; private set; }

    public bool CanUserSkip { get; private set; }

    public void SetStartingParameters(
      string videoPath,
      string audioPath,
      string subtitleFileBasePath,
      float frameRate,
      bool canUserSkip)
    {
      this.VideoPath = videoPath;
      this.AudioPath = audioPath;
      this.FrameRate = frameRate;
      this.SubtitleFileBasePath = subtitleFileBasePath;
      this.CanUserSkip = canUserSkip;
    }

    public void SetOnVideoFinisedDelegate(Action onVideoFinised) => this._onVideoFinised = onVideoFinised;

    public void OnVideoFinished()
    {
      Action onVideoFinised = this._onVideoFinised;
      if (onVideoFinised == null)
        return;
      onVideoFinised();
    }
  }
}
