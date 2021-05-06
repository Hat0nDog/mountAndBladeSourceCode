// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBBannerlordTableauManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBBannerlordTableauManager
  {
    [EngineMethod("request_character_tableau_render", false)]
    void RequestCharacterTableauRender(
      int characterCodeId,
      string path,
      UIntPtr poseEntity,
      UIntPtr cameraObject,
      int tableauType);

    [EngineMethod("initialize_character_tableau_render_system", false)]
    void InitializeCharacterTableauRenderSystem();

    [EngineMethod("get_number_of_pending_tableau_requests", false)]
    int GetNumberOfPendingTableauRequests();
  }
}
