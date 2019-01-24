using Spectrum.API;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spectrum.API.Configuration;
using System.IO;
using Harmony;
using System.Reflection;
using Spectrum.API.Experimental;

namespace NitronicHUD
{
    public class Entry : IPlugin, IUpdatable
    {
        const string bundleName = "hud";

        HUD hud = null;

        public void Initialize(IManager manager, string ipcIdentifier)
        {
            var asset = new Assets(bundleName);
            if(asset.Bundle == null)
            {
                LogError("Can't load the " + bundleName + " bundle!");
                return;
            }

            string assetName = "";

            foreach (var n in asset.Bundle.GetAllAssetNames())
            {
                if (n.EndsWith(".prefab"))
                {
                    assetName = n;
                    break;
                }
            }

            if(assetName.Length == 0)
            {
                LogError("The " + bundleName + " bundle doesn't countain a prefab");
                return;
            }

            var obj = asset.Bundle.LoadAsset<GameObject>(assetName);
            if(obj == null)
            {
                LogError("Error when loading the prefab " + assetName + " from bundle " + bundleName);
                return;
            }

            hud = new HUD(obj);
        }

        public void Update()
        {
            if (hud != null)
                hud.update();
        }

        public static void LogError(string message)
        {
            Console.Out.WriteLine("ERROR: " + message);
        }
    }
}
