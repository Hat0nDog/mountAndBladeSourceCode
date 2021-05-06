// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentVisualHolder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class AgentVisualHolder : IAgentVisual
  {
    private MatrixFrame _frame;
    private Equipment _equipment;
    private string _characterObjectStringID;
    private BodyProperties _bodyProperties;

    public AgentVisualHolder(
      MatrixFrame frame,
      Equipment equipment,
      string name,
      BodyProperties bodyProperties)
    {
      this.SetFrame(ref frame);
      this._equipment = equipment;
      this._characterObjectStringID = name;
      this._bodyProperties = bodyProperties;
    }

    public void SetAction(ActionIndexCache actionName, float startProgress = 0.0f)
    {
    }

    public GameEntity GetEntity() => (GameEntity) null;

    public MBAgentVisuals GetVisuals() => (MBAgentVisuals) null;

    public void SetFrame(ref MatrixFrame frame) => this._frame = frame;

    public MatrixFrame GetFrame() => this._frame;

    public BodyProperties GetBodyProperties() => this._bodyProperties;

    public void SetBodyProperties(BodyProperties bodyProperties) => this._bodyProperties = bodyProperties;

    public bool GetIsFemale() => false;

    public string GetCharacterObjectID() => this._characterObjectStringID;

    public void SetCharacterObjectID(string id) => this._characterObjectStringID = id;

    public Equipment GetEquipment() => this._equipment;

    public void RefreshWithNewEquipment(Equipment equipment) => this._equipment = equipment;

    public void SetClothingColors(uint color1, uint color2)
    {
    }

    public void GetClothingColors(out uint color1, out uint color2)
    {
      color1 = uint.MaxValue;
      color2 = uint.MaxValue;
    }

    public AgentVisualsData GetCopyAgentVisualsData() => (AgentVisualsData) null;

    public void Refresh(bool needBatchedVersionForWeaponMeshes, AgentVisualsData data)
    {
    }
  }
}
