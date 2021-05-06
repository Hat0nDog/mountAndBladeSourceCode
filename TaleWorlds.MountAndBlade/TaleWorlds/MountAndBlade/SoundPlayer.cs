// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SoundPlayer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class SoundPlayer : ScriptComponentBehaviour
  {
    private bool Playing;
    private int SoundCode = -1;
    private SoundEvent SoundEvent;
    public bool AutoLoop;
    public bool AutoStart;
    public string SoundName;

    private void ValidateSoundEvent()
    {
      if (this.SoundEvent != null && this.SoundEvent.IsValid || this.SoundName.Length <= 0)
        return;
      if (this.SoundCode == -1)
        this.SoundCode = SoundManager.GetEventGlobalIndex(this.SoundName);
      this.SoundEvent = SoundEvent.CreateEvent(this.SoundCode, this.GameEntity.Scene);
    }

    public void UpdatePlaying() => this.Playing = this.SoundEvent != null && this.SoundEvent.IsValid && this.SoundEvent.IsPlaying();

    public void PlaySound()
    {
      if (this.Playing || this.SoundEvent == null || !this.SoundEvent.IsValid)
        return;
      this.SoundEvent.SetPosition(this.GameEntity.GlobalPosition);
      this.SoundEvent.Play();
      this.Playing = true;
    }

    public void ResumeSound()
    {
      if (this.Playing || this.SoundEvent == null || (!this.SoundEvent.IsValid || !this.SoundEvent.IsPaused()))
        return;
      this.SoundEvent.Resume();
      this.Playing = true;
    }

    public void PauseSound()
    {
      if (!this.Playing || this.SoundEvent == null || !this.SoundEvent.IsValid)
        return;
      this.SoundEvent.Pause();
      this.Playing = false;
    }

    public void StopSound()
    {
      if (!this.Playing || this.SoundEvent == null || !this.SoundEvent.IsValid)
        return;
      this.SoundEvent.Stop();
      this.Playing = false;
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      MBDebug.Print("SoundPlayer : OnInit called.", color: Debug.DebugColor.Yellow);
      this.ValidateSoundEvent();
      if (this.AutoStart)
        this.PlaySound();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt)
    {
      this.UpdatePlaying();
      if (this.Playing || !this.AutoLoop)
        return;
      this.ValidateSoundEvent();
      this.PlaySound();
    }

    protected internal override bool MovesEntity() => false;
  }
}
