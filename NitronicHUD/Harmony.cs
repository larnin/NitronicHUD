using Harmony;

namespace NitronicHUD
{
    [HarmonyPatch(typeof(Phantom), "Play")]
    public class Phantom__Play__Patch
    {
        static void Prefix(string name)
        {
            AnnouncerOptions options = G.Sys.OptionsManager_.Audio_.AnnouncerOptions_;
            if ((options & AnnouncerOptions.AllModes) == AnnouncerOptions.AllModes || (options & AnnouncerOptions.ArcadeOnly) == AnnouncerOptions.ArcadeOnly)
                COUNTDOWN_ANNOUNCER.GetClip(name).PlayFromStart();
        }
    }

    [HarmonyPatch(typeof(AudioManager), "CleanUpNAudio")]
    public class AudioManager__CleanUpNAudio__Patch
    {
        static void Prefix(AudioManager __instance)
        {
            COUNTDOWN_ANNOUNCER.Dispose();
        }
    }
}
