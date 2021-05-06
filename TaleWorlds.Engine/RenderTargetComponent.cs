// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.RenderTargetComponent
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  public sealed class RenderTargetComponent : DotNetObject
  {
    private readonly WeakNativeObjectReference _renderTargetWeakReference;

    public Texture RenderTarget => (Texture) this._renderTargetWeakReference.GetNativeObject();

    public object UserData { get; internal set; }

    internal RenderTargetComponent(Texture renderTarget) => this._renderTargetWeakReference = new WeakNativeObjectReference((NativeObject) renderTarget);

    [EngineCallback]
    internal static RenderTargetComponent CreateRenderTargetComponent(
      Texture renderTarget)
    {
      return new RenderTargetComponent(renderTarget);
    }

    internal event RenderTargetComponent.TextureUpdateEventHandler PaintNeeded;

    [EngineCallback]
    internal void OnPaintNeeded()
    {
      if (this.PaintNeeded == null)
        return;
      this.PaintNeeded(this.RenderTarget, EventArgs.Empty);
    }

    public delegate void TextureUpdateEventHandler(Texture sender, EventArgs e);
  }
}
