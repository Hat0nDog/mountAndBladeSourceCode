// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.TextureView
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineClass("rglTexture_view")]
  public sealed class TextureView : View
  {
    internal TextureView(UIntPtr meshPointer)
      : base(meshPointer)
    {
    }

    public static TextureView CreateTextureView() => EngineApplicationInterface.ITextureView.CreateTextureView();

    public void SetTexture(Texture texture) => EngineApplicationInterface.ITextureView.SetTexture(this.Pointer, texture.Pointer);
  }
}
