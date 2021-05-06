// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.SaveCodeGenerationContextAssembly
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.Library.CodeGeneration;

namespace TaleWorlds.SaveSystem.Definition
{
  internal class SaveCodeGenerationContextAssembly
  {
    private List<TypeDefinition> _definitions;
    private List<TypeDefinition> _structDefinitions;
    private List<ContainerDefinition> _containerDefinitions;
    private CodeGenerationContext _codeGenerationContext;
    private DefinitionContext _definitionContext;
    private ClassCode _managerClass;
    private MethodCode _managerMethod;
    private int _delegateCount;
    private int _containerNumber;

    public Assembly Assembly { get; private set; }

    public string Location { get; private set; }

    public string FileName { get; private set; }

    public string DefaultNamespace { get; private set; }

    public SaveCodeGenerationContextAssembly(
      DefinitionContext definitionContext,
      Assembly assembly,
      string defaultNamespace,
      string location,
      string fileName)
    {
      this.Assembly = assembly;
      this.Location = location;
      this.FileName = fileName;
      this.DefaultNamespace = defaultNamespace;
      this._definitionContext = definitionContext;
      this._definitions = new List<TypeDefinition>();
      this._structDefinitions = new List<TypeDefinition>();
      this._containerDefinitions = new List<ContainerDefinition>();
      this._codeGenerationContext = new CodeGenerationContext();
    }

    public void AddClassDefinition(TypeDefinition classDefinition) => this._definitions.Add(classDefinition);

    public void AddStructDefinition(TypeDefinition classDefinition) => this._structDefinitions.Add(classDefinition);

    public bool CheckIfGotAnyNonPrimitiveMembers(TypeDefinition typeDefinition)
    {
      foreach (MemberDefinition memberDefinition in typeDefinition.MemberDefinitions.ToArray())
      {
        Type memberType = memberDefinition.GetMemberType();
        if (memberType.IsClass || !(this._definitionContext.GetTypeDefinition(memberType) is BasicTypeDefinition))
          return true;
      }
      return false;
    }

    private string[] GetNestedClasses(string fullClassName) => fullClassName.Split(new char[1]
    {
      '.'
    }, StringSplitOptions.None);

    private bool CheckIfBaseTypeDefind(Type type)
    {
      Type baseType = type.BaseType;
      TypeDefinitionBase typeDefinitionBase = (TypeDefinitionBase) null;
      for (; baseType != (Type) null && baseType != typeof (object); baseType = baseType.BaseType)
      {
        Type type1 = baseType;
        if (baseType.IsGenericType && !baseType.IsGenericTypeDefinition)
          type1 = baseType.GetGenericTypeDefinition();
        typeDefinitionBase = this._definitionContext.GetTypeDefinition(type1);
        if (typeDefinitionBase != null)
          break;
      }
      return typeDefinitionBase != null && !(typeDefinitionBase is BasicTypeDefinition);
    }

    private bool CheckIfTypeDefined(Type type)
    {
      Type type1 = type;
      if (type.IsGenericType && !type.IsGenericTypeDefinition)
        type1 = type.GetGenericTypeDefinition();
      bool flag;
      switch (this._definitionContext.GetTypeDefinition(type1))
      {
        case null:
        case BasicTypeDefinition _:
          flag = false;
          break;
        default:
          flag = true;
          break;
      }
      return flag;
    }

    private bool CheckIfPrimitiveOrPrimiteHolderStruct(Type type)
    {
      bool flag = false;
      TypeDefinitionBase typeDefinition1 = this._definitionContext.GetTypeDefinition(type);
      if (typeDefinition1 is BasicTypeDefinition)
        flag = true;
      else if (typeDefinition1 is EnumDefinition)
        flag = true;
      if (!flag && typeDefinition1 is TypeDefinition)
      {
        TypeDefinition typeDefinition2 = (TypeDefinition) typeDefinition1;
        if (!typeDefinition2.IsClassDefinition && !this.CheckIfGotAnyNonPrimitiveMembers(typeDefinition2))
          flag = true;
      }
      return flag;
    }

    private int GetClassGenericInformation(string className) => className.Length > 2 && className[className.Length - 2] == '`' ? Convert.ToInt32(className[className.Length - 1].ToString()) : -1;

    private void GenerateForClassOrStruct(TypeDefinition typeDefinition)
    {
      Type type1 = typeDefinition.Type;
      bool isClass = type1.IsClass;
      bool flag1 = !isClass;
      bool flag2 = this.CheckIfBaseTypeDefind(type1);
      string name1 = type1.Namespace;
      string str1 = type1.FullName.Replace('+', '.');
      string fullClassName = str1.Substring(name1.Length + 1, str1.Length - name1.Length - 1).Replace('+', '.');
      string[] nestedClasses = this.GetNestedClasses(fullClassName);
      NamespaceCode orCreateNamespace = this._codeGenerationContext.FindOrCreateNamespace(name1);
      string str2 = nestedClasses[nestedClasses.Length - 1];
      ClassCode classCode = (ClassCode) null;
      for (int index = 0; index < nestedClasses.Length; ++index)
      {
        string className = nestedClasses[index];
        ClassCode clasCode = new ClassCode();
        clasCode.IsPartial = true;
        if (index + 1 == nestedClasses.Length)
          clasCode.IsClass = isClass;
        clasCode.AccessModifier = ClassCodeAccessModifier.DoNotMention;
        int genericInformation = this.GetClassGenericInformation(className);
        if (genericInformation >= 0)
        {
          clasCode.IsGeneric = true;
          clasCode.GenericTypeCount = genericInformation;
          className = className.Substring(0, className.Length - 2);
        }
        clasCode.Name = className;
        if (classCode != null)
          classCode.AddNestedClass(clasCode);
        else
          orCreateNamespace.AddClass(clasCode);
        classCode = clasCode;
      }
      TypeSaveId saveId = (TypeSaveId) typeDefinition.SaveId;
      int delegateCount = this._delegateCount;
      ++this._delegateCount;
      this._managerMethod.AddLine("var typeDefinition" + (object) delegateCount + " =  (global::TaleWorlds.SaveSystem.Definition.TypeDefinition)definitionContext.TryGetTypeDefinition(new global::TaleWorlds.SaveSystem.Definition.TypeSaveId(" + (object) saveId.Id + "));");
      if (!type1.IsGenericTypeDefinition && !type1.IsAbstract)
      {
        MethodCode methodCode = new MethodCode();
        methodCode.IsStatic = true;
        methodCode.AccessModifier = flag1 ? MethodCodeAccessModifier.Public : MethodCodeAccessModifier.Internal;
        methodCode.Name = "AutoGeneratedStaticCollectObjects" + str2;
        methodCode.MethodSignature = "(object o, global::System.Collections.Generic.List<object> collectedObjects)";
        methodCode.AddLine("var target = (global::" + str1 + ")o;");
        methodCode.AddLine("target.AutoGeneratedInstanceCollectObjects(collectedObjects);");
        classCode.AddMethod(methodCode);
        this._managerMethod.AddLine("TaleWorlds.SaveSystem.Definition.CollectObjectsDelegate d" + (object) delegateCount + " = global::" + name1 + "." + fullClassName + "." + methodCode.Name + ";");
        this._managerMethod.AddLine(nameof (typeDefinition) + (object) delegateCount + ".InitializeForAutoGeneration(d" + (object) delegateCount + ");");
      }
      this._managerMethod.AddLine("");
      MethodCode methodCode1 = new MethodCode();
      if (flag2)
      {
        methodCode1.PolymorphismInfo = MethodCodePolymorphismInfo.Override;
        methodCode1.AccessModifier = MethodCodeAccessModifier.Protected;
      }
      else if (!type1.IsSealed)
      {
        methodCode1.PolymorphismInfo = MethodCodePolymorphismInfo.Virtual;
        methodCode1.AccessModifier = MethodCodeAccessModifier.Protected;
      }
      else
      {
        methodCode1.PolymorphismInfo = MethodCodePolymorphismInfo.None;
        methodCode1.AccessModifier = MethodCodeAccessModifier.Private;
      }
      methodCode1.Name = "AutoGeneratedInstanceCollectObjects";
      methodCode1.MethodSignature = "(global::System.Collections.Generic.List<object> collectedObjects)";
      if (flag2)
      {
        methodCode1.AddLine("base.AutoGeneratedInstanceCollectObjects(collectedObjects);");
        methodCode1.AddLine("");
      }
      foreach (MemberDefinition memberDefinition in typeDefinition.MemberDefinitions)
      {
        if (memberDefinition is FieldDefinition)
        {
          FieldInfo fieldInfo = (memberDefinition as FieldDefinition).FieldInfo;
          Type fieldType = fieldInfo.FieldType;
          string name2 = fieldInfo.Name;
          if (fieldType.IsClass || fieldType.IsInterface)
          {
            if (fieldType != typeof (string))
            {
              bool flag3 = false;
              Type declaringType = fieldInfo.DeclaringType;
              if (declaringType != type1)
                flag3 = this.CheckIfTypeDefined(declaringType);
              string str3 = "";
              if (flag3)
                str3 += "//";
              string line = str3 + "collectedObjects.Add(this." + name2 + ");";
              methodCode1.AddLine(line);
            }
          }
          else if (!fieldType.IsClass && this._definitionContext.GetStructDefinition(fieldType) != null)
          {
            string str3 = "";
            bool flag3 = false;
            Type declaringType = fieldInfo.DeclaringType;
            if (declaringType != type1)
              flag3 = this.CheckIfTypeDefined(declaringType);
            if (flag3)
              str3 += "//";
            string str4 = fieldType.FullName.Replace('+', '.');
            string line = str3 + "global::" + str4 + ".AutoGeneratedStaticCollectObjects" + fieldType.Name + "(this." + name2 + ", collectedObjects);";
            methodCode1.AddLine(line);
          }
        }
      }
      methodCode1.AddLine("");
      foreach (MemberDefinition memberDefinition in typeDefinition.MemberDefinitions)
      {
        if (memberDefinition is PropertyDefinition)
        {
          PropertyDefinition propertyDefinition = memberDefinition as PropertyDefinition;
          PropertyInfo propertyInfo = propertyDefinition.PropertyInfo;
          Type propertyType = propertyDefinition.PropertyInfo.PropertyType;
          string name2 = propertyInfo.Name;
          if (propertyType.IsClass || propertyType.IsInterface)
          {
            if (propertyType != typeof (string))
            {
              bool flag3 = false;
              Type declaringType = propertyInfo.DeclaringType;
              if (declaringType != type1)
                flag3 = this.CheckIfTypeDefined(declaringType);
              string str3 = "";
              if (flag3)
                str3 += "//";
              string line = str3 + "collectedObjects.Add(this." + name2 + ");";
              methodCode1.AddLine(line);
            }
          }
          else if (!propertyType.IsClass && this._definitionContext.GetStructDefinition(propertyType) != null)
          {
            bool flag3 = false;
            Type declaringType = propertyInfo.DeclaringType;
            if (declaringType != type1)
              flag3 = this.CheckIfTypeDefined(declaringType);
            string str3 = "";
            if (flag3)
              str3 += "//";
            string str4 = propertyType.FullName.Replace('+', '.');
            string line = str3 + "global::" + str4 + ".AutoGeneratedStaticCollectObjects" + propertyType.Name + "(this." + name2 + ", collectedObjects);";
            methodCode1.AddLine(line);
          }
        }
      }
      classCode.AddMethod(methodCode1);
      foreach (MemberDefinition memberDefinition in typeDefinition.MemberDefinitions)
      {
        if (!type1.IsGenericTypeDefinition)
        {
          MethodCode methodCode2 = new MethodCode();
          string str3 = "";
          Type type2 = (Type) null;
          switch (memberDefinition)
          {
            case PropertyDefinition _:
              PropertyDefinition propertyDefinition = memberDefinition as PropertyDefinition;
              str3 = propertyDefinition.PropertyInfo.Name;
              type2 = propertyDefinition.PropertyInfo.DeclaringType;
              break;
            case FieldDefinition _:
              FieldDefinition fieldDefinition = memberDefinition as FieldDefinition;
              str3 = fieldDefinition.FieldInfo.Name;
              type2 = fieldDefinition.FieldInfo.DeclaringType;
              break;
          }
          bool flag3 = false;
          if (type2 != type1)
            flag3 = this.CheckIfTypeDefined(type2);
          if (!flag3)
          {
            methodCode2.Name = "AutoGeneratedGetMemberValue" + str3;
            methodCode2.MethodSignature = "(object o)";
            methodCode2.IsStatic = true;
            methodCode2.AccessModifier = MethodCodeAccessModifier.Internal;
            methodCode2.PolymorphismInfo = MethodCodePolymorphismInfo.None;
            methodCode2.ReturnParameter = "object";
            methodCode2.AddLine("var target = (global::" + str1 + ")o;");
            methodCode2.AddLine("return (object)target." + str3 + ";");
            classCode.AddMethod(methodCode2);
            string str4 = "GetPropertyDefinitionWithId";
            if (memberDefinition is FieldDefinition)
              str4 = "GetFieldDefinitionWithId";
            this._managerMethod.AddLine("{");
            this._managerMethod.AddLine("var memberDefinition = typeDefinition" + (object) delegateCount + "." + str4 + "(new global::TaleWorlds.SaveSystem.Definition.MemberTypeId(" + (object) memberDefinition.Id.TypeLevel + "," + (object) memberDefinition.Id.LocalSaveId + "));");
            this._managerMethod.AddLine("memberDefinition.InitializeForAutoGeneration(" + ("global::" + name1 + "." + fullClassName + "." + methodCode2.Name) + ");");
            this._managerMethod.AddLine("}");
            this._managerMethod.AddLine("");
          }
        }
      }
    }

    private void GenerateForList(ContainerDefinition containerDefinition)
    {
      Type type = containerDefinition.Type;
      Type genericArgument = type.GetGenericArguments()[0];
      bool flag = this.CheckIfPrimitiveOrPrimiteHolderStruct(genericArgument);
      TypeDefinition structDefinition = this._definitionContext.GetStructDefinition(genericArgument);
      if (!(genericArgument != typeof (object)))
        return;
      MethodCode methodCode = new MethodCode();
      methodCode.IsStatic = true;
      methodCode.AccessModifier = MethodCodeAccessModifier.Private;
      methodCode.Name = "AutoGeneratedStaticCollectObjectsForList" + (object) this._containerNumber;
      methodCode.MethodSignature = "(object o, global::System.Collections.Generic.List<object> collectedObjects)";
      if (flag)
        methodCode.AddLine("//Got no child, type: " + type.FullName);
      else if (structDefinition != null)
      {
        string str = genericArgument.FullName.Replace('+', '.');
        methodCode.AddLine("var target = (global::System.Collections.IList)o;");
        methodCode.AddLine("");
        methodCode.AddLine("for (int i = 0; i < target.Count; i++)");
        methodCode.AddLine("{");
        methodCode.AddLine("var element = target[i];");
        methodCode.AddLine("");
        methodCode.AddLine("global::" + str + ".AutoGeneratedStaticCollectObjects" + genericArgument.Name + "(element, collectedObjects);");
        methodCode.AddLine("}");
      }
      else
      {
        methodCode.AddLine("var target = (global::System.Collections.IList)o;");
        methodCode.AddLine("");
        methodCode.AddLine("for (int i = 0; i < target.Count; i++)");
        methodCode.AddLine("{");
        methodCode.AddLine("var element = target[i];");
        methodCode.AddLine("collectedObjects.Add(element);");
        methodCode.AddLine("}");
      }
      SaveId saveId = containerDefinition.SaveId;
      StringWriter stringWriter1 = new StringWriter();
      StringWriter stringWriter2 = stringWriter1;
      saveId.WriteTo((IWriter) stringWriter2);
      string str1 = flag ? "true" : "false";
      this._managerMethod.AddLine("var saveId" + (object) this._delegateCount + " = global::TaleWorlds.SaveSystem.Definition.SaveId.ReadSaveIdFrom(new global::TaleWorlds.Library.StringReader(\"" + stringWriter1.Data + "\"));");
      this._managerMethod.AddLine("var typeDefinition" + (object) this._delegateCount + " =  (global::TaleWorlds.SaveSystem.Definition.ContainerDefinition)definitionContext.TryGetTypeDefinition(saveId" + (object) this._delegateCount + ");");
      this._managerMethod.AddLine("TaleWorlds.SaveSystem.Definition.CollectObjectsDelegate d" + (object) this._delegateCount + " = " + methodCode.Name + ";");
      this._managerMethod.AddLine("typeDefinition" + (object) this._delegateCount + ".InitializeForAutoGeneration(d" + (object) this._delegateCount + ", " + str1 + ");");
      this._managerMethod.AddLine("");
      ++this._delegateCount;
      this._managerClass.AddMethod(methodCode);
      ++this._containerNumber;
    }

    private void GenerateForArray(ContainerDefinition containerDefinition)
    {
      Type type = containerDefinition.Type;
      Type elementType = type.GetElementType();
      bool flag = this.CheckIfPrimitiveOrPrimiteHolderStruct(elementType);
      TypeDefinition structDefinition = this._definitionContext.GetStructDefinition(elementType);
      if (!(elementType != typeof (object)))
        return;
      MethodCode methodCode = new MethodCode();
      methodCode.IsStatic = true;
      methodCode.AccessModifier = MethodCodeAccessModifier.Private;
      methodCode.Name = "AutoGeneratedStaticCollectObjectsForArray" + (object) this._containerNumber;
      methodCode.MethodSignature = "(object o, global::System.Collections.Generic.List<object> collectedObjects)";
      if (flag)
        methodCode.AddLine("//Got no child, type: " + type.FullName);
      else if (structDefinition != null)
      {
        string str1 = elementType.FullName.Replace('+', '.');
        string str2 = type.FullName.Replace('+', '.');
        methodCode.AddLine("var target = (global::" + str2 + ")o;");
        methodCode.AddLine("");
        methodCode.AddLine("for (int i = 0; i < target.Length; i++)");
        methodCode.AddLine("{");
        methodCode.AddLine("var element = target[i];");
        methodCode.AddLine("");
        methodCode.AddLine("global::" + str1 + ".AutoGeneratedStaticCollectObjects" + elementType.Name + "(element, collectedObjects);");
        methodCode.AddLine("}");
      }
      else
      {
        methodCode.AddLine("var target = (global::System.Array)o;");
        methodCode.AddLine("");
        methodCode.AddLine("for (int i = 0; i < target.Length; i++)");
        methodCode.AddLine("{");
        methodCode.AddLine("var element = target.GetValue(i);");
        methodCode.AddLine("collectedObjects.Add(element);");
        methodCode.AddLine("}");
      }
      SaveId saveId = containerDefinition.SaveId;
      StringWriter stringWriter1 = new StringWriter();
      StringWriter stringWriter2 = stringWriter1;
      saveId.WriteTo((IWriter) stringWriter2);
      string str = flag ? "true" : "false";
      this._managerMethod.AddLine("var saveId" + (object) this._delegateCount + " = global::TaleWorlds.SaveSystem.Definition.SaveId.ReadSaveIdFrom(new global::TaleWorlds.Library.StringReader(\"" + stringWriter1.Data + "\"));");
      this._managerMethod.AddLine("var typeDefinition" + (object) this._delegateCount + " =  (global::TaleWorlds.SaveSystem.Definition.ContainerDefinition)definitionContext.TryGetTypeDefinition(saveId" + (object) this._delegateCount + ");");
      this._managerMethod.AddLine("TaleWorlds.SaveSystem.Definition.CollectObjectsDelegate d" + (object) this._delegateCount + " = " + methodCode.Name + ";");
      this._managerMethod.AddLine("typeDefinition" + (object) this._delegateCount + ".InitializeForAutoGeneration(d" + (object) this._delegateCount + ", " + str + ");");
      this._managerMethod.AddLine("");
      ++this._delegateCount;
      this._managerClass.AddMethod(methodCode);
      ++this._containerNumber;
    }

    private void GenerateForQueue(ContainerDefinition containerDefinition)
    {
      Type type = containerDefinition.Type;
      if (!this.CheckIfPrimitiveOrPrimiteHolderStruct(type.GetGenericArguments()[0]))
        return;
      MethodCode methodCode = new MethodCode();
      methodCode.IsStatic = true;
      methodCode.AccessModifier = MethodCodeAccessModifier.Private;
      methodCode.Name = "AutoGeneratedStaticCollectObjectsForQueue" + (object) this._containerNumber;
      methodCode.MethodSignature = "(object o, global::System.Collections.Generic.List<object> collectedObjects)";
      methodCode.AddLine("//Got no child, type: " + type.FullName);
      SaveId saveId = containerDefinition.SaveId;
      StringWriter stringWriter1 = new StringWriter();
      StringWriter stringWriter2 = stringWriter1;
      saveId.WriteTo((IWriter) stringWriter2);
      this._managerMethod.AddLine("var saveId" + (object) this._delegateCount + " = global::TaleWorlds.SaveSystem.Definition.SaveId.ReadSaveIdFrom(new global::TaleWorlds.Library.StringReader(\"" + stringWriter1.Data + "\"));");
      this._managerMethod.AddLine("var typeDefinition" + (object) this._delegateCount + " =  (global::TaleWorlds.SaveSystem.Definition.ContainerDefinition)definitionContext.TryGetTypeDefinition(saveId" + (object) this._delegateCount + ");");
      this._managerMethod.AddLine("TaleWorlds.SaveSystem.Definition.CollectObjectsDelegate d" + (object) this._delegateCount + " = " + methodCode.Name + ";");
      this._managerMethod.AddLine("typeDefinition" + (object) this._delegateCount + ".InitializeForAutoGeneration(d" + (object) this._delegateCount + ", true);");
      this._managerMethod.AddLine("");
      ++this._delegateCount;
      this._managerClass.AddMethod(methodCode);
      ++this._containerNumber;
    }

    private void GenerateForDictionary(ContainerDefinition containerDefinition)
    {
      Type type = containerDefinition.Type;
      Type genericArgument1 = type.GetGenericArguments()[0];
      Type genericArgument2 = type.GetGenericArguments()[1];
      int num = this.CheckIfPrimitiveOrPrimiteHolderStruct(genericArgument1) ? 1 : 0;
      bool flag1 = this.CheckIfPrimitiveOrPrimiteHolderStruct(genericArgument2);
      TypeDefinition structDefinition1 = this._definitionContext.GetStructDefinition(genericArgument1);
      TypeDefinition structDefinition2 = this._definitionContext.GetStructDefinition(genericArgument2);
      bool flag2 = (num & (flag1 ? 1 : 0)) != 0;
      if (num == 0 && structDefinition1 == null || !flag1 && structDefinition2 == null || (!(genericArgument1 != typeof (object)) || !(genericArgument2 != typeof (object))))
        return;
      MethodCode methodCode = new MethodCode();
      methodCode.IsStatic = true;
      methodCode.AccessModifier = MethodCodeAccessModifier.Private;
      methodCode.Name = "AutoGeneratedStaticCollectObjectsForDictionary" + (object) this._containerNumber;
      methodCode.MethodSignature = "(object o, global::System.Collections.Generic.List<object> collectedObjects)";
      if (flag2)
      {
        methodCode.AddLine("//Got no child, type: " + type.FullName);
      }
      else
      {
        methodCode.AddLine("var target = (global::System.Collections.IDictionary)o;");
        methodCode.AddLine("");
        if (structDefinition1 != null)
        {
          string str = genericArgument1.FullName.Replace('+', '.');
          methodCode.AddLine("foreach (var key in target.Keys)");
          methodCode.AddLine("{");
          methodCode.AddLine("global::" + str + ".AutoGeneratedStaticCollectObjects" + genericArgument1.Name + "(key, collectedObjects);");
          methodCode.AddLine("}");
        }
        methodCode.AddLine("");
        if (structDefinition2 != null)
        {
          string str = genericArgument2.FullName.Replace('+', '.');
          methodCode.AddLine("foreach (var value in target.Values)");
          methodCode.AddLine("{");
          methodCode.AddLine("global::" + str + ".AutoGeneratedStaticCollectObjects" + genericArgument2.Name + "(value, collectedObjects);");
          methodCode.AddLine("}");
        }
      }
      SaveId saveId = containerDefinition.SaveId;
      StringWriter stringWriter1 = new StringWriter();
      StringWriter stringWriter2 = stringWriter1;
      saveId.WriteTo((IWriter) stringWriter2);
      string str1 = flag2 ? "true" : "false";
      this._managerMethod.AddLine("var saveId" + (object) this._delegateCount + " = global::TaleWorlds.SaveSystem.Definition.SaveId.ReadSaveIdFrom(new global::TaleWorlds.Library.StringReader(\"" + stringWriter1.Data + "\"));");
      this._managerMethod.AddLine("var typeDefinition" + (object) this._delegateCount + " =  (global::TaleWorlds.SaveSystem.Definition.ContainerDefinition)definitionContext.TryGetTypeDefinition(saveId" + (object) this._delegateCount + ");");
      this._managerMethod.AddLine("TaleWorlds.SaveSystem.Definition.CollectObjectsDelegate d" + (object) this._delegateCount + " = " + methodCode.Name + ";");
      this._managerMethod.AddLine("typeDefinition" + (object) this._delegateCount + ".InitializeForAutoGeneration(d" + (object) this._delegateCount + ", " + str1 + ");");
      this._managerMethod.AddLine("");
      ++this._delegateCount;
      this._managerClass.AddMethod(methodCode);
      ++this._containerNumber;
    }

    public void Generate()
    {
      NamespaceCode orCreateNamespace = this._codeGenerationContext.FindOrCreateNamespace(this.DefaultNamespace);
      ClassCode classCode = new ClassCode();
      classCode.AccessModifier = ClassCodeAccessModifier.Internal;
      classCode.Name = "AutoGeneratedSaveManager";
      classCode.AddInterface("global::TaleWorlds.SaveSystem.Definition.IAutoGeneratedSaveManager");
      MethodCode methodCode = new MethodCode();
      methodCode.IsStatic = false;
      methodCode.AccessModifier = MethodCodeAccessModifier.Public;
      methodCode.Name = "Initialize";
      methodCode.MethodSignature = "(global::TaleWorlds.SaveSystem.Definition.DefinitionContext definitionContext)";
      classCode.AddMethod(methodCode);
      this._managerMethod = methodCode;
      this._managerClass = classCode;
      ClassCode clasCode = classCode;
      orCreateNamespace.AddClass(clasCode);
      foreach (TypeDefinition definition in this._definitions)
        this.GenerateForClassOrStruct(definition);
      foreach (TypeDefinition structDefinition in this._structDefinitions)
        this.GenerateForClassOrStruct(structDefinition);
      foreach (ContainerDefinition containerDefinition in this._containerDefinitions)
      {
        ContainerType containerType;
        if (containerDefinition.Type.IsContainer(out containerType))
        {
          switch (containerType)
          {
            case ContainerType.List:
              this.GenerateForList(containerDefinition);
              continue;
            case ContainerType.Dictionary:
              this.GenerateForDictionary(containerDefinition);
              continue;
            case ContainerType.Array:
              this.GenerateForArray(containerDefinition);
              continue;
            case ContainerType.Queue:
              this.GenerateForQueue(containerDefinition);
              continue;
            default:
              continue;
          }
        }
      }
    }

    public string GenerateText()
    {
      CodeGenerationFile codeGenerationFile = new CodeGenerationFile();
      this._codeGenerationContext.GenerateInto(codeGenerationFile);
      return codeGenerationFile.GenerateText();
    }

    internal void AddContainerDefinition(ContainerDefinition containerDefinition) => this._containerDefinitions.Add(containerDefinition);
  }
}
