using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using Spectrum.API.Storage;
using Harmony;
using System.Reflection;
using Spectrum.API.Logging;
using static NitronicHUD.Plugin;

namespace NitronicHUD
{
    public class Entry : IPlugin, IUpdatable
    {
        HUD hud = null;
        COUNTDOWN countdown = null;

        public void Initialize(IManager manager, string ipcIdentifier)
        {
            HarmonyInstance.Create("com.Larnin.NitronicHUD").PatchAll(Assembly.GetExecutingAssembly());

            hud = HUD.Create();
            countdown = COUNTDOWN.Create();

            COUNTDOWN_ANNOUNCER.Initialize();
        }

        public void Update()
        {
            COUNTDOWN_ANNOUNCER.Update();
            if (hud != null) hud.update();
            if (countdown != null) countdown.update();
        }

        public static void LogError(string message) => Log.Error($"ERROR: {message}");
    }

    public static class Plugin
    {
        public static Logger Log => new Logger("OutputLog")
        {
            WriteToConsole = true,
            ColorizeLines = true
        };
        public static FileSystem Files => new FileSystem();
    }
}
