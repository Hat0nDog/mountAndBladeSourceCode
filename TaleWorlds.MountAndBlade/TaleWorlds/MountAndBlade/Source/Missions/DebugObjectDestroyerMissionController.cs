// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Missions.DebugObjectDestroyerMissionController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
  public class DebugObjectDestroyerMissionController : MissionLogic
  {
    private GameEntity _contouredEntity;

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      Vec3 renderCameraPosition = this.Mission.Scene.LastFinalRenderCameraPosition;
      Vec3 vec3_1 = -this.Mission.Scene.LastFinalRenderCameraFrame.rotation.u;
      float collisionDistance;
      GameEntity collidedEntity;
      bool flag1 = Mission.Current.Scene.RayCastForClosestEntityOrTerrain(renderCameraPosition, renderCameraPosition + vec3_1 * 100f, out collisionDistance, out collidedEntity, excludeBodyFlags: BodyFlags.OnlyCollideWithRaycast);
      if (Input.DebugInput.IsShiftDown() && Agent.Main != null && ((NativeObject) collidedEntity != (NativeObject) null && !collidedEntity.HasScriptOfType<DestructableComponent>()))
      {
        float num = 5f;
        foreach (DestructableComponent destructableComponent in Mission.Current.ActiveMissionObjects.Where<MissionObject>((Func<MissionObject, bool>) (x => x is DestructableComponent)))
        {
          if ((double) (destructableComponent.GameEntity.GlobalPosition - Agent.Main.Position).Length < (double) num)
            collidedEntity = destructableComponent.GameEntity;
        }
      }
      GameEntity gameEntity1 = (GameEntity) null;
      if (flag1 && (Input.DebugInput.IsKeyDown(InputKey.MiddleMouseButton) || Input.DebugInput.IsKeyReleased(InputKey.MiddleMouseButton)))
      {
        Vec3 vec3_2 = renderCameraPosition + vec3_1 * collisionDistance;
        if ((NativeObject) collidedEntity == (NativeObject) null)
          return;
        bool flag2 = Input.DebugInput.IsKeyReleased(InputKey.MiddleMouseButton);
        int weaponKind = 0;
        if (flag2)
          weaponKind = !Input.DebugInput.IsAltDown() ? (!Input.DebugInput.IsControlDown() ? (int) Game.Current.ObjectManager.GetObject<ItemObject>("ballista_projectile").Id.InternalValue : (int) Game.Current.ObjectManager.GetObject<ItemObject>("pot").Id.InternalValue) : (int) Game.Current.ObjectManager.GetObject<ItemObject>("boulder").Id.InternalValue;
        GameEntity gameEntity2 = collidedEntity;
        DestructableComponent destructableComponent1;
        for (destructableComponent1 = (DestructableComponent) null; destructableComponent1 == null && (NativeObject) gameEntity2 != (NativeObject) null; gameEntity2 = gameEntity2.Parent)
          destructableComponent1 = gameEntity2.GetFirstScriptOfType<DestructableComponent>();
        if (destructableComponent1 != null && !destructableComponent1.IsDestroyed)
        {
          if (flag2)
          {
            if (Agent.Main != null)
            {
              DestructableComponent destructableComponent2 = destructableComponent1;
              Agent main = Agent.Main;
              Vec3 impactPosition = vec3_2 - vec3_1 * 0.1f;
              Vec3 impactDirection = vec3_1;
              MissionWeapon missionWeapon = new MissionWeapon(ItemObject.GetItemFromWeaponKind(weaponKind), (ItemModifier) null, (Banner) null);
              ref MissionWeapon local = ref missionWeapon;
              destructableComponent2.TriggerOnHit(main, 400, impactPosition, impactDirection, in local, (ScriptComponentBehaviour) null);
            }
          }
          else
            gameEntity1 = destructableComponent1.GameEntity;
        }
      }
      if ((NativeObject) gameEntity1 != (NativeObject) this._contouredEntity && (NativeObject) this._contouredEntity != (NativeObject) null)
        this._contouredEntity.SetContourColor(new uint?());
      this._contouredEntity = gameEntity1;
      if (!((NativeObject) this._contouredEntity != (NativeObject) null))
        return;
      this._contouredEntity.SetContourColor(new uint?(4294967040U));
    }
  }
}
