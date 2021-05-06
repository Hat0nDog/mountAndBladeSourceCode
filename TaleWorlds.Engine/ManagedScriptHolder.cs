// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ManagedScriptHolder
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  public sealed class ManagedScriptHolder : DotNetObject
  {
    private readonly List<ScriptComponentBehaviour> _scriptComponentsToTick = new List<ScriptComponentBehaviour>(512);
    private readonly List<ScriptComponentBehaviour> _scriptComponentsToTickOccasionally = new List<ScriptComponentBehaviour>(512);
    private readonly List<ScriptComponentBehaviour> _scriptComponentsToTickForEditor = new List<ScriptComponentBehaviour>(512);
    private int _nextIndexToTickOccasionally;
    private readonly List<ScriptComponentBehaviour> _scriptComponentsToAddToTick = new List<ScriptComponentBehaviour>();
    private readonly List<ScriptComponentBehaviour> _scriptComponentsToRemoveFromTick = new List<ScriptComponentBehaviour>();
    private readonly List<ScriptComponentBehaviour> _scriptComponentsToAddToTickOccasionally = new List<ScriptComponentBehaviour>();
    private readonly List<ScriptComponentBehaviour> _scriptComponentsToRemoveFromTickOccasionally = new List<ScriptComponentBehaviour>();
    private readonly List<ScriptComponentBehaviour> _scriptComponentsToAddToTickForEditor = new List<ScriptComponentBehaviour>();
    private readonly List<ScriptComponentBehaviour> _scriptComponentsToRemoveFromTickForEditor = new List<ScriptComponentBehaviour>();

    [EngineCallback]
    internal static ManagedScriptHolder CreateManagedScriptHolder() => new ManagedScriptHolder();

    [EngineCallback]
    public void SetScriptComponentHolder(ScriptComponentBehaviour sc)
    {
      sc.SetOwnerManagedScriptHolder(this);
      if (this._scriptComponentsToRemoveFromTickForEditor.IndexOf(sc) != -1)
        this._scriptComponentsToRemoveFromTickForEditor.Remove(sc);
      else
        this._scriptComponentsToAddToTickForEditor.Add(sc);
    }

    public void AddScriptComponentToTickOccasionallyList(ScriptComponentBehaviour sc)
    {
      if (this._scriptComponentsToRemoveFromTickOccasionally.IndexOf(sc) != -1)
        this._scriptComponentsToRemoveFromTickOccasionally.Remove(sc);
      else
        this._scriptComponentsToAddToTickOccasionally.Add(sc);
    }

    public void AddScriptComponentToTickList(ScriptComponentBehaviour sc)
    {
      if (this._scriptComponentsToRemoveFromTick.IndexOf(sc) != -1)
        this._scriptComponentsToRemoveFromTick.Remove(sc);
      else
        this._scriptComponentsToAddToTick.Add(sc);
    }

    [EngineCallback]
    public void RemoveScriptComponentFromAllTickLists(ScriptComponentBehaviour sc)
    {
      if (this._scriptComponentsToAddToTick.IndexOf(sc) >= 0)
        this._scriptComponentsToAddToTick.Remove(sc);
      else if (this._scriptComponentsToAddToTickOccasionally.IndexOf(sc) >= 0)
        this._scriptComponentsToAddToTickOccasionally.Remove(sc);
      else if (this._scriptComponentsToRemoveFromTick.IndexOf(sc) == -1 && this._scriptComponentsToTick.IndexOf(sc) != -1)
        this._scriptComponentsToRemoveFromTick.Add(sc);
      else if (this._scriptComponentsToRemoveFromTickOccasionally.IndexOf(sc) == -1 && this._scriptComponentsToTickOccasionally.IndexOf(sc) != -1)
        this._scriptComponentsToRemoveFromTickOccasionally.Add(sc);
      if (this._scriptComponentsToAddToTickForEditor.IndexOf(sc) != -1)
      {
        this._scriptComponentsToAddToTickForEditor.Remove(sc);
      }
      else
      {
        if (this._scriptComponentsToRemoveFromTickForEditor.IndexOf(sc) != -1)
          return;
        this._scriptComponentsToRemoveFromTickForEditor.Add(sc);
      }
    }

    public void RemoveScriptComponentFromTickList(ScriptComponentBehaviour sc)
    {
      if (this._scriptComponentsToAddToTick.IndexOf(sc) >= 0)
      {
        this._scriptComponentsToAddToTick.Remove(sc);
      }
      else
      {
        if (this._scriptComponentsToRemoveFromTick.IndexOf(sc) != -1 || this._scriptComponentsToTick.IndexOf(sc) == -1)
          return;
        this._scriptComponentsToRemoveFromTick.Add(sc);
      }
    }

    public void RemoveScriptComponentFromTickOccasionallyList(ScriptComponentBehaviour sc)
    {
      if (this._scriptComponentsToAddToTickOccasionally.IndexOf(sc) >= 0)
      {
        this._scriptComponentsToAddToTickOccasionally.Remove(sc);
      }
      else
      {
        if (this._scriptComponentsToRemoveFromTickOccasionally.IndexOf(sc) != -1 || this._scriptComponentsToTickOccasionally.IndexOf(sc) == -1)
          return;
        this._scriptComponentsToRemoveFromTickOccasionally.Add(sc);
      }
    }

    [EngineCallback]
    internal int GetNumberOfScripts() => this._scriptComponentsToTick.Count;

    [EngineCallback]
    internal void TickComponents(float dt)
    {
      for (int index = 0; index < this._scriptComponentsToRemoveFromTick.Count; ++index)
        this._scriptComponentsToTick.Remove(this._scriptComponentsToRemoveFromTick[index]);
      this._scriptComponentsToRemoveFromTick.Clear();
      for (int index = 0; index < this._scriptComponentsToAddToTick.Count; ++index)
        this._scriptComponentsToTick.Add(this._scriptComponentsToAddToTick[index]);
      this._scriptComponentsToAddToTick.Clear();
      for (int index = 0; index < this._scriptComponentsToTick.Count; ++index)
      {
        if (this._scriptComponentsToRemoveFromTick.IndexOf(this._scriptComponentsToTick[index]) == -1)
          this._scriptComponentsToTick[index].OnTick(dt);
      }
      for (int index = 0; index < this._scriptComponentsToRemoveFromTickOccasionally.Count; ++index)
        this._scriptComponentsToTickOccasionally.Remove(this._scriptComponentsToRemoveFromTickOccasionally[index]);
      this._nextIndexToTickOccasionally = Math.Max(0, this._nextIndexToTickOccasionally - this._scriptComponentsToRemoveFromTickOccasionally.Count);
      this._scriptComponentsToRemoveFromTickOccasionally.Clear();
      for (int index = 0; index < this._scriptComponentsToAddToTickOccasionally.Count; ++index)
        this._scriptComponentsToTickOccasionally.Add(this._scriptComponentsToAddToTickOccasionally[index]);
      int num = this._scriptComponentsToTickOccasionally.Count / 10 + 1;
      this._scriptComponentsToAddToTickOccasionally.Clear();
      for (int index = 0; this._nextIndexToTickOccasionally < this._scriptComponentsToTickOccasionally.Count && index < num; ++index)
      {
        if (this._scriptComponentsToRemoveFromTickOccasionally.IndexOf(this._scriptComponentsToTickOccasionally[this._nextIndexToTickOccasionally]) == -1)
          this._scriptComponentsToTickOccasionally[this._nextIndexToTickOccasionally].OnTickOccasionally(dt);
        ++this._nextIndexToTickOccasionally;
      }
      this._nextIndexToTickOccasionally = this._scriptComponentsToTickOccasionally.Count > 0 ? this._nextIndexToTickOccasionally % this._scriptComponentsToTickOccasionally.Count : 0;
    }

    [EngineCallback]
    internal void TickComponentsEditor(float dt)
    {
      for (int index = 0; index < this._scriptComponentsToRemoveFromTickForEditor.Count; ++index)
        this._scriptComponentsToTickForEditor.Remove(this._scriptComponentsToRemoveFromTickForEditor[index]);
      this._scriptComponentsToRemoveFromTickForEditor.Clear();
      for (int index = 0; index < this._scriptComponentsToAddToTickForEditor.Count; ++index)
        this._scriptComponentsToTickForEditor.Add(this._scriptComponentsToAddToTickForEditor[index]);
      this._scriptComponentsToAddToTickForEditor.Clear();
      for (int index = 0; index < this._scriptComponentsToTickForEditor.Count; ++index)
      {
        if (this._scriptComponentsToRemoveFromTickForEditor.IndexOf(this._scriptComponentsToTickForEditor[index]) == -1)
          this._scriptComponentsToTickForEditor[index].OnEditorTick(dt);
      }
    }
  }
}
