using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Seasons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Enums
{
    public enum CustomSeasons
    {
        Winter,
        FoggySpirits,
        GreatWar
    }

    public static class CustomSeasonsHelper
    {
        private static Dictionary<CustomSeasons, BasicSeason> _seasons;

        public static Dictionary<CustomSeasons, BasicSeason> Seasons
        {
            get
            {
                if (_seasons == null || AreSeasonsInvalid())
                {
                    RebuildSeasons();
                }
                return _seasons;
            }
        }

        private static void RebuildSeasons()
        {
            #if DEBUG
                OutwardGameSettings.LogMessage("Rebuilding custom seasons (scene reload detected)");
            #endif

            if (_seasons != null)
            {
                foreach (var pair in _seasons)
                {
                    if (pair.Value?.Season != null)
                        UnityEngine.Object.Destroy(pair.Value.Season.gameObject);
                }
            }

            _seasons = new Dictionary<CustomSeasons, BasicSeason>
            {
                { CustomSeasons.Winter, GetWinterSeason() },
                { CustomSeasons.FoggySpirits, GetFoggySpiritsSeason() },
                { CustomSeasons.GreatWar, GetGreatWarSeason() }
            };
        }

        public static readonly Dictionary<CustomSeasons, string> SeasonsNames = new()
        {
            { CustomSeasons.Winter, "GameSettings_Winter_Season" },
            { CustomSeasons.FoggySpirits, "GameSettings_Foggy_Spirits_Season" },
            { CustomSeasons.GreatWar, "GameSettings_Great_War_Season" },
        };

        public static Season GetSafeSeason(CustomSeasons season)
        {
            Season finalSeason = null;

            switch(season)
            {
                case CustomSeasons.Winter:
                    {
                        finalSeason = GetWinterSeason().Season;
                        break;
                    }
                case CustomSeasons.GreatWar:
                    {
                        finalSeason = GetGreatWarSeason().Season;
                        break;
                    }
                case CustomSeasons.FoggySpirits:
                default:
                    {
                        finalSeason = GetFoggySpiritsSeason().Season;
                        break;
                    }
            }

            return MakeSeasonSafe(finalSeason);
        }

        public static Season MakeSeasonSafe(Season originalSeason)
        {
            originalSeason.MaxNightTemperature = TemperatureSteps.Neutral;
            originalSeason.MinNightTemperature = TemperatureSteps.Neutral;

            originalSeason.MaxDayTemperature = TemperatureSteps.Neutral;
            originalSeason.MinDayTemperature = TemperatureSteps.Neutral;

            return originalSeason;
        }

        public static BasicSeason GetWinterSeason()
        {
            if (!SeasonsNames.TryGetValue(CustomSeasons.Winter, out string name))
                return null;

            var go = new GameObject(name);
            Season winter = go.AddComponent<Season>();

            winter.name = name;

            winter.WinterLerp = MakeAnimationCurve();
            winter.AutumnLerp = MakeAnimationCurve();
            winter.SnowEnabled = true;
            winter.SnowingEnabled = true;
            winter.DayTemperatureTransition = new AnimationCurve();

            winter.MaxNightTemperature = TemperatureSteps.Coldest;
            winter.MinNightTemperature = TemperatureSteps.Coldest;

            winter.MaxDayTemperature = TemperatureSteps.VeryCold;
            winter.MinDayTemperature = TemperatureSteps.VeryCold;

            winter.WeatherProgressClamp = new Vector2();
            winter.StartMonth = 1;
            winter.EndMonth = 3;
            winter.NextSeason = new Vector2(360, 480);

            winter.FogDensity = 0.02f;
            winter.EventThreshold = 1f;

            BasicSeason basicSeason = new BasicSeason(winter, CustomSeasons.Winter);
            basicSeason.IsSafe = false;

            EnvironmentConditions.Instance.MinMaxSnowQuantity = new Vector2(2.5f, 5);

            return basicSeason;
        }

        public static AnimationCurve MakeAnimationCurve()
        {
            AnimationCurve curve = new AnimationCurve();

            curve.keys = new Keyframe[] 
            {
                new(),
                new(),
            };

            for(int currentKey = 0; currentKey < curve.keys.Length; currentKey++)
            {
                curve.keys[currentKey].inWeight = 0.3333f;
                curve.keys[currentKey].outWeight = 0.3333f;
                curve.keys[currentKey].value = 1f;
            }

            curve.postWrapMode = WrapMode.ClampForever;
            curve.preWrapMode = WrapMode.ClampForever;

            return curve;
        }

        public static BasicSeason GetFoggySpiritsSeason()
        {
            if (!SeasonsNames.TryGetValue(CustomSeasons.FoggySpirits, out string name))
                return null;

            var go = new GameObject(name);
            Season foggySpirits = go.AddComponent<Season>();

            foggySpirits.name = name;

            foggySpirits.WinterLerp = MakeAnimationCurve();
            foggySpirits.AutumnLerp = MakeAnimationCurve();
            foggySpirits.SnowEnabled = true;
            foggySpirits.SnowingEnabled = true;
            foggySpirits.DayTemperatureTransition = new AnimationCurve();

            foggySpirits.MaxNightTemperature = TemperatureSteps.VeryCold;
            foggySpirits.MinNightTemperature = TemperatureSteps.Cold;

            foggySpirits.MaxDayTemperature = TemperatureSteps.VeryHot;
            foggySpirits.MinDayTemperature = TemperatureSteps.Warm;

            foggySpirits.WeatherProgressClamp = new Vector2();
            foggySpirits.StartMonth = 1;
            foggySpirits.EndMonth = 3;
            foggySpirits.NextSeason = new Vector2(360, 480);

            foggySpirits.FogDensity = 0.1f;
            foggySpirits.EventThreshold = 0.5f;

            BasicSeason basicSeason = new BasicSeason(foggySpirits, CustomSeasons.Winter);
            basicSeason.IsSafe = false;
            basicSeason.OnHour += () =>
            {
                float lowestFog = Mathf.Clamp(foggySpirits.FogDensity - 0.05f, 0, 0.2f);
                float highestFog = Mathf.Clamp(foggySpirits.FogDensity + 0.05f, 0, 0.2f);
                int shouldIncrease = UnityEngine.Random.Range(0, 1);
                float nextFog = 0f;
                float minChange = 0.015f;

                if(shouldIncrease == 1)
                    nextFog = UnityEngine.Random.Range(foggySpirits.FogDensity + minChange, highestFog);
                else
                    nextFog = UnityEngine.Random.Range(lowestFog, foggySpirits.FogDensity + minChange);
                OutwardGameSettings.LogMessage($"Setting fog by Foggy Spirits season.");

                // only used for being unity MonoBehaviour
                EnemyWaveManager.Instance.StartCoroutine(ShiftFogTo(nextFog));
            };

            return basicSeason;
        }

        public static IEnumerator ShiftFogTo(float nextFogDensity, float stepSize = 0.002f)
        {
            if(EnvironmentConditions.Instance.CurrentSeason.FogDensity < nextFogDensity)
            {
                while(EnvironmentConditions.Instance.CurrentSeason.FogDensity < nextFogDensity)
                {
                    EnvironmentConditions.Instance.CurrentSeason.FogDensity += stepSize;
                    yield return new WaitForSeconds(0.3f); // delay between fog changes for immersion
                }
            }
            else
            {
                while(EnvironmentConditions.Instance.CurrentSeason.FogDensity > nextFogDensity)
                {
                    EnvironmentConditions.Instance.CurrentSeason.FogDensity -= stepSize;
                    yield return new WaitForSeconds(0.3f);
                }
            }
        }

        public static BasicSeason GetGreatWarSeason()
        {
            if (!SeasonsNames.TryGetValue(CustomSeasons.GreatWar, out string name))
                return null;

            var go = new GameObject(name);
            Season foggySpirits = go.AddComponent<Season>();

            foggySpirits.name = name;

            foggySpirits.WinterLerp = MakeAnimationCurve();
            foggySpirits.AutumnLerp = MakeAnimationCurve();
            foggySpirits.SnowEnabled = true;
            foggySpirits.SnowingEnabled = true;
            foggySpirits.DayTemperatureTransition = new AnimationCurve();

            foggySpirits.MaxNightTemperature = TemperatureSteps.Cold;
            foggySpirits.MinNightTemperature = TemperatureSteps.Neutral;

            foggySpirits.MaxDayTemperature = TemperatureSteps.Warm;
            foggySpirits.MinDayTemperature = TemperatureSteps.Fresh;

            foggySpirits.WeatherProgressClamp = new Vector2();
            foggySpirits.StartMonth = 1;
            foggySpirits.EndMonth = 3;
            foggySpirits.NextSeason = new Vector2(360, 480);

            foggySpirits.FogDensity = 0.008f;
            foggySpirits.EventThreshold = 1.0f;

            BasicSeason basicSeason = new BasicSeason(foggySpirits, CustomSeasons.Winter);
            basicSeason.IsSafe = false;
            basicSeason.OnHour += () =>
            {
                int spawnEnemyType = UnityEngine.Random.Range(0, 4);

                OutwardGameSettings.LogMessage($"Setting enemies by Great War season.");

                switch (spawnEnemyType)
                {
                    case 0:
                        {
                            EnemyWaveManager.Instance.StartWarOfRandomSize();
                            break;
                        }
                    case 1:
                        {
                            EnemyWaveManager.Instance.SpawnRandomWave(false);
                            break;
                        }
                    case 2:
                        {
                            EnemyWaveManager.Instance.SpawnRandomWanderer();
                            break;
                        }
                    case 3:
                    default:
                        {
                            return;
                        }
                }
            };

            return basicSeason;
        }
        
        public static void RemovePreviousSeasonEffects()
        {
            TOD_Time time = TOD_Sky.Instance.TODTime;

            if (time == null)
            {
                OutwardGameSettings.LogMessage($"Patch_EnvironmentConditions_StartSeasonTime@Prefix TODTime is missing! Cannot set season effects.");
                return;
            }

            foreach (KeyValuePair<CustomSeasons, BasicSeason> season in CustomSeasonsHelper.Seasons)
            {
                time.OnHour -= season.Value.OnHour;
            }
        }

        public static bool AreSeasonsInvalid()
        {
            if (_seasons == null)
                return true;

            foreach (var kvp in _seasons)
            {
                if (kvp.Value == null)
                    return true;

                if (kvp.Value.Season == null) // Unity destroyed
                    return true;
            }
            return false;
        }
    }
}
