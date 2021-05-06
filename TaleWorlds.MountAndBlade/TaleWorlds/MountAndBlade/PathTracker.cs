// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.PathTracker
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class PathTracker
  {
    private readonly Path _path;
    private readonly Vec3 _initialScale;
    private int _version = -1;

    public float TotalDistanceTravelled { get; set; }

    public bool HasChanged => (NativeObject) this._path != (NativeObject) null && this._version < this._path.GetVersion();

    public bool IsValid => (NativeObject) this._path != (NativeObject) null;

    public void UpdateVersion() => this._version = this._path.GetVersion();

    public bool PathExists() => (NativeObject) this._path != (NativeObject) null;

    public PathTracker(Path path, Vec3 initialScaleOfEntity)
    {
      this._path = path;
      this._initialScale = initialScaleOfEntity;
      if ((NativeObject) path != (NativeObject) null)
        this.UpdateVersion();
      this.Reset();
    }

    public void Advance(float deltaDistance)
    {
      this.TotalDistanceTravelled += deltaDistance;
      this.TotalDistanceTravelled = Math.Min(this.TotalDistanceTravelled, this._path.TotalDistance);
    }

    public bool HasReachedEnd => (double) this.TotalDistanceTravelled >= (double) this._path.TotalDistance;

    public float GetPathLength() => this._path.TotalDistance;

    public MatrixFrame CurrentFrame
    {
      get
      {
        MatrixFrame frameForDistance = this._path.GetFrameForDistance(this.TotalDistanceTravelled);
        frameForDistance.rotation.RotateAboutUp(3.141593f);
        frameForDistance.rotation.ApplyScaleLocal(this._initialScale);
        return frameForDistance;
      }
    }

    public void CurrentFrameAndColor(out MatrixFrame frame, out Vec3 color)
    {
      this._path.GetFrameAndColorForDistance(this.TotalDistanceTravelled, out frame, out color);
      frame.rotation.RotateAboutUp(3.141593f);
      frame.rotation.ApplyScaleLocal(this._initialScale);
    }

    public void Reset() => this.TotalDistanceTravelled = 0.0f;
  }
}
