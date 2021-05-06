// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.FormationClassExtensions
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public static class FormationClassExtensions
  {
    public static string GetName(this FormationClass formationClass)
    {
      switch (formationClass)
      {
        case FormationClass.Infantry:
          return "Infantry";
        case FormationClass.Ranged:
          return "Ranged";
        case FormationClass.Cavalry:
          return "Cavalry";
        case FormationClass.HorseArcher:
          return "HorseArcher";
        case FormationClass.NumberOfDefaultFormations:
          return "Skirmisher";
        case FormationClass.HeavyInfantry:
          return "HeavyInfantry";
        case FormationClass.LightCavalry:
          return "LightCavalry";
        case FormationClass.HeavyCavalry:
          return "HeavyCavalry";
        case FormationClass.NumberOfRegularFormations:
          return "General";
        case FormationClass.Bodyguard:
          return "Bodyguard";
        default:
          return "Unset";
      }
    }

    public static string GetLocalizedName(this FormationClass formationClass) => GameTexts.FindText("str_troop_group_name", ((int) formationClass).ToString()).ToString();
  }
}
