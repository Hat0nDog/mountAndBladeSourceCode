// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.JsonSocketMessage
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TaleWorlds.Network
{
  public class JsonSocketMessage
  {
    public MessageInfo MessageInfo { get; private set; }

    [Obsolete]
    public JsonSocketMessage()
    {
      this.MessageInfo = new MessageInfo();
      foreach (Attribute customAttribute in Attribute.GetCustomAttributes((MemberInfo) this.GetType(), true))
      {
        PostBoxId postBoxId = customAttribute as PostBoxId;
      }
    }

    [JsonProperty]
    public string SocketMessageTypeId => this.GetType().FullName;

    public static string GetTypeId(Type messageType) => messageType.FullName;

    public static Dictionary<string, Type> GetMessageDictionary()
    {
      Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        foreach (KeyValuePair<string, Type> jsonSocketMessage in JsonSocketMessage.RetrieveJSONSocketMessages(assembly))
          dictionary.Add(jsonSocketMessage.Key, jsonSocketMessage.Value);
      }
      return dictionary;
    }

    private static Dictionary<string, Type> RetrieveJSONSocketMessages(
      Assembly assembly)
    {
      Type[] exportedTypes = assembly.GetExportedTypes();
      Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
      foreach (Type type in ((IEnumerable<Type>) exportedTypes).Where<Type>((Func<Type, bool>) (q => q.IsSubclassOf(typeof (JsonSocketMessage)))))
        dictionary.Add(type.FullName, type);
      return dictionary;
    }
  }
}
