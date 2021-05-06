// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BannerlordTableauManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public static class BannerlordTableauManager
  {
    private static Scene[] _tableauCharacterScenes = new Scene[5];
    private static bool _isTableauRenderSystemInitialized = false;
    public static BannerlordTableauManager.RequestCharacterTableauSetupDelegate RequestCallback;

    public static Scene[] TableauCharacterScenes => BannerlordTableauManager._tableauCharacterScenes;

    public static void RequestCharacterTableauRender(
      int characterCodeId,
      string path,
      GameEntity poseEntity,
      Camera cameraObject,
      int tableauType)
    {
      MBAPI.IMBBannerlordTableauManager.RequestCharacterTableauRender(characterCodeId, path, poseEntity.Pointer, cameraObject.Pointer, tableauType);
    }

    public static void ClearManager()
    {
      BannerlordTableauManager._tableauCharacterScenes = (Scene[]) null;
      BannerlordTableauManager.RequestCallback = (BannerlordTableauManager.RequestCharacterTableauSetupDelegate) null;
      BannerlordTableauManager._isTableauRenderSystemInitialized = false;
    }

    public static void InitializeCharacterTableauRenderSystem()
    {
      if (BannerlordTableauManager._isTableauRenderSystemInitialized)
        return;
      MBAPI.IMBBannerlordTableauManager.InitializeCharacterTableauRenderSystem();
      BannerlordTableauManager._isTableauRenderSystemInitialized = true;
    }

    public static int GetNumberOfPendingTableauRequests() => MBAPI.IMBBannerlordTableauManager.GetNumberOfPendingTableauRequests();

    [MBCallback]
    internal static void RequestCharacterTableauSetup(
      int characterCodeId,
      Scene scene,
      GameEntity poseEntity)
    {
      BannerlordTableauManager.RequestCallback(characterCodeId, scene, poseEntity);
    }

    [MBCallback]
    internal static void RegisterCharacterTableauScene(Scene scene, int type) => BannerlordTableauManager.TableauCharacterScenes[type] = scene;

    public delegate void RequestCharacterTableauSetupDelegate(
      int characterCodeId,
      Scene scene,
      GameEntity poseEntity);
  }
}
