// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Missions.DebugAgentTeleporterMissionController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
  public class DebugAgentTeleporterMissionController : MissionLogic
  {
    public override void AfterStart()
    {
    }

    public override void OnMissionTick(float dt)
    {
      Agent agent1 = (Agent) null;
      int debugAgent = this.Mission.GetDebugAgent();
      foreach (Agent agent2 in (IEnumerable<Agent>) this.Mission.Agents)
      {
        if (debugAgent == agent2.Index)
        {
          agent1 = agent2;
          break;
        }
      }
      if (agent1 == null && this.Mission.Agents.Count > 0)
      {
        int num = MBRandom.RandomInt(this.Mission.Agents.Count);
        int count = this.Mission.Agents.Count;
        int index = num;
        do
        {
          Agent agent2 = this.Mission.Agents[index];
          if (agent2 != Agent.Main && agent2.IsActive())
          {
            agent1 = agent2;
            this.Mission.SetDebugAgent(index);
            break;
          }
          index = (index + 1) % count;
        }
        while (index != num);
      }
      if (agent1 == null)
        return;
      MatrixFrame renderCameraFrame = this.Mission.Scene.LastFinalRenderCameraFrame;
      if (Input.DebugInput.IsKeyDown(InputKey.MiddleMouseButton))
        this.Mission.Scene.RayCastForClosestEntityOrTerrain(renderCameraFrame.origin, renderCameraFrame.origin + -renderCameraFrame.rotation.u * 100f, out float _, excludeBodyFlags: BodyFlags.CommonCollisionExcludeFlags);
      float collisionDistance;
      if (Input.DebugInput.IsKeyReleased(InputKey.MiddleMouseButton) && this.Mission.Scene.RayCastForClosestEntityOrTerrain(renderCameraFrame.origin, renderCameraFrame.origin + -renderCameraFrame.rotation.u * 100f, out collisionDistance, excludeBodyFlags: BodyFlags.CommonCollisionExcludeFlags))
      {
        Vec3 position = renderCameraFrame.origin + -renderCameraFrame.rotation.u * collisionDistance;
        if (Input.DebugInput.IsHotKeyReleased("DebugAgentTeleportMissionControllerHotkeyTeleportMainAgent"))
        {
          agent1.TeleportToPosition(position);
        }
        else
        {
          Vec2 vec2 = -renderCameraFrame.rotation.u.AsVec2;
          WorldPosition scriptedPosition = new WorldPosition(this.Mission.Scene, UIntPtr.Zero, position, false);
          agent1.SetScriptedPositionAndDirection(ref scriptedPosition, vec2.RotationInRadians, false, Agent.AIScriptedFrameFlags.NoAttack);
        }
      }
      if (!Input.DebugInput.IsHotKeyPressed("DebugAgentTeleportMissionControllerHotkeyDisableScriptedMovement"))
        return;
      agent1.DisableScriptedMovement();
    }
  }
}
