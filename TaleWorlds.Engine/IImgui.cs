// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IImgui
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IImgui
  {
    [EngineMethod("begin_main_thread_scope", false)]
    void BeginMainThreadScope();

    [EngineMethod("end_main_thread_scope", false)]
    void EndMainThreadScope();

    [EngineMethod("push_style_color", false)]
    void PushStyleColor(int style, ref Vec3 color);

    [EngineMethod("pop_style_color", false)]
    void PopStyleColor();

    [EngineMethod("new_frame", false)]
    void NewFrame();

    [EngineMethod("render", false)]
    void Render();

    [EngineMethod("begin", false)]
    void Begin(string text);

    [EngineMethod("begin_with_close_button", false)]
    void BeginWithCloseButton(string text, ref bool is_open);

    [EngineMethod("end", false)]
    void End();

    [EngineMethod("text", false)]
    void Text(string text);

    [EngineMethod("checkbox", false)]
    bool Checkbox(string text, ref bool is_checked);

    [EngineMethod("tree_node", false)]
    bool TreeNode(string name);

    [EngineMethod("tree_pop", false)]
    void TreePop();

    [EngineMethod("separator", false)]
    void Separator();

    [EngineMethod("button", false)]
    bool Button(string text);

    [EngineMethod("plot_lines", false)]
    void PlotLines(
      string name,
      float[] values,
      int valuesCount,
      int valuesOffset,
      string overlayText,
      float minScale,
      float maxScale,
      float graphWidth,
      float graphHeight,
      int stride);

    [EngineMethod("progress_bar", false)]
    void ProgressBar(float value);

    [EngineMethod("new_line", false)]
    void NewLine();

    [EngineMethod("same_line", false)]
    void SameLine(float posX, float spacingWidth);

    [EngineMethod("combo", false)]
    bool Combo(string label, ref int selectedIndex, string items);

    [EngineMethod("input_int", false)]
    bool InputInt(string label, ref int value);

    [EngineMethod("slider_float", false)]
    bool SliderFloat(string label, ref float value, float min, float max);

    [EngineMethod("columns", false)]
    void Columns(int count = 1, string id = "", bool border = true);

    [EngineMethod("next_column", false)]
    void NextColumn();

    [EngineMethod("radio_button", false)]
    bool RadioButton(string label, bool active);

    [EngineMethod("collapsing_header", false)]
    bool CollapsingHeader(string label);

    [EngineMethod("is_item_hovered", false)]
    bool IsItemHovered();

    [EngineMethod("set_tool_tip", false)]
    void SetTooltip(string label);

    [EngineMethod("small_button", false)]
    bool SmallButton(string label);

    [EngineMethod("input_float", false)]
    bool InputFloat(
      string label,
      ref float val,
      float step,
      float stepFast,
      int decimalPrecision = -1);

    [EngineMethod("input_float2", false)]
    bool InputFloat2(string label, ref float val0, ref float val1, int decimalPrecision = -1);

    [EngineMethod("input_float3", false)]
    bool InputFloat3(
      string label,
      ref float val0,
      ref float val1,
      ref float val2,
      int decimalPrecision = -1);

    [EngineMethod("input_float4", false)]
    bool InputFloat4(
      string label,
      ref float val0,
      ref float val1,
      ref float val2,
      ref float val3,
      int decimalPrecision = -1);
  }
}
