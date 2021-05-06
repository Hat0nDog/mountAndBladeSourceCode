// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.InputRestrictions
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public class InputRestrictions
  {
    public int Order { get; private set; }

    public Guid Id { get; private set; }

    public bool MouseVisibility { get; private set; }

    public InputUsageMask InputUsageMask { get; private set; }

    public InputRestrictions(int order)
    {
      this.Id = new Guid();
      this.InputUsageMask = InputUsageMask.Invalid;
      this.Order = order;
    }

    public void SetMouseVisibility(bool isVisible)
    {
      this.MouseVisibility = isVisible;
      ScreenManager.UpdateMouseVisibility();
    }

    public void SetInputRestrictions(bool isMouseVisible = true, InputUsageMask mask = InputUsageMask.All)
    {
      this.InputUsageMask = mask;
      this.SetMouseVisibility(isMouseVisible);
    }

    public void ResetInputRestrictions()
    {
      this.InputUsageMask = InputUsageMask.Invalid;
      this.SetMouseVisibility(false);
    }
  }
}
