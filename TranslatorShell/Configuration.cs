using Dalamud.Configuration;
using System;

namespace TranslatorShell;

/// <summary>
/// Store user configuration for TranslationShell
/// </summary>
[Serializable]
public class Configuration : IPluginConfiguration
{
    /// <summary>
    /// The version of TranslatorShell
    /// </summary>
    public int Version { get; set; } = 0;
    /// <summary>
    /// Enable the overlay or not
    /// Defualt: true
    /// </summary>
    public bool EnableOverlay { get; set; } = true;
    /// <summary>
    /// The scale of font for the overlay
    /// Defualt:1.0f
    /// </summary>
    public float OverlayFontScale { get; set; } = 1.0f;
    /// <summary>
    /// Whether the user has seen the notice or not
    /// Defualt: false
    /// </summary>
    public bool HasSeenNotice { get; set; } = false;
}
