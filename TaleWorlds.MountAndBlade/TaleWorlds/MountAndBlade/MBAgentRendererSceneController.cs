// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBAgentRendererSceneController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class MBAgentRendererSceneController
  {
    private UIntPtr _pointer;

    internal MBAgentRendererSceneController(UIntPtr pointer) => this._pointer = pointer;

    ~MBAgentRendererSceneController()
    {
    }

    public static MBAgentRendererSceneController CreateNewAgentRendererSceneController(
      Scene scene,
      int maxRenderCount)
    {
      return new MBAgentRendererSceneController(MBAPI.IMBAgentVisuals.CreateAgentRendererSceneController(scene.Pointer, maxRenderCount));
    }

    public static void DestructAgentRendererSceneController(
      Scene scene,
      MBAgentRendererSceneController rendererSceneController)
    {
      MBAPI.IMBAgentVisuals.DestructAgentRendererSceneController(scene.Pointer, rendererSceneController._pointer);
      rendererSceneController._pointer = UIntPtr.Zero;
    }
  }
}
