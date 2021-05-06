// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.InitialStateOption
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class InitialStateOption
  {
    private Action _action;

    public int OrderIndex { get; private set; }

    public TextObject Name { get; private set; }

    public string Id { get; private set; }

    public Func<bool> IsDisabled { get; private set; }

    public TextObject DisabledReason { get; private set; }

    public InitialStateOption(
      string id,
      TextObject name,
      int orderIndex,
      Action action,
      Func<bool> isDisabled,
      TextObject disabledReason = null)
    {
      this.Name = name;
      this.Id = id;
      this.OrderIndex = orderIndex;
      this._action = action;
      this.IsDisabled = isDisabled;
      this.DisabledReason = disabledReason == null ? TextObject.Empty : disabledReason;
      int num1 = this.IsDisabled() ? 1 : 0;
      int num2 = this.DisabledReason.ToString() == string.Empty ? 1 : 0;
    }

    public void DoAction()
    {
      if (this._action == null)
        return;
      this._action();
    }
  }
}
