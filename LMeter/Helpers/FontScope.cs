using System;
using ImGuiNET;

namespace LMeter.Helpers;

public class FontScope : IDisposable
{
    private bool _fontPushed;

    public FontScope(bool fontPushed)
    {
        _fontPushed = fontPushed;
    }

    public void Dispose()
    {
        if (_fontPushed)
        {
            ImGui.PopFont();
        }
    }
}