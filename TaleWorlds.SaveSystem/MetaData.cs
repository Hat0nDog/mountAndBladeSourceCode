// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.MetaData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TaleWorlds.SaveSystem
{
  public class MetaData
  {
    [JsonProperty("List")]
    private Dictionary<string, string> _list = new Dictionary<string, string>();

    public void Add(string key, string value) => this._list.Add(key, value);

    [JsonIgnore]
    public int Count => this._list.Count;

    public bool TryGetValue(string key, out string value) => this._list.TryGetValue(key, out value);

    public string this[string key]
    {
      get => !this._list.ContainsKey(key) ? (string) null : this._list[key];
      set => this._list[key] = value;
    }

    [JsonIgnore]
    public Dictionary<string, string>.KeyCollection Keys => this._list.Keys;

    public void Serialize(Stream stream)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) this));
      stream.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
      stream.Write(bytes, 0, bytes.Length);
    }

    public static MetaData Deserialize(Stream stream)
    {
      try
      {
        byte[] buffer = new byte[4];
        stream.Read(buffer, 0, 4);
        int int32 = BitConverter.ToInt32(buffer, 0);
        byte[] numArray = new byte[int32];
        stream.Read(numArray, 0, int32);
        return JsonConvert.DeserializeObject<MetaData>(Encoding.UTF8.GetString(numArray));
      }
      catch
      {
        return (MetaData) null;
      }
    }
  }
}
