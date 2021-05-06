// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Options.ManagedOptions.ManagedOptionData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine.Options;

namespace TaleWorlds.MountAndBlade.Options.ManagedOptions
{
  public abstract class ManagedOptionData : IOptionData
  {
    public readonly TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType Type;
    private float _value;

    protected ManagedOptionData(TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType type)
    {
      this.Type = type;
      this._value = TaleWorlds.MountAndBlade.ManagedOptions.GetConfig(type);
    }

    public virtual float GetDefaultValue() => TaleWorlds.MountAndBlade.ManagedOptions.GetDefaultConfig(this.Type);

    public void Commit()
    {
      if ((double) this._value == (double) TaleWorlds.MountAndBlade.ManagedOptions.GetConfig(this.Type))
        return;
      TaleWorlds.MountAndBlade.ManagedOptions.SetConfig(this.Type, this._value);
    }

    public float GetValue(bool forceRefresh)
    {
      if (forceRefresh)
        this._value = TaleWorlds.MountAndBlade.ManagedOptions.GetConfig(this.Type);
      return this._value;
    }

    public void SetValue(float value) => this._value = value;

    public object GetOptionType() => (object) this.Type;

    public bool IsNative() => false;

    public bool IsAction() => false;
  }
}
