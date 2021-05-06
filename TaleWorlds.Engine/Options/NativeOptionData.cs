// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Options.NativeOptionData
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

namespace TaleWorlds.Engine.Options
{
  public abstract class NativeOptionData : IOptionData
  {
    public readonly NativeOptions.NativeOptionsType Type;
    private float _value;

    protected NativeOptionData(NativeOptions.NativeOptionsType type)
    {
      this.Type = type;
      this._value = NativeOptions.GetConfig(type);
    }

    public virtual float GetDefaultValue() => NativeOptions.GetDefaultConfig(this.Type);

    public void Commit() => NativeOptions.SetConfig(this.Type, this._value);

    public float GetValue(bool forceRefresh)
    {
      if (forceRefresh)
        this._value = NativeOptions.GetConfig(this.Type);
      return this._value;
    }

    public void SetValue(float value) => this._value = value;

    public object GetOptionType() => (object) this.Type;

    public bool IsNative() => true;

    public bool IsAction() => false;
  }
}
