using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace NitronicHUD
{
    public class HUD
    {
        const float hudOpacity = 0.6f;
        const float multiplayerAppearDelay = 4.0f;
        const float appearTime = 1.0f;

        float currentOpacity = 0.0f;
        float startTime = 0.0f;
        bool appearFinished = false;
        bool startedLate = false;

        GameObject prefab;

        GameObject instance = null;
        Text scoreText = null;
        Text speedText = null;
        Text timeText = null;
        Image[] huds = new Image[2] { null, null };
        Image[] heatLow = new Image[2] { null, null };
        Image[] heatHight = new Image[2] { null, null };
        Image[] flame = new Image[2] { null, null };
        Graphic[] renderers;

        public HUD(GameObject _prefab)
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

        public void update()
        {
            if (instance == null) return;

            var car = GetCarLogic();
            if (car == null) return;

            if (!appearFinished)
            {
                startTime += Time.deltaTime;
                if (!startedLate)
                {
                    if (startTime < appearTime)
                        currentOpacity = hudOpacity * startTime / appearTime;
                    else
                    {
                        currentOpacity = hudOpacity;
                        appearFinished = true;
                    }
                    updateRenderersOpacity();
                }
                else
                {
                    if (startTime < multiplayerAppearDelay)
                        currentOpacity = 0;
                    else if (startTime - multiplayerAppearDelay < appearTime)
                        currentOpacity = hudOpacity * (startTime - multiplayerAppearDelay) / appearTime;
                    else
                    {
                        currentOpacity = hudOpacity;
                        appearFinished = true;
                    }
                    updateRenderersOpacity();
                }
            }
            else currentOpacity = hudOpacity;

            updateHeat(car.Heat_);

            speedText.text = getSpeedValue().ToString();

            updateTimer();

            updateScore();
        }

        void onMapStart()
        {
            if (prefab == null || G.Sys.ReplayManager_.IsReplayMode_) return;

            var gamemode = G.Sys.GameManager_.Mode_;
            if (gamemode != null && gamemode is LevelEditorPlayMode)
                return;

            instance = GameObject.Instantiate(prefab);
            if (instance == null) return;
            
            var leftHUD = instance.transform.Find("Hud_Left");
            var rightHUD = instance.transform.Find("Hud_Right");
            timeText = instance.transform.Find("Time").GetComponent<Text>();

            setSpeedLabel(rightHUD.transform.Find("Speed_Label").GetComponent<Text>());

            speedText = rightHUD.transform.Find("Speed").GetComponent<Text>();
            scoreText = leftHUD.transform.Find("Score").GetComponent<Text>();

            huds[0] = leftHUD.GetComponent<Image>();
            huds[1] = rightHUD.GetComponent<Image>();

            heatLow[0] = leftHUD.Find("Heat_Low").GetComponent<Image>();
            heatLow[1] = rightHUD.Find("Heat_Low").GetComponent<Image>();

            heatHight[0] = leftHUD.Find("Heat_Hight").GetComponent<Image>();
            heatHight[1] = rightHUD.Find("Heat_Hight").GetComponent<Image>();

            flame[0] = leftHUD.Find("Flame").GetComponent<Image>();
            flame[1] = rightHUD.Find("Flame").GetComponent<Image>();

            renderers = instance.GetComponentsInChildren<Graphic>();

            if (G.Sys.NetworkingManager_.IsOnline_)
                currentOpacity = 0;
            else currentOpacity = hudOpacity;

            updateRenderersOpacity();

            startTime = 0;
            appearFinished = false;
            
            if (gamemode != null)
            {
                var time = gamemode.GetDisplayTime(0);
                startedLate = time > 1.0f;
            }
            else startedLate = false;
        }

        void onMapEnd()
        {
            if (instance == null)
                return;

            GameObject.Destroy(instance);

            instance = null;
            scoreText = null;
            speedText = null;
            timeText = null;
            huds[0] = null;
            huds[1] = null;
            heatLow[0] = null;
            heatLow[1] = null;
            heatHight[0] = null;
            heatHight[1] = null;
            flame[0] = null;
            flame[1] = null;
        }

        void onPause(bool paused) => instance.SetActive(!paused);

        void updateHeat(float heat)
        {
            const float startBlinkAmount = 0.8f;
            const float blinkFrequence = 2f;
            const float startFlameAmount = 0.5f;

            heat = Mathf.Clamp(heat, 0, 1);

            foreach (var h in heatHight)
            {
                h.color = new Color(1, 1, 1, heat * currentOpacity);
                h.fillAmount = heat;
            }
            foreach (var h in heatLow)
                h.fillAmount = heat;

            float blink = 0f;
            if (heat > startBlinkAmount)
                blink = (heat - startBlinkAmount) / (1 - startBlinkAmount);
            blink *= 0.5f * Mathf.Sin(Time.time * blinkFrequence * 3 * Mathf.PI) + 0.5f;
            float colorValue = 1 - blink;

            foreach (var h in huds)
                h.color = new Color(1, colorValue, colorValue, currentOpacity);

            float flameValue = 0f;
            if (heat > startFlameAmount)
                flameValue = (heat - startFlameAmount) / (1 - startFlameAmount);
            foreach (var f in flame)
                f.color = new Color(1, 1, 1, flameValue * currentOpacity);

        }

        void updateTimer()
        {
            var gamemode = G.Sys.GameManager_.Mode_;
            if (gamemode == null)
                return;
            var time = gamemode.GetDisplayTime(0);
            time = Math.Max(time, 0);

            int timeMS = (int)((time - Math.Floor(time)) * 100);
            int timeH = (int)time;
            int timeS = timeH % 60;
            timeH /= 60;
            int timeM = timeH % 60;
            timeH /= 60;

            string sTime = "";
            if (timeH > 0)
                sTime += timeH + ":";
            if (timeM < 10)
                sTime += "0";
            sTime += timeM + ":";
            if (timeS < 10)
                sTime += "0";
            sTime += timeS + ".";
            if (timeMS < 10)
                sTime += "0";
            sTime += timeMS.ToString();
            timeText.text = sTime;
        }

        void updateScore()
        {
            if(G.Sys.PlayerManager_.LocalPlayerCount_ <= 0)
            {
                scoreText.text = "0";
            }
            var data = G.Sys.PlayerManager_.LocalPlayers_[0].playerData_;
            var stats = G.Sys.StatsManager_.GetMatchStats(data);
            scoreText.text = stats.totalPoints_.ToString();
        }

        void updateRenderersOpacity()
        {
            foreach(var r in renderers)
            {
                var color = r.color;
                color.a = currentOpacity;
                r.color = color;
            }
        }

        static void setSpeedLabel(Text text)
        {
            OptionsManager optionsManager = G.Sys.OptionsManager_;
            if (optionsManager == null)
                return;

            if (optionsManager.General_.Units_ == global::Units.Imperial)
                text.text = "MPH";
            else text.text = "KM/H";
        }

        static int getSpeedValue()
        {
            OptionsManager optionsManager = G.Sys.OptionsManager_;
            if (optionsManager == null)
                return 0;

            var carLogic = GetCarLogic();
            if (carLogic == null)
                return 0;

            if (optionsManager.General_.Units_ == global::Units.Imperial)
                return (int)carLogic.CarStats_.GetMilesPerHour();
            else return (int)carLogic.CarStats_.GetKilometersPerHour();
        }

        static CarLogic GetCarLogic()
        {
            var carLogic = G.Sys.PlayerManager_?.Current_?.playerData_?.Car_?.GetComponent<CarLogic>();
            if (carLogic == null)
                carLogic = G.Sys.PlayerManager_?.Current_?.playerData_?.CarLogic_;
            return carLogic;
        }
    }
}
