// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBSubModuleBase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MBSubModuleBase
  {
    protected internal virtual void OnSubModuleLoad()
    {
    }

    protected internal virtual void OnSubModuleUnloaded()
    {
    }

    protected internal virtual void OnBeforeInitialModuleScreenSetAsRoot()
    {
    }

    public virtual void OnConfigChanged()
    {
    }

    protected internal virtual void OnGameStart(Game game, IGameStarter gameStarterObject)
    {
    }

    protected internal virtual void OnApplicationTick(float dt)
    {
    }

    public virtual void OnGameLoaded(Game game, object initializerObject)
    {
    }

    public virtual void OnNewGameCreated(Game game, object initializerObject)
    {
    }

    public virtual void BeginGameStart(Game game)
    {
    }

    public virtual void OnCampaignStart(Game game, object starterObject)
    {
    }

    public virtual void OnMultiplayerGameStart(Game game, object starterObject)
    {
    }

    public virtual void OnGameInitializationFinished(Game game)
    {
    }

    public virtual bool DoLoading(Game game) => true;

    public virtual void OnGameEnd(Game game)
    {
    }

    public virtual void OnMissionBehaviourInitialize(Mission mission)
    {
    }
  }
}
