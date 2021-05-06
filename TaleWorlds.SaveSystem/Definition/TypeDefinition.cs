// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.TypeDefinition
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
  public class TypeDefinition : TypeDefinitionBase
  {
    private Dictionary<MemberTypeId, PropertyDefinition> _properties;
    private Dictionary<MemberTypeId, FieldDefinition> _fields;
    private List<string> _errors;
    private List<MethodInfo> _initializationCallbacks;
    private bool _isClass;

    public List<MemberDefinition> MemberDefinitions { get; private set; }

    public IEnumerable<MethodInfo> InitializationCallbacks => (IEnumerable<MethodInfo>) this._initializationCallbacks;

    public IEnumerable<string> Errors => (IEnumerable<string>) this._errors.AsReadOnly();

    public bool IsClassDefinition => this._isClass;

    public IObjectResolver ObjectResolver { get; private set; }

    public List<CustomField> CustomFields { get; private set; }

    public CollectObjectsDelegate CollectObjectsMethod { get; private set; }

    public TypeDefinition(Type type, SaveId saveId, IObjectResolver objectResolver)
      : base(type, saveId)
    {
      this._isClass = this.Type.IsClass;
      this._errors = new List<string>();
      this._properties = new Dictionary<MemberTypeId, PropertyDefinition>();
      this._fields = new Dictionary<MemberTypeId, FieldDefinition>();
      this.MemberDefinitions = new List<MemberDefinition>();
      this.CustomFields = new List<CustomField>();
      this._initializationCallbacks = new List<MethodInfo>();
      this.ObjectResolver = objectResolver;
    }

    public TypeDefinition(Type type, int saveId, IObjectResolver objectResolver)
      : base(type, (SaveId) new TypeSaveId(saveId))
    {
      this._isClass = this.Type.IsClass;
      this._errors = new List<string>();
      this._properties = new Dictionary<MemberTypeId, PropertyDefinition>();
      this._fields = new Dictionary<MemberTypeId, FieldDefinition>();
      this.MemberDefinitions = new List<MemberDefinition>();
      this.CustomFields = new List<CustomField>();
      this._initializationCallbacks = new List<MethodInfo>();
      this.ObjectResolver = objectResolver;
    }

    public void CollectInitializationCallbacks()
    {
      for (Type type = this.Type; type != typeof (object); type = type.BaseType)
      {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
          if (method.DeclaringType == type && method.GetCustomAttributes(typeof (LoadInitializationCallback)).ToArray<Attribute>().Length != 0 && !this._initializationCallbacks.Contains(method))
            this._initializationCallbacks.Insert(0, method);
        }
      }
    }

    public void CollectProperties()
    {
      foreach (PropertyInfo property in this.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        Attribute[] array = property.GetCustomAttributes(typeof (SaveablePropertyAttribute)).ToArray<Attribute>();
        if (array.Length != 0)
        {
          SaveablePropertyAttribute propertyAttribute = (SaveablePropertyAttribute) array[0];
          MemberTypeId memberTypeId = new MemberTypeId(TypeDefinitionBase.GetClassLevel(property.DeclaringType), propertyAttribute.LocalSaveId);
          PropertyDefinition propertyDefinition = new PropertyDefinition(property, memberTypeId);
          if (this._properties.ContainsKey(memberTypeId))
          {
            this._errors.Add("SaveId " + (object) memberTypeId + " of property " + propertyDefinition.PropertyInfo.Name + " is already defined in type " + this.Type.FullName);
          }
          else
          {
            this._properties.Add(memberTypeId, propertyDefinition);
            this.MemberDefinitions.Add((MemberDefinition) propertyDefinition);
          }
        }
      }
    }

    private static IEnumerable<FieldInfo> GetFieldsOfType(Type type)
    {
      FieldInfo[] fieldInfoArray = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      int index;
      for (index = 0; index < fieldInfoArray.Length; ++index)
      {
        FieldInfo fieldInfo = fieldInfoArray[index];
        if (!fieldInfo.IsPrivate)
          yield return fieldInfo;
      }
      fieldInfoArray = (FieldInfo[]) null;
      for (Type typeToCheck = type; typeToCheck != typeof (object); typeToCheck = typeToCheck.BaseType)
      {
        fieldInfoArray = typeToCheck.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        for (index = 0; index < fieldInfoArray.Length; ++index)
        {
          FieldInfo fieldInfo = fieldInfoArray[index];
          if (fieldInfo.IsPrivate)
            yield return fieldInfo;
        }
        fieldInfoArray = (FieldInfo[]) null;
      }
    }

    public void CollectFields()
    {
      foreach (FieldInfo fieldInfo in TypeDefinition.GetFieldsOfType(this.Type).ToArray<FieldInfo>())
      {
        Attribute[] array = fieldInfo.GetCustomAttributes(typeof (SaveableFieldAttribute)).ToArray<Attribute>();
        if (array.Length != 0)
        {
          SaveableFieldAttribute saveableFieldAttribute = (SaveableFieldAttribute) array[0];
          MemberTypeId memberTypeId = new MemberTypeId(TypeDefinitionBase.GetClassLevel(fieldInfo.DeclaringType), saveableFieldAttribute.LocalSaveId);
          FieldDefinition fieldDefinition = new FieldDefinition(fieldInfo, memberTypeId);
          if (this._fields.ContainsKey(memberTypeId))
          {
            this._errors.Add("SaveId " + (object) memberTypeId + " of field " + (object) fieldDefinition.FieldInfo + " is already defined in type " + this.Type.FullName);
          }
          else
          {
            this._fields.Add(memberTypeId, fieldDefinition);
            this.MemberDefinitions.Add((MemberDefinition) fieldDefinition);
          }
        }
      }
      foreach (CustomField customField in this.CustomFields)
      {
        string name = customField.Name;
        short saveId = customField.SaveId;
        FieldInfo field = this.Type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        MemberTypeId memberTypeId = new MemberTypeId(TypeDefinitionBase.GetClassLevel(field.DeclaringType), saveId);
        FieldDefinition fieldDefinition = new FieldDefinition(field, memberTypeId);
        if (this._fields.ContainsKey(memberTypeId))
        {
          this._errors.Add("SaveId " + (object) memberTypeId + " of field " + (object) fieldDefinition.FieldInfo + " is already defined in type " + this.Type.FullName);
        }
        else
        {
          this._fields.Add(memberTypeId, fieldDefinition);
          this.MemberDefinitions.Add((MemberDefinition) fieldDefinition);
        }
      }
    }

    public void AddCustomField(string fieldName, short saveId) => this.CustomFields.Add(new CustomField(fieldName, saveId));

    public PropertyDefinition GetPropertyDefinitionWithId(MemberTypeId id)
    {
      PropertyDefinition propertyDefinition;
      this._properties.TryGetValue(id, out propertyDefinition);
      return propertyDefinition;
    }

    public FieldDefinition GetFieldDefinitionWithId(MemberTypeId id)
    {
      FieldDefinition fieldDefinition;
      this._fields.TryGetValue(id, out fieldDefinition);
      return fieldDefinition;
    }

    public Dictionary<MemberTypeId, PropertyDefinition>.ValueCollection PropertyDefinitions => this._properties.Values;

    public Dictionary<MemberTypeId, FieldDefinition>.ValueCollection FieldDefinitions => this._fields.Values;

    public void InitializeForAutoGeneration(CollectObjectsDelegate collectObjectsDelegate) => this.CollectObjectsMethod = collectObjectsDelegate;
  }
}
