// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.Banner
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
  [SaveableClass(10002)]
  public class Banner
  {
    public const int BannerFullSize = 1536;
    public const int BannerEditableAreaSize = 512;
    public const int MaxIconCount = 32;
    private const char Splitter = '.';
    private const int BackgroundDataIndex = 0;
    private const int BannerIconDataIndex = 1;
    [SaveableField(1)]
    private readonly List<BannerData> _bannerDataList;
    [CachedData]
    private IBannerVisual _bannerVisual;

    public MBReadOnlyList<BannerData> BannerDataList { get; private set; }

    public IBannerVisual BannerVisual
    {
      get
      {
        if (this._bannerVisual == null)
          this._bannerVisual = Game.Current.CreateBannerVisual(this);
        return this._bannerVisual;
      }
    }

    public Banner()
    {
      this._bannerDataList = new List<BannerData>();
      this.BannerDataList = this._bannerDataList.GetReadOnlyList<BannerData>();
    }

    public Banner(Banner banner)
    {
      this._bannerDataList = new List<BannerData>();
      this.BannerDataList = this._bannerDataList.GetReadOnlyList<BannerData>();
      foreach (BannerData bannerData in banner.BannerDataList)
        this._bannerDataList.Add(new BannerData(bannerData));
    }

    public Banner(string bannerKey)
    {
      this._bannerDataList = new List<BannerData>();
      this.BannerDataList = this._bannerDataList.GetReadOnlyList<BannerData>();
      this.Deserialize(bannerKey);
    }

    public Banner(string bannerKey, uint color1, uint color2)
    {
      this._bannerDataList = new List<BannerData>();
      this.BannerDataList = this._bannerDataList.GetReadOnlyList<BannerData>();
      this.Deserialize(bannerKey);
      this.ChangePrimaryColor(color1);
      this.ChangeIconColors(color2);
    }

    [LoadInitializationCallback]
    private void InitReadOnlyLists(MetaData metaData) => this.BannerDataList = this._bannerDataList.GetReadOnlyList<BannerData>();

    public void SetBannerVisual(IBannerVisual visual) => this._bannerVisual = visual;

    public void ChangePrimaryColor(uint mainColor)
    {
      int colorId = BannerManager.GetColorId(mainColor);
      if (colorId < 0)
        return;
      this.BannerDataList[0].ColorId = colorId;
      this.BannerDataList[0].ColorId2 = colorId;
    }

    public void ChangeIconColors(uint color)
    {
      int colorId = BannerManager.GetColorId(color);
      if (colorId < 0)
        return;
      for (int index = 1; index < this.BannerDataList.Count; ++index)
      {
        this.BannerDataList[index].ColorId = colorId;
        this.BannerDataList[index].ColorId2 = colorId;
      }
    }

    public void RotateBackgroundToRight()
    {
      this.BannerDataList[0].RotationValue -= 0.00278f;
      this.BannerDataList[0].RotationValue = (double) this.BannerDataList[0].RotationValue < 0.0 ? this.BannerDataList[0].RotationValue + 1f : this.BannerDataList[0].RotationValue;
    }

    public void RotateBackgroundToLeft()
    {
      this.BannerDataList[0].RotationValue += 0.00278f;
      this.BannerDataList[0].RotationValue = (double) this.BannerDataList[0].RotationValue > 0.0 ? this.BannerDataList[0].RotationValue - 1f : this.BannerDataList[0].RotationValue;
    }

    public void SetBackgroundMeshId(int meshId) => this.BannerDataList[0].MeshId = meshId;

    public string Serialize()
    {
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (Serialize));
      bool flag = true;
      foreach (BannerData bannerData in this.BannerDataList)
      {
        if (!flag)
          mbStringBuilder.Append('.');
        flag = false;
        mbStringBuilder.Append(bannerData.MeshId);
        mbStringBuilder.Append('.');
        mbStringBuilder.Append(bannerData.ColorId);
        mbStringBuilder.Append('.');
        mbStringBuilder.Append(bannerData.ColorId2);
        mbStringBuilder.Append('.');
        mbStringBuilder.Append((int) bannerData.Size.x);
        mbStringBuilder.Append('.');
        mbStringBuilder.Append((int) bannerData.Size.y);
        mbStringBuilder.Append('.');
        mbStringBuilder.Append((int) bannerData.Position.x);
        mbStringBuilder.Append('.');
        mbStringBuilder.Append((int) bannerData.Position.y);
        mbStringBuilder.Append('.');
        mbStringBuilder.Append(bannerData.DrawStroke ? 1 : 0);
        mbStringBuilder.Append('.');
        mbStringBuilder.Append(bannerData.Mirror ? 1 : 0);
        mbStringBuilder.Append('.');
        mbStringBuilder.Append((int) ((double) bannerData.RotationValue / 0.002779999980703));
      }
      return mbStringBuilder.ToStringAndRelease();
    }

    public void Deserialize(string message)
    {
      string[] strArray = message.Split('.');
      this._bannerDataList.Clear();
      this._bannerVisual = (IBannerVisual) null;
      for (int index = 0; index + 10 <= strArray.Length; index += 10)
        this._bannerDataList.Add(new BannerData(int.Parse(strArray[index]), int.Parse(strArray[index + 1]), int.Parse(strArray[index + 2]), new Vec2((float) int.Parse(strArray[index + 3]), (float) int.Parse(strArray[index + 4])), new Vec2((float) int.Parse(strArray[index + 5]), (float) int.Parse(strArray[index + 6])), int.Parse(strArray[index + 7]) == 1, int.Parse(strArray[index + 8]) == 1, (float) int.Parse(strArray[index + 9]) * 0.00278f));
    }

    public void ClearAllIcons()
    {
      BannerData bannerData = this._bannerDataList[0];
      this._bannerDataList.Clear();
      this._bannerDataList.Add(bannerData);
    }

    public void AddIconData(BannerData iconData)
    {
      if (this._bannerDataList.Count >= 33)
        return;
      this._bannerDataList.Add(iconData);
    }

    public static Banner CreateRandomClanBanner(int seed = -1) => Banner.CreateRandomBannerInternal(seed, Banner.BannerIconOrientation.CentralPositionedOneIcon);

    public static Banner CreateRandomBanner(int seed = -1) => Banner.CreateRandomBannerInternal(seed);

    private static Banner CreateRandomBannerInternal(
      int seed = -1,
      Banner.BannerIconOrientation orientation = Banner.BannerIconOrientation.None)
    {
      Game current = Game.Current;
      Random random = seed == -1 ? MBRandom.Random : new Random(seed);
      Banner banner = new Banner();
      BannerData iconData = new BannerData(BannerManager.Instance.GetRandomBackgroundId(random), random.Next(BannerManager.ColorPalette.Count), random.Next(BannerManager.ColorPalette.Count), new Vec2(1536f, 1536f), new Vec2(768f, 768f), false, false, 0.0f);
      banner.AddIconData(iconData);
      switch (orientation == Banner.BannerIconOrientation.None ? random.Next(6) : (int) orientation)
      {
        case 0:
          banner.CentralPositionedOneIcon(random);
          break;
        case 1:
          banner.CenteredTwoMirroredIcons(random);
          break;
        case 2:
          banner.DiagonalIcons(random);
          break;
        case 3:
          banner.HorizontalIcons(random);
          break;
        case 4:
          banner.VerticalIcons(random);
          break;
        case 5:
          banner.SquarePositionedFourIcons(random);
          break;
      }
      return banner;
    }

    public static Banner CreateOneColoredEmptyBanner(int colorIndex)
    {
      Banner banner = new Banner();
      banner.AddIconData(new BannerData(BannerManager.Instance.GetRandomBackgroundId(new Random()), colorIndex, colorIndex, new Vec2(1536f, 1536f), new Vec2(768f, 768f), false, false, 0.0f));
      return banner;
    }

    public static Banner CreateOneColoredBannerWithOneIcon(
      uint backgroundColor,
      uint iconColor,
      int iconMeshId)
    {
      Banner coloredEmptyBanner = Banner.CreateOneColoredEmptyBanner(BannerManager.GetColorId(backgroundColor));
      if (iconMeshId == -1)
        iconMeshId = BannerManager.Instance.GetRandomBannerIconId(MBRandom.Random);
      coloredEmptyBanner.AddIconData(new BannerData(iconMeshId, BannerManager.GetColorId(iconColor), BannerManager.GetColorId(iconColor), new Vec2(512f, 512f), new Vec2(768f, 768f), false, false, 0.0f));
      return coloredEmptyBanner;
    }

    private void CentralPositionedOneIcon(Random random)
    {
      int randomBannerIconId = BannerManager.Instance.GetRandomBannerIconId(random);
      int num1 = random.Next(BannerManager.ColorPalette.Count);
      bool hasStroke = (double) random.NextFloat() < 0.5;
      int colorIdForStroke = this.GetRandomColorIdForStroke(hasStroke, random);
      bool flag = random.Next(2) == 0;
      float num2 = random.NextFloat();
      float num3 = 0.0f;
      if ((double) num2 > 0.899999976158142)
        num3 = 0.25f;
      else if ((double) num2 > 0.800000011920929)
        num3 = 0.5f;
      else if ((double) num2 > 0.699999988079071)
        num3 = 0.75f;
      int colorId = num1;
      int colorId2 = colorIdForStroke;
      Vec2 size = new Vec2(512f, 512f);
      Vec2 position = new Vec2(768f, 768f);
      int num4 = hasStroke ? 1 : 0;
      int num5 = flag ? 1 : 0;
      double num6 = (double) num3;
      this.AddIconData(new BannerData(randomBannerIconId, colorId, colorId2, size, position, num4 != 0, num5 != 0, (float) num6));
    }

    private void DiagonalIcons(Random random)
    {
      int num1 = (double) random.NextFloat() < 0.5 ? 2 : 3;
      bool flag1 = (double) random.NextFloat() < 0.5;
      int num2 = (512 - 20 * (num1 + 1)) / num1;
      int meshId = BannerManager.Instance.GetRandomBannerIconId(random);
      int colorId = random.Next(BannerManager.ColorPalette.Count);
      bool flag2 = (double) random.NextFloat() < 0.5;
      int colorIdForStroke = this.GetRandomColorIdForStroke(flag2, random);
      int num3 = (512 - num1 * num2) / (num1 + 1);
      bool flag3 = (double) random.NextFloat() < 0.300000011920929;
      bool flag4 = flag3 || (double) random.NextFloat() < 0.300000011920929;
      for (int index = 0; index < num1; ++index)
      {
        meshId = flag3 ? BannerManager.Instance.GetRandomBannerIconId(random) : meshId;
        colorId = flag4 ? random.Next(BannerManager.ColorPalette.Count) : colorId;
        int num4 = index * (num2 + num3) + num3 + num2 / 2;
        int num5 = index * (num2 + num3) + num3 + num2 / 2;
        if (flag1)
          num5 = 512 - num5;
        this.AddIconData(new BannerData(meshId, colorId, colorIdForStroke, new Vec2((float) num2, (float) num2), new Vec2((float) (num4 + 512), (float) (num5 + 512)), flag2, false, 0.0f));
      }
    }

    private void HorizontalIcons(Random random)
    {
      int num1 = (double) random.NextFloat() < 0.5 ? 2 : 3;
      int num2 = (512 - 20 * (num1 + 1)) / num1;
      int meshId = BannerManager.Instance.GetRandomBannerIconId(random);
      int colorId = random.Next(BannerManager.ColorPalette.Count);
      bool flag1 = (double) random.NextFloat() < 0.5;
      int colorIdForStroke = this.GetRandomColorIdForStroke(flag1, random);
      int num3 = (512 - num1 * num2) / (num1 + 1);
      bool flag2 = (double) random.NextFloat() < 0.300000011920929;
      bool flag3 = flag2 || (double) random.NextFloat() < 0.300000011920929;
      for (int index = 0; index < num1; ++index)
      {
        meshId = flag2 ? BannerManager.Instance.GetRandomBannerIconId(random) : meshId;
        colorId = flag3 ? random.Next(BannerManager.ColorPalette.Count) : colorId;
        int num4 = index * (num2 + num3) + num3 + num2 / 2;
        this.AddIconData(new BannerData(meshId, colorId, colorIdForStroke, new Vec2((float) num2, (float) num2), new Vec2((float) (num4 + 512), 768f), flag1, false, 0.0f));
      }
    }

    private void VerticalIcons(Random random)
    {
      int num1 = (double) random.NextFloat() < 0.5 ? 2 : 3;
      int num2 = (512 - 20 * (num1 + 1)) / num1;
      int meshId = BannerManager.Instance.GetRandomBannerIconId(random);
      int colorId = random.Next(BannerManager.ColorPalette.Count);
      bool flag1 = (double) random.NextFloat() < 0.5;
      int colorIdForStroke = this.GetRandomColorIdForStroke(flag1, random);
      int num3 = (512 - num1 * num2) / (num1 + 1);
      bool flag2 = (double) random.NextFloat() < 0.300000011920929;
      bool flag3 = flag2 || (double) random.NextFloat() < 0.300000011920929;
      for (int index = 0; index < num1; ++index)
      {
        meshId = flag2 ? BannerManager.Instance.GetRandomBannerIconId(random) : meshId;
        colorId = flag3 ? random.Next(BannerManager.ColorPalette.Count) : colorId;
        int num4 = index * (num2 + num3) + num3 + num2 / 2;
        this.AddIconData(new BannerData(meshId, colorId, colorIdForStroke, new Vec2((float) num2, (float) num2), new Vec2(768f, (float) (num4 + 512)), flag1, false, 0.0f));
      }
    }

    private void SquarePositionedFourIcons(Random random)
    {
      bool mirror = (double) random.NextFloat() < 0.5;
      int num = mirror ? 0 : ((double) random.NextFloat() < 0.5 ? 1 : 0);
      bool flag1 = num != 0 || (double) random.NextFloat() < 0.5;
      bool flag2 = (double) random.NextFloat() < 0.5;
      int randomBannerIconId = BannerManager.Instance.GetRandomBannerIconId(random);
      int colorIdForStroke = this.GetRandomColorIdForStroke(flag2, random);
      int colorId1 = random.Next(BannerManager.ColorPalette.Count);
      this.AddIconData(new BannerData(randomBannerIconId, colorId1, colorIdForStroke, new Vec2(220f, 220f), new Vec2(658f, 658f), flag2, false, 0.0f));
      int meshId1 = num != 0 ? BannerManager.Instance.GetRandomBannerIconId(random) : randomBannerIconId;
      int colorId2 = flag1 ? random.Next(BannerManager.ColorPalette.Count) : colorId1;
      this.AddIconData(new BannerData(meshId1, colorId2, colorIdForStroke, new Vec2(220f, 220f), new Vec2(878f, 658f), flag2, mirror, 0.0f));
      int meshId2 = num != 0 ? BannerManager.Instance.GetRandomBannerIconId(random) : meshId1;
      int colorId3 = flag1 ? random.Next(BannerManager.ColorPalette.Count) : colorId2;
      this.AddIconData(new BannerData(meshId2, colorId3, colorIdForStroke, new Vec2(220f, 220f), new Vec2(658f, 878f), flag2, mirror, mirror ? 0.5f : 0.0f));
      this.AddIconData(new BannerData(num != 0 ? BannerManager.Instance.GetRandomBannerIconId(random) : meshId2, flag1 ? random.Next(BannerManager.ColorPalette.Count) : colorId3, colorIdForStroke, new Vec2(220f, 220f), new Vec2(878f, 878f), flag2, false, mirror ? 0.5f : 0.0f));
    }

    private void CenteredTwoMirroredIcons(Random random)
    {
      bool flag1 = (double) random.NextFloat() < 0.5;
      bool flag2 = (double) random.NextFloat() < 0.5;
      int randomBannerIconId = BannerManager.Instance.GetRandomBannerIconId(random);
      int colorIdForStroke = this.GetRandomColorIdForStroke(flag2, random);
      int colorId = random.Next(BannerManager.ColorPalette.Count);
      this.AddIconData(new BannerData(randomBannerIconId, colorId, colorIdForStroke, new Vec2(200f, 200f), new Vec2(668f, 768f), flag2, false, 0.0f));
      this.AddIconData(new BannerData(randomBannerIconId, flag1 ? random.Next(BannerManager.ColorPalette.Count) : colorId, colorIdForStroke, new Vec2(200f, 200f), new Vec2(868f, 768f), flag2, true, 0.0f));
    }

    private int GetRandomColorIdForStroke(bool hasStroke, Random random) => !hasStroke ? BannerManager.ColorPalette.Count - 1 : random.Next(BannerManager.ColorPalette.Count);

    public uint GetPrimaryColor() => !this.BannerDataList.Any<BannerData>() ? uint.MaxValue : BannerManager.GetColor(this.BannerDataList[0].ColorId);

    public uint GetFirstIconColor() => !this.BannerDataList.Any<BannerData>() ? uint.MaxValue : BannerManager.GetColor(this.BannerDataList[1].ColorId);

    internal static void AutoGeneratedStaticCollectObjectsBanner(
      object o,
      List<object> collectedObjects)
    {
      ((Banner) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects) => collectedObjects.Add((object) this._bannerDataList);

    internal static object AutoGeneratedGetMemberValue_bannerDataList(object o) => (object) ((Banner) o)._bannerDataList;

    private enum BannerIconOrientation
    {
      None = -1, // 0xFFFFFFFF
      CentralPositionedOneIcon = 0,
      CenteredTwoMirroredIcons = 1,
      DiagonalIcons = 2,
      HorizontalIcons = 3,
      VerticalIcons = 4,
      SquarePositionedFourIcons = 5,
      NumberOfOrientation = 6,
    }
  }
}
