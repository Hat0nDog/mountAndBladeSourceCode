// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticalPosition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TacticalPosition : MissionObject
  {
    private WorldPosition _position;
    private Vec2 _direction;
    [EditableScriptComponentVariable(true)]
    private float _width;
    [EditableScriptComponentVariable(true)]
    private float _slope;
    [EditableScriptComponentVariable(true)]
    private bool _isInsurmountable;
    [EditableScriptComponentVariable(true)]
    private bool _isOuterEdge;
    private List<TacticalPosition> _linkedTacticalPositions;
    [EditableScriptComponentVariable(true)]
    private TacticalPosition.TacticalPositionTypeEnum _tacticalPositionType;
    [EditableScriptComponentVariable(true)]
    private TacticalRegion.TacticalRegionTypeEnum _tacticalRegionMembership;
    [EditableScriptComponentVariable(true)]
    private FormationAI.BehaviorSide _tacticalPositionSide = FormationAI.BehaviorSide.BehaviorSideNotSet;
    [EditableScriptComponentVariable(true)]
    private float _oldWidth = 1f;

    internal WorldPosition Position
    {
      get => this._position;
      set => this._position = value;
    }

    internal Vec2 Direction
    {
      get => this._direction;
      set => this._direction = value;
    }

    internal float Width => this._width;

    internal float Slope => this._slope;

    internal bool IsInsurmountable => this._isInsurmountable;

    internal bool IsOuterEdge => this._isOuterEdge;

    internal List<TacticalPosition> LinkedTacticalPositions
    {
      get => this._linkedTacticalPositions;
      set => this._linkedTacticalPositions = value;
    }

    internal TacticalPosition.TacticalPositionTypeEnum TacticalPositionType => this._tacticalPositionType;

    internal TacticalRegion.TacticalRegionTypeEnum TacticalRegionMembership => this._tacticalRegionMembership;

    internal FormationAI.BehaviorSide TacticalPositionSide => this._tacticalPositionSide;

    public TacticalPosition()
    {
      this._width = 1f;
      this._slope = 0.0f;
    }

    public TacticalPosition(
      WorldPosition position,
      Vec2 direction,
      float width,
      float slope = 0.0f,
      bool isInsurmountable = false,
      TacticalPosition.TacticalPositionTypeEnum tacticalPositionType = TacticalPosition.TacticalPositionTypeEnum.Regional,
      TacticalRegion.TacticalRegionTypeEnum tacticalRegionMembership = TacticalRegion.TacticalRegionTypeEnum.Opening)
    {
      this._position = position;
      this._direction = direction;
      this._width = width;
      this._slope = slope;
      this._isInsurmountable = isInsurmountable;
      this._tacticalPositionType = tacticalPositionType;
      this._tacticalRegionMembership = tacticalRegionMembership;
      this._tacticalPositionSide = FormationAI.BehaviorSide.BehaviorSideNotSet;
      this._isOuterEdge = false;
      this._linkedTacticalPositions = new List<TacticalPosition>();
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this._position = new WorldPosition(Mission.Current.Scene, this.GameEntity.GlobalPosition);
      this._direction = this.GameEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized();
      this._oldWidth = this._width;
    }

    public override void AfterMissionStart()
    {
      base.AfterMissionStart();
      this._linkedTacticalPositions = this.GameEntity.GetChildren().Where<GameEntity>((Func<GameEntity, bool>) (c => c.HasScriptOfType<TacticalPosition>())).Select<GameEntity, TacticalPosition>((Func<GameEntity, TacticalPosition>) (c => c.GetFirstScriptOfType<TacticalPosition>())).ToList<TacticalPosition>();
    }

    protected internal override void OnEditorInit()
    {
      base.OnEditorInit();
      MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
      if ((double) this._width > 0.0 && (double) this._oldWidth > 0.0 && (double) this._oldWidth != (double) this._width)
      {
        globalFrame.rotation.MakeUnit();
        globalFrame.rotation.ApplyScaleLocal(new Vec3(this._width / this._oldWidth, 1f, 1f));
        this.GameEntity.SetGlobalFrame(globalFrame);
        this._oldWidth = this._width;
      }
      this._direction = globalFrame.rotation.f.AsVec2.Normalized();
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
      if ((double) this._width > 0.0 && (double) this._oldWidth > 0.0 && (double) this._oldWidth != (double) this._width)
      {
        globalFrame.Scale(new Vec3(this._width / this._oldWidth, 1f, 1f));
        this.GameEntity.SetGlobalFrame(globalFrame);
        this._oldWidth = this._width;
      }
      this._direction = globalFrame.rotation.f.AsVec2.Normalized();
    }

    public void SetWidthFromSpawner(float width) => this._width = width;

    public enum TacticalPositionTypeEnum
    {
      Regional,
      HighGround,
      ChokePoint,
      Cliff,
      SpecialMissionPosition,
    }
  }
}
