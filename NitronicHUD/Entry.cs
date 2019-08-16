using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using UnityEngine;
using System;
using Spectrum.API.Storage;

namespace NitronicHUD
{
    public class Entry : IPlugin, IUpdatable
    {
        const string hudBundleName = "hud";
        const string countdownBundleName = "countdown";

        HUD hud = null;
        COUNTDOWN countdown = null;

        public void Initialize(IManager manager, string ipcIdentifier)
        {
            InitHUD();
            InitCOUNTDOWN();
        }

        public void InitHUD()
        {
            var asset_hud = new Assets(hudBundleName);
            if (asset_hud.Bundle == null)
            {
                LogError("Can't load the " + hudBundleName + " bundle!");
                return;
            }

            string assetName = "";

            foreach (var n in asset_hud.Bundle.GetAllAssetNames())
                if (n.EndsWith(".prefab"))
                {
                    assetName = n;
                    break;
                }

            if (assetName.Length == 0)
            {
                LogError("The " + hudBundleName + " bundle doesn't countain a prefab");
                return;
            }

            var obj = asset_hud.Bundle.LoadAsset<GameObject>(assetName);
            if (obj == null)
            {
                LogError("Error when loading the prefab " + assetName + " from bundle " + hudBundleName);
                return;
            }

            hud = new HUD(obj);
        }

        public void InitCOUNTDOWN()
        {
            var asset_countdown = new Assets(countdownBundleName);
            if (asset_countdown.Bundle == null)
            {
                LogError("Can't load the " + countdownBundleName + " bundle!");
                return;
            }

            string assetName = "Assets/Prefabs/NitronicCountdownHUD.prefab";

            var obj = asset_countdown.Bundle.LoadAsset<GameObject>(assetName);
            if (obj == null)
            {
                LogError("Error when loading the prefab " + assetName + " from bundle " + countdownBundleName);
                return;
            }

            countdown = new COUNTDOWN(obj);
        }

        public void Update()
        {
            if (hud != null) hud.update();
            if (countdown != null) countdown.update();
        }

        public static void LogError(string message) => Console.Out.WriteLine("ERROR: " + message);
    }
}
