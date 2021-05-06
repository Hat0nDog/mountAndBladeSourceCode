// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TrainingIcon
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class TrainingIcon : UsableMachine
  {
    private static readonly ActionIndexCache act_pickup_middle_begin = ActionIndexCache.Create(nameof (act_pickup_middle_begin));
    private static readonly ActionIndexCache act_pickup_middle_begin_left_stance = ActionIndexCache.Create(nameof (act_pickup_middle_begin_left_stance));
    private static readonly ActionIndexCache act_pickup_middle_end = ActionIndexCache.Create(nameof (act_pickup_middle_end));
    private static readonly ActionIndexCache act_pickup_middle_end_left_stance = ActionIndexCache.Create(nameof (act_pickup_middle_end_left_stance));
    private static readonly string HighlightBeamTag = "highlight_beam";
    private bool _activated;
    private float _markerAlpha;
    private float _targetMarkerAlpha;
    private float _markerAlphaChangeAmount = 110f;
    private List<GameEntity> _weaponIcons = new List<GameEntity>();
    private GameEntity _markerBeam;
    [EditableScriptComponentVariable(true)]
    private string _descriptionTextOfIcon = "";
    [EditableScriptComponentVariable(true)]
    private string _trainingSubTypeTag = "";

    public bool Focused { get; private set; }

    protected internal override void OnInit()
    {
      base.OnInit();
      this._markerBeam = this.GameEntity.GetFirstChildEntityWithTag(TrainingIcon.HighlightBeamTag);
      this._weaponIcons = this.GameEntity.GetChildren().Where<GameEntity>((Func<GameEntity, bool>) (x => x.GetScriptComponents().Count<ScriptComponentBehaviour>() == 0 && (NativeObject) x != (NativeObject) this._markerBeam)).ToList<GameEntity>();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if ((NativeObject) this._markerBeam != (NativeObject) null)
      {
        if ((double) Math.Abs(this._markerAlpha - this._targetMarkerAlpha) > (double) dt * (double) this._markerAlphaChangeAmount)
        {
          this._markerAlpha += dt * this._markerAlphaChangeAmount * (float) Math.Sign(this._targetMarkerAlpha - this._markerAlpha);
          this._markerBeam.GetChildren().First<GameEntity>().GetFirstMesh().SetVectorArgument(this._markerAlpha, 1f, 0.49f, 11.65f);
        }
        else
        {
          this._markerAlpha = this._targetMarkerAlpha;
          if ((double) this._targetMarkerAlpha == 0.0)
            this._markerBeam?.SetVisibilityExcludeParents(false);
        }
      }
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        if (standingPoint.HasUser)
        {
          Agent userAgent = standingPoint.UserAgent;
          ActionIndexCache currentAction1 = userAgent.GetCurrentAction(0);
          ActionIndexCache currentAction2 = userAgent.GetCurrentAction(1);
          if (!(currentAction2 == ActionIndexCache.act_none) || !(currentAction1 == TrainingIcon.act_pickup_middle_begin) && !(currentAction1 == TrainingIcon.act_pickup_middle_begin_left_stance))
          {
            if (currentAction2 == ActionIndexCache.act_none && (currentAction1 == TrainingIcon.act_pickup_middle_end || currentAction1 == TrainingIcon.act_pickup_middle_end_left_stance))
            {
              this._activated = true;
              userAgent.StopUsingGameObject();
            }
            else if (currentAction2 != ActionIndexCache.act_none || !userAgent.SetActionChannel(0, userAgent.GetIsLeftStance() ? TrainingIcon.act_pickup_middle_begin_left_stance : TrainingIcon.act_pickup_middle_begin))
              userAgent.StopUsingGameObject();
          }
        }
      }
    }

    public void SetMarked(bool highlight)
    {
      if (highlight)
      {
        this._targetMarkerAlpha = 75f;
        this._markerBeam.GetChildren().First<GameEntity>().GetFirstMesh().SetVectorArgument(this._markerAlpha, 1f, 0.49f, 11.65f);
        this._markerBeam?.SetVisibilityExcludeParents(true);
      }
      else
        this._targetMarkerAlpha = 0.0f;
    }

    public bool GetIsActivated()
    {
      int num = this._activated ? 1 : 0;
      this._activated = false;
      return num != 0;
    }

    public string GetTrainingSubTypeTag() => this._trainingSubTypeTag;

    public void DisableIcon()
    {
      foreach (GameEntity weaponIcon in this._weaponIcons)
        weaponIcon.SetVisibilityExcludeParents(false);
    }

    public void EnableIcon()
    {
      foreach (GameEntity weaponIcon in this._weaponIcons)
        weaponIcon.SetVisibilityExcludeParents(true);
    }

    public override string GetDescriptionText(GameEntity gameEntity = null)
    {
      TextObject textObject = new TextObject("{=5zGsfiMM}{TRAINING_TYPE}");
      textObject.SetTextVariable("TRAINING_TYPE", GameTexts.FindText("str_tutorial_" + this._descriptionTextOfIcon));
      return textObject.ToString();
    }

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject = null)
    {
      TextObject textObject = new TextObject("{=wY1qP2qj}({KEY}) Select");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject;
    }

    public override void OnFocusGain(Agent userAgent) => base.OnFocusGain(userAgent);

    public override void OnFocusLose(Agent userAgent) => base.OnFocusLose(userAgent);
  }
}
