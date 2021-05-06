// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMultiplayerInventoryLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public interface IMultiplayerInventoryLogic
  {
    int TransferFromEquipmentSlotToChest(int draggedEquipmentIndex);

    bool TransferFromChestToEquipmentSlot(
      int draggedChestNo,
      int droppedEquipmentIndex,
      out int oldEquipmentNewChestIndex);

    bool TransferFromEquipmentSlotToEquipmentSlot(
      int draggedEquipmentIndex,
      int droppedEquipmentIndex);

    int CharacterCount { get; }

    int CurrentCharacterIndex { get; }

    void SetNextCharacter();

    void SetPreviousCharacter();

    void OnEditSelectedCharacter(BodyProperties bodyProperties, bool isFemale);

    int FindProperEquipmentSlotIndex(string itemType);

    bool SingleCharacterMode { get; }
  }
}
