// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.PathLastNodeFixer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class PathLastNodeFixer : UsableMissionObjectComponent
  {
    public IPathHolder PathHolder;
    private Scene _scene;

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this.Update(this._scene.GetPathWithName(this.PathHolder.PathEntity));
    }

    protected internal override void OnAdded(Scene scene)
    {
      base.OnAdded(scene);
      this._scene = scene;
      this.Update();
    }

    public void Update() => this.Update(this._scene.GetPathWithName(this.PathHolder.PathEntity));

    private void Update(Path path)
    {
      int num = (NativeObject) path != (NativeObject) null ? 1 : 0;
    }
  }
}
