// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ActionIndexCache
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  public class ActionIndexCache : IEquatable<ActionIndexCache>
  {
    private const int UnresolvedActionIndex = -2;
    private int _actionIndex = -2;
    private string _name;

    public static ActionIndexCache act_none { get; private set; } = new ActionIndexCache(-1, nameof (act_none));

    public static ActionIndexCache Create(string actName) => string.IsNullOrWhiteSpace(actName) ? ActionIndexCache.act_none : new ActionIndexCache(actName);

    private ActionIndexCache(string act_name) => this._name = act_name;

    private ActionIndexCache(int actionIndex, string actName)
    {
      this._actionIndex = actionIndex;
      this._name = actName;
    }

    internal ActionIndexCache(int actionIndex) => this._actionIndex = actionIndex;

    public int Index
    {
      get
      {
        if (this._actionIndex == -2)
          this.Resolve();
        return this._actionIndex;
      }
    }

    public string Name
    {
      get
      {
        if (this._name == null)
          this.ResolveName();
        return this._name;
      }
    }

    private void Resolve() => this._actionIndex = MBAnimation.GetActionCodeWithName(this._name);

    private void ResolveName() => this._name = MBAnimation.GetActionNameWithCode(this._actionIndex);

    public override bool Equals(object obj) => this.Equals(obj as ActionIndexCache);

    public bool Equals(ActionIndexCache other)
    {
      if ((object) other == null)
        return false;
      return this._actionIndex == ActionIndexCache.act_none.Index || other._actionIndex == ActionIndexCache.act_none.Index ? this._actionIndex == other._actionIndex : this.Index == other.Index;
    }

    public static bool operator ==(ActionIndexCache action0, ActionIndexCache action1)
    {
      if ((object) action0 == (object) action1)
        return true;
      return (object) action0 != null && action0.Equals(action1);
    }

    public static bool operator !=(ActionIndexCache action0, ActionIndexCache action1) => !(action0 == action1);

    public override int GetHashCode() => this.Index.GetHashCode();
  }
}
