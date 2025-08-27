using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using TranslatorShell.Windows;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text;
using TranslatorShell.Services;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using System.Numerics;
using System;

namespace TranslatorShell;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; }= null!;
    //[PluginService] internal static IDisposable disposable { get; private set; } = null!;

    private const string CommandName = "/tsconfig";

    public Configuration Configuration { get; private set; }

    public readonly WindowSystem WindowSystem = new("TranslatorShell");
    private MainWindow MainWindow { get; init; }
    public TranslationOverlay TranslationOverlay { get; private set; }

    private readonly TranslatorClient translatorClient = new();

    // Have seen the notice or not
    private bool showNotice = true;


    public Plugin()
    {
        // Make sure initialize the configuration
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        ChatGui.ChatMessage += OnChatMessage;

        TranslationOverlay = new TranslationOverlay(this);
        WindowSystem.AddWindow(TranslationOverlay);

        // This adds a button to the plugin installer entry of this plugin which allows
        // toggling the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += OpenConfigUI;

        // Adds another button doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        Log.Information($"===Successfully load {PluginInterface.Manifest.Name}===");
    }

    public void Save()
    {
        PluginInterface.SavePluginConfig(Configuration);
    }
    public void ResetConfig()
    {
        Configuration = new Configuration();
        Save();
    }

    private const string PluginTag = "[TranslatorShell]";

   private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        var plainText = message.TextValue;

        if (string.IsNullOrWhiteSpace(plainText)) return;
        if (plainText.StartsWith(PluginTag)) return;

        if(type == XivChatType.NPCDialogue || type == XivChatType.CrossLinkShell2) {
            _= HandleTranslationAsync(plainText);
        }
    }
    // To prevent being stuck in the I/O request
    private async Task HandleTranslationAsync(string plainText)
    {
        var result = await translatorClient.TranslateAsync(plainText);
        TranslationOverlay.SetText(result);
    }

    private void OpenConfigUI()
    {
        MainWindow.IsOpen = true;
    }


    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        MainWindow.Dispose();

        PluginInterface.UiBuilder.Draw -= DrawUI;

        PluginInterface.UiBuilder.OpenConfigUi -= OpenConfigUI;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUI;

        CommandManager.RemoveHandler(CommandName);

        ChatGui.ChatMessage -= OnChatMessage;
    }

    private void OnCommand(string command, string args)
    {
        // In response to the slash command, toggle the display status of our main ui
        ToggleMainUI();
    }

    private void NoticePopUp()
    {
        if (showNotice)
        {
            ImGui.OpenPopup("TranslatorShell Notice");
        }
        // create a notice window for user to switch the language setting of the Dalamud
        var vp = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(vp.GetCenter(), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));

        ImGui.SetNextWindowSize(new Vector2(400, 0), ImGuiCond.Appearing);
        if (ImGui.BeginPopupModal("TranslatorShell Notice", ref showNotice, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoMove))
        {
            ImGui.TextWrapped("To display Chinese correctly in the overlay, please set Dalamud UI Language to simplified Chinese.");
            ImGui.Separator();

            var w = ImGui.GetWindowWidth();
            var btn = new Vector2(220, 0);
            ImGui.SetCursorPosX((w - btn.X) * 0.5f);
            if (ImGui.Button("OK, don't show again", btn))
            {
                showNotice = false;
                Configuration.HasSeenNotice = true;
                Save();
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
    }

    private void DrawUI()
    {
        WindowSystem.Draw();
        NoticePopUp();
    }
    public void ToggleMainUI() => MainWindow.Toggle();

}
