using HarmonyLib;
using OutwardGameSettings.BepInEx.Configs;
using OutwardGameSettings.Utility.Enums;
using OutwardGameSettings.Utility.Helpers;
using OutwardGameSettings.Utility.Seasons;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OutwardGameSettings.Managers
{
    public class SeasonsManager
    {
        private static SeasonsManager _instance;

        private SeasonsManager()
        {
        }

        public static SeasonsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SeasonsManager();

                return _instance;
            }
        }

        public bool HasAddedSeasons = false;
        public bool HasAddedSeasonEffects = false;

        public void Init()
        {
            SL.OnSceneLoaded += () =>
            {
                List<Season> seasons = EnvironmentConditions.Instance.Seasons;

                foreach(Season season in seasons)
                {
                    OutwardGameSettings.LogMessage($"name {season.name} day " + $" minT: {season.MinDayTemperature} maxT:{season.MaxDayTemperature}" +
                        $" night minT: {season.MinNightTemperature} maxT: {season.MaxNightTemperature}, start {season.StartMonth} end {season.EndMonth}" +
                        $" fog {season.FogDensity} next {season.NextSeason}");
                }
            };

            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode _) =>
            {
                OutwardGameSettings.LogMessage($"Changed HasAddedSeasons");
                SeasonsManager.Instance.HasAddedSeasons = false;
            };
        }

        public void AddHourEffectsToSeason()
        {
#if DEBUG
            OutwardGameSettings.LogMessage($"SeasonsManager@AddHourEffectsToSeason triggered!");
#endif

            if (EnvironmentConditions.Instance.CurrentSeason == null)
                return;

            foreach (KeyValuePair<CustomSeasons, string> season in CustomSeasonsHelper.SeasonsNames)
            {
                OutwardGameSettings.LogMessage($"SeasonsManager@AddHourEffectsToSeason season: {season.Value} current: {EnvironmentConditions.Instance.CurrentSeason.name}.");
                if (EnvironmentConditions.Instance.CurrentSeason.name.Equals(season.Value, StringComparison.OrdinalIgnoreCase))
                {
                    if (!CustomSeasonsHelper.Seasons.TryGetValue(season.Key, out BasicSeason basicSeason))
                        return;

                    TOD_Time time = TOD_Sky.Instance.TODTime;

                    if (time == null)
                    {
                        OutwardGameSettings.LogMessage($"SeasonsManager@AddHourEffectsToSeason TODTime is missing! Cannot set season effects.");
                        return;
                    }

                    OutwardGameSettings.LogMessage($"SeasonsManager@AddHourEffectsToSeason Removing previous season effects.");
                    CustomSeasonsHelper.RemovePreviousSeasonEffects();
                    if (basicSeason.OnHour != null)
                        time.OnHour += basicSeason.OnHour;

                    HasAddedSeasonEffects = true;
                    break;
                }
            }
        }

        public static bool HasInsertableSeason()
        {
            if(AreaManager.Instance.CurrentArea == null)
            {
                return false;
            }

            AreaManager.AreaEnum currentAreaEnum = AreasHelper.GetAreaEnumFromArea(AreaManager.Instance.CurrentArea);

            switch(currentAreaEnum)
            {
                case AreaManager.AreaEnum.CierzoVillage:
                case AreaManager.AreaEnum.CierzoOutside:
                    {
                        if (SeasonsConfigs.AddFoggySpiritsToChersonese.Value)
                            return true;

                        if (SeasonsConfigs.AddGreatWarToChersonese.Value)
                            return true;
                        break;
                    }
                case AreaManager.AreaEnum.Monsoon:
                case AreaManager.AreaEnum.HallowedMarsh:
                    {
                        if(SeasonsConfigs.AddWinterToEnmerkarForest.Value)
                            return true;

                        if(SeasonsConfigs.AddFoggySpiritsToEnmerkarForest.Value)
                            return true;

                        if(SeasonsConfigs.AddGreatWarToHallowedMarsh.Value)
                            return true;
                        break;
                    }
                case AreaManager.AreaEnum.Berg:
                case AreaManager.AreaEnum.Emercar:
                    {
                        if(SeasonsConfigs.AddWinterToEnmerkarForest.Value)
                            return true;

                        if(SeasonsConfigs.AddFoggySpiritsToEnmerkarForest.Value)
                            return true;

                        if(SeasonsConfigs.AddGreatWarToEnmerkarForest.Value)
                            return true;
                        break;
                    }
                case AreaManager.AreaEnum.Levant:
                case AreaManager.AreaEnum.Abrassar:
                    {
                        if(SeasonsConfigs.AddWinterToAbrassar.Value)
                            return true;

                        if(SeasonsConfigs.AddFoggySpiritsToAbrassar.Value)
                            return true;

                        if(SeasonsConfigs.AddGreatWarToAbrassar.Value)
                            return true;
                        break;
                    }
                case AreaManager.AreaEnum.Harmattan:
                case AreaManager.AreaEnum.AntiqueField:
                    {
                        if(SeasonsConfigs.AddFoggySpiritsToAntiquePlateau.Value)
                            return true;

                        if(SeasonsConfigs.AddGreatWarToAntiquePlateau.Value)
                            return true;
                        break;
                    }
                case AreaManager.AreaEnum.NewSirocco:
                case AreaManager.AreaEnum.Caldera:
                    {
                        if(SeasonsConfigs.AddWinterToCaldera.Value)
                            return true;

                        if(SeasonsConfigs.AddFoggySpiritsToCaldera.Value)
                            return true;

                        if(SeasonsConfigs.AddGreatWarToCaldera.Value)
                            return true;
                        break;
                    }
                default:
                    break;
            }

            return false;
        }

        public static bool TryAddSeasons()
        {
            if(AreaManager.Instance.CurrentArea == null)
            {
                return false;
            }

            AreaManager.AreaEnum currentAreaEnum = AreasHelper.GetAreaEnumFromArea(AreaManager.Instance.CurrentArea);
            bool addedSeasons = false;
            BasicSeason season = null;

            switch(currentAreaEnum)
            {
                case AreaManager.AreaEnum.CierzoVillage:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToChersonese.Value, true))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddGreatWarToChersonese.Value, true, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.CierzoOutside:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToChersonese.Value))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddGreatWarToChersonese.Value, false, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.Monsoon:
                    {
                        if (TryAddSeason(out season, SeasonsConfigs.AddWinterToHallowedMarsh.Value, true, CustomSeasons.Winter))
                            addedSeasons = true;

                        if (TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToHallowedMarsh.Value, true))
                            addedSeasons = true;

                        if (TryAddSeason(out season, SeasonsConfigs.AddGreatWarToHallowedMarsh.Value, true, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.HallowedMarsh:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddWinterToHallowedMarsh.Value, false, CustomSeasons.Winter))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToHallowedMarsh.Value))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddGreatWarToHallowedMarsh.Value, false, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.Berg:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddWinterToEnmerkarForest.Value, true, CustomSeasons.Winter))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToEnmerkarForest.Value, true))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddGreatWarToEnmerkarForest.Value, true, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.Emercar:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddWinterToEnmerkarForest.Value, false, CustomSeasons.Winter))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToEnmerkarForest.Value))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToEnmerkarForest.Value, false, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.Levant:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddWinterToAbrassar.Value, true, CustomSeasons.Winter))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToAbrassar.Value, true))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddGreatWarToAbrassar.Value, true, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.Abrassar:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddWinterToAbrassar.Value, false, CustomSeasons.Winter))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToAbrassar.Value))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddGreatWarToAbrassar.Value, false, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.Harmattan:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToAntiquePlateau.Value, true))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddGreatWarToAntiquePlateau.Value, true, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.AntiqueField:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToAntiquePlateau.Value))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddGreatWarToAntiquePlateau.Value, false, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.NewSirocco:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddWinterToCaldera.Value, true, CustomSeasons.Winter))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToCaldera.Value, true))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToCaldera.Value, true, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                case AreaManager.AreaEnum.Caldera:
                    {
                        if(TryAddSeason(out season, SeasonsConfigs.AddWinterToCaldera.Value, false, CustomSeasons.Winter))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToCaldera.Value))
                            addedSeasons = true;

                        if(TryAddSeason(out season, SeasonsConfigs.AddFoggySpiritsToCaldera.Value, false, CustomSeasons.GreatWar))
                            addedSeasons = true;
                        break;
                    }
                default:
                    break;
            }

            return addedSeasons;
        }

        public static bool TryAddSeason(out BasicSeason retrievedSeason, bool configValue = true, bool isSafe = false, CustomSeasons season = CustomSeasons.FoggySpirits)
        {
            retrievedSeason = null;

            if (!configValue)
                return false;

            if(isSafe)
            {
                Season safeSeason = CustomSeasonsHelper.GetSafeSeason(season);
                retrievedSeason = new BasicSeason(safeSeason, season);
                retrievedSeason.IsSafe = true;
            }
            else
            {
                if (!CustomSeasonsHelper.Seasons.TryGetValue(season, out retrievedSeason))
                    return false;
            }

            if(retrievedSeason == null)
            {
                OutwardGameSettings.LogMessage($"Retrieved null season!");
                return false;
            }

            if(retrievedSeason.Season == null)
            {
                OutwardGameSettings.LogMessage($"Retrieved null basicSeason.Season!");
                return false;
            }

            EnvironmentConditions.Instance.Seasons.Add(retrievedSeason.Season);

            if (EnvironmentConditions.Instance.Seasons.Count > 0)
                retrievedSeason.Season.transform.SetParent(EnvironmentConditions.Instance.Seasons[0].transform.parent);
            else
            {
                var go = new GameObject("Seasons");
                go.transform.SetParent(EnvironmentConditions.Instance.transform);
                retrievedSeason.Season.transform.SetParent(go.transform);
            }

            // will just copy it as a filler for now
            //FillWeatherEffects(ref WeatherManagerNew.Instance.RainEffects);
            //FillWeatherEffects(ref WeatherManagerNew.Instance.SnowEffects);
            //FillWeatherEffects(ref WeatherManagerNew.Instance.SeasonEffects);

            //if(WeatherManagerNew.Instance.SnowEffects.Length < 1 && retrievedSeason.Season.SnowEnabled)
            //{
            //    FillSnowEffectsFromRain();
            //}

            return true;
        }

        public static void FillSnowEffectsFromRain()
        {
            if (WeatherManagerNew.Instance.RainEffects.Length < 1)
                return;

            CopyEffects(
                WeatherManagerNew.Instance.RainEffects, 
                ref WeatherManagerNew.Instance.SnowEffects, 
                WeatherManagerNew.Instance.RainEffects[0].transform.parent
            );
        }

        public static void FillWeatherEffects(ref WeatherFollowCharacter[] targetEffects)
        {
            if (targetEffects.Length < 1)
                return;

            PopulateEffects(
                targetEffects, 
                ref targetEffects, 
                targetEffects[0].transform.parent
            );
        }

        static void CopyEffects(
            WeatherFollowCharacter[] originalArray,
            ref WeatherFollowCharacter[] targetArray,
            Transform parent)
        {
            // If no effects exist (vanilla does this sometimes), skip entirely.
            if (originalArray == null || originalArray.Length == 0)
            {
                OutwardGameSettings.LogMessage("[SeasonMod] No original effects to clone — skipping.");
                return;
            }

            int count = originalArray.Length;
            WeatherFollowCharacter[] newArray = new WeatherFollowCharacter[count];

            for (int i = 0; i < count; i++)
            {
                newArray[i] = originalArray[i];
            }

            targetArray = newArray;
        }

        static void PopulateEffects(
            WeatherFollowCharacter[] originalArray,
            ref WeatherFollowCharacter[] targetArray,
            Transform parent)
        {
            // If no effects exist (vanilla does this sometimes), skip entirely.
            if (originalArray == null || originalArray.Length == 0)
            {
                OutwardGameSettings.LogMessage("[SeasonMod] No original effects to clone — skipping.");
                return;
            }

            int count = originalArray.Length;
            int finalCount = count + 1;
            WeatherFollowCharacter[] newArray = new WeatherFollowCharacter[finalCount];

            for (int i = 0; i < count; i++)
            {
                newArray[i] = originalArray[i];
            }

            if (originalArray.Length > 0)
            {
                WeatherFollowCharacter newWeather = CloneEffect(originalArray[count - 1], parent);
                if (newWeather == null)
                {
                    OutwardGameSettings.LogMessage($"[SeasonMod] Effect slot {count} is null, skipping.");
                }

                newArray[count] = newWeather;
            }

            // Replace with cloned array
            targetArray = newArray;
        }

        static WeatherFollowCharacter CloneEffect(WeatherFollowCharacter src, Transform parent)
        {
            if (src == null) return null;

            GameObject cloneGO = UnityEngine.Object.Instantiate(src.gameObject, parent);
            cloneGO.name = src.gameObject.name + "_Clone";

            WeatherFollowCharacter clone = cloneGO.GetComponent<WeatherFollowCharacter>();

            // --- REQUIRED FIX ---
            //clone.m_particleSystem = cloneGO.GetComponent<ParticleSystem>();
            FI_ParticleSystem.SetValue(clone, cloneGO.GetComponent<ParticleSystem>());

            // m_followCharacter normally stays the same reference
            //clone.m_followCharacter = src.m_followCharacter;
            //FI_FollowCharacter.SetValue(clone, cloneGO.GetComponent<WeatherFollowCharacter>());

            // Copy simple fields
            clone.Offset = src.Offset;

            // Optional: clone Sound
            if (src.Sound != null)
                clone.Sound = cloneGO.GetComponentInChildren<SoundPlayer>();

            return clone;
        }

        static readonly FieldInfo FI_FollowCharacter = 
            AccessTools.Field(typeof(WeatherFollowCharacter), "m_followCharacter");

        static readonly FieldInfo FI_ParticleSystem = 
            AccessTools.Field(typeof(WeatherFollowCharacter), "m_particleSystem");
    }
}
