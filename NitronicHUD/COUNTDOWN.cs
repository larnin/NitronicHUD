using NitronicHUD.UnityScripts;
using System;
using UnityEngine;

namespace NitronicHUD
{
    public class COUNTDOWN
    {
        GameObject prefab;
        GameObject instance = null;

        Countdown[] digits = new Countdown[4] { null, null, null, null };

        public COUNTDOWN(GameObject _prefab)
        {
            prefab = _prefab;
            if (prefab == null) return;

            Events.GameMode.ModeStarted.Subscribe(data =>
            {
                onMapStart();
            });

            Events.GameMode.AllLocalPlayersFinished.Subscribe(data =>
            {
                onMapEnd();
            });

            Events.Game.PauseToggled.Subscribe(data =>
            {
                onPause(data.paused_);
            });
        }

        void onMapStart()
        {
            if (prefab == null || G.Sys.ReplayManager_.IsReplayMode_) return;

            var gamemode = G.Sys.GameManager_.Mode_;
            if (gamemode != null && gamemode is LevelEditorPlayMode)
                return;

            instance = GameObject.Instantiate(prefab);
            if (instance == null) return;

            digits[0] = instance.transform.FindChild("NR-3").gameObject.AddComponent<Countdown>();
            digits[0].StartKey = -3.5f;
            digits[0].DurationKey = 0.75f;

            digits[1] = instance.transform.FindChild("NR-2").gameObject.AddComponent<Countdown>();
            digits[1].StartKey = -2.5f;
            digits[1].DurationKey = 0.75f;

            digits[2] = instance.transform.FindChild("NR-1").gameObject.AddComponent<Countdown>();
            digits[2].StartKey = -1.5f;
            digits[2].DurationKey = 0.75f;

            digits[3] = instance.transform.FindChild("NR-Rush").gameObject.AddComponent<Countdown>();
            digits[3].StartKey = -0.5f;
            digits[3].DurationKey = 1.0f;

        }

        void onPause(bool paused) => instance.SetActive(!paused);

        void onMapEnd()
        {
            if (instance == null) return;

            GameObject.Destroy(instance);

            instance = null;
            for (int i = 0; i < digits.Length; i++)
                digits[i] = null;
        }

        public void update()
        {
            if (instance == null) return;

            foreach (Countdown digit in digits)
                digit.Time = Convert.ToSingle(Math.Min(Timex.ModeTime_,5.0d));
        }
    }
}