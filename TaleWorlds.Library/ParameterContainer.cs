// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ParameterContainer
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace TaleWorlds.Library
{
  public class ParameterContainer
  {
    private Dictionary<string, string> _parameters;

    public ParameterContainer() => this._parameters = new Dictionary<string, string>();

    public void AddParameter(string key, string value, bool overwriteIfExists)
    {
      if (this._parameters.ContainsKey(key))
      {
        if (!overwriteIfExists)
          return;
        this._parameters[key] = value;
      }
      else
        this._parameters.Add(key, value);
    }

    public void AddParameterConcurrent(string key, string value, bool overwriteIfExists)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IDictionary<string, string>) this._parameters);
      if (dictionary.ContainsKey(key))
      {
        if (overwriteIfExists)
          dictionary[key] = value;
      }
      else
        dictionary.Add(key, value);
      this._parameters = dictionary;
    }

    public void AddParametersConcurrent(
      IEnumerable<KeyValuePair<string, string>> parameters,
      bool overwriteIfExists)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IDictionary<string, string>) this._parameters);
      foreach (KeyValuePair<string, string> parameter in parameters)
      {
        if (dictionary.ContainsKey(parameter.Key))
        {
          if (overwriteIfExists)
            dictionary[parameter.Key] = parameter.Value;
        }
        else
          dictionary.Add(parameter.Key, parameter.Value);
      }
      this._parameters = dictionary;
    }

    public void ClearParameters() => this._parameters = new Dictionary<string, string>();

    public bool TryGetParameter(string key, out string outValue) => this._parameters.TryGetValue(key, out outValue);

    public bool TryGetParameterAsBool(string key, out bool outValue)
    {
      outValue = false;
      string outValue1;
      if (!this.TryGetParameter(key, out outValue1))
        return false;
      outValue = outValue1 == "true" || outValue1 == "True";
      return true;
    }

    public bool TryGetParameterAsInt(string key, out int outValue)
    {
      outValue = 0;
      string outValue1;
      if (!this.TryGetParameter(key, out outValue1))
        return false;
      outValue = Convert.ToInt32(outValue1);
      return true;
    }

    public bool TryGetParameterAsUInt16(string key, out ushort outValue)
    {
      outValue = (ushort) 0;
      string outValue1;
      if (!this.TryGetParameter(key, out outValue1))
        return false;
      outValue = Convert.ToUInt16(outValue1);
      return true;
    }

    public bool TryGetParameterAsFloat(string key, out float outValue)
    {
      outValue = 0.0f;
      string outValue1;
      if (!this.TryGetParameter(key, out outValue1))
        return false;
      outValue = Convert.ToSingle(outValue1, (IFormatProvider) CultureInfo.InvariantCulture);
      return true;
    }

    public bool TryGetParameterAsByte(string key, out byte outValue)
    {
      outValue = (byte) 0;
      string outValue1;
      if (!this.TryGetParameter(key, out outValue1))
        return false;
      outValue = Convert.ToByte(outValue1);
      return true;
    }

    public bool TryGetParameterAsSByte(string key, out sbyte outValue)
    {
      outValue = (sbyte) 0;
      string outValue1;
      if (!this.TryGetParameter(key, out outValue1))
        return false;
      outValue = Convert.ToSByte(outValue1);
      return true;
    }

    public bool TryGetParameterAsVec3(string key, out Vec3 outValue)
    {
      outValue = new Vec3();
      string outValue1;
      if (!this.TryGetParameter(key, out outValue1))
        return false;
      string[] strArray = outValue1.Split(';');
      float single1 = Convert.ToSingle(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture);
      float single2 = Convert.ToSingle(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
      float single3 = Convert.ToSingle(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture);
      outValue = new Vec3(single1, single2, single3);
      return true;
    }

    public bool TryGetParameterAsVec2(string key, out Vec2 outValue)
    {
      outValue = new Vec2();
      string outValue1;
      if (!this.TryGetParameter(key, out outValue1))
        return false;
      string[] strArray = outValue1.Split(';');
      float single1 = Convert.ToSingle(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture);
      float single2 = Convert.ToSingle(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
      outValue = new Vec2(single1, single2);
      return true;
    }

    public string GetParameter(string key) => this._parameters[key];

    public IEnumerable<KeyValuePair<string, string>> Iterator => (IEnumerable<KeyValuePair<string, string>>) this._parameters;

    public ParameterContainer Clone()
    {
      ParameterContainer parameterContainer = new ParameterContainer();
      foreach (KeyValuePair<string, string> parameter in this._parameters)
        parameterContainer._parameters.Add(parameter.Key, parameter.Value);
      return parameterContainer;
    }
  }
}
