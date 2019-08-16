using System.Collections.Generic;
using System.IO;
using static NitronicHUD.Util;
using static NitronicHUD.Plugin;
using UnityEngine;

namespace NitronicHUD
{
    public static class COUNTDOWN_ANNOUNCER
    {
        public static bool AllowCustomMusic_ = false;

        public static Dictionary<string, AudioClip> AudioFiles = new Dictionary<string, AudioClip>();

        public static float Volume => Audio.MasterVolume_ * Audio.AnnouncerVolume_;
        private static AudioSettings Audio = G.Sys.OptionsManager_.Audio_;

        public static void Initialize()
        {
            if (!AudioManager.AllowCustomMusic_) return;

            AddClip("StartBeep3.wav");
            AddClip("StartBeep2.wav");
            AddClip("StartBeep1.wav");
            AddClip("StartBeepGo.wav");

            AllowCustomMusic_ = true;
        }

        public static void AddClip(string file)
        {
            FileInfo info = GetFile($"Audio/{file}");
            if (info.Exists)
            {
                AudioFiles.Add(Path.GetFileNameWithoutExtension(info.FullName), new AudioClip(info.FullName));
                Log.Success($"Added clip \"{Path.GetFileNameWithoutExtension(info.FullName)}\"");

            }
        }

        public static AudioClip GetClip(string name)
        {
            return AudioFiles.TryGetValue(name, out AudioClip clip) ? clip : AudioClip.Empty;
        }

        public static void Dispose()
        {
            foreach (KeyValuePair<string, AudioClip> item in AudioFiles.ToArray())
            {
                item.Value.Dispose();
                AudioFiles.Remove(item.Key);
            }
        }

        public static void Update()
        {
            if(AllowCustomMusic_)
                foreach (AudioClip clip in AudioFiles.Values.ToArray())
                    clip.SetVolume(Volume);
        }
    }
}
