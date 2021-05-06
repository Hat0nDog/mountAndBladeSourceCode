// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ScriptComponentBehaviour
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  public abstract class ScriptComponentBehaviour : DotNetObject
  {
    private static List<ScriptComponentBehaviour> _prefabScriptComponents;
    private static List<ScriptComponentBehaviour> _undoStackScriptComponents;
    private WeakNativeObjectReference _gameEntity;
    private WeakNativeObjectReference _scriptComponent;
    private ScriptComponentBehaviour.TickRequirement _lastTickRequirement;
    private static readonly Dictionary<string, string[]> CachedFields;
    private WeakNativeObjectReference _scene;
    private static readonly Dictionary<System.Type, ScriptComponentBehaviour.ScriptComponentBehaviourCallbackFlags> CallbackFlagsCache = new Dictionary<System.Type, ScriptComponentBehaviour.ScriptComponentBehaviourCallbackFlags>();

    protected void InvalidateWeakPointersIfValid()
    {
      this._gameEntity.ManualInvalidate();
      this._scriptComponent.ManualInvalidate();
    }

    public GameEntity GameEntity
    {
      get => this._gameEntity?.GetNativeObject() as GameEntity;
      private set => this._gameEntity = new WeakNativeObjectReference((NativeObject) value);
    }

    public ManagedScriptComponent ScriptComponent
    {
      get => this._scriptComponent != null ? this._scriptComponent.GetNativeObject() as ManagedScriptComponent : (ManagedScriptComponent) null;
      private set => this._scriptComponent = new WeakNativeObjectReference((NativeObject) value);
    }

    protected ManagedScriptHolder ManagedScriptHolder { get; private set; }

    public Scene Scene
    {
      get => this._scene != null ? this._scene.GetNativeObject() as Scene : (Scene) null;
      private set => this._scene = new WeakNativeObjectReference((NativeObject) value);
    }

    static ScriptComponentBehaviour()
    {
      ScriptComponentBehaviour._prefabScriptComponents = new List<ScriptComponentBehaviour>();
      ScriptComponentBehaviour._undoStackScriptComponents = new List<ScriptComponentBehaviour>();
      if (ScriptComponentBehaviour.CachedFields != null)
        return;
      ScriptComponentBehaviour.CachedFields = new Dictionary<string, string[]>();
      ScriptComponentBehaviour.CacheEditableFieldsForAllScriptComponents();
    }

    internal void Construct(GameEntity myEntity, ManagedScriptComponent scriptComponent)
    {
      this.GameEntity = myEntity;
      this.Scene = myEntity.Scene;
      this.ScriptComponent = scriptComponent;
    }

    internal void SetOwnerManagedScriptHolder(ManagedScriptHolder managedScriptHolder) => this.ManagedScriptHolder = managedScriptHolder;

    public void SetScriptComponentToTick(ScriptComponentBehaviour.TickRequirement value)
    {
      if (this._lastTickRequirement == value)
        return;
      switch (value)
      {
        case ScriptComponentBehaviour.TickRequirement.None:
          if (this._lastTickRequirement == ScriptComponentBehaviour.TickRequirement.Tick)
          {
            this.ManagedScriptHolder.RemoveScriptComponentFromTickList(this);
            break;
          }
          this.ManagedScriptHolder.RemoveScriptComponentFromTickOccasionallyList(this);
          break;
        case ScriptComponentBehaviour.TickRequirement.TickOccasionally:
          if (this._lastTickRequirement == ScriptComponentBehaviour.TickRequirement.Tick)
          {
            this.ManagedScriptHolder.RemoveScriptComponentFromTickList(this);
            this.ManagedScriptHolder.AddScriptComponentToTickOccasionallyList(this);
            break;
          }
          this.ManagedScriptHolder.AddScriptComponentToTickOccasionallyList(this);
          break;
        case ScriptComponentBehaviour.TickRequirement.Tick:
          if (this._lastTickRequirement == ScriptComponentBehaviour.TickRequirement.TickOccasionally)
          {
            this.ManagedScriptHolder.RemoveScriptComponentFromTickOccasionallyList(this);
            this.ManagedScriptHolder.AddScriptComponentToTickList(this);
            break;
          }
          this.ManagedScriptHolder.AddScriptComponentToTickList(this);
          break;
      }
      this._lastTickRequirement = value;
    }

    [EngineCallback]
    internal void AddScriptComponentToTick()
    {
      lock (ScriptComponentBehaviour._prefabScriptComponents)
      {
        if (ScriptComponentBehaviour._prefabScriptComponents.Contains(this))
          return;
        ScriptComponentBehaviour._prefabScriptComponents.Add(this);
      }
    }

    [EngineCallback]
    internal void RegisterAsPrefabScriptComponent()
    {
      lock (ScriptComponentBehaviour._prefabScriptComponents)
      {
        if (ScriptComponentBehaviour._prefabScriptComponents.Contains(this))
          return;
        ScriptComponentBehaviour._prefabScriptComponents.Add(this);
      }
    }

    [EngineCallback]
    internal void DeregisterAsPrefabScriptComponent()
    {
      lock (ScriptComponentBehaviour._prefabScriptComponents)
        ScriptComponentBehaviour._prefabScriptComponents.Remove(this);
    }

    [EngineCallback]
    internal void RegisterAsUndoStackScriptComponent()
    {
      if (ScriptComponentBehaviour._undoStackScriptComponents.Contains(this))
        return;
      ScriptComponentBehaviour._undoStackScriptComponents.Add(this);
    }

    [EngineCallback]
    internal void DeregisterAsUndoStackScriptComponent()
    {
      if (!ScriptComponentBehaviour._undoStackScriptComponents.Contains(this))
        return;
      ScriptComponentBehaviour._undoStackScriptComponents.Remove(this);
    }

    [EngineCallback]
    protected internal virtual void SetScene(Scene scene) => this.Scene = scene;

    [EngineCallback]
    protected internal virtual void OnInit()
    {
    }

    [EngineCallback]
    internal virtual void HandleOnRemoved(int removeReason)
    {
      this.OnRemoved(removeReason);
      this._scene = (WeakNativeObjectReference) null;
      this._gameEntity = (WeakNativeObjectReference) null;
    }

    protected virtual void OnRemoved(int removeReason)
    {
    }

    protected virtual ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.None;

    protected internal virtual void OnTick(float dt)
    {
    }

    protected internal virtual void OnTickOccasionally(float currentFrameDeltaTime)
    {
    }

    [EngineCallback]
    protected internal virtual void OnPreInit()
    {
    }

    [EngineCallback]
    protected internal virtual void OnEditorInit()
    {
    }

    [EngineCallback]
    protected internal virtual void OnEditorTick(float dt)
    {
    }

    [EngineCallback]
    protected internal virtual void OnEditorValidate()
    {
    }

    [EngineCallback]
    protected internal virtual bool IsOnlyVisual() => false;

    [EngineCallback]
    protected internal virtual bool MovesEntity() => true;

    [EngineCallback]
    protected internal virtual bool DisablesOroCreation() => true;

    [EngineCallback]
    protected internal virtual void OnEditorVariableChanged(string variableName)
    {
    }

    [EngineCallback]
    protected internal virtual void OnSceneSave(string saveFolder)
    {
    }

    [EngineCallback]
    protected internal virtual bool OnCheckForProblems() => false;

    [EngineCallback]
    protected internal virtual void OnPhysicsCollision(ref PhysicsContact contact)
    {
    }

    [EngineCallback]
    protected internal virtual void OnEditModeVisibilityChanged(bool currentVisibility)
    {
    }

    protected internal bool IsOnHitCallable() => (uint) (this.GetCallbackFlags() & ScriptComponentBehaviour.ScriptComponentBehaviourCallbackFlags.OnHit) > 0U;

    [EngineCallback]
    protected internal ScriptComponentBehaviour.ScriptComponentBehaviourCallbackFlags GetCallbackFlags()
    {
      System.Type type = this.GetType();
      ScriptComponentBehaviour.ScriptComponentBehaviourCallbackFlags behaviourCallbackFlags;
      if (!ScriptComponentBehaviour.CallbackFlagsCache.TryGetValue(type, out behaviourCallbackFlags))
      {
        behaviourCallbackFlags = ScriptComponentBehaviour.ScriptComponentBehaviourCallbackFlags.None;
        if (this.IsMethodOverridden("OnTick"))
          behaviourCallbackFlags |= ScriptComponentBehaviour.ScriptComponentBehaviourCallbackFlags.Tick;
        if (this.IsMethodOverridden("OnHit"))
          behaviourCallbackFlags |= ScriptComponentBehaviour.ScriptComponentBehaviourCallbackFlags.OnHit;
        ScriptComponentBehaviour.CallbackFlagsCache.Add(type, behaviourCallbackFlags);
      }
      return behaviourCallbackFlags;
    }

    [EngineCallback]
    protected internal bool IsMethodOverridden(string methodName)
    {
      System.Type type1 = this.GetType();
      MethodInfo method = type1.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
      bool flag;
      if (method == (MethodInfo) null)
      {
        flag = false;
      }
      else
      {
        System.Type type2 = type1;
        for (System.Type type3 = type1; type3 != (System.Type) null; type3 = type3.BaseType)
        {
          if (type3.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic) != (MethodInfo) null)
            type2 = type3;
        }
        flag = method.DeclaringType != type2;
      }
      return flag;
    }

    private static bool CheckIfOverrideValid(MethodInfo mInfo)
    {
      byte[] ilAsByteArray = mInfo.GetMethodBody().GetILAsByteArray();
      Dictionary<short, OpCode> dictionary = ((IEnumerable<FieldInfo>) typeof (OpCodes).GetFields()).Select<FieldInfo, OpCode>((Func<FieldInfo, OpCode>) (fieldInfo => (OpCode) fieldInfo.GetValue((object) null))).ToDictionary<OpCode, short>((Func<OpCode, short>) (opCode => opCode.Value));
      int index = 0;
      bool flag = false;
      while (index < ilAsByteArray.Length)
      {
        short key = (short) ilAsByteArray[index++];
        if (key == (short) 254)
          key = (short) ((int) ilAsByteArray[index++] | 65024);
        switch (dictionary[key].OperandType)
        {
          case OperandType.InlineBrTarget:
            index += 4;
            continue;
          case OperandType.InlineField:
            index += 4;
            flag = true;
            continue;
          case OperandType.InlineI:
            index += 4;
            continue;
          case OperandType.InlineI8:
            index += 8;
            continue;
          case OperandType.InlineMethod:
            int metadataToken = (int) ilAsByteArray[index] | (int) ilAsByteArray[index + 1] << 8 | (int) ilAsByteArray[index + 2] << 16 | (int) ilAsByteArray[index + 3] << 24;
            System.Type[] genericMethodArguments = (System.Type[]) null;
            if (mInfo.IsGenericMethod)
              genericMethodArguments = mInfo.GetGenericArguments();
            MethodInfo methodInfo1 = mInfo.Module.ResolveMethod(metadataToken, mInfo.DeclaringType.GetGenericArguments(), genericMethodArguments) as MethodInfo;
            if (methodInfo1 != (MethodInfo) null)
            {
              if (mInfo.Name == methodInfo1.Name)
              {
                MethodInfo methodInfo2 = mInfo;
                while (methodInfo2 != methodInfo2.GetBaseDefinition())
                  methodInfo2 = methodInfo2.GetBaseDefinition();
                MethodInfo methodInfo3 = methodInfo1;
                while (methodInfo3 != methodInfo3.GetBaseDefinition())
                  methodInfo3 = methodInfo3.GetBaseDefinition();
                if (!(methodInfo2 == methodInfo3))
                  flag = true;
              }
              else
                flag = true;
            }
            index += 4;
            continue;
          case OperandType.InlineR:
            index += 8;
            continue;
          case OperandType.InlineSig:
            index += 4;
            continue;
          case OperandType.InlineString:
            index += 4;
            continue;
          case OperandType.InlineSwitch:
            int num = ((int) ilAsByteArray[index] | (int) ilAsByteArray[index + 1] << 8 | (int) ilAsByteArray[index + 2] << 16 | (int) ilAsByteArray[index + 3] << 24) + 1;
            index += 4 * num;
            continue;
          case OperandType.InlineTok:
            index += 4;
            continue;
          case OperandType.InlineType:
            index += 4;
            continue;
          case OperandType.InlineVar:
            index += 2;
            continue;
          case OperandType.ShortInlineBrTarget:
            ++index;
            continue;
          case OperandType.ShortInlineI:
            ++index;
            continue;
          case OperandType.ShortInlineR:
            index += 4;
            continue;
          case OperandType.ShortInlineVar:
            ++index;
            continue;
          default:
            continue;
        }
      }
      return flag;
    }

    private static void CacheEditableFieldsForAllScriptComponents()
    {
      foreach (KeyValuePair<string, System.Type> moduleType in Managed.ModuleTypes)
      {
        string key = moduleType.Key;
        ScriptComponentBehaviour.CachedFields.Add(key, ScriptComponentBehaviour.CollectEditableFields(key));
      }
    }

    private static string[] CollectEditableFields(string className)
    {
      List<string> stringList = new List<string>();
      System.Type type;
      if (Managed.ModuleTypes.TryGetValue(className, out type))
      {
        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        for (int index = 0; index < fields.Length; ++index)
        {
          FieldInfo fieldInfo = fields[index];
          object[] customAttributes = fieldInfo.GetCustomAttributes(typeof (EditableScriptComponentVariable), true);
          bool flag = false;
          if (customAttributes.Length != 0)
            flag = ((EditableScriptComponentVariable) customAttributes[0]).Visible;
          else if (!fieldInfo.IsPrivate && !fieldInfo.IsFamily)
            flag = true;
          if (flag)
            stringList.Add(fields[index].Name);
        }
      }
      return stringList.ToArray();
    }

    [EngineCallback]
    internal static string[] GetEditableFields(string className)
    {
      string[] strArray;
      ScriptComponentBehaviour.CachedFields.TryGetValue(className, out strArray);
      return strArray;
    }

    public enum TickRequirement
    {
      None,
      TickOccasionally,
      Tick,
    }

    [Flags]
    protected internal enum ScriptComponentBehaviourCallbackFlags : byte
    {
      None = 0,
      Tick = 1,
      OnHit = 2,
    }
  }
}
