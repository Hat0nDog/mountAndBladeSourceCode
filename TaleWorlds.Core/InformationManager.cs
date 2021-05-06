// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.InformationManager
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
  public static class InformationManager
  {
    public static event Action<InformationMessage> DisplayMessageInternal;

    public static event Action ClearAllMessagesInternal;

    public static event Action<string, int, BasicCharacterObject, string> FiringQuickInformation;

    public static event Action<string> OnAddSystemNotification;

    public static event Action<InformationData> OnAddMapNotice;

    public static event Action<InformationData> OnRemoveMapNotice;

    public static event Action<string> OnAddHintInformation;

    public static event Action<Type, object[]> OnAddTooltipInformation;

    public static event Action<int, int, object[]> OnBuildTooltipFromArray;

    public static event Action OnHideHintInformation;

    public static event Action OnHideTooltipInformation;

    public static event Action<InquiryData, bool> OnShowInquiry;

    public static event Action<TextInquiryData, bool> OnShowTextInquiry;

    public static event Action<MultiSelectionInquiryData, bool> OnShowMultiSelectionInquiry;

    public static event Action<SceneNotificationData, bool> OnShowSceneNotification;

    public static event Action OnHideSceneNotification;

    public static event Action OnHideInquiry;

    public static void DisplayMessage(InformationMessage message)
    {
      Action<InformationMessage> displayMessageInternal = InformationManager.DisplayMessageInternal;
      if (displayMessageInternal == null)
        return;
      displayMessageInternal(message);
    }

    public static void ClearAllMessages()
    {
      Action messagesInternal = InformationManager.ClearAllMessagesInternal;
      if (messagesInternal == null)
        return;
      messagesInternal();
    }

    public static void MapNoticeRemoved(InformationData data)
    {
      Action<InformationData> onRemoveMapNotice = InformationManager.OnRemoveMapNotice;
      if (onRemoveMapNotice == null)
        return;
      onRemoveMapNotice(data);
    }

    public static void AddNotice(InformationData data)
    {
      Action<InformationData> onAddMapNotice = InformationManager.OnAddMapNotice;
      if (onAddMapNotice == null)
        return;
      onAddMapNotice(data);
    }

    public static void AddQuickInformation(
      TextObject message,
      int priorty = 0,
      BasicCharacterObject announcerCharacter = null,
      string soundEventPath = "")
    {
      Action<string, int, BasicCharacterObject, string> quickInformation = InformationManager.FiringQuickInformation;
      if (quickInformation != null)
        quickInformation(message.ToString(), priorty, announcerCharacter, soundEventPath);
      Debug.Print(message.ToString(), debugFilter: 1125899906842624UL);
    }

    public static void AddSystemNotification(string message)
    {
      Action<string> systemNotification = InformationManager.OnAddSystemNotification;
      if (systemNotification == null)
        return;
      systemNotification(message);
    }

    public static void AddHintInformation(string message)
    {
      Action<string> addHintInformation = InformationManager.OnAddHintInformation;
      if (addHintInformation == null)
        return;
      addHintInformation(message);
    }

    public static void AddTooltipInformation(Type type, params object[] args)
    {
      Action<Type, object[]> tooltipInformation = InformationManager.OnAddTooltipInformation;
      if (tooltipInformation == null)
        return;
      tooltipInformation(type, args);
    }

    public static void UpdateTooltipFromArray(int id, int mode, object[] props)
    {
      Action<int, int, object[]> tooltipFromArray = InformationManager.OnBuildTooltipFromArray;
      if (tooltipFromArray == null)
        return;
      tooltipFromArray(id, mode, props);
    }

    public static void HideInformations()
    {
      Action tooltipInformation = InformationManager.OnHideTooltipInformation;
      if (tooltipInformation != null)
        tooltipInformation();
      Action hideHintInformation = InformationManager.OnHideHintInformation;
      if (hideHintInformation == null)
        return;
      hideHintInformation();
    }

    public static void ShowInquiry(InquiryData data, bool pauseGameActiveState = false)
    {
      Action<InquiryData, bool> onShowInquiry = InformationManager.OnShowInquiry;
      if (onShowInquiry == null)
        return;
      onShowInquiry(data, pauseGameActiveState);
    }

    public static void ShowTextInquiry(TextInquiryData textData, bool pauseGameActiveState = false)
    {
      Action<TextInquiryData, bool> onShowTextInquiry = InformationManager.OnShowTextInquiry;
      if (onShowTextInquiry == null)
        return;
      onShowTextInquiry(textData, pauseGameActiveState);
    }

    public static void ShowMultiSelectionInquiry(
      MultiSelectionInquiryData data,
      bool pauseGameActiveState = false)
    {
      Action<MultiSelectionInquiryData, bool> selectionInquiry = InformationManager.OnShowMultiSelectionInquiry;
      if (selectionInquiry == null)
        return;
      selectionInquiry(data, pauseGameActiveState);
    }

    public static void ShowSceneNotification(SceneNotificationData data, bool pauseGameActiveState = false)
    {
      Action<SceneNotificationData, bool> sceneNotification = InformationManager.OnShowSceneNotification;
      if (sceneNotification == null)
        return;
      sceneNotification(data, pauseGameActiveState);
    }

    public static void HideSceneNotification()
    {
      Action sceneNotification = InformationManager.OnHideSceneNotification;
      if (sceneNotification == null)
        return;
      sceneNotification();
    }

    public static void HideInquiry()
    {
      Action onHideInquiry = InformationManager.OnHideInquiry;
      if (onHideInquiry == null)
        return;
      onHideInquiry();
    }

    public static void Clear()
    {
      InformationManager.DisplayMessageInternal = (Action<InformationMessage>) null;
      InformationManager.FiringQuickInformation = (Action<string, int, BasicCharacterObject, string>) null;
      InformationManager.OnAddHintInformation = (Action<string>) null;
      InformationManager.OnAddTooltipInformation = (Action<Type, object[]>) null;
      InformationManager.OnBuildTooltipFromArray = (Action<int, int, object[]>) null;
      InformationManager.OnHideHintInformation = (Action) null;
      InformationManager.OnHideTooltipInformation = (Action) null;
      InformationManager.OnShowInquiry = (Action<InquiryData, bool>) null;
      InformationManager.OnShowTextInquiry = (Action<TextInquiryData, bool>) null;
      InformationManager.OnShowMultiSelectionInquiry = (Action<MultiSelectionInquiryData, bool>) null;
      InformationManager.OnHideInquiry = (Action) null;
    }
  }
}
