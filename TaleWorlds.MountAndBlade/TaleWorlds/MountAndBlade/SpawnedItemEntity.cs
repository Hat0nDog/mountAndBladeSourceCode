// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SpawnedItemEntity
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class SpawnedItemEntity : UsableMissionObject
  {
    private static readonly ActionIndexCache act_pickup_down_begin = ActionIndexCache.Create(nameof (act_pickup_down_begin));
    private static readonly ActionIndexCache act_pickup_down_end = ActionIndexCache.Create(nameof (act_pickup_down_end));
    private static readonly ActionIndexCache act_pickup_down_begin_left_stance = ActionIndexCache.Create(nameof (act_pickup_down_begin_left_stance));
    private static readonly ActionIndexCache act_pickup_down_end_left_stance = ActionIndexCache.Create(nameof (act_pickup_down_end_left_stance));
    private static readonly ActionIndexCache act_pickup_down_left_begin = ActionIndexCache.Create(nameof (act_pickup_down_left_begin));
    private static readonly ActionIndexCache act_pickup_down_left_end = ActionIndexCache.Create(nameof (act_pickup_down_left_end));
    private static readonly ActionIndexCache act_pickup_down_left_begin_left_stance = ActionIndexCache.Create(nameof (act_pickup_down_left_begin_left_stance));
    private static readonly ActionIndexCache act_pickup_down_left_end_left_stance = ActionIndexCache.Create(nameof (act_pickup_down_left_end_left_stance));
    private static readonly ActionIndexCache act_pickup_middle_begin = ActionIndexCache.Create(nameof (act_pickup_middle_begin));
    private static readonly ActionIndexCache act_pickup_middle_end = ActionIndexCache.Create(nameof (act_pickup_middle_end));
    private static readonly ActionIndexCache act_pickup_middle_begin_left_stance = ActionIndexCache.Create(nameof (act_pickup_middle_begin_left_stance));
    private static readonly ActionIndexCache act_pickup_middle_end_left_stance = ActionIndexCache.Create(nameof (act_pickup_middle_end_left_stance));
    private static readonly ActionIndexCache act_pickup_middle_left_begin = ActionIndexCache.Create(nameof (act_pickup_middle_left_begin));
    private static readonly ActionIndexCache act_pickup_middle_left_end = ActionIndexCache.Create(nameof (act_pickup_middle_left_end));
    private static readonly ActionIndexCache act_pickup_middle_left_begin_left_stance = ActionIndexCache.Create(nameof (act_pickup_middle_left_begin_left_stance));
    private static readonly ActionIndexCache act_pickup_middle_left_end_left_stance = ActionIndexCache.Create(nameof (act_pickup_middle_left_end_left_stance));
    private static readonly ActionIndexCache act_pickup_up_begin = ActionIndexCache.Create(nameof (act_pickup_up_begin));
    private static readonly ActionIndexCache act_pickup_up_end = ActionIndexCache.Create(nameof (act_pickup_up_end));
    private static readonly ActionIndexCache act_pickup_up_begin_left_stance = ActionIndexCache.Create(nameof (act_pickup_up_begin_left_stance));
    private static readonly ActionIndexCache act_pickup_up_end_left_stance = ActionIndexCache.Create(nameof (act_pickup_up_end_left_stance));
    private static readonly ActionIndexCache act_pickup_up_left_begin = ActionIndexCache.Create(nameof (act_pickup_up_left_begin));
    private static readonly ActionIndexCache act_pickup_up_left_end = ActionIndexCache.Create(nameof (act_pickup_up_left_end));
    private static readonly ActionIndexCache act_pickup_up_left_begin_left_stance = ActionIndexCache.Create(nameof (act_pickup_up_left_begin_left_stance));
    private static readonly ActionIndexCache act_pickup_up_left_end_left_stance = ActionIndexCache.Create(nameof (act_pickup_up_left_end_left_stance));
    private static readonly ActionIndexCache act_pickup_from_right_down_horseback_begin = ActionIndexCache.Create(nameof (act_pickup_from_right_down_horseback_begin));
    private static readonly ActionIndexCache act_pickup_from_right_down_horseback_end = ActionIndexCache.Create(nameof (act_pickup_from_right_down_horseback_end));
    private static readonly ActionIndexCache act_pickup_from_right_down_horseback_left_begin = ActionIndexCache.Create(nameof (act_pickup_from_right_down_horseback_left_begin));
    private static readonly ActionIndexCache act_pickup_from_right_down_horseback_left_end = ActionIndexCache.Create(nameof (act_pickup_from_right_down_horseback_left_end));
    private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_begin = ActionIndexCache.Create(nameof (act_pickup_from_right_middle_horseback_begin));
    private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_end = ActionIndexCache.Create(nameof (act_pickup_from_right_middle_horseback_end));
    private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_left_begin = ActionIndexCache.Create(nameof (act_pickup_from_right_middle_horseback_left_begin));
    private static readonly ActionIndexCache act_pickup_from_right_middle_horseback_left_end = ActionIndexCache.Create(nameof (act_pickup_from_right_middle_horseback_left_end));
    private static readonly ActionIndexCache act_pickup_from_right_up_horseback_begin = ActionIndexCache.Create(nameof (act_pickup_from_right_up_horseback_begin));
    private static readonly ActionIndexCache act_pickup_from_right_up_horseback_end = ActionIndexCache.Create(nameof (act_pickup_from_right_up_horseback_end));
    private static readonly ActionIndexCache act_pickup_from_right_up_horseback_left_begin = ActionIndexCache.Create(nameof (act_pickup_from_right_up_horseback_left_begin));
    private static readonly ActionIndexCache act_pickup_from_right_up_horseback_left_end = ActionIndexCache.Create(nameof (act_pickup_from_right_up_horseback_left_end));
    private static readonly ActionIndexCache act_pickup_from_left_down_horseback_begin = ActionIndexCache.Create(nameof (act_pickup_from_left_down_horseback_begin));
    private static readonly ActionIndexCache act_pickup_from_left_down_horseback_end = ActionIndexCache.Create(nameof (act_pickup_from_left_down_horseback_end));
    private static readonly ActionIndexCache act_pickup_from_left_down_horseback_left_begin = ActionIndexCache.Create(nameof (act_pickup_from_left_down_horseback_left_begin));
    private static readonly ActionIndexCache act_pickup_from_left_down_horseback_left_end = ActionIndexCache.Create(nameof (act_pickup_from_left_down_horseback_left_end));
    private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_begin = ActionIndexCache.Create(nameof (act_pickup_from_left_middle_horseback_begin));
    private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_end = ActionIndexCache.Create(nameof (act_pickup_from_left_middle_horseback_end));
    private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_left_begin = ActionIndexCache.Create(nameof (act_pickup_from_left_middle_horseback_left_begin));
    private static readonly ActionIndexCache act_pickup_from_left_middle_horseback_left_end = ActionIndexCache.Create(nameof (act_pickup_from_left_middle_horseback_left_end));
    private static readonly ActionIndexCache act_pickup_from_left_up_horseback_begin = ActionIndexCache.Create(nameof (act_pickup_from_left_up_horseback_begin));
    private static readonly ActionIndexCache act_pickup_from_left_up_horseback_end = ActionIndexCache.Create(nameof (act_pickup_from_left_up_horseback_end));
    private static readonly ActionIndexCache act_pickup_from_left_up_horseback_left_begin = ActionIndexCache.Create(nameof (act_pickup_from_left_up_horseback_left_begin));
    private static readonly ActionIndexCache act_pickup_from_left_up_horseback_left_end = ActionIndexCache.Create(nameof (act_pickup_from_left_up_horseback_left_end));
    private static readonly ActionIndexCache act_pickup_boulder_begin = ActionIndexCache.Create(nameof (act_pickup_boulder_begin));
    private static readonly ActionIndexCache act_pickup_boulder_end = ActionIndexCache.Create(nameof (act_pickup_boulder_end));
    private MissionWeapon _weapon;
    private bool _hasLifeTime;
    public string WeaponName = "";
    private const float LongLifeTime = 180f;
    private const float DisablePhysicsTime = 10f;
    private const float QuickFadeoutLifeTime = 5f;
    private const float TotalFadeOutInDuration = 0.5f;
    private const float PreventStationaryCheckTime = 1f;
    private Timer _disablePhysicsTimer;
    private bool _physicsStopped;
    private bool _readyToBeDeleted;
    private Timer _deletionTimer;
    private int _usedChannelIndex;
    private ActionIndexCache _progressActionIndex;
    private ActionIndexCache _successActionIndex;
    private float _lastSoundPlayTime;
    private const float MinSoundDelay = 0.333f;
    private SoundEvent _rollingSoundEvent;
    private SpawnedItemEntity.ClientSyncData _clientSyncData;
    private GameEntity _ownerGameEntity;
    private GameEntity _groundEntityWhenDisabled;

    public MissionWeapon WeaponCopy => this._weapon;

    public bool HasLifeTime
    {
      get => this._hasLifeTime;
      set
      {
        if (this._hasLifeTime == value)
          return;
        this._hasLifeTime = value;
        this.SetScriptComponentToTick(this.GetTickRequirement());
      }
    }

    private bool PhysicsStopped
    {
      get => this._physicsStopped;
      set
      {
        if (this._physicsStopped == value)
          return;
        this._physicsStopped = value;
        this.SetScriptComponentToTick(this.GetTickRequirement());
      }
    }

    public TextObject GetActionMessage(ItemObject weaponToReplaceWith, bool fillUp)
    {
      GameTexts.SetVariable("USE_KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      MBTextManager.SetTextVariable("KEY", GameTexts.FindText("str_ui_agent_interaction_use"), false);
      if (weaponToReplaceWith == null)
      {
        MBTextManager.SetTextVariable("ACTION", fillUp ? GameTexts.FindText("str_ui_fill") : GameTexts.FindText("str_ui_equip"), false);
      }
      else
      {
        MBTextManager.SetTextVariable("ACTION", GameTexts.FindText("str_ui_swap"), false);
        MBTextManager.SetTextVariable("ITEM_NAME", weaponToReplaceWith.Name, false);
      }
      return GameTexts.FindText("str_key_action");
    }

    public TextObject GetDescriptionMessage(bool fillUp) => !fillUp ? this.WeaponCopy.GetModifiedItemName() : GameTexts.FindText("str_inventory_weapon", ((int) this.WeaponCopy.CurrentUsageItem.WeaponClass).ToString());

    protected internal override bool LockUserFrames => false;

    public Mission.WeaponSpawnFlags SpawnFlags { get; private set; }

    public void Initialize(
      MissionWeapon weapon,
      bool hasLifeTime,
      Mission.WeaponSpawnFlags spawnFlags)
    {
      this._weapon = weapon;
      this.HasLifeTime = hasLifeTime;
      this.SpawnFlags = spawnFlags;
      if (this.HasLifeTime)
      {
        float duration = 0.0f;
        MissionWeapon weaponCopy = this.WeaponCopy;
        if (!weaponCopy.IsEmpty)
        {
          weaponCopy = this.WeaponCopy;
          duration = weaponCopy.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.QuickFadeOut) ? 5f : 180f;
          weaponCopy = this.WeaponCopy;
          this.IsDeactivated = weaponCopy.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.CannotBePickedUp);
          weaponCopy = this.WeaponCopy;
          this._lastSoundPlayTime = !weaponCopy.CurrentUsageItem.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.RangedWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.Consumable) ? -0.333f : 0.333f;
        }
        else
          this.IsDeactivated = true;
        this._deletionTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), duration);
      }
      else
        this._deletionTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), float.MaxValue);
      if (!spawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithPhysics))
        return;
      this._disablePhysicsTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 10f);
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this._ownerGameEntity = this.GameEntity;
      if (!string.IsNullOrEmpty(this.WeaponName))
        this._weapon = new MissionWeapon(Game.Current.ObjectManager.GetObject<ItemObject>(this.WeaponName), (ItemModifier) null, (Banner) null);
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement()
    {
      if (GameNetwork.IsClientOrReplay || this.HasUser || !this.PhysicsStopped)
        return ScriptComponentBehaviour.TickRequirement.Tick;
      if (!this.HasLifeTime)
        return base.GetTickRequirement();
      return base.GetTickRequirement() != ScriptComponentBehaviour.TickRequirement.Tick ? ScriptComponentBehaviour.TickRequirement.TickOccasionally : ScriptComponentBehaviour.TickRequirement.Tick;
    }

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!GameNetwork.IsClientOrReplay)
      {
        if (this.HasUser)
        {
          ActionIndexCache currentAction = this.UserAgent.GetCurrentAction(this._usedChannelIndex);
          if (currentAction == this._successActionIndex)
            this.UserAgent.StopUsingGameObject(this.UserAgent.CanUseObject((UsableMissionObject) this));
          else if (currentAction != this._progressActionIndex)
            this.UserAgent.StopUsingGameObject(false);
        }
        else if (this.HasLifeTime && this._deletionTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
          this._readyToBeDeleted = true;
        if (this.PhysicsStopped)
          return;
        if ((NativeObject) this._ownerGameEntity != (NativeObject) null)
        {
          Vec3 globalPosition = this._ownerGameEntity.GlobalPosition;
          if ((double) globalPosition.z <= (double) CompressionBasic.PositionCompressionInfo.GetMinimumValue() + 5.0)
            this._readyToBeDeleted = true;
          if (this._ownerGameEntity.BodyFlag.HasAnyFlag<BodyFlags>(BodyFlags.Dynamic))
          {
            if (!this._disablePhysicsTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)) && ((double) this._disablePhysicsTimer.ElapsedTime() <= 1.0 || !this._ownerGameEntity.IsDynamicBodyStationary()))
              return;
            this._ownerGameEntity.Scene.RayCastForClosestEntityOrTerrain(globalPosition + new Vec3(z: 0.2f), globalPosition - new Vec3(z: 0.2f), out float _, out this._groundEntityWhenDisabled, excludeBodyFlags: BodyFlags.CommonCollisionExcludeFlags);
            this._groundEntityWhenDisabled = this._groundEntityWhenDisabled?.GetFirstScriptOfTypeInFamily<MissionObject>()?.GameEntity;
            if ((NativeObject) this._groundEntityWhenDisabled != (NativeObject) null)
              this._groundEntityWhenDisabled.AddChild(this.GameEntity, true);
            if (!this.WeaponCopy.IsEmpty && !this._ownerGameEntity.BodyFlag.HasAnyFlag<BodyFlags>(BodyFlags.Disabled))
              this._ownerGameEntity.DisableDynamicBodySimulation();
            else
              this._ownerGameEntity.RemovePhysics();
            this.ClampEntityPositionForStoppingIfNeeded();
            this.PhysicsStopped = true;
            if (this.IsDeactivated && !((NativeObject) this._groundEntityWhenDisabled != (NativeObject) null) || (this.WeaponCopy.IsEmpty || !GameNetwork.IsServerOrRecorder))
              return;
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage) new StopPhysicsAndSetFrameOfMissionObject(this.Id, this._groundEntityWhenDisabled?.GetFirstScriptOfType<MissionObject>(), this._ownerGameEntity.GetFrame()));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
          }
          else
            this.PhysicsStopped = true;
        }
        else
          this.PhysicsStopped = true;
      }
      else
      {
        if (this._clientSyncData == null)
          return;
        if (this._clientSyncData.Timer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        {
          this._ownerGameEntity.SetAlpha(1f);
          this._clientSyncData = (SpawnedItemEntity.ClientSyncData) null;
        }
        else
        {
          float duration = this._clientSyncData.Timer.Duration;
          float num = MBMath.ClampFloat(this._clientSyncData.Timer.ElapsedTime() / duration, 0.0f, 1f);
          if ((double) num < (1.0 - 0.100000001490116 / (double) duration) * 0.5)
            this._ownerGameEntity.SetAlpha((float) (1.0 - (double) num * 2.0));
          else if ((double) num < (1.0 + 0.100000001490116 / (double) duration) * 0.5)
          {
            this._ownerGameEntity.SetAlpha(0.0f);
            this._ownerGameEntity.SetGlobalFrame(this._clientSyncData.Frame);
            this._clientSyncData.Parent?.AddChild(this._ownerGameEntity, true);
            this._clientSyncData.Timer.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission) - (float) ((double) duration * (1.0 + 0.100000001490116 / (double) duration) * 0.5));
          }
          else
            this._ownerGameEntity.SetAlpha((float) ((double) num * 2.0 - 1.0));
        }
      }
    }

    protected internal override void OnTickOccasionally(float currentFrameDeltaTime) => this.OnTick(currentFrameDeltaTime);

    private void ClampEntityPositionForStoppingIfNeeded()
    {
      GameEntity gameEntity = this.GameEntity;
      float minimumValue = CompressionBasic.PositionCompressionInfo.GetMinimumValue();
      float maximumValue = CompressionBasic.PositionCompressionInfo.GetMaximumValue();
      Vec3 position = gameEntity.GetFrame().origin;
      bool valueClamped;
      position = position.ClampedCopy(minimumValue, maximumValue, out valueClamped);
      if (!valueClamped)
        return;
      gameEntity.SetLocalPosition(position);
    }

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      if (this.HasUser && !GameNetwork.IsClientOrReplay)
        this.UserAgent.StopUsingGameObject(false);
      this.InvalidateWeakPointersIfValid();
      this._ownerGameEntity = (GameEntity) null;
    }

    public void AttachWeaponToWeapon(MissionWeapon attachedWeapon, ref MatrixFrame attachLocalFrame) => this._weapon.AttachWeapon(attachedWeapon, ref attachLocalFrame);

    public bool IsReadyToBeDeleted()
    {
      if (!this.HasUser && this._readyToBeDeleted || (NativeObject) this._groundEntityWhenDisabled != (NativeObject) null && (NativeObject) this._groundEntityWhenDisabled.Scene == (NativeObject) null)
        return true;
      if (!((NativeObject) this._groundEntityWhenDisabled != (NativeObject) null) || this._groundEntityWhenDisabled.IsVisibleIncludeParents())
        return false;
      return !this._groundEntityWhenDisabled.HasBody() || this._groundEntityWhenDisabled.BodyFlag.HasAnyFlag<BodyFlags>(BodyFlags.Disabled);
    }

    public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
    {
      base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
      if (!isSuccessful)
        return;
      if (this._clientSyncData != null)
      {
        this._clientSyncData = (SpawnedItemEntity.ClientSyncData) null;
        this.GameEntity.SetAlpha(1f);
      }
      bool removeWeapon;
      userAgent.OnItemPickup(this, (EquipmentIndex) preferenceIndex, out removeWeapon);
      if (!removeWeapon)
        return;
      this._readyToBeDeleted = true;
      this.PhysicsStopped = true;
      this.IsDeactivated = true;
    }

    public override void OnUse(Agent userAgent)
    {
      base.OnUse(userAgent);
      if (GameNetwork.IsClientOrReplay)
        return;
      float z = this.GameEntity.GetGlobalFrame().origin.z;
      float eyeGlobalHeight = userAgent.GetEyeGlobalHeight();
      bool isLeftStance = userAgent.GetIsLeftStance();
      ItemObject.ItemTypeEnum itemType = this.WeaponCopy.Item.ItemType;
      if (userAgent.HasMount)
      {
        this._usedChannelIndex = 1;
        MatrixFrame frame = userAgent.Frame;
        bool flag = (double) Vec2.DotProduct(frame.rotation.f.AsVec2.LeftVec(), (this.GameEntity.GetGlobalFrame().origin - frame.origin).AsVec2) > 0.0;
        if ((double) z < (double) eyeGlobalHeight * 0.699999988079071 + (double) userAgent.Position.z)
        {
          if (this.WeaponCopy.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
          {
            this._progressActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_down_horseback_left_begin : SpawnedItemEntity.act_pickup_from_right_down_horseback_left_begin;
            this._successActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_down_horseback_left_end : SpawnedItemEntity.act_pickup_from_right_down_horseback_left_end;
          }
          else
          {
            this._progressActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_down_horseback_begin : SpawnedItemEntity.act_pickup_from_right_down_horseback_begin;
            this._successActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_down_horseback_end : SpawnedItemEntity.act_pickup_from_right_down_horseback_end;
          }
        }
        else if ((double) z < (double) eyeGlobalHeight * 1.10000002384186 + (double) userAgent.Position.z)
        {
          if (this.WeaponCopy.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
          {
            this._progressActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_middle_horseback_left_begin : SpawnedItemEntity.act_pickup_from_right_middle_horseback_left_begin;
            this._successActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_middle_horseback_left_end : SpawnedItemEntity.act_pickup_from_right_middle_horseback_left_end;
          }
          else
          {
            this._progressActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_middle_horseback_begin : SpawnedItemEntity.act_pickup_from_right_middle_horseback_begin;
            this._successActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_middle_horseback_end : SpawnedItemEntity.act_pickup_from_right_middle_horseback_end;
          }
        }
        else if (this.WeaponCopy.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
        {
          this._progressActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_up_horseback_left_begin : SpawnedItemEntity.act_pickup_from_right_up_horseback_left_begin;
          this._successActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_up_horseback_left_end : SpawnedItemEntity.act_pickup_from_right_up_horseback_left_end;
        }
        else
        {
          this._progressActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_up_horseback_begin : SpawnedItemEntity.act_pickup_from_right_up_horseback_begin;
          this._successActionIndex = flag ? SpawnedItemEntity.act_pickup_from_left_up_horseback_end : SpawnedItemEntity.act_pickup_from_right_up_horseback_end;
        }
      }
      else if (this.WeaponCopy.CurrentUsageItem.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.RangedWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.Consumable))
      {
        this._usedChannelIndex = 0;
        this._progressActionIndex = SpawnedItemEntity.act_pickup_boulder_begin;
        this._successActionIndex = SpawnedItemEntity.act_pickup_boulder_end;
      }
      else if ((double) z < (double) eyeGlobalHeight * 0.5 + (double) userAgent.Position.z)
      {
        this._usedChannelIndex = 0;
        if (this.WeaponCopy.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
        {
          this._progressActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_down_left_begin_left_stance : SpawnedItemEntity.act_pickup_down_left_begin;
          this._successActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_down_left_end_left_stance : SpawnedItemEntity.act_pickup_down_left_end;
        }
        else
        {
          this._progressActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_down_begin_left_stance : SpawnedItemEntity.act_pickup_down_begin;
          this._successActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_down_end_left_stance : SpawnedItemEntity.act_pickup_down_end;
        }
      }
      else if ((double) z < (double) eyeGlobalHeight * 1.10000002384186 + (double) userAgent.Position.z)
      {
        this._usedChannelIndex = 1;
        if (this.WeaponCopy.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
        {
          this._progressActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_middle_left_begin_left_stance : SpawnedItemEntity.act_pickup_middle_left_begin;
          this._successActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_middle_left_end_left_stance : SpawnedItemEntity.act_pickup_middle_left_end;
        }
        else
        {
          this._progressActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_middle_begin_left_stance : SpawnedItemEntity.act_pickup_middle_begin;
          this._successActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_middle_end_left_stance : SpawnedItemEntity.act_pickup_middle_end;
        }
      }
      else
      {
        this._usedChannelIndex = 1;
        if (this.WeaponCopy.Item.ItemFlags.HasAnyFlag<ItemFlags>(ItemFlags.AttachmentMask) || itemType == ItemObject.ItemTypeEnum.Bow || itemType == ItemObject.ItemTypeEnum.Shield)
        {
          this._progressActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_up_left_begin_left_stance : SpawnedItemEntity.act_pickup_up_left_begin;
          this._successActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_up_left_end_left_stance : SpawnedItemEntity.act_pickup_up_left_end;
        }
        else
        {
          this._progressActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_up_begin_left_stance : SpawnedItemEntity.act_pickup_up_begin;
          this._successActionIndex = isLeftStance ? SpawnedItemEntity.act_pickup_up_end_left_stance : SpawnedItemEntity.act_pickup_up_end;
        }
      }
      userAgent.SetActionChannel(this._usedChannelIndex, this._progressActionIndex);
    }

    public override bool IsDisabledForAgent(Agent agent) => this.WeaponCopy.IsAnyConsumable(out WeaponComponentData _) && this.WeaponCopy.Amount == (short) 0;

    protected internal override void OnPhysicsCollision(ref PhysicsContact contact)
    {
      if (GameNetwork.IsDedicatedServer || contact.NumberOfContactPairs <= 0)
        return;
      PhysicsContactInfo physicsContactInfo = new PhysicsContactInfo();
      bool flag = false;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      for (int index1 = 0; index1 < contact.NumberOfContactPairs; ++index1)
      {
        for (int index2 = 0; index2 < contact[index1].NumberOfContacts; ++index2)
        {
          if (!flag || (double) contact[index1][index2].Impulse.LengthSquared > (double) physicsContactInfo.Impulse.LengthSquared)
          {
            physicsContactInfo = contact[index1][index2];
            flag = true;
          }
        }
        switch (contact[index1].ContactEventType)
        {
          case PhysicsEventType.CollisionStart:
            ++num1;
            break;
          case PhysicsEventType.CollisionStay:
            ++num2;
            break;
          case PhysicsEventType.CollisionEnd:
            ++num3;
            break;
        }
      }
      if (num2 > 0)
      {
        this.PlayPhysicsRollSound(physicsContactInfo.Impulse, physicsContactInfo.Position, physicsContactInfo.PhysicsMaterial1);
      }
      else
      {
        if (num1 <= 0)
          return;
        this.PlayPhysicsCollisionSound(physicsContactInfo.Impulse, physicsContactInfo.PhysicsMaterial1, physicsContactInfo.Position);
      }
    }

    private void PlayPhysicsCollisionSound(
      Vec3 impulse,
      PhysicsMaterial collidedMat,
      Vec3 collisionPoint)
    {
      float num1 = this._deletionTimer.ElapsedTime();
      if (((double) impulse.LengthSquared <= 0.00250000017695129 ? 0 : ((double) this._lastSoundPlayTime + 0.333000004291534 < (double) num1 ? 1 : 0)) == 0)
        return;
      this._lastSoundPlayTime = num1;
      WeaponClass weaponClass = this.WeaponCopy.CurrentUsageItem.WeaponClass;
      float length = impulse.Length;
      bool flag = false;
      int num2;
      int num3;
      int num4;
      switch (weaponClass)
      {
        case WeaponClass.Dagger:
        case WeaponClass.ThrowingAxe:
        case WeaponClass.ThrowingKnife:
          num2 = ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeDefault;
          num3 = ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeWood;
          num4 = ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeStone;
          break;
        case WeaponClass.OneHandedSword:
        case WeaponClass.OneHandedAxe:
        case WeaponClass.Mace:
          num2 = ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeDefault;
          num3 = ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeWood;
          num4 = ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeStone;
          break;
        case WeaponClass.TwoHandedSword:
        case WeaponClass.TwoHandedAxe:
        case WeaponClass.TwoHandedMace:
          num2 = ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeDefault;
          num3 = ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeWood;
          num4 = ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeStone;
          break;
        case WeaponClass.OneHandedPolearm:
        case WeaponClass.TwoHandedPolearm:
        case WeaponClass.LowGripPolearm:
          num2 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeDefault;
          num3 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeWood;
          num4 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeStone;
          break;
        case WeaponClass.Arrow:
        case WeaponClass.Bolt:
          num2 = ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeDefault;
          num3 = ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeWood;
          num4 = ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeStone;
          break;
        case WeaponClass.Bow:
        case WeaponClass.Crossbow:
        case WeaponClass.Javelin:
          num2 = ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeDefault;
          num3 = ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeWood;
          num4 = ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeStone;
          break;
        case WeaponClass.Stone:
          num2 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeDefault;
          num3 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeWood;
          num4 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeStone;
          break;
        case WeaponClass.Boulder:
          num2 = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderDefault;
          num3 = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderWood;
          num4 = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderStone;
          flag = true;
          break;
        case WeaponClass.SmallShield:
        case WeaponClass.LargeShield:
          num2 = ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeDefault;
          num3 = ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeWood;
          num4 = ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeStone;
          break;
        default:
          num2 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeDefault;
          num3 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeWood;
          num4 = ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeStone;
          break;
      }
      float num5 = flag ? MBMath.ClampFloat((float) (((double) length - 7.0) * 0.030303031206131 * 0.100000001490116 + 0.899999976158142), 0.9f, 1f) : MBMath.ClampFloat(length * 0.1666667f, 0.0f, 1f);
      string name = collidedMat.Name;
      int soundIndex = num2;
      if (name.Contains("wood"))
        soundIndex = num3;
      else if (name.Contains("stone"))
        soundIndex = num4;
      SoundEventParameter parameter = new SoundEventParameter("Force", num5);
      Mission.Current.MakeSound(soundIndex, collisionPoint, true, false, -1, -1, ref parameter);
    }

    private void PlayPhysicsRollSound(
      Vec3 impulse,
      Vec3 collisionPoint,
      PhysicsMaterial collidedMat)
    {
      WeaponComponentData currentUsageItem = this.WeaponCopy.CurrentUsageItem;
      if (currentUsageItem.WeaponClass != WeaponClass.Boulder || !currentUsageItem.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.RangedWeapon | WeaponFlags.NotUsableWithOneHand | WeaponFlags.Consumable))
        return;
      float num = this._deletionTimer.ElapsedTime();
      if (((double) impulse.LengthSquared <= 9.99999974737875E-05 ? 0 : ((double) this._lastSoundPlayTime + 0.333000004291534 < (double) num ? 1 : 0)) == 0)
        return;
      if (this._rollingSoundEvent == null || !this._rollingSoundEvent.IsValid)
      {
        this._lastSoundPlayTime = num;
        int soundCodeId = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderDefault;
        string name = collidedMat.Name;
        if (name.Contains("stone"))
          soundCodeId = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderStone;
        else if (name.Contains("wood"))
          soundCodeId = ItemPhysicsSoundContainer.SoundCodePhysicsBoulderWood;
        this._rollingSoundEvent = SoundEvent.CreateEvent(soundCodeId, Mission.Current.Scene);
        this._rollingSoundEvent.PlayInPosition(collisionPoint);
      }
      this._rollingSoundEvent.SetParameter("Force", MBMath.ClampFloat(impulse.Length * 0.03333334f, 0.0f, 1f));
      this._rollingSoundEvent.SetPosition(collisionPoint);
    }

    public bool IsStuckMissile() => this.SpawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.AsMissile);

    public bool IsQuiverAndNotEmpty() => this.WeaponCopy.Item.PrimaryWeapon.IsConsumable && this.WeaponCopy.Amount > (short) 0;

    public override TextObject GetInfoTextForBeingNotInteractable(Agent userAgent) => !this.IsDeactivated && this.WeaponCopy.IsAnyConsumable(out WeaponComponentData _) && this.WeaponCopy.Amount == (short) 0 ? GameTexts.FindText("str_ui_empty_quiver") : base.GetInfoTextForBeingNotInteractable(userAgent);

    public void StopPhysicsAndSetFrameForClient(MatrixFrame frame, GameEntity parent)
    {
      if ((NativeObject) parent != (NativeObject) null)
        frame = parent.GetGlobalFrame().TransformToParent(frame);
      frame.rotation.Orthonormalize();
      this._clientSyncData = new SpawnedItemEntity.ClientSyncData();
      this._clientSyncData.Frame = frame;
      this._clientSyncData.Timer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.5f, false);
      this._clientSyncData.Parent = parent;
      if (this.PhysicsStopped)
        return;
      this.PhysicsStopped = true;
      GameEntity gameEntity = this.GameEntity;
      if (!this.WeaponCopy.IsEmpty && !gameEntity.BodyFlag.HasAnyFlag<BodyFlags>(BodyFlags.Disabled))
        gameEntity.DisableDynamicBodySimulation();
      else
        gameEntity.RemovePhysics();
    }

    public void OnSpawnedItemEntityRemoved()
    {
      this.UserAgent?.OnItemRemovedFromScene();
      foreach (Agent key in this.MovingAgents.Keys)
        key.OnItemRemovedFromScene();
    }

    public void ConsumeWeaponAmount(short consumedAmount) => this._weapon.Consume(consumedAmount);

    public override string GetDescriptionText(GameEntity gameEntity = null) => string.Empty;

    public SpawnedItemEntity()
      : base()
    {
    }

    private class ClientSyncData
    {
      public MatrixFrame Frame;
      public GameEntity Parent;
      public Timer Timer;
    }
  }
}
