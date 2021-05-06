// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ScenePropPositiveLight
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class ScenePropPositiveLight : ScriptComponentBehaviour
  {
    public float Flatness_X;
    public float Flatness_Y;
    public float Flatness_Z;
    public float DirectLightRed = 1f;
    public float DirectLightGreen = 1f;
    public float DirectLightBlue = 1f;
    public float DirectLightIntensity = 1f;
    public float AmbientLightRed;
    public float AmbientLightGreen;
    public float AmbientLightBlue = 1f;
    public float AmbientLightIntensity = 1f;

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this.SetMeshParams();
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this.SetMeshParams();
    }

    private void SetMeshParams()
    {
      MetaMesh metaMesh = this.GameEntity.GetMetaMesh(0);
      if (!((NativeObject) metaMesh != (NativeObject) null))
        return;
      uint factor1 = this.CalculateFactor(new Vec3(this.DirectLightRed, this.DirectLightGreen, this.DirectLightBlue), this.DirectLightIntensity);
      metaMesh.SetFactor1Linear(factor1);
      uint factor2 = this.CalculateFactor(new Vec3(this.AmbientLightRed, this.AmbientLightGreen, this.AmbientLightBlue), this.AmbientLightIntensity);
      metaMesh.SetFactor2Linear(factor2);
      metaMesh.SetVectorArgument(this.Flatness_X, this.Flatness_Y, this.Flatness_Z, 1f);
    }

    private uint CalculateFactor(Vec3 color, float alpha)
    {
      float val2 = 10f;
      byte maxValue = byte.MaxValue;
      alpha = Math.Min(Math.Max(0.0f, alpha), val2);
      return (uint) (((int) (uint) ((double) alpha / (double) val2 * (double) maxValue) << 24) + ((int) (uint) ((double) color.x * (double) maxValue) << 16) + ((int) (uint) ((double) color.y * (double) maxValue) << 8)) + (uint) ((double) color.z * (double) maxValue);
    }

    protected internal override bool IsOnlyVisual() => true;
  }
}
