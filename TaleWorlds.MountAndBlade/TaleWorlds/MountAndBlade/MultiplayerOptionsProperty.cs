// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerOptionsProperty
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  [AttributeUsage(AttributeTargets.Field)]
  public class MultiplayerOptionsProperty : Attribute
  {
    public readonly MultiplayerOptions.OptionValueType OptionValueType;
    public readonly MultiplayerOptionsProperty.ReplicationOccurrence Replication;
    public readonly string Description;
    public readonly int BoundsMin;
    public readonly int BoundsMax;
    public readonly string[] ValidGameModes;
    public readonly bool HasMultipleSelections;
    public readonly Type EnumType;

    public bool HasBounds => this.BoundsMax > this.BoundsMin;

    public MultiplayerOptionsProperty(
      MultiplayerOptions.OptionValueType optionValueType,
      MultiplayerOptionsProperty.ReplicationOccurrence replicationOccurrence,
      string description = null,
      int boundsMin = 0,
      int boundsMax = 0,
      string[] validGameModes = null,
      bool hasMultipleSelections = false,
      Type enumType = null)
    {
      this.OptionValueType = optionValueType;
      this.Replication = replicationOccurrence;
      this.Description = description;
      this.BoundsMin = boundsMin;
      this.BoundsMax = boundsMax;
      this.ValidGameModes = validGameModes;
      this.HasMultipleSelections = hasMultipleSelections;
      this.EnumType = enumType;
    }

    public enum ReplicationOccurrence
    {
      Never,
      AtMapLoad,
      Immediately,
    }
  }
}
