// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public abstract class BehaviorComponent
  {
    internal Formation formation;
    protected FormationAI.BehaviorSide behaviorSide;
    protected const float FormArrangementDistanceToOrderPosition = 10f;
    private const float _playerInformCooldown = 60f;
    protected float _lastPlayerInformTime;
    private Timer _navmeshlessTargetPenaltyTime;
    private float _navmeshlessTargetPositionPenalty = 1f;
    internal bool IsCurrentOrderChanged;
    private MovementOrder _currentOrder;
    protected FacingOrder CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
    protected bool dontBlend = true;
    protected bool isSideImportant;

    internal float BehaviorCoherence { get; set; }

    internal BehaviorComponent(Formation formation)
    {
      this.formation = formation;
      this.PreserveExpireTime = 0.0f;
      this._navmeshlessTargetPenaltyTime = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 20f);
    }

    protected BehaviorComponent()
    {
    }

    private void InformSergeantPlayer()
    {
      if (Mission.Current.MainAgent == null || this.formation.Team.GeneralAgent == null || (this.formation.Team.IsPlayerGeneral || !this.formation.Team.IsPlayerSergeant) || this.formation.PlayerOwner != Agent.Main)
        return;
      MBTextManager.SetTextVariable("BEHAVIOR", this.GetBehaviorString(), false);
      MBTextManager.SetTextVariable("PLAYER_NAME", Mission.Current.MainAgent.Name, false);
      MBTextManager.SetTextVariable("TEAM_LEADER", this.formation.Team.GeneralAgent.Name, false);
      InformationManager.AddQuickInformation(new TextObject("{=L91XKoMD}{TEAM_LEADER}: {PLAYER_NAME}, {BEHAVIOR}"), 4000, this.formation.Team.GeneralAgent.Character);
    }

    protected virtual void OnBehaviorActivatedAux()
    {
    }

    internal void OnBehaviorActivated()
    {
      if (!this.formation.Team.IsPlayerGeneral && !this.formation.Team.IsPlayerSergeant && (this.formation.IsPlayerInFormation && Mission.Current.MainAgent != null))
      {
        MBTextManager.SetTextVariable("BEHAVIOUR_NAME_BEGIN", new TextObject(this.ToString().Replace("MBModule.Behavior", "")), false);
        InformationManager.AddQuickInformation(GameTexts.FindText("str_formation_ai_soldier_instruction_text"), 2000, Mission.Current.MainAgent.Character);
      }
      if (Game.Current.GameType != MultiplayerGame.Current)
      {
        this.InformSergeantPlayer();
        this._lastPlayerInformTime = MBCommon.GetTime(MBCommon.TimeType.Mission);
      }
      if (!this.formation.IsAIControlled)
        return;
      this.OnBehaviorActivatedAux();
    }

    public virtual void OnBehaviorCanceled()
    {
    }

    internal void RemindSergeantPlayer()
    {
      float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
      if (this != this.formation.AI.ActiveBehavior || (double) this._lastPlayerInformTime + 60.0 >= (double) time)
        return;
      this.InformSergeantPlayer();
      this._lastPlayerInformTime = time;
    }

    protected internal virtual void TickOccasionally()
    {
    }

    protected internal virtual float NavmeshlessTargetPositionPenalty
    {
      get
      {
        if ((double) this._navmeshlessTargetPositionPenalty == 1.0)
          return 1f;
        float num = this._navmeshlessTargetPenaltyTime.ElapsedTime();
        if ((double) num >= 10.0)
        {
          this._navmeshlessTargetPositionPenalty = 1f;
          return 1f;
        }
        return (double) num <= 5.0 ? this._navmeshlessTargetPositionPenalty : MBMath.Lerp(this._navmeshlessTargetPositionPenalty, 1f, (float) (((double) num - 5.0) / 5.0));
      }
      set
      {
        this._navmeshlessTargetPenaltyTime.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission));
        this._navmeshlessTargetPositionPenalty = value;
      }
    }

    internal float GetAIWeight() => this.GetAiWeight() * this.NavmeshlessTargetPositionPenalty;

    protected abstract float GetAiWeight();

    protected internal MovementOrder CurrentOrder
    {
      get => this._currentOrder;
      protected set
      {
        this._currentOrder = value;
        this.IsCurrentOrderChanged = true;
      }
    }

    internal float PreserveExpireTime { get; set; }

    public bool IsSideImportant => this.isSideImportant;

    public float WeightFactor { get; set; }

    internal virtual void ResetBehavior() => this.WeightFactor = 0.0f;

    public virtual TextObject GetBehaviorString() => GameTexts.FindText("str_formation_ai_sergeant_instruction_behavior_text", this.GetType().Name);

    internal virtual void OnValidBehaviorSideSet() => this.behaviorSide = this.formation.AI.Side;

    protected virtual void CalculateCurrentOrder()
    {
    }

    public void PrecalculateMovementOrder()
    {
      this.CalculateCurrentOrder();
      this.CurrentOrder.GetPosition(this.formation);
    }

    public override bool Equals(object obj) => this.GetType() == obj.GetType();

    public override int GetHashCode() => base.GetHashCode();
  }
}
