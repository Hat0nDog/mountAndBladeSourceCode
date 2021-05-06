// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LightCycle
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class LightCycle : ScriptComponentBehaviour
  {
    public bool alwaysBurn;

    private void SetVisibility()
    {
      Light light = this.GameEntity.GetLight();
      float timeOfDay = this.Scene.TimeOfDay;
      bool visible = (double) timeOfDay < 6.0 || (double) timeOfDay > 20.0 || this.Scene.IsAtmosphereIndoor || this.alwaysBurn;
      if ((NativeObject) light != (NativeObject) null)
        light.SetVisibility(visible);
      foreach (GameEntity child in this.GameEntity.GetChildren())
        child.SetVisibilityExcludeParents(visible);
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this.SetVisibility();
    }

    protected internal override void OnEditorTick(float dt) => this.SetVisibility();

    protected internal override bool MovesEntity() => false;
  }
}
