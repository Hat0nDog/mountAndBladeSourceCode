﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Localization.TextObject
// Assembly: TaleWorlds.Localization, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 26BB3E5A-EB48-4ABD-B2FC-10EF6D7A7285
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Localization.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace TaleWorlds.Localization
{
  [SaveableClass(30000)]
  [Serializable]
  public class TextObject
  {
    [SaveableField(1)]
    internal string Value;
    public static TextObject Empty = new TextObject();

    internal static void AutoGeneratedStaticCollectObjectsTextObject(
      object o,
      List<object> collectedObjects)
    {
      ((TextObject) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects) => collectedObjects.Add((object) this.Attributes);

    internal static object AutoGeneratedGetMemberValueAttributes(object o) => (object) ((TextObject) o).Attributes;

    internal static object AutoGeneratedGetMemberValueValue(object o) => (object) ((TextObject) o).Value;

    [SaveableProperty(2)]
    public Dictionary<string, object> Attributes { get; private set; }

    public int Length => this.Value.Length;

    private static bool CheckAttributes(Dictionary<string, object> attributes)
    {
      if (attributes != null)
      {
        foreach (KeyValuePair<string, object> attribute in attributes)
        {
          switch (attribute.Value)
          {
            case TextObject _:
            case string _:
            case float _:
            case int _:
              continue;
            default:
              return false;
          }
        }
      }
      return true;
    }

    public TextObject(string value = "", Dictionary<string, object> attributes = null)
    {
      this.Value = value;
      this.Attributes = attributes;
    }

    public TextObject(int value, Dictionary<string, object> attributes = null)
    {
      this.Value = value.ToString();
      this.Attributes = attributes;
    }

    public TextObject(float value, Dictionary<string, object> attributes = null)
    {
      this.Value = value.ToString("R");
      this.Attributes = attributes;
    }

    [LoadInitializationCallback]
    private void OnLoad(MetaData metaData, ObjectLoadData objectLoadData)
    {
      if (this.Attributes != null || !(objectLoadData.GetDataValueBySaveId(2) is Dictionary<string, TextObject> dataValueBySaveId))
        return;
      this.Attributes = new Dictionary<string, object>();
      foreach (KeyValuePair<string, TextObject> keyValuePair in dataValueBySaveId)
        this.Attributes.Add(keyValuePair.Key, (object) keyValuePair.Value);
    }

    internal static TextObject TryGetOrCreateFromObject(object o)
    {
      switch (o)
      {
        case TextObject _:
          return (TextObject) o;
        case string _:
          return new TextObject((string) o);
        case int num1:
          return new TextObject(num1, (Dictionary<string, object>) null);
        case float num2:
          return new TextObject(num2);
        default:
          return (TextObject) null;
      }
    }

    public static bool IsNullOrEmpty(TextObject to)
    {
      if (to == TextObject.Empty || to == null)
        return true;
      return string.IsNullOrEmpty(to.Value) && to.Attributes == null;
    }

    public static TextObject ToLower(TextObject to) => to.ToLower();

    public TextObject ToLower()
    {
      Dictionary<string, object> attributes = new Dictionary<string, object>();
      if (this.Attributes != null)
      {
        foreach (KeyValuePair<string, object> attribute in this.Attributes)
        {
          string key = attribute.Key;
          object obj = attribute.Value;
          switch (obj)
          {
            case TextObject _:
              TextObject lower1 = TextObject.ToLower((TextObject) attribute.Value);
              attributes.Add(key, (object) lower1);
              continue;
            case string _:
              string lower2 = ((string) attribute.Value).ToLower();
              attributes.Add(key, (object) lower2);
              continue;
            default:
              attributes.Add(key, obj);
              continue;
          }
        }
      }
      return new TextObject(this.Value.ToLower(), attributes);
    }

    public override string ToString()
    {
      try
      {
        return MBTextManager.ProcessTextToString(this);
      }
      catch (Exception ex)
      {
        return ex.ToString();
      }
    }

    public string Format(float p1)
    {
      MBTextManager.SetTextVariable("A0", p1.ToString("F1"), false);
      return new TextObject(this.Value).ToString();
    }

    public bool Contains(TextObject to) => this.Value.Contains(to.Value);

    public bool Contains(string text) => this.Value.Contains(text);

    public bool Equals(TextObject to) => this.Value == to.Value && this.Attributes == to.Attributes;

    public bool Equals(string text) => this.Value == text;

    public static List<string> ConvertToStringList(List<TextObject> to)
    {
      List<string> stringList = new List<string>();
      foreach (TextObject textObject in to)
        stringList.Add(textObject.Value);
      return stringList;
    }

    private TextObject SetTextVariableFromObject(string tag, object variable)
    {
      if (this.Attributes == null)
        this.Attributes = new Dictionary<string, object>();
      if (this.Attributes.ContainsKey(tag))
        this.Attributes[tag] = variable;
      else
        this.Attributes.Add(tag, variable);
      return this;
    }

    public TextObject SetTextVariable(string tag, TextObject variable) => this.SetTextVariableFromObject(tag, (object) variable);

    public TextObject SetTextVariable(string tag, string variable)
    {
      this.SetTextVariableFromObject(tag, (object) variable);
      return this;
    }

    public TextObject SetTextVariable(string tag, float variable)
    {
      this.SetTextVariableFromObject(tag, (object) variable);
      return this;
    }

    public TextObject SetTextVariable(string tag, int variable)
    {
      this.SetTextVariableFromObject(tag, (object) variable);
      return this;
    }

    public void AddIDToValue(string id)
    {
      if (this.Value.Contains(id) || this.Value.StartsWith("{="))
        return;
      string str = this.Value;
      this.Value = "{=" + id + "}" + str;
    }

    public bool GetVariableValue(string tag, out TextObject variable)
    {
      variable = TextObject.Empty;
      object o;
      if (this.Attributes == null || !this.Attributes.TryGetValue(tag, out o))
        return false;
      variable = TextObject.TryGetOrCreateFromObject(o);
      if (variable?.Value != "" || variable?.Attributes == null)
        return true;
      variable.GetVariableValue(tag, out variable);
      return true;
    }

    public TextObject CopyTextObject() => new TextObject(this.Value, this.Attributes);

    public string GetID()
    {
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (GetID));
      if (this.Value != null && this.Value.Length > 2 && (this.Value[0] == '{' && this.Value[1] == '='))
      {
        for (int index = 2; index < this.Value.Length && this.Value[index] != '}'; ++index)
          mbStringBuilder.Append(this.Value[index]);
      }
      return mbStringBuilder.ToStringAndRelease();
    }

    internal bool TryGetAttributesValue(string attribute, out TextObject value)
    {
      object o;
      if (this.Attributes.TryGetValue(attribute, out o))
      {
        value = TextObject.TryGetOrCreateFromObject(o);
        return true;
      }
      value = (TextObject) null;
      return false;
    }
  }
}
