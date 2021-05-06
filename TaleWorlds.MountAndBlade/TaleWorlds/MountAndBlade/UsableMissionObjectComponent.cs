// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.UsableMissionObjectComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public abstract class UsableMissionObjectComponent
  {
    protected internal virtual void OnAdded(Scene scene)
    {
    }

    protected internal virtual void OnRemoved()
    {
    }

    protected internal virtual void OnFocusGain(Agent userAgent)
    {
    }

    protected internal virtual void OnFocusLose(Agent userAgent)
    {
    }

    public virtual bool IsOnTickRequired() => false;

    protected internal virtual void OnTick(float dt)
    {
    }

    protected internal virtual void OnEditorTick(float dt)
    {
    }

    protected internal virtual void OnEditorValidate()
    {
    }

    protected internal virtual void OnUse(Agent userAgent)
    {
    }

    protected internal virtual void OnUseStopped(Agent userAgent, bool isSuccessful = true)
    {
    }

    protected internal virtual void OnMissionReset()
    {
    }

    protected internal virtual bool ReadFromNetwork() => true;

    protected internal virtual void WriteToNetwork()
    {
    }
  }
}
