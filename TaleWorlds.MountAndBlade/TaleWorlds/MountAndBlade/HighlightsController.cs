// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.HighlightsController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class HighlightsController : MissionLogic
  {
    private bool _isKillingSpreeHappening;
    private List<float> _playerKillTimes;
    private const int MinKillingSpreeKills = 4;
    private const float MaxKillingSpreeDuration = 10f;
    private const float HighShotDifficultyThreshold = 7.5f;
    private bool _isArcherSalvoHappening;
    private List<float> _archerSalvoKillTimes;
    private const int MinArcherSalvoKills = 5;
    private const float MaxArcherSalvoDuration = 4f;
    private bool _isFirstImpact = true;
    private List<float> _cavalryChargeHitTimes;
    private const float CavalryChargeImpactTimeFrame = 3f;
    private const int MinCavalryChargeHits = 5;
    private Tuple<float, float> _lastSavedHighlightData;
    private List<HighlightsController.Highlight> _highlightSaveQueue;
    private const float IgnoreIfOverlapsLastVideoPercent = 0.5f;
    private List<string> _savedHighlightGroups;
    private List<string> _highlightGroupIds = new List<string>()
    {
      "grpid_incidents",
      "grpid_achievements"
    };

    protected static List<HighlightsController.HighlightType> HighlightTypes { get; private set; }

    public static bool IsHighlightsInitialized { get; private set; }

    public bool IsAnyHighlightSaved => this._savedHighlightGroups.Count > 0;

    public static void RemoveHighlights()
    {
      if (!HighlightsController.IsHighlightsInitialized)
        return;
      foreach (HighlightsController.HighlightType highlightType in HighlightsController.HighlightTypes)
        Highlights.RemoveHighlight(highlightType.Id);
    }

    public HighlightsController.HighlightType GetHighlightTypeWithId(
      string highlightId)
    {
      return HighlightsController.HighlightTypes.First<HighlightsController.HighlightType>((Func<HighlightsController.HighlightType, bool>) (h => h.Id == highlightId));
    }

    private void SaveVideo(string highlightID, string groupID, int startDelta, int endDelta)
    {
      Highlights.SaveVideo(highlightID, groupID, startDelta, endDelta);
      if (this._savedHighlightGroups.Contains(groupID))
        return;
      this._savedHighlightGroups.Add(groupID);
    }

    public override void AfterStart()
    {
      if (!HighlightsController.IsHighlightsInitialized)
      {
        HighlightsController.HighlightTypes = new List<HighlightsController.HighlightType>()
        {
          new HighlightsController.HighlightType("hlid_killing_spree", "Killing Spree", "grpid_incidents", -2010, 3000, 0.25f, float.MaxValue, true),
          new HighlightsController.HighlightType("hlid_high_ranged_shot_difficulty", "Sharpshooter", "grpid_incidents", -5000, 3000, 0.25f, float.MaxValue, true),
          new HighlightsController.HighlightType("hlid_archer_salvo_kills", "Death from Above", "grpid_incidents", -5004, 3000, 0.5f, 150f, false),
          new HighlightsController.HighlightType("hlid_couched_lance_against_mounted_opponent", "Lance A Lot", "grpid_incidents", -5000, 3000, 0.25f, float.MaxValue, true),
          new HighlightsController.HighlightType("hlid_cavalry_charge_first_impact", "Cavalry Charge First Impact", "grpid_incidents", -5000, 5000, 0.25f, float.MaxValue, false),
          new HighlightsController.HighlightType("hlid_headshot_kill", "Headshot!", "grpid_incidents", -5000, 3000, 0.25f, 150f, true),
          new HighlightsController.HighlightType("hlid_burning_ammunition_kill", "Burn Baby", "grpid_incidents", -5000, 3000, 0.25f, 100f, true),
          new HighlightsController.HighlightType("hlid_throwing_weapon_kill_against_charging_enemy", "Throwing Weapon Kill Against Charging Enemy", "grpid_incidents", -5000, 3000, 0.25f, 150f, true)
        };
        Highlights.Initialize();
        foreach (HighlightsController.HighlightType highlightType in HighlightsController.HighlightTypes)
          Highlights.AddHighlight(highlightType.Id, highlightType.Description);
        HighlightsController.IsHighlightsInitialized = true;
      }
      foreach (string highlightGroupId in this._highlightGroupIds)
        Highlights.OpenGroup(highlightGroupId);
      this._highlightSaveQueue = new List<HighlightsController.Highlight>();
      this._playerKillTimes = new List<float>();
      this._archerSalvoKillTimes = new List<float>();
      this._cavalryChargeHitTimes = new List<float>();
      this._savedHighlightGroups = new List<string>();
    }

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow killingBlow)
    {
      bool flag1 = affectorAgent?.Team != null && affectorAgent.Team.IsPlayerTeam;
      bool flag2 = affectorAgent != null && affectorAgent.IsMainAgent;
      if (affectorAgent == null || affectedAgent == null || (!affectorAgent.IsHuman || !affectedAgent.IsHuman) || agentState != AgentState.Killed && agentState != AgentState.Unconscious)
        return;
      if ((flag2 | flag1 && !affectedAgent.Team.IsPlayerAlly && killingBlow.WeaponClass == 12 || killingBlow.WeaponClass == 13) && this.CanSaveHighlight(this.GetHighlightTypeWithId("hlid_archer_salvo_kills"), affectedAgent.Position))
      {
        if (!this._isArcherSalvoHappening)
          this._archerSalvoKillTimes.RemoveAll((Predicate<float>) (ht => (double) ht + 4.0 < (double) Mission.Current.Time));
        this._archerSalvoKillTimes.Add(Mission.Current.Time);
        if (this._archerSalvoKillTimes.Count >= 5)
          this._isArcherSalvoHappening = true;
      }
      if (flag2 && this.CanSaveHighlight(this.GetHighlightTypeWithId("hlid_killing_spree"), affectedAgent.Position))
      {
        if (!this._isKillingSpreeHappening)
          this._playerKillTimes.RemoveAll((Predicate<float>) (ht => (double) ht + 10.0 < (double) Mission.Current.Time));
        this._playerKillTimes.Add(Mission.Current.Time);
        if (this._playerKillTimes.Count >= 4)
          this._isKillingSpreeHappening = true;
      }
      HighlightsController.Highlight highlight = new HighlightsController.Highlight();
      highlight.Start = Mission.Current.Time;
      highlight.End = Mission.Current.Time;
      bool flag3 = false;
      if (flag2 && killingBlow.WeaponRecordWeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.Burning))
      {
        highlight.HighlightType = this.GetHighlightTypeWithId("hlid_burning_ammunition_kill");
        flag3 = true;
      }
      if (flag2 && killingBlow.IsMissile && killingBlow.BoneIndex == (sbyte) 13)
      {
        highlight.HighlightType = this.GetHighlightTypeWithId("hlid_headshot_kill");
        flag3 = true;
      }
      if (flag2 && killingBlow.IsMissile && (affectedAgent.HasMount && affectedAgent.IsDoingPassiveAttack) && (killingBlow.WeaponClass == 19 || killingBlow.WeaponClass == 20))
      {
        highlight.HighlightType = this.GetHighlightTypeWithId("hlid_throwing_weapon_kill_against_charging_enemy");
        flag3 = true;
      }
      if (this._isFirstImpact && affectorAgent.Formation != null && (affectorAgent.Formation.IsCavalry() && affectorAgent.Formation.MovementOrder == (object) MovementOrder.MovementOrderCharge) && this.CanSaveHighlight(this.GetHighlightTypeWithId("hlid_cavalry_charge_first_impact"), affectedAgent.Position))
      {
        this._cavalryChargeHitTimes.RemoveAll((Predicate<float>) (ht => (double) ht + 3.0 < (double) Mission.Current.Time));
        this._cavalryChargeHitTimes.Add(Mission.Current.Time);
        if (this._cavalryChargeHitTimes.Count >= 5)
        {
          highlight.HighlightType = this.GetHighlightTypeWithId("hlid_cavalry_charge_first_impact");
          highlight.Start = this._cavalryChargeHitTimes[0];
          highlight.End = this._cavalryChargeHitTimes.Last<float>();
          flag3 = true;
          this._isFirstImpact = false;
          this._cavalryChargeHitTimes.Clear();
        }
      }
      if (!flag3)
        return;
      this.SaveHighlight(highlight, affectedAgent.Position);
    }

    public override void OnScoreHit(
      Agent affectedAgent,
      Agent affectorAgent,
      WeaponComponentData attackerWeapon,
      bool isBlocked,
      float damage,
      float movementSpeedDamageModifier,
      float hitDistance,
      AgentAttackType attackType,
      float shotDifficulty,
      BoneBodyPartType victimHitBodyPart)
    {
      bool flag1 = affectorAgent != null && affectorAgent.IsMainAgent;
      if (affectorAgent == null || affectedAgent == null || (!affectorAgent.IsHuman || !affectedAgent.IsHuman))
        return;
      HighlightsController.Highlight highlight = new HighlightsController.Highlight();
      highlight.Start = Mission.Current.Time;
      highlight.End = Mission.Current.Time;
      bool flag2 = false;
      if (flag1 && (double) shotDifficulty >= 7.5)
      {
        highlight.HighlightType = this.GetHighlightTypeWithId("hlid_high_ranged_shot_difficulty");
        flag2 = true;
      }
      if (flag1 && affectedAgent.HasMount && (attackType == AgentAttackType.Standard && affectorAgent.HasMount) && affectorAgent.IsDoingPassiveAttack)
      {
        highlight.HighlightType = this.GetHighlightTypeWithId("hlid_couched_lance_against_mounted_opponent");
        flag2 = true;
      }
      if (this._isFirstImpact && affectorAgent.Formation != null && (affectorAgent.Formation.IsCavalry() && affectorAgent.Formation.MovementOrder == (object) MovementOrder.MovementOrderCharge) && this.CanSaveHighlight(this.GetHighlightTypeWithId("hlid_cavalry_charge_first_impact"), affectedAgent.Position))
      {
        this._cavalryChargeHitTimes.RemoveAll((Predicate<float>) (ht => (double) ht + 3.0 < (double) Mission.Current.Time));
        this._cavalryChargeHitTimes.Add(Mission.Current.Time);
        if (this._cavalryChargeHitTimes.Count >= 5)
        {
          highlight.HighlightType = this.GetHighlightTypeWithId("hlid_cavalry_charge_first_impact");
          highlight.Start = this._cavalryChargeHitTimes[0];
          highlight.End = this._cavalryChargeHitTimes.Last<float>();
          flag2 = true;
          this._isFirstImpact = false;
          this._cavalryChargeHitTimes.Clear();
        }
      }
      if (!flag2)
        return;
      this.SaveHighlight(highlight, affectedAgent.Position);
    }

    public override void OnMissionTick(float dt)
    {
      if (this._isArcherSalvoHappening && (double) this._archerSalvoKillTimes[0] + 4.0 < (double) Mission.Current.Time)
      {
        HighlightsController.Highlight highlight;
        highlight.HighlightType = this.GetHighlightTypeWithId("hlid_archer_salvo_kills");
        highlight.Start = this._archerSalvoKillTimes[0];
        highlight.End = this._archerSalvoKillTimes.Last<float>();
        this.SaveHighlight(highlight);
        this._isArcherSalvoHappening = false;
        this._archerSalvoKillTimes.Clear();
      }
      if (this._isKillingSpreeHappening && (double) this._playerKillTimes[0] + 10.0 < (double) Mission.Current.Time)
      {
        HighlightsController.Highlight highlight;
        highlight.HighlightType = this.GetHighlightTypeWithId("hlid_killing_spree");
        highlight.Start = this._playerKillTimes[0];
        highlight.End = this._playerKillTimes.Last<float>();
        this.SaveHighlight(highlight);
        this._isKillingSpreeHappening = false;
        this._playerKillTimes.Clear();
      }
      this.TickHighlightsToBeSaved();
    }

    public override void HandleOnCloseMission()
    {
      foreach (string highlightGroupId in this._highlightGroupIds)
        Highlights.CloseGroup(highlightGroupId);
      this._highlightSaveQueue = (List<HighlightsController.Highlight>) null;
      this._lastSavedHighlightData = (Tuple<float, float>) null;
      this._playerKillTimes = (List<float>) null;
      this._archerSalvoKillTimes = (List<float>) null;
      this._cavalryChargeHitTimes = (List<float>) null;
    }

    public static void AddHighlightType(HighlightsController.HighlightType highlightType)
    {
      if (HighlightsController.HighlightTypes.Any<HighlightsController.HighlightType>((Func<HighlightsController.HighlightType, bool>) (h => h.Id == highlightType.Id)))
        return;
      if (HighlightsController.IsHighlightsInitialized)
        Highlights.AddHighlight(highlightType.Id, highlightType.Description);
      HighlightsController.HighlightTypes.Add(highlightType);
    }

    public void SaveHighlight(HighlightsController.Highlight highlight) => this._highlightSaveQueue.Add(highlight);

    public void SaveHighlight(HighlightsController.Highlight highlight, Vec3 position)
    {
      if (!this.CanSaveHighlight(highlight.HighlightType, position))
        return;
      this._highlightSaveQueue.Add(highlight);
    }

    public bool CanSaveHighlight(HighlightsController.HighlightType highlightType, Vec3 position)
    {
      if ((double) highlightType.MaxHighlightDistance < (double) Mission.Current.Scene.LastFinalRenderCameraFrame.origin.Distance(position) || (double) highlightType.MinVisibilityScore > (double) this.GetPlayerIsLookingAtPositionScore(position))
        return false;
      return !highlightType.IsVisibilityRequired || this.CanSeePosition(position);
    }

    public float GetPlayerIsLookingAtPositionScore(Vec3 position)
    {
      Vec3 vec3 = -Mission.Current.Scene.LastFinalRenderCameraFrame.rotation.u;
      Vec3 origin = Mission.Current.Scene.LastFinalRenderCameraFrame.origin;
      return Math.Max(Vec3.DotProduct(vec3.NormalizedCopy(), (position - origin).NormalizedCopy()), 0.0f);
    }

    public bool CanSeePosition(Vec3 position)
    {
      Vec3 origin = Mission.Current.Scene.LastFinalRenderCameraFrame.origin;
      float collisionDistance;
      return !Mission.Current.Scene.RayCastForClosestEntityOrTerrain(origin, position, out collisionDistance, excludeBodyFlags: BodyFlags.CameraCollisionRayCastExludeFlags) || (double) Math.Abs(position.Distance(origin) - collisionDistance) < 0.100000001490116;
    }

    public void ShowSummary()
    {
      if (!this.IsAnyHighlightSaved)
        return;
      Highlights.OpenSummary(this._savedHighlightGroups);
    }

    private void TickHighlightsToBeSaved()
    {
      if (this._highlightSaveQueue == null)
        return;
      HighlightsController.Highlight highlightSave;
      if (this._lastSavedHighlightData != null && this._highlightSaveQueue.Count > 0)
      {
        float num1 = this._lastSavedHighlightData.Item1;
        double num2 = (double) this._lastSavedHighlightData.Item2;
        float num3 = (float) (num2 - (num2 - (double) num1) * 0.5);
        for (int index = 0; index < this._highlightSaveQueue.Count; ++index)
        {
          double start = (double) this._highlightSaveQueue[index].Start;
          highlightSave = this._highlightSaveQueue[index];
          double startDelta = (double) highlightSave.HighlightType.StartDelta;
          if (start - startDelta < (double) num3)
          {
            this._highlightSaveQueue.Remove(this._highlightSaveQueue[index]);
            --index;
          }
        }
      }
      if (this._highlightSaveQueue.Count <= 0)
        return;
      double start1 = (double) this._highlightSaveQueue[0].Start;
      highlightSave = this._highlightSaveQueue[0];
      double num4 = (double) (highlightSave.HighlightType.StartDelta / 1000);
      float num5 = (float) (start1 + num4);
      double end1 = (double) this._highlightSaveQueue[0].End;
      highlightSave = this._highlightSaveQueue[0];
      double num6 = (double) (highlightSave.HighlightType.EndDelta / 1000);
      float num7 = (float) (end1 + num6);
      for (int index = 1; index < this._highlightSaveQueue.Count; ++index)
      {
        double start2 = (double) this._highlightSaveQueue[index].Start;
        highlightSave = this._highlightSaveQueue[index];
        double num1 = (double) (highlightSave.HighlightType.StartDelta / 1000);
        float num2 = (float) (start2 + num1);
        double end2 = (double) this._highlightSaveQueue[index].End;
        highlightSave = this._highlightSaveQueue[index];
        double num3 = (double) (highlightSave.HighlightType.EndDelta / 1000);
        float num8 = (float) (end2 + num3);
        if ((double) num2 < (double) num5)
          num5 = num2;
        if ((double) num8 > (double) num7)
          num7 = num8;
      }
      highlightSave = this._highlightSaveQueue[0];
      string id = highlightSave.HighlightType.Id;
      highlightSave = this._highlightSaveQueue[0];
      string groupId = highlightSave.HighlightType.GroupId;
      int startDelta1 = (int) ((double) num5 - (double) Mission.Current.Time) * 1000;
      int endDelta = (int) ((double) num7 - (double) Mission.Current.Time) * 1000;
      this.SaveVideo(id, groupId, startDelta1, endDelta);
      this._lastSavedHighlightData = new Tuple<float, float>(num5, num7);
      this._highlightSaveQueue.Clear();
    }

    public struct HighlightType
    {
      public string Id { get; private set; }

      public string Description { get; private set; }

      public string GroupId { get; private set; }

      public int StartDelta { get; private set; }

      public int EndDelta { get; private set; }

      public float MinVisibilityScore { get; private set; }

      public float MaxHighlightDistance { get; private set; }

      public bool IsVisibilityRequired { get; private set; }

      public HighlightType(
        string id,
        string description,
        string groupId,
        int startDelta,
        int endDelta,
        float minVisibilityScore,
        float maxHighlightDistance,
        bool isVisibilityRequired)
      {
        this.Id = id;
        this.Description = description;
        this.GroupId = groupId;
        this.StartDelta = startDelta;
        this.EndDelta = endDelta;
        this.MinVisibilityScore = minVisibilityScore;
        this.MaxHighlightDistance = maxHighlightDistance;
        this.IsVisibilityRequired = isVisibilityRequired;
      }
    }

    public struct Highlight
    {
      public HighlightsController.HighlightType HighlightType;
      public float Start;
      public float End;
    }
  }
}
