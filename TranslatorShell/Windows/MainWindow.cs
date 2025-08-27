using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Configuration;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Lumina.Excel.Sheets;
using TranslatorShell;

namespace TranslatorShell.Windows;

public class MainWindow : Window, IDisposable
{
    private Plugin plugin;

    // We give this window a hidden ID using ##.
    // The user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin)
        : base("TranslatorShell##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        // GoatImagePath = goatImagePath;
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var enableOverlay = plugin.Configuration.EnableOverlay;
        if (ImGui.Checkbox("Enable Overlay", ref enableOverlay))
        {
            plugin.Configuration.EnableOverlay = enableOverlay;
            plugin.Save();
            plugin.TranslationOverlay.OnConfigChanged();
        }

        ImGui.Text("Overlay Font Size");
        float fontSize = plugin.Configuration.OverlayFontScale;
        if (ImGui.SliderFloat("##Overlay Font Size", ref fontSize, 0.9f, 1.6f, "%.1f"))
        {// "##" used to hide the label
            plugin.Configuration.OverlayFontScale = fontSize;
            plugin.Save();
        }

        //ImGui.Text("Reset to Default");
        if (ImGui.Button("Reset to Default", new Vector2(150, 20))) 
        {
            plugin.ResetConfig();
        }
    }
}
