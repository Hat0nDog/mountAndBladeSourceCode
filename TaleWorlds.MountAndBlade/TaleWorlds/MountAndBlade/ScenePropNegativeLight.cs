// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ScenePropNegativeLight
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class ScenePropNegativeLight : ScriptComponentBehaviour
  {
    public float Flatness_X;
    public float Flatness_Y;
    public float Flatness_Z;
    public float Alpha = 1f;
    public bool Is_Dark_Light = true;

    protected internal override void OnEditorTick(float dt) => this.SetMeshParameters();

    private void SetMeshParameters()
    {
      MetaMesh metaMesh = this.GameEntity.GetMetaMesh(0);
      if (!((NativeObject) metaMesh != (NativeObject) null))
        return;
      metaMesh.SetVectorArgument(this.Flatness_X, this.Flatness_Y, this.Flatness_Z, this.Alpha);
      if (this.Is_Dark_Light)
        metaMesh.SetVectorArgument2(1f, 0.0f, 0.0f, 0.0f);
      else
        metaMesh.SetVectorArgument2(0.0f, 0.0f, 0.0f, 0.0f);
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this.SetMeshParameters();
    }

    protected internal override bool IsOnlyVisual() => true;
  }
}
