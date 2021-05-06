// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.RoadStart
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq.Expressions;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class RoadStart : ScriptComponentBehaviour
  {
    public float textureSweepX;
    public float textureSweepY;
    public string materialName = "blood_decal_terrain_material";
    private GameEntity pathEntity;
    private MetaMesh pathMesh;

    protected internal override void OnInit()
    {
      this.pathEntity = GameEntity.CreateEmpty(this.Scene, false);
      this.pathEntity.Name = "Road_Entity";
      this.UpdatePathMesh();
    }

    protected internal override void OnEditorInit() => this.OnInit();

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      if (!((NativeObject) this.pathEntity != (NativeObject) null))
        return;
      this.pathEntity.Remove(removeReason);
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      if (!this.Scene.IsEntityFrameChanged(this.GameEntity.Name))
        return;
      this.UpdatePathMesh();
    }

    protected internal override void OnEditorVariableChanged(string variableName)
    {
      base.OnEditorVariableChanged(variableName);
      if (variableName == MBGlobals.GetMemberName<string>((Expression<Func<string>>) (() => this.materialName)))
        this.UpdatePathMesh();
      if (!((NativeObject) this.pathMesh != (NativeObject) null))
        return;
      this.pathMesh.SetVectorArgument2(this.textureSweepX, this.textureSweepY, 0.0f, 0.0f);
    }

    private void UpdatePathMesh()
    {
      this.pathEntity.ClearComponents();
      this.pathMesh = MetaMesh.CreateMetaMesh();
      Material fromResource = Material.GetFromResource(this.materialName);
      if ((NativeObject) fromResource != (NativeObject) null)
        this.pathMesh.SetMaterial(fromResource);
      else
        this.pathMesh.SetMaterial(Material.GetDefaultMaterial());
      this.pathEntity.AddMultiMesh(this.pathMesh);
      this.pathMesh.SetVectorArgument2(this.textureSweepX, this.textureSweepY, 0.0f, 0.0f);
    }

    protected internal override bool MovesEntity() => false;
  }
}
