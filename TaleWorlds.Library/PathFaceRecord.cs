// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.PathFaceRecord
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public struct PathFaceRecord
  {
    public int FaceGroupIndex;
    public int FaceIslandIndex;
    public static readonly PathFaceRecord NullFaceRecord = new PathFaceRecord(-1, -1, -1);

    public int FaceIndex { get; private set; }

    public PathFaceRecord(int index, int groupIndex, int islandIndex)
    {
      this.FaceIndex = index;
      this.FaceGroupIndex = groupIndex;
      this.FaceIslandIndex = islandIndex;
    }

    public bool IsValid() => this.FaceIndex != -1;

    public struct StackArray6PathFaceRecord
    {
      private PathFaceRecord _element0;
      private PathFaceRecord _element1;
      private PathFaceRecord _element2;
      private PathFaceRecord _element3;
      private PathFaceRecord _element4;
      private PathFaceRecord _element5;
      public const int Length = 6;

      public PathFaceRecord this[int index]
      {
        get
        {
          switch (index)
          {
            case 0:
              return this._element0;
            case 1:
              return this._element1;
            case 2:
              return this._element2;
            case 3:
              return this._element3;
            case 4:
              return this._element4;
            case 5:
              return this._element5;
            default:
              return PathFaceRecord.NullFaceRecord;
          }
        }
        set
        {
          switch (index)
          {
            case 0:
              this._element0 = value;
              break;
            case 1:
              this._element1 = value;
              break;
            case 2:
              this._element2 = value;
              break;
            case 3:
              this._element3 = value;
              break;
            case 4:
              this._element4 = value;
              break;
            case 5:
              this._element5 = value;
              break;
          }
        }
      }
    }
  }
}
