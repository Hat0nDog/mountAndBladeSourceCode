// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Markable
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class Markable : ScriptComponentBehaviour
  {
    public string MarkerPrefabName = "highlight_beam";
    private GameEntity _marker;
    private DestructableComponent _destructibleComponent;
    private bool _markerActive;
    private bool _markerVisible;
    private float _markerEventBeginningTime;
    private float _markerActiveDuration;
    private float _markerPassiveDuration;

    private bool MarkerActive
    {
      get => this._markerActive;
      set
      {
        if (this._markerActive == value)
          return;
        this._markerActive = value;
        this.SetScriptComponentToTick(this.GetTickRequirement());
      }
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this._marker = GameEntity.Instantiate(Mission.Current.Scene, "highlight_beam", this.GameEntity.GetGlobalFrame());
      this.DeactivateMarker();
      this._destructibleComponent = this.GameEntity.GetFirstScriptOfType<DestructableComponent>();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this.MarkerActive ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      if (!this.MarkerActive)
        return;
      if (this._destructibleComponent != null && this._destructibleComponent.IsDestroyed)
      {
        if (!this._markerVisible)
          return;
        this.DisableMarkerActivation();
      }
      else if (this._markerVisible)
      {
        if ((double) Mission.Current.Time - (double) this._markerEventBeginningTime <= (double) this._markerActiveDuration)
          return;
        this.DeactivateMarker();
      }
      else
      {
        if (this._markerVisible || (double) Mission.Current.Time - (double) this._markerEventBeginningTime <= (double) this._markerPassiveDuration)
          return;
        this.ActivateMarkerFor(this._markerActiveDuration, this._markerPassiveDuration);
      }
    }

    public void DisableMarkerActivation()
    {
      this.MarkerActive = false;
      this.DeactivateMarker();
    }

    public void ActivateMarkerFor(float activeSeconds, float passiveSeconds)
    {
      if (this._destructibleComponent != null && this._destructibleComponent.IsDestroyed)
        return;
      this.MarkerActive = true;
      this._markerVisible = true;
      this._markerEventBeginningTime = Mission.Current.Time;
      this._markerActiveDuration = activeSeconds;
      this._markerPassiveDuration = passiveSeconds;
      this._marker.SetVisibilityExcludeParents(true);
      this._marker.BurstEntityParticle(true);
    }

    private void DeactivateMarker()
    {
      this._markerVisible = false;
      this._marker.SetVisibilityExcludeParents(false);
      this._markerEventBeginningTime = Mission.Current.Time;
    }

    public void ResetPassiveDurationTimer()
    {
      if (this._markerVisible || !this.MarkerActive)
        return;
      this._markerEventBeginningTime = Mission.Current.Time;
    }
  }
}
