// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.VertexAnimator
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class VertexAnimator : SynchedMissionObject
  {
    public float Speed;
    public int BeginKey;
    public int EndKey;
    private bool _playOnce;
    private float _curAnimTime;
    private bool _isPlaying;
    private readonly List<Mesh> _animatedMeshes = new List<Mesh>();

    public VertexAnimator() => this.Speed = 20f;

    private void SetIsPlaying(bool value)
    {
      if (this._isPlaying == value)
        return;
      this._isPlaying = value;
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this.RefreshEditDataUsers();
      this.SetIsPlaying(true);
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected internal override void OnEditorInit() => this.OnInit();

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this._isPlaying ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!this._isPlaying)
        return;
      if ((double) this._curAnimTime < (double) this.BeginKey)
        this._curAnimTime = (float) this.BeginKey;
      this.GameEntity.SetMorphFrameOfComponents(this._curAnimTime);
      this._curAnimTime += dt * this.Speed;
      if ((double) this._curAnimTime <= (double) this.EndKey)
        return;
      if ((double) this._curAnimTime > (double) this.EndKey && this._playOnce)
      {
        this.SetIsPlaying(false);
        this._curAnimTime = (float) this.EndKey;
        this.GameEntity.SetMorphFrameOfComponents(this._curAnimTime);
      }
      else
      {
        int num = 0;
        while ((double) this._curAnimTime > (double) this.EndKey && ++num < 100)
          this._curAnimTime = (float) this.BeginKey + (this._curAnimTime - (float) this.EndKey);
      }
    }

    public void PlayOnce()
    {
      this.Play();
      this._playOnce = true;
    }

    public void Pause() => this.SetIsPlaying(false);

    public void Play()
    {
      this.Stop();
      this.Resume();
    }

    public void Resume() => this.SetIsPlaying(true);

    public void Stop()
    {
      this.SetIsPlaying(false);
      this._curAnimTime = (float) this.BeginKey;
      Mesh firstMesh = this.GameEntity.GetFirstMesh();
      if (!((NativeObject) firstMesh != (NativeObject) null))
        return;
      firstMesh.MorphTime = this._curAnimTime;
    }

    public void StopAndGoToEnd()
    {
      this.SetIsPlaying(false);
      this._curAnimTime = (float) this.EndKey;
      Mesh firstMesh = this.GameEntity.GetFirstMesh();
      if (!((NativeObject) firstMesh != (NativeObject) null))
        return;
      firstMesh.MorphTime = this._curAnimTime;
    }

    public void SetAnimation(int beginKey, int endKey, float speed)
    {
      this.BeginKey = beginKey;
      this.EndKey = endKey;
      this.Speed = speed;
    }

    public void SetAnimationSynched(int beginKey, int endKey, float speed)
    {
      if (beginKey == this.BeginKey && endKey == this.EndKey && (double) speed == (double) this.Speed)
        return;
      this.BeginKey = beginKey;
      this.EndKey = endKey;
      this.Speed = speed;
      if (!GameNetwork.IsServerOrRecorder)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectVertexAnimation((MissionObject) this, beginKey, endKey, speed));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
    }

    public void SetProgressSynched(float value)
    {
      if ((double) Math.Abs(this.Progress - value) <= 9.99999974737875E-05)
        return;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectVertexAnimationProgress((MissionObject) this, value));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.Progress = value;
    }

    private float Progress
    {
      get => (this._curAnimTime - (float) this.BeginKey) / (float) (this.EndKey - this.BeginKey);
      set
      {
        this._curAnimTime = (float) this.BeginKey + value * (float) (this.EndKey - this.BeginKey);
        Mesh firstMesh = this.GameEntity.GetFirstMesh();
        if (!((NativeObject) firstMesh != (NativeObject) null))
          return;
        firstMesh.MorphTime = this._curAnimTime;
      }
    }

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      int count = this._animatedMeshes.Count;
      for (int index = 0; index < count; ++index)
        this._animatedMeshes[index].ReleaseEditDataUser();
    }

    protected internal override void OnEditorTick(float dt)
    {
      int componentCount = this.GameEntity.GetComponentCount(GameEntity.ComponentType.MetaMesh);
      bool flag1 = false;
      for (int index1 = 0; index1 < componentCount; ++index1)
      {
        MetaMesh componentAtIndex = this.GameEntity.GetComponentAtIndex(index1, GameEntity.ComponentType.MetaMesh) as MetaMesh;
        for (int meshIndex = 0; meshIndex < componentAtIndex.MeshCount; ++meshIndex)
        {
          int count = this._animatedMeshes.Count;
          bool flag2 = false;
          for (int index2 = 0; index2 < count; ++index2)
          {
            if ((NativeObject) componentAtIndex.GetMeshAtIndex(meshIndex) == (NativeObject) this._animatedMeshes[index2])
            {
              flag2 = true;
              break;
            }
          }
          if (!flag2)
            flag1 = true;
        }
      }
      if (flag1)
        this.RefreshEditDataUsers();
      this.OnTick(dt);
    }

    private void RefreshEditDataUsers()
    {
      foreach (Mesh animatedMesh in this._animatedMeshes)
        animatedMesh.ReleaseEditDataUser();
      this._animatedMeshes.Clear();
      int componentCount = this.GameEntity.GetComponentCount(GameEntity.ComponentType.MetaMesh);
      for (int index = 0; index < componentCount; ++index)
      {
        MetaMesh componentAtIndex = this.GameEntity.GetComponentAtIndex(index, GameEntity.ComponentType.MetaMesh) as MetaMesh;
        for (int meshIndex = 0; meshIndex < componentAtIndex.MeshCount; ++meshIndex)
        {
          Mesh meshAtIndex = componentAtIndex.GetMeshAtIndex(meshIndex);
          meshAtIndex.AddEditDataUser();
          meshAtIndex.HintVerticesDynamic();
          meshAtIndex.HintIndicesDynamic();
          this._animatedMeshes.Add(meshAtIndex);
          Mesh baseMesh = meshAtIndex.GetBaseMesh();
          if ((NativeObject) baseMesh != (NativeObject) null)
          {
            baseMesh.AddEditDataUser();
            this._animatedMeshes.Add(baseMesh);
          }
        }
      }
    }

    public override void WriteToNetwork()
    {
      base.WriteToNetwork();
      GameNetworkMessage.WriteIntToPacket(this.BeginKey, CompressionBasic.AnimationKeyCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.EndKey, CompressionBasic.AnimationKeyCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.Speed, CompressionBasic.VertexAnimationSpeedCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this.Progress, CompressionBasic.UnitVectorCompressionInfo);
    }

    public override bool ReadFromNetwork()
    {
      bool bufferReadValid = base.ReadFromNetwork();
      if (bufferReadValid)
      {
        int num1 = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref bufferReadValid);
        int num2 = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationKeyCompressionInfo, ref bufferReadValid);
        float num3 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.VertexAnimationSpeedCompressionInfo, ref bufferReadValid);
        float num4 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
        if (bufferReadValid)
        {
          this.BeginKey = num1;
          this.EndKey = num2;
          this.Speed = num3;
          this.Progress = num4;
        }
      }
      return bufferReadValid;
    }
  }
}
