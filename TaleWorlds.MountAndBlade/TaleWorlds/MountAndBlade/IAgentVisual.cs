// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IAgentVisual
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public interface IAgentVisual
  {
    void SetAction(ActionIndexCache actionName, float startProgress = 0.0f);

    MBAgentVisuals GetVisuals();

    MatrixFrame GetFrame();

    BodyProperties GetBodyProperties();

    void SetBodyProperties(BodyProperties bodyProperties);

    bool GetIsFemale();

    string GetCharacterObjectID();

    void SetCharacterObjectID(string id);

    Equipment GetEquipment();

    void SetClothingColors(uint color1, uint color2);

    void GetClothingColors(out uint color1, out uint color2);

    AgentVisualsData GetCopyAgentVisualsData();

    void Refresh(bool needBatchedVersionForWeaponMeshes, AgentVisualsData data);
  }
}
