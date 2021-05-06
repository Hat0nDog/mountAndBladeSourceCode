// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.UsableMissionObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public abstract class UsableMissionObject : SynchedMissionObject, IFocusable, IUsable, IVisible
  {
    private Agent _userAgent;
    private readonly Dictionary<Agent, UsableMissionObject.MoveInfo> _movingAgents;
    private readonly List<UsableMissionObjectComponent> _components;
    [EditableScriptComponentVariable(false)]
    public TextObject DescriptionMessage = TextObject.Empty;
    [EditableScriptComponentVariable(false)]
    public TextObject ActionMessage = TextObject.Empty;
    private bool _isDeactivated;
    private bool _isDisabledForPlayers;

    public virtual FocusableObjectType FocusableObjectType => FocusableObjectType.Item;

    public virtual void OnUserConversationStart()
    {
    }

    public virtual void OnUserConversationEnd()
    {
    }

    public Agent UserAgent
    {
      get => this._userAgent;
      private set
      {
        if (this._userAgent == value)
          return;
        this._userAgent = value;
        this.SetScriptComponentToTick(this.GetTickRequirement());
      }
    }

    public GameEntityWithWorldPosition GameEntityWithWorldPosition { get; private set; }

    public Dictionary<Agent, UsableMissionObject.MoveInfo> MovingAgents => this._movingAgents;

    public bool IsInstantUse { get; protected set; }

    public bool IsDeactivated
    {
      get => this._isDeactivated;
      set
      {
        if (value == this._isDeactivated)
          return;
        this._isDeactivated = value;
        if (!this._isDeactivated || GameNetwork.IsClientOrReplay)
          return;
        this.UserAgent?.StopUsingGameObject();
        if (!this.HasAIMovingTo)
          return;
        foreach (KeyValuePair<Agent, UsableMissionObject.MoveInfo> keyValuePair in this._movingAgents.ToList<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>())
          keyValuePair.Key.StopUsingGameObject();
        this._movingAgents.Clear();
        this.SetScriptComponentToTick(this.GetTickRequirement());
      }
    }

    public bool IsDisabledForPlayers
    {
      get => this._isDisabledForPlayers;
      set
      {
        if (value == this._isDisabledForPlayers)
          return;
        this._isDisabledForPlayers = value;
        if (!this._isDisabledForPlayers || GameNetwork.IsClientOrReplay || (this.UserAgent == null || this.UserAgent.IsAIControlled))
          return;
        this.UserAgent.StopUsingGameObject();
      }
    }

    public void SetIsDeactivatedSynched(bool value)
    {
      if (this.IsDeactivated == value)
        return;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetUsableMissionObjectIsDeactivated(this, value));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      this.IsDeactivated = value;
    }

    public void SetIsDisabledForPlayersSynched(bool value)
    {
      if (this.IsDisabledForPlayers == value)
        return;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetUsableMissionObjectIsDisabledForPlayers(this, value));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      this.IsDisabledForPlayers = value;
    }

    public virtual bool IsDisabledForAgent(Agent agent) => this.IsDeactivated || agent.MountAgent != null || this.IsDisabledForPlayers && !agent.IsAIControlled || !agent.IsOnLand();

    protected UsableMissionObject(bool isInstantUse = false)
    {
      this._movingAgents = new Dictionary<Agent, UsableMissionObject.MoveInfo>();
      this._components = new List<UsableMissionObjectComponent>();
      this.IsInstantUse = isInstantUse;
      this.GameEntityWithWorldPosition = (GameEntityWithWorldPosition) null;
    }

    public void AddComponent(UsableMissionObjectComponent component)
    {
      this._components.Add(component);
      component.OnAdded(this.Scene);
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public void RemoveComponent(UsableMissionObjectComponent component)
    {
      component.OnRemoved();
      this._components.Remove(component);
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public T GetComponent<T>() where T : UsableMissionObjectComponent => this._components.Find((Predicate<UsableMissionObjectComponent>) (c => c is T)) as T;

    private void CollectChildEntities() => this.CollectChildEntitiesAux(this.GameEntity);

    private void CollectChildEntitiesAux(GameEntity entity)
    {
      foreach (GameEntity child in entity.GetChildren())
      {
        this.CollectChildEntity(child);
        if (child.GetScriptComponents().IsEmpty<ScriptComponentBehaviour>())
          this.CollectChildEntitiesAux(child);
      }
    }

    public void RefreshGameEntityWithWorldPosition() => this.GameEntityWithWorldPosition = new GameEntityWithWorldPosition(this.GameEntity);

    protected virtual void CollectChildEntity(GameEntity childEntity)
    {
    }

    protected virtual bool VerifyChildEntities(ref string errorMessage) => true;

    protected internal override void OnInit()
    {
      base.OnInit();
      this.CollectChildEntities();
      this.LockUserFrames = !this.IsInstantUse;
      this.RefreshGameEntityWithWorldPosition();
    }

    protected internal override void OnEditorInit()
    {
      base.OnEditorInit();
      this.CollectChildEntities();
    }

    protected internal override void OnMissionReset()
    {
      base.OnMissionReset();
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnMissionReset();
    }

    public virtual void OnFocusGain(Agent userAgent)
    {
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnFocusGain(userAgent);
    }

    public virtual void OnFocusLose(Agent userAgent)
    {
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnFocusLose(userAgent);
    }

    public virtual TextObject GetInfoTextForBeingNotInteractable(Agent userAgent) => TextObject.Empty;

    public virtual void SetUserForClient(Agent userAgent)
    {
      this.UserAgent?.SetUsedGameObjectForClient((UsableMissionObject) null);
      this.UserAgent = userAgent;
      userAgent?.SetUsedGameObjectForClient(this);
    }

    public virtual void OnUse(Agent userAgent)
    {
      if (!GameNetwork.IsClientOrReplay)
      {
        if (!userAgent.IsAIControlled && this.HasAIUser)
          this.UserAgent.StopUsingGameObject(false);
        if (this._movingAgents.ContainsKey(userAgent))
        {
          this._movingAgents.Remove(userAgent);
          this.SetScriptComponentToTick(this.GetTickRequirement());
        }
        if (this.HasAIMovingTo && !this.IsInstantUse)
        {
          while (this.HasAIMovingTo)
            this._movingAgents.ElementAt<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>(0).Key.StopUsingGameObject(false);
        }
        foreach (UsableMissionObjectComponent component in this._components)
          component.OnUse(userAgent);
        this.UserAgent = userAgent;
        if (!GameNetwork.IsServerOrRecorder)
          return;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new UseObject(userAgent, this));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      else if (this.LockUserFrames)
      {
        WorldFrame userFrameForAgent = this.GetUserFrameForAgent(userAgent);
        userAgent.SetTargetPositionAndDirection(userFrameForAgent.Origin.AsVec2, userFrameForAgent.Rotation.f);
      }
      else
      {
        if (!this.LockUserPositions)
          return;
        userAgent.SetTargetPosition(this.GetUserFrameForAgent(userAgent).Origin.AsVec2);
      }
    }

    public virtual void OnAIMoveToUse(Agent userAgent)
    {
      this._movingAgents.Add(userAgent, new UsableMissionObject.MoveInfo());
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public virtual void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
    {
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnUseStopped(userAgent, isSuccessful);
      this.UserAgent = (Agent) null;
    }

    public virtual void OnMoveToStopped(Agent movingAgent)
    {
      this._movingAgents.Remove(movingAgent);
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public virtual void SimulateTick(float dt)
    {
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement()
    {
      bool flag = false;
      foreach (UsableMissionObjectComponent component in this._components)
      {
        if (component.IsOnTickRequired())
        {
          flag = true;
          break;
        }
      }
      return flag || this.HasUser || this.HasAIMovingTo ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();
    }

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnTick(dt);
      if (this.HasUser && this.HasUserPositionsChanged(this.UserAgent))
      {
        if (this.LockUserFrames)
        {
          WorldFrame userFrameForAgent = this.GetUserFrameForAgent(this.UserAgent);
          this.UserAgent.SetTargetPositionAndDirection(userFrameForAgent.Origin.AsVec2, userFrameForAgent.Rotation.f);
        }
        else if (this.LockUserPositions)
          this.UserAgent.SetTargetPosition(this.GetUserFrameForAgent(this.UserAgent).Origin.AsVec2);
      }
      if (this.MovingAgents.Count <= 0)
        return;
      foreach (Agent key in this.MovingAgents.Where<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>((Func<KeyValuePair<Agent, UsableMissionObject.MoveInfo>, bool>) (ma => !ma.Key.IsActive())).Select<KeyValuePair<Agent, UsableMissionObject.MoveInfo>, Agent>((Func<KeyValuePair<Agent, UsableMissionObject.MoveInfo>, Agent>) (ma => ma.Key)).ToList<Agent>())
      {
        this._movingAgents.Remove(key);
        this.SetScriptComponentToTick(this.GetTickRequirement());
      }
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnEditorTick(dt);
    }

    protected internal override void OnEditorValidate()
    {
      base.OnEditorValidate();
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnEditorValidate();
      string errorMessage = (string) null;
      if (this.VerifyChildEntities(ref errorMessage))
        return;
      MBDebug.ShowWarning(errorMessage);
    }

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      foreach (UsableMissionObjectComponent component in this._components)
        component.OnRemoved();
    }

    public virtual GameEntity InteractionEntity => this.GameEntity;

    public virtual WorldFrame GetUserFrameForAgent(Agent agent) => this.GameEntityWithWorldPosition.WorldFrame;

    public override string ToString()
    {
      string str = this.GetType().ToString() + " with Components:";
      foreach (UsableMissionObjectComponent component in this._components)
        str = str + "[" + (object) component + "]";
      return str;
    }

    public bool HasAIUser => this.HasUser && this.UserAgent.IsAIControlled;

    public bool HasUser => this.UserAgent != null;

    public bool HasAIMovingTo => this._movingAgents.Count > 0;

    public bool IsAIMovingTo(Agent agent) => this._movingAgents.ContainsKey(agent);

    public virtual bool HasUserPositionsChanged(Agent agent) => (this.LockUserFrames || this.LockUserPositions) && this.GameEntity.HasFrameChanged;

    public virtual bool DisableCombatActionsOnUse => !this.IsInstantUse;

    protected internal virtual bool LockUserFrames { get; set; }

    protected internal virtual bool LockUserPositions { get; set; }

    public override void WriteToNetwork()
    {
      base.WriteToNetwork();
      GameNetworkMessage.WriteBoolToPacket(this.IsDeactivated);
      GameNetworkMessage.WriteBoolToPacket(this.IsDisabledForPlayers);
      GameNetworkMessage.WriteBoolToPacket(this.UserAgent != null);
      if (this.UserAgent == null)
        return;
      GameNetworkMessage.WriteAgentReferenceToPacket(this.UserAgent);
    }

    public override bool ReadFromNetwork()
    {
      bool bufferReadValid = base.ReadFromNetwork();
      bool flag1 = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      bool flag2 = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      Agent userAgent = (Agent) null;
      if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
        userAgent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      if (bufferReadValid)
      {
        this.IsDeactivated = flag1;
        this.IsDisabledForPlayers = flag2;
        if (userAgent != null)
          this.SetUserForClient(userAgent);
      }
      return bufferReadValid;
    }

    public virtual bool IsUsableByAgent(Agent userAgent) => true;

    public bool IsVisible
    {
      get => this.GameEntity.IsVisibleIncludeParents();
      set
      {
        this.GameEntity.SetVisibilityExcludeParents(value);
        foreach (UsableMissionObjectComponent component in this._components)
        {
          if (component is IVisible)
            ((IVisible) component).IsVisible = value;
        }
      }
    }

    public override void OnEndMission()
    {
      this.UserAgent = (Agent) null;
      this._movingAgents.Clear();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public abstract string GetDescriptionText(GameEntity gameEntity = null);

    public class MoveInfo
    {
      private bool _waypointChangedRecently;
      private int _currentWaypoint;

      public int CurrentWaypoint
      {
        get => this._currentWaypoint;
        set
        {
          if (this._currentWaypoint == value)
            return;
          this._currentWaypoint = value;
          this._waypointChangedRecently = true;
        }
      }

      public bool HasPositionsChanged
      {
        get
        {
          if (!this._waypointChangedRecently)
            return false;
          this._waypointChangedRecently = false;
          return true;
        }
      }
    }
  }
}
