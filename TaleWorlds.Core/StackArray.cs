// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.StackArray
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public class StackArray
  {
    public struct StackArray5Float
    {
      private float _element0;
      private float _element1;
      private float _element2;
      private float _element3;
      private float _element4;
      public const int Length = 5;

      public float this[int index]
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
            default:
              return 0.0f;
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
          }
        }
      }
    }

    public struct StackArray4Int
    {
      private int _element0;
      private int _element1;
      private int _element2;
      private int _element3;
      public const int Length = 4;

      public int this[int index]
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
            default:
              return 0;
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
          }
        }
      }
    }
  }
}
