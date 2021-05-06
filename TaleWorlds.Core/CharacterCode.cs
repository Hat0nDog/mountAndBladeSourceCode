// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.CharacterCode
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public class CharacterCode
  {
    public const string SpecialCodeSeparator = "@---@";
    public const int SpecialCodeSeparatorLength = 5;
    public BodyProperties BodyProperties;

    public string EquipmentCode { get; private set; }

    public string Code { get; private set; }

    public bool IsFemale { get; private set; }

    public bool IsHero { get; private set; }

    public float FaceDirtAmount => 0.0f;

    public Banner Banner => (Banner) null;

    public FormationClass FormationClass { get; set; }

    public uint Color1 { get; set; } = Color.White.ToUnsignedInteger();

    public uint Color2 { get; set; } = Color.White.ToUnsignedInteger();

    public Equipment CalculateEquipment() => this.EquipmentCode == null ? (Equipment) null : Equipment.CreateFromEquipmentCode(this.EquipmentCode);

    private CharacterCode()
    {
    }

    public static CharacterCode CreateFrom(BasicCharacterObject character)
    {
      CharacterCode characterCode = new CharacterCode();
      string equipmentCode = character.Equipment?.CalculateEquipmentCode();
      characterCode.EquipmentCode = equipmentCode;
      characterCode.BodyProperties = character.GetBodyProperties(character.Equipment);
      characterCode.IsFemale = character.IsFemale;
      characterCode.IsHero = character.IsHero;
      characterCode.FormationClass = character.DefaultFormationClass;
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (CreateFrom));
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(equipmentCode);
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(characterCode.BodyProperties.ToString());
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(characterCode.IsFemale ? "1" : "0");
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(characterCode.IsHero ? "1" : "0");
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(((int) characterCode.FormationClass).ToString());
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(characterCode.Color1.ToString());
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(characterCode.Color2.ToString());
      mbStringBuilder.Append<string>("@---@");
      characterCode.Code = mbStringBuilder.ToStringAndRelease();
      return characterCode;
    }

    public static CharacterCode CreateFrom(
      string equipmentCode,
      BodyProperties bodyProperties,
      bool isFemale,
      bool isHero,
      uint color1,
      uint color2,
      FormationClass formationClass)
    {
      CharacterCode characterCode = new CharacterCode();
      characterCode.EquipmentCode = equipmentCode;
      characterCode.BodyProperties = bodyProperties;
      characterCode.IsFemale = isFemale;
      characterCode.IsHero = isHero;
      characterCode.Color1 = color1;
      characterCode.Color2 = color2;
      characterCode.FormationClass = formationClass;
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (CreateFrom));
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(equipmentCode);
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(characterCode.BodyProperties.ToString());
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(characterCode.IsFemale ? "1" : "0");
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(characterCode.IsHero ? "1" : "0");
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(((int) characterCode.FormationClass).ToString());
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(characterCode.Color1.ToString());
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(characterCode.Color2.ToString());
      mbStringBuilder.Append<string>("@---@");
      characterCode.Code = mbStringBuilder.ToStringAndRelease();
      return characterCode;
    }

    public string CreateNewCodeString()
    {
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (CreateNewCodeString));
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(this.EquipmentCode);
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(this.BodyProperties.ToString());
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(this.IsFemale ? "1" : "0");
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(this.IsHero ? "1" : "0");
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(((int) this.FormationClass).ToString());
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(this.Color1.ToString());
      mbStringBuilder.Append<string>("@---@");
      mbStringBuilder.Append<string>(this.Color2.ToString());
      mbStringBuilder.Append<string>("@---@");
      return mbStringBuilder.ToStringAndRelease();
    }

    public static CharacterCode CreateEmpty() => new CharacterCode();

    public static CharacterCode CreateFrom(string code)
    {
      CharacterCode characterCode = new CharacterCode();
      int startIndex1 = 0;
      int num1;
      for (num1 = code.IndexOf("@---@", StringComparison.InvariantCulture); num1 == startIndex1; num1 = code.IndexOf("@---@", startIndex1, StringComparison.InvariantCulture))
        startIndex1 = num1 + 5;
      string str1 = code.Substring(startIndex1, num1 - startIndex1);
      int startIndex2;
      do
      {
        startIndex2 = num1 + 5;
        num1 = code.IndexOf("@---@", startIndex2, StringComparison.InvariantCulture);
      }
      while (num1 == startIndex2);
      string keyValue = code.Substring(startIndex2, num1 - startIndex2);
      int startIndex3;
      do
      {
        startIndex3 = num1 + 5;
        num1 = code.IndexOf("@---@", startIndex3, StringComparison.InvariantCulture);
      }
      while (num1 == startIndex3);
      string str2 = code.Substring(startIndex3, num1 - startIndex3);
      int startIndex4;
      do
      {
        startIndex4 = num1 + 5;
        num1 = code.IndexOf("@---@", startIndex4, StringComparison.InvariantCulture);
      }
      while (num1 == startIndex4);
      string str3 = code.Substring(startIndex4, num1 - startIndex4);
      int startIndex5;
      do
      {
        startIndex5 = num1 + 5;
        num1 = code.IndexOf("@---@", startIndex5, StringComparison.InvariantCulture);
      }
      while (num1 == startIndex5);
      string str4 = code.Substring(startIndex5, num1 - startIndex5);
      int startIndex6;
      do
      {
        startIndex6 = num1 + 5;
        num1 = code.IndexOf("@---@", startIndex6, StringComparison.InvariantCulture);
      }
      while (num1 == startIndex6);
      string str5 = code.Substring(startIndex6, num1 - startIndex6);
      int startIndex7 = num1 + 5;
      int num2 = code.IndexOf("@---@", startIndex7, StringComparison.InvariantCulture);
      string str6 = num2 >= 0 ? code.Substring(startIndex7, num2 - startIndex7) : code.Substring(startIndex7);
      characterCode.EquipmentCode = str1;
      BodyProperties bodyProperties;
      if (BodyProperties.FromString(keyValue, out bodyProperties))
        characterCode.BodyProperties = bodyProperties;
      characterCode.IsFemale = Convert.ToInt32(str2) == 1;
      characterCode.IsHero = Convert.ToInt32(str3) == 1;
      characterCode.FormationClass = (FormationClass) Convert.ToInt32(str4);
      characterCode.Color1 = Convert.ToUInt32(str5);
      characterCode.Color2 = Convert.ToUInt32(str6);
      characterCode.Code = code;
      return characterCode;
    }
  }
}
