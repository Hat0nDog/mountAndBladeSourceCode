// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ManagedExtensions
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public static class ManagedExtensions
  {
    [EngineCallback]
    internal static void SetObjectField(
      DotNetObject managedObject,
      string fieldName,
      ref ScriptComponentFieldHolder scriptComponentHolder,
      int type,
      int callFieldChangeEventAsInteger)
    {
      bool flag1 = (uint) callFieldChangeEventAsInteger > 0U;
      Managed.RglScriptFieldType rglScriptFieldType = (Managed.RglScriptFieldType) type;
      FieldInfo fieldOfClass = Managed.GetFieldOfClass(managedObject.GetType().Name, fieldName);
      switch (rglScriptFieldType)
      {
        case Managed.RglScriptFieldType.RglSftString:
          fieldOfClass.SetValue((object) managedObject, (object) scriptComponentHolder.s);
          break;
        case Managed.RglScriptFieldType.RglSftDouble:
          fieldOfClass.SetValue((object) managedObject, Convert.ChangeType((object) scriptComponentHolder.d, fieldOfClass.FieldType));
          break;
        case Managed.RglScriptFieldType.RglSftFloat:
          fieldOfClass.SetValue((object) managedObject, Convert.ChangeType((object) scriptComponentHolder.f, fieldOfClass.FieldType));
          break;
        case Managed.RglScriptFieldType.RglSftBool:
          bool flag2 = scriptComponentHolder.b > 0;
          fieldOfClass.SetValue((object) managedObject, (object) flag2);
          break;
        case Managed.RglScriptFieldType.RglSftInt:
          fieldOfClass.SetValue((object) managedObject, Convert.ChangeType((object) scriptComponentHolder.i, fieldOfClass.FieldType));
          break;
        case Managed.RglScriptFieldType.RglSftVec3:
          Vec3 vec3 = new Vec3(scriptComponentHolder.v3.x, scriptComponentHolder.v3.y, scriptComponentHolder.v3.z, scriptComponentHolder.v3.w);
          fieldOfClass.SetValue((object) managedObject, (object) vec3);
          break;
        case Managed.RglScriptFieldType.RglSftEntity:
          fieldOfClass.SetValue((object) managedObject, scriptComponentHolder.entityPointer != UIntPtr.Zero ? (object) new GameEntity(scriptComponentHolder.entityPointer) : (object) (GameEntity) null);
          break;
        case Managed.RglScriptFieldType.RglSftTexture:
          fieldOfClass.SetValue((object) managedObject, scriptComponentHolder.texturePointer != UIntPtr.Zero ? (object) new Texture(scriptComponentHolder.texturePointer) : (object) (Texture) null);
          break;
        case Managed.RglScriptFieldType.RglSftMesh:
          fieldOfClass.SetValue((object) managedObject, scriptComponentHolder.meshPointer != UIntPtr.Zero ? (object) new MetaMesh(scriptComponentHolder.meshPointer) : (object) (MetaMesh) null);
          break;
        case Managed.RglScriptFieldType.RglSftEnum:
          object obj = Enum.Parse(fieldOfClass.FieldType, scriptComponentHolder.enumValue);
          fieldOfClass.SetValue((object) managedObject, obj);
          break;
        case Managed.RglScriptFieldType.RglSftMaterial:
          fieldOfClass.SetValue((object) managedObject, scriptComponentHolder.materialPointer != UIntPtr.Zero ? (object) new Material(scriptComponentHolder.materialPointer) : (object) (Material) null);
          break;
        case Managed.RglScriptFieldType.RglSftMatrixFrame:
          MatrixFrame matrixFrame = scriptComponentHolder.matrixFrame;
          fieldOfClass.SetValue((object) managedObject, (object) matrixFrame);
          break;
      }
      if (!(rglScriptFieldType != Managed.RglScriptFieldType.RglSftButton & flag1) || !(managedObject is ScriptComponentBehaviour))
        return;
      ((ScriptComponentBehaviour) managedObject).OnEditorVariableChanged(fieldName);
    }

    [EngineCallback]
    internal static void GetObjectField(
      DotNetObject managedObject,
      ref ScriptComponentFieldHolder scriptComponentFieldHolder,
      string fieldName,
      int type)
    {
      Managed.RglScriptFieldType rglScriptFieldType = (Managed.RglScriptFieldType) type;
      FieldInfo fieldOfClass = Managed.GetFieldOfClass(managedObject.GetType().Name, fieldName);
      switch (rglScriptFieldType)
      {
        case Managed.RglScriptFieldType.RglSftString:
          scriptComponentFieldHolder.s = (string) fieldOfClass.GetValue((object) managedObject);
          break;
        case Managed.RglScriptFieldType.RglSftDouble:
          scriptComponentFieldHolder.d = (double) Convert.ChangeType(fieldOfClass.GetValue((object) managedObject), typeof (double));
          break;
        case Managed.RglScriptFieldType.RglSftFloat:
          scriptComponentFieldHolder.f = (float) Convert.ChangeType(fieldOfClass.GetValue((object) managedObject), typeof (float));
          break;
        case Managed.RglScriptFieldType.RglSftBool:
          bool flag = (bool) fieldOfClass.GetValue((object) managedObject);
          scriptComponentFieldHolder.b = flag ? 1 : 0;
          break;
        case Managed.RglScriptFieldType.RglSftInt:
          scriptComponentFieldHolder.i = (int) Convert.ChangeType(fieldOfClass.GetValue((object) managedObject), typeof (int));
          break;
        case Managed.RglScriptFieldType.RglSftVec3:
          Vec3 c = (Vec3) fieldOfClass.GetValue((object) managedObject);
          scriptComponentFieldHolder.v3 = new Vec3(c, c.w);
          break;
        case Managed.RglScriptFieldType.RglSftEntity:
          GameEntity gameEntity = (GameEntity) fieldOfClass.GetValue((object) managedObject);
          scriptComponentFieldHolder.entityPointer = (NativeObject) gameEntity != (NativeObject) null ? (UIntPtr) Convert.ChangeType((object) gameEntity.Pointer, typeof (UIntPtr)) : (UIntPtr) 0UL;
          break;
        case Managed.RglScriptFieldType.RglSftTexture:
          Texture texture = (Texture) fieldOfClass.GetValue((object) managedObject);
          scriptComponentFieldHolder.texturePointer = (NativeObject) texture != (NativeObject) null ? (UIntPtr) Convert.ChangeType((object) texture.Pointer, typeof (UIntPtr)) : (UIntPtr) 0UL;
          break;
        case Managed.RglScriptFieldType.RglSftMesh:
          MetaMesh metaMesh = (MetaMesh) fieldOfClass.GetValue((object) managedObject);
          scriptComponentFieldHolder.meshPointer = (NativeObject) metaMesh != (NativeObject) null ? (UIntPtr) Convert.ChangeType((object) metaMesh.Pointer, typeof (UIntPtr)) : (UIntPtr) 0UL;
          break;
        case Managed.RglScriptFieldType.RglSftEnum:
          scriptComponentFieldHolder.enumValue = fieldOfClass.GetValue((object) managedObject).ToString();
          break;
        case Managed.RglScriptFieldType.RglSftMaterial:
          Material material = (Material) fieldOfClass.GetValue((object) managedObject);
          scriptComponentFieldHolder.materialPointer = (NativeObject) material != (NativeObject) null ? (UIntPtr) Convert.ChangeType((object) material.Pointer, typeof (UIntPtr)) : (UIntPtr) 0UL;
          break;
        case Managed.RglScriptFieldType.RglSftMatrixFrame:
          MatrixFrame matrixFrame = (MatrixFrame) fieldOfClass.GetValue((object) managedObject);
          scriptComponentFieldHolder.matrixFrame = matrixFrame;
          break;
      }
    }

    [EngineCallback]
    internal static void CopyObjectFieldsFrom(
      DotNetObject dst,
      DotNetObject src,
      int callFieldChangeEventAsInteger)
    {
      bool flag1 = (uint) callFieldChangeEventAsInteger > 0U;
      foreach (FieldInfo field in dst.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        object[] customAttributes = field.GetCustomAttributes(typeof (EditableScriptComponentVariable), true);
        bool flag2 = false;
        if (customAttributes.Length != 0)
          flag2 = ((EditableScriptComponentVariable) customAttributes[0]).Visible;
        else if (!field.IsPrivate && !field.IsFamily)
          flag2 = true;
        if (flag2)
        {
          field.SetValue((object) dst, field.GetValue((object) src));
          if (flag1 && dst is ScriptComponentBehaviour)
            ((ScriptComponentBehaviour) dst).OnEditorVariableChanged(field.Name);
        }
      }
    }

    [EngineCallback]
    internal static DotNetObject CreateScriptComponentInstance(
      string className,
      GameEntity entity,
      ManagedScriptComponent managedScriptComponent)
    {
      System.Type moduleType = Managed.ModuleTypes[className];
      if (moduleType != (System.Type) null)
      {
        ConstructorInfo constructor = moduleType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder) null, new System.Type[0], (ParameterModifier[]) null);
        if (constructor != (ConstructorInfo) null && constructor.Invoke(new object[0]) is ScriptComponentBehaviour componentBehaviour2)
        {
          componentBehaviour2.Construct(entity, managedScriptComponent);
          return (DotNetObject) componentBehaviour2;
        }
      }
      else
        MBDebug.ShowWarning("CreateScriptComponentInstance failed: " + className);
      return (DotNetObject) null;
    }

    [EngineCallback]
    internal static string GetScriptComponentClassNames()
    {
      List<System.Type> typeList = new List<System.Type>();
      foreach (System.Type c in Managed.ModuleTypes.Values)
      {
        if (!c.IsAbstract && typeof (ScriptComponentBehaviour).IsAssignableFrom(c))
          typeList.Add(c);
      }
      string str = "";
      for (int index = 0; index < typeList.Count; ++index)
      {
        System.Type type = typeList[index];
        str = str + type.Name + "-" + type.BaseType.Name;
        if (index + 1 != typeList.Count)
          str += " ";
      }
      return str;
    }

    [EngineCallback]
    internal static bool GetEditorVisibilityOfField(string className, string fieldName)
    {
      object[] customAttributes = Managed.GetFieldOfClass(className, fieldName).GetCustomAttributes(typeof (EditorVisibleScriptComponentVariable), true);
      return customAttributes.Length == 0 || (customAttributes[0] as EditorVisibleScriptComponentVariable).Visible;
    }

    [EngineCallback]
    internal static int GetTypeOfField(string className, string fieldName)
    {
      FieldInfo fieldOfClass = Managed.GetFieldOfClass(className, fieldName);
      if (fieldOfClass == (FieldInfo) null)
        return -1;
      System.Type fieldType = fieldOfClass.FieldType;
      if (fieldOfClass.FieldType == typeof (string))
        return 0;
      if (fieldOfClass.FieldType == typeof (double))
        return 1;
      if (fieldOfClass.FieldType.IsEnum)
        return 9;
      if (fieldOfClass.FieldType == typeof (float))
        return 2;
      if (fieldOfClass.FieldType == typeof (bool))
        return 3;
      if (fieldType == typeof (byte) || fieldType == typeof (sbyte) || (fieldType == typeof (short) || fieldType == typeof (ushort)) || (fieldType == typeof (int) || fieldType == typeof (uint) || (fieldType == typeof (long) || fieldType == typeof (ulong))))
        return 4;
      if (fieldOfClass.FieldType == typeof (Vec3))
        return 5;
      if (fieldOfClass.FieldType == typeof (GameEntity))
        return 6;
      if (fieldOfClass.FieldType == typeof (Texture))
        return 7;
      if (fieldOfClass.FieldType == typeof (MetaMesh))
        return 8;
      if (fieldOfClass.FieldType == typeof (Material))
        return 10;
      if (fieldOfClass.FieldType == typeof (SimpleButton))
        return 11;
      return fieldOfClass.FieldType == typeof (MatrixFrame) ? 13 : -1;
    }

    [EngineCallback]
    internal static void ForceGarbageCollect() => Utilities.FlushManagedObjectsMemory();

    [EngineCallback]
    internal static void CollectCommandLineFunctions()
    {
      foreach (string commandLineFunction in CommandLineFunctionality.CollectCommandLineFunctions())
        Utilities.AddCommandLineFunction(commandLineFunction);
    }
  }
}
