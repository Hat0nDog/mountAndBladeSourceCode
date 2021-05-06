// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Screens.GlobalLayer
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Engine.Screens
{
  public class GlobalLayer : IComparable
  {
    public ScreenLayer Layer { get; protected set; }

    internal void EarlyTick(float dt) => this.OnEarlyTick(dt);

    internal void Tick(float dt)
    {
      this.OnTick(dt);
      this.Layer.Tick(dt);
    }

    internal void LateTick(float dt)
    {
      this.OnLateTick(dt);
      this.Layer.LateTick(dt);
    }

    protected virtual void OnEarlyTick(float dt)
    {
    }

    protected virtual void OnTick(float dt)
    {
    }

    protected virtual void OnLateTick(float dt)
    {
    }

    internal void Update(IReadOnlyList<int> lastKeysPressed) => this.Layer.Update(lastKeysPressed);

    public int CompareTo(object obj) => !(obj is GlobalLayer globalLayer) ? -1 : this.Layer.CompareTo((object) globalLayer.Layer);

    public void UpdateLayout() => this.Layer.UpdateLayout();

    public virtual bool OnGamepadNavigation(
      ScreenManager.CursorAreas currentCursorArea,
      ScreenManager.GamepadNavigationTypes type)
    {
      return false;
    }
  }
}
