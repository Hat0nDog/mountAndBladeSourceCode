// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FormationAI
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class FormationAI
  {
    private readonly Formation formation;
    private List<FormationAI.BehaviorData> behaviorData;
    private List<FormationAI.BehaviorData> specialBehaviorData;
    private const float _behaviorPreserveTime = 5f;
    private List<BehaviorComponent> _behaviors = new List<BehaviorComponent>();
    private BehaviorComponent _activeBehavior;
    private FormationAI.BehaviorSide _side = FormationAI.BehaviorSide.Middle;
    private Timer tickTimer;
    private Timer behaviorPreprocessTimer;

    public event Action<Formation> OnActiveBehaviorChanged;

    private List<FormationAI.BehaviorData> allBehaviorData => this.behaviorData.Concat<FormationAI.BehaviorData>((IEnumerable<FormationAI.BehaviorData>) this.specialBehaviorData).ToList<FormationAI.BehaviorData>();

    public BehaviorComponent ActiveBehavior
    {
      get => this._activeBehavior;
      private set
      {
        if (this._activeBehavior == value)
          return;
        if (this._activeBehavior != null)
          this._activeBehavior.OnBehaviorCanceled();
        BehaviorComponent activeBehavior = this._activeBehavior;
        this._activeBehavior = value;
        this._activeBehavior.OnBehaviorActivated();
        this.ActiveBehavior.PreserveExpireTime = MBCommon.GetTime(MBCommon.TimeType.Mission) + 10f;
        if (this.OnActiveBehaviorChanged == null || activeBehavior != null && activeBehavior.Equals((object) value))
          return;
        this.OnActiveBehaviorChanged(this.formation);
      }
    }

    public FormationAI.BehaviorSide Side
    {
      get => this._side;
      set
      {
        this._side = value;
        if (this._side == FormationAI.BehaviorSide.BehaviorSideNotSet)
          return;
        foreach (BehaviorComponent behavior in this._behaviors)
          behavior.OnValidBehaviorSideSet();
      }
    }

    internal bool IsMainFormation { get; set; }

    public FormationAI(Formation formation)
    {
      this.formation = formation;
      float num1 = 0.0f;
      if (formation.Team != null)
      {
        double num2 = 0.100000001490116 * (double) formation.FormationIndex;
        float num3 = 0.0f;
        if (formation.Team.TeamIndex >= 0)
          num3 = (float) ((double) formation.Team.TeamIndex * 0.5 * 0.100000001490116);
        double num4 = (double) num3;
        num1 = (float) (num2 + num4);
      }
      this.tickTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission) + 0.5f * num1, 0.5f);
      this.behaviorData = new List<FormationAI.BehaviorData>();
      this.specialBehaviorData = new List<FormationAI.BehaviorData>();
    }

    public void AddBehavior(BehaviorComponent behaviour)
    {
      this.behaviorData.Add(new FormationAI.BehaviorData()
      {
        Behavior = behaviour
      });
      this.behaviorPreprocessTimer = new Timer(MBCommon.TimeType.Mission.GetTime(), this.tickTimer.Duration / (float) (this.behaviorData.Count + this.specialBehaviorData.Count + 1));
    }

    public T SetBehaviorWeight<T>(float w) where T : BehaviorComponent
    {
      T obj = this.EnsureBehavior<T>();
      obj.WeightFactor = w;
      return obj;
    }

    public T EnsureBehavior<T>() where T : BehaviorComponent => this.FindBehavior<T>() ?? this.AddAiBehavior<T>();

    private T FindBehavior<T>() where T : BehaviorComponent => this._behaviors.Find((Predicate<BehaviorComponent>) (x => x is T)) as T;

    public T AddAiBehavior<T>() where T : BehaviorComponent
    {
      if (!(Activator.CreateInstance(typeof (T), (object) this.formation) is T instance))
        throw new ArgumentException();
      this._behaviors.Add((BehaviorComponent) instance);
      return instance;
    }

    public T GetBehavior<T>() where T : BehaviorComponent
    {
      BehaviorComponent behaviorComponent = this._behaviors.FirstOrDefault<BehaviorComponent>((Func<BehaviorComponent, bool>) (b => b is T));
      if (behaviorComponent != null)
        return (T) behaviorComponent;
      FormationAI.BehaviorData behaviorData = this.behaviorData.FirstOrDefault<FormationAI.BehaviorData>((Func<FormationAI.BehaviorData, bool>) (bd => bd.Behavior is T)) ?? this.specialBehaviorData.FirstOrDefault<FormationAI.BehaviorData>((Func<FormationAI.BehaviorData, bool>) (bd => bd.Behavior is T));
      return behaviorData != null ? (T) behaviorData.Behavior : default (T);
    }

    public void ClearSpecialBehaviors() => this.specialBehaviorData.Clear();

    public void AddSpecialBehavior(
      IEnumerable<BehaviorComponent> behaviours,
      bool purgePreviousSpecialBehaviours = false)
    {
      if (purgePreviousSpecialBehaviours)
        this.specialBehaviorData.Clear();
      foreach (BehaviorComponent behaviour in behaviours)
      {
        this.specialBehaviorData.Add(new FormationAI.BehaviorData()
        {
          Behavior = behaviour
        });
        this.behaviorPreprocessTimer = new Timer(MBCommon.TimeType.Mission.GetTime(), this.tickTimer.Duration / (float) (this.behaviorData.Count + this.specialBehaviorData.Count + 1));
      }
    }

    public void AddSpecialBehavior(BehaviorComponent behaviour, bool purgePreviousSpecialBehaviours = false)
    {
      if (purgePreviousSpecialBehaviours)
        this.specialBehaviorData.Clear();
      this.specialBehaviorData.Add(new FormationAI.BehaviorData()
      {
        Behavior = behaviour
      });
      this.behaviorPreprocessTimer = new Timer(MBCommon.TimeType.Mission.GetTime(), this.tickTimer.Duration / (float) (this.behaviorData.Count + this.specialBehaviorData.Count + 1));
    }

    private bool FindBestBehavior()
    {
      BehaviorComponent behaviorComponent = (BehaviorComponent) null;
      float num1 = float.MinValue;
      foreach (BehaviorComponent behavior in this._behaviors)
      {
        if ((double) behavior.WeightFactor > 1.0000000116861E-07)
        {
          float num2 = behavior.GetAIWeight() * behavior.WeightFactor;
          if (behavior == this.ActiveBehavior)
            num2 *= MBMath.Lerp(1.2f, 2f, MBMath.ClampFloat((float) (((double) behavior.PreserveExpireTime - (double) MBCommon.GetTime(MBCommon.TimeType.Mission)) / 5.0), 0.0f, 1f), float.MinValue);
          if ((double) num2 > (double) num1)
          {
            if ((double) behavior.NavmeshlessTargetPositionPenalty > 0.0)
              num2 /= behavior.NavmeshlessTargetPositionPenalty;
            behavior.PrecalculateMovementOrder();
            float num3 = num2 * behavior.NavmeshlessTargetPositionPenalty;
            if ((double) num3 > (double) num1)
            {
              behaviorComponent = behavior;
              num1 = num3;
            }
          }
        }
      }
      if (behaviorComponent == null)
        return false;
      this.ActiveBehavior = behaviorComponent;
      return true;
    }

    private bool PreprocessBehaviors()
    {
      if (Mission.Current.Teams.GetEnemiesOf(this.formation.Team).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).IsEmpty<Formation>())
        return false;
      FormationAI.BehaviorData behaviorData = this.behaviorData.FirstOrDefault<FormationAI.BehaviorData>((Func<FormationAI.BehaviorData, bool>) (d => !d.IsPreprocessed)) ?? this.specialBehaviorData.FirstOrDefault<FormationAI.BehaviorData>((Func<FormationAI.BehaviorData, bool>) (sd => !sd.IsPreprocessed));
      if (behaviorData == null)
        return false;
      behaviorData.Behavior.TickOccasionally();
      float aiWeight = behaviorData.Behavior.GetAIWeight();
      if (behaviorData.Behavior == this.ActiveBehavior)
        aiWeight *= MBMath.Lerp(1.01f, 1.5f, MBMath.ClampFloat((float) (((double) behaviorData.Behavior.PreserveExpireTime - (double) MBCommon.GetTime(MBCommon.TimeType.Mission)) / 5.0), 0.0f, 1f), float.MinValue);
      behaviorData.Weight = aiWeight * behaviorData.Preference;
      behaviorData.IsPreprocessed = true;
      return true;
    }

    public void Tick()
    {
      if (!Mission.Current.AllowAiTicking || !Mission.Current.ForceTickOccasionally && !this.tickTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        return;
      this.TickOccasionally(this.tickTimer.PreviousDeltaTime);
    }

    private void TickOccasionally(float dt)
    {
      this.formation.IsAITickedAfterSplit = true;
      if (this.FindBestBehavior())
      {
        if (!this.formation.IsAIControlled)
        {
          if (MultiplayerGame.Current != null || Mission.Current.MainAgent == null || (this.formation.Team.IsPlayerGeneral || !this.formation.Team.IsPlayerSergeant) || this.formation.PlayerOwner != Agent.Main)
            return;
          this.ActiveBehavior.RemindSergeantPlayer();
        }
        else
          this.ActiveBehavior.TickOccasionally();
      }
      else
      {
        BehaviorComponent activeBehaviour = this.ActiveBehavior;
        if (!Mission.Current.Teams.GetEnemiesOf(this.formation.Team).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).Any<Formation>())
          return;
        this.PreprocessBehaviors();
        foreach (FormationAI.BehaviorData behaviorData in this.allBehaviorData)
          behaviorData.IsPreprocessed = false;
        this.behaviorData.RemoveAll((Predicate<FormationAI.BehaviorData>) (bd => bd.Behavior != activeBehaviour && bd.IsRemovedOnCancel));
        if (activeBehaviour is BehaviorStop && this.specialBehaviorData.Any<FormationAI.BehaviorData>())
        {
          IEnumerable<FormationAI.BehaviorData> source = this.specialBehaviorData.Where<FormationAI.BehaviorData>((Func<FormationAI.BehaviorData, bool>) (sbd => (double) sbd.Weight > 0.0));
          if (source.Any<FormationAI.BehaviorData>())
            activeBehaviour = source.MaxBy<FormationAI.BehaviorData, float>((Func<FormationAI.BehaviorData, float>) (abd => abd.Weight)).Behavior;
        }
        int num = this.formation.IsAIControlled ? 1 : 0;
        bool flag = false;
        if (this.ActiveBehavior != activeBehaviour)
        {
          BehaviorComponent activeBehavior = this.ActiveBehavior;
          this.ActiveBehavior = activeBehaviour;
          flag = true;
        }
        if (!flag && (activeBehaviour == null || !activeBehaviour.IsCurrentOrderChanged))
          return;
        if (this.formation.IsAIControlled)
          this.formation.MovementOrder = activeBehaviour.CurrentOrder;
        activeBehaviour.IsCurrentOrderChanged = false;
      }
    }

    public void SetBehaviorPreference(
      Func<BehaviorComponent, bool> predicate,
      float preferenceScore = 10f,
      bool resetOtherBehaviourPreferences = true)
    {
      if (resetOtherBehaviourPreferences)
        this.allBehaviorData.ForEach((Action<FormationAI.BehaviorData>) (bd => bd.Preference = 1f));
      foreach (FormationAI.BehaviorData behaviorData in this.allBehaviorData.Where<FormationAI.BehaviorData>((Func<FormationAI.BehaviorData, bool>) (bd => predicate(bd.Behavior))))
        behaviorData.Preference = preferenceScore;
    }

    public void AddSpecificBehavior(
      BehaviorComponent behaviour,
      float preferenceScore = 10f,
      bool resetOtherBehaviourPreferences = true)
    {
      if (resetOtherBehaviourPreferences)
        this.allBehaviorData.ForEach((Action<FormationAI.BehaviorData>) (bd => bd.Preference = 1f));
      this.specialBehaviorData.Add(new FormationAI.BehaviorData()
      {
        Behavior = behaviour,
        Preference = preferenceScore,
        Weight = 0.0f,
        IsRemovedOnCancel = true
      });
    }

    [Conditional("DEBUG")]
    public void DebugMore()
    {
      if (!MBDebug.IsDisplayingHighLevelAI)
        return;
      foreach (FormationAI.BehaviorData behaviorData in (IEnumerable<FormationAI.BehaviorData>) this.allBehaviorData.OrderBy<FormationAI.BehaviorData, string>((Func<FormationAI.BehaviorData, string>) (d => d.Behavior.GetType().ToString())))
      {
        behaviorData.Behavior.GetType().ToString().Replace("MBModule.Behavior", "");
        behaviorData.Weight.ToString("0.00");
        behaviorData.Preference.ToString("0.00");
      }
    }

    [Conditional("DEBUG")]
    public void DebugScores()
    {
      if (this.formation.IsRanged())
        MBDebug.Print("Ranged");
      else if (this.formation.IsCavalry())
        MBDebug.Print("Cavalry");
      else
        MBDebug.Print("Infantry");
      foreach (FormationAI.BehaviorData behaviorData in (IEnumerable<FormationAI.BehaviorData>) this.allBehaviorData.OrderBy<FormationAI.BehaviorData, string>((Func<FormationAI.BehaviorData, string>) (d => d.Behavior.GetType().ToString())))
        MBDebug.Print(behaviorData.Behavior.GetType().ToString().Replace("MBModule.Behavior", "") + " \t\t w:" + behaviorData.Weight.ToString("0.00") + "\t p:" + behaviorData.Preference.ToString("0.00"));
    }

    public void ResetBehaviorWeights()
    {
      foreach (BehaviorComponent behavior in this._behaviors)
        behavior.ResetBehavior();
    }

    public enum BehaviorSide
    {
      Left = 0,
      Middle = 1,
      Right = 2,
      BehaviorSideNotSet = 3,
      ValidBehaviorSideCount = 3,
    }

    internal class BehaviorData
    {
      public BehaviorComponent Behavior;
      public float Preference = 1f;
      public float Weight;
      public bool IsRemovedOnCancel;
      public bool IsPreprocessed;
    }
  }
}
