﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.WeaponDesign
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableClass(10005)]
  public class WeaponDesign
  {
    [SaveableField(10)]
    public readonly WeaponFlags WeaponFlags;
    [SaveableField(20)]
    public readonly string WeaponName;
    [SaveableField(30)]
    private readonly WeaponDesignElement[] _usedPieces;
    private string _hashedCode;
    [SaveableField(40)]
    private readonly float[] _piecePivotDistances;
    [SaveableField(60)]
    public readonly float CraftedWeaponLength;
    [SaveableField(70)]
    public readonly CraftingTemplate Template;
    [SaveableField(80)]
    public List<float> TopPivotOffsets;
    [SaveableField(90)]
    public List<float> BottomPivotOffsets;
    [SaveableField(100)]
    public readonly Vec3 HolsterShiftAmount;

    internal static void AutoGeneratedStaticCollectObjectsWeaponDesign(
      object o,
      List<object> collectedObjects)
    {
      ((WeaponDesign) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
    {
      collectedObjects.Add((object) this.Template);
      collectedObjects.Add((object) this.TopPivotOffsets);
      collectedObjects.Add((object) this.BottomPivotOffsets);
      collectedObjects.Add((object) this._usedPieces);
      collectedObjects.Add((object) this._piecePivotDistances);
    }

    internal static object AutoGeneratedGetMemberValueHandToBottomLength(object o) => (object) ((WeaponDesign) o).HandToBottomLength;

    internal static object AutoGeneratedGetMemberValueWeaponFlags(object o) => (object) ((WeaponDesign) o).WeaponFlags;

    internal static object AutoGeneratedGetMemberValueWeaponName(object o) => (object) ((WeaponDesign) o).WeaponName;

    internal static object AutoGeneratedGetMemberValueCraftedWeaponLength(object o) => (object) ((WeaponDesign) o).CraftedWeaponLength;

    internal static object AutoGeneratedGetMemberValueTemplate(object o) => (object) ((WeaponDesign) o).Template;

    internal static object AutoGeneratedGetMemberValueTopPivotOffsets(object o) => (object) ((WeaponDesign) o).TopPivotOffsets;

    internal static object AutoGeneratedGetMemberValueBottomPivotOffsets(object o) => (object) ((WeaponDesign) o).BottomPivotOffsets;

    internal static object AutoGeneratedGetMemberValueHolsterShiftAmount(object o) => (object) ((WeaponDesign) o).HolsterShiftAmount;

    internal static object AutoGeneratedGetMemberValue_usedPieces(object o) => (object) ((WeaponDesign) o)._usedPieces;

    internal static object AutoGeneratedGetMemberValue_piecePivotDistances(object o) => (object) ((WeaponDesign) o)._piecePivotDistances;

    public WeaponDesignElement[] UsedPieces => this._usedPieces;

    public string HashedCode
    {
      get
      {
        if (this._hashedCode == null)
        {
          string str = "";
          foreach (WeaponDesignElement usedPiece in this.UsedPieces)
          {
            if (usedPiece.IsValid)
              str = str + usedPiece.CraftingPiece.StringId + ";" + (object) usedPiece.ScalePercentage + ";";
            else
              str += "invalid_piece;";
          }
          this._hashedCode = Common.CalculateMD5Hash(str + this.Template.StringId);
        }
        return this._hashedCode;
      }
    }

    public float[] PiecePivotDistances => this._piecePivotDistances;

    public float TotalLength => this.CraftedWeaponLength + this.HandToBottomLength;

    [SaveableProperty(50)]
    public float HandToBottomLength { get; private set; }

    public float BottomPivotOffset => this.BottomPivotOffsets[this.BottomPivotOffsets.Count - 1];

    public WeaponDesign(
      CraftingTemplate template,
      string weaponName,
      WeaponDesignElement[] usedPieces)
    {
      this.Template = template;
      this._usedPieces = ((IEnumerable<WeaponDesignElement>) usedPieces).ToArray<WeaponDesignElement>();
      this.WeaponName = weaponName;
      this._piecePivotDistances = new float[usedPieces.Length];
      this.CalculatePivotDistances();
      this.CraftedWeaponLength = this.CalculateWeaponLength();
      this.HolsterShiftAmount = this.CalculateHolsterShiftAmount();
      foreach (WeaponDesignElement usedPiece in usedPieces)
        this.WeaponFlags |= usedPiece.CraftingPiece.AdditionalWeaponFlags;
    }

    public WeaponDesign(WeaponDesign other)
    {
      this.Template = other.Template;
      this._usedPieces = ((IEnumerable<WeaponDesignElement>) other._usedPieces).ToArray<WeaponDesignElement>();
      this.WeaponName = other.WeaponName.ToString();
      this._piecePivotDistances = new float[this._usedPieces.Length];
      this.CalculatePivotDistances();
      this.CraftedWeaponLength = this.CalculateWeaponLength();
      this.HolsterShiftAmount = this.CalculateHolsterShiftAmount();
    }

    private void CalculatePivotDistances()
    {
      float offset1 = 0.0f;
      float offset2 = 0.0f;
      foreach (PieceData buildOrder in this.Template.BuildOrders)
      {
        WeaponDesignElement usedPiece = this.UsedPieces[(int) buildOrder.PieceType];
        if (usedPiece == null || !usedPiece.IsValid)
        {
          this._piecePivotDistances[(int) buildOrder.PieceType] = float.NaN;
        }
        else
        {
          int num = Math.Sign(buildOrder.Order);
          if (num == 0)
          {
            offset2 += usedPiece.ScaledPieceOffset;
            offset1 -= usedPiece.ScaledPieceOffset;
          }
          else if (num < 0)
          {
            offset1 += usedPiece.ScaledDistanceToNextPiece;
            offset1 += usedPiece.ScaledPieceOffset;
            offset1 -= usedPiece.ScaledNextPieceOffset;
          }
          else if (num > 0)
          {
            offset2 += usedPiece.ScaledDistanceToPreviousPiece;
            offset2 += usedPiece.ScaledPieceOffset;
            offset2 -= usedPiece.ScaledPreviousPieceOffset;
          }
          this._piecePivotDistances[(int) buildOrder.PieceType] = (float) ((double) num * (num < 0 ? (double) offset1 : (double) offset2) + (num == 0 ? (double) usedPiece.ScaledPieceOffset : 0.0));
          this.AddTopPivotOffset(offset2);
          this.AddBottomPivotOffset(offset1);
          if (num == 0)
          {
            offset1 += usedPiece.ScaledDistanceToPreviousPiece - usedPiece.ScaledPreviousPieceOffset;
            offset2 += usedPiece.ScaledDistanceToNextPiece - usedPiece.ScaledNextPieceOffset;
          }
          if (num < 0)
            offset1 += usedPiece.ScaledDistanceToPreviousPiece - usedPiece.ScaledPreviousPieceOffset;
          if (num > 0)
            offset2 += usedPiece.ScaledDistanceToNextPiece - usedPiece.ScaledNextPieceOffset;
        }
      }
      this.AddTopPivotOffset(offset2);
      this.AddBottomPivotOffset(offset1);
      this.HandToBottomLength = offset1;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      WeaponDesign weaponDesign = obj as WeaponDesign;
      return (object) weaponDesign != null && this.HashedCode == weaponDesign.HashedCode && this.WeaponName == weaponDesign.WeaponName;
    }

    public override int GetHashCode() => base.GetHashCode();

    public static bool operator ==(WeaponDesign x, WeaponDesign y)
    {
      bool flag1 = (object) x == null;
      bool flag2 = (object) y == null;
      if (flag1 & flag2)
        return true;
      return !flag1 && x.Equals((object) y);
    }

    public static bool operator !=(WeaponDesign x, WeaponDesign y) => !(x == y);

    private void AddTopPivotOffset(float offset)
    {
      if (this.TopPivotOffsets == null)
        this.TopPivotOffsets = new List<float>();
      this.TopPivotOffsets.Add(offset);
    }

    private void AddBottomPivotOffset(float offset)
    {
      if (this.BottomPivotOffsets == null)
        this.BottomPivotOffsets = new List<float>();
      this.BottomPivotOffsets.Add(offset);
    }

    private Vec3 CalculateHolsterShiftAmount()
    {
      WeaponDesignElement usedPiece = this.UsedPieces[2];
      Vec3 vec3 = (this.Template.ItemHolsterPositionShift + usedPiece.CraftingPiece.ItemHolsterPosShift) * usedPiece.ScaleFactor;
      if (this.UsedPieces[1] != null)
        vec3 += Vec3.Up * this.UsedPieces[1].ScaledLength;
      return vec3;
    }

    private float CalculateWeaponLength()
    {
      int index = 0;
      float val1 = this._piecePivotDistances[index] + this._usedPieces[index].ScaledDistanceToNextPiece;
      float val2 = 0.0f;
      foreach (WeaponDesignElement usedPiece in this._usedPieces)
      {
        if (usedPiece.IsValid && (double) usedPiece.ScaledDistanceToNextPiece > (double) val2)
        {
          float scaledPieceOffset = usedPiece.ScaledPieceOffset;
          val2 = usedPiece.ScaledDistanceToNextPiece + scaledPieceOffset;
        }
      }
      return Math.Max(val1, val2);
    }

    public string CalculateUsedPieces()
    {
      string str = "";
      for (int index = 0; index < 4; ++index)
        str = str + this._usedPieces[index].CraftingPiece.StringId + "-";
      return str.Substring(0, str.Length - 1);
    }

    public int[] CalculateTotalMaterialCosts()
    {
      int[] numArray = new int[9];
      foreach (WeaponDesignElement usedPiece in this.UsedPieces)
      {
        IReadOnlyList<int> materialCosts = usedPiece.CraftingPiece.MaterialCosts;
        for (int index = 0; index < numArray.Length; ++index)
          numArray[index] += materialCosts[index];
      }
      return numArray;
    }
  }
}
