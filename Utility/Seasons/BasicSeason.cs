using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Seasons
{
    public class BasicSeason
    {
        Season season;
        CustomSeasons customSeason;
        bool isSafe = false;
        public Action OnHour { get; set; } = null;

        public BasicSeason(Season season, CustomSeasons customSeason)
        {
            this.Season = season;
            this.CustomSeason = customSeason;
        }

        public Season Season { get => season; set => season = value; }
        public CustomSeasons CustomSeason { get => customSeason; set => customSeason = value; }
        public bool IsSafe { get => isSafe; set => isSafe = value; }

        public void init()
        {
            EnvironmentConditions.Instance.Seasons.Add(Season);

            if (EnvironmentConditions.Instance.Seasons.Count > 0)
                Season.transform.SetParent(EnvironmentConditions.Instance.Seasons[0].transform.parent);
            else
            {
                var go = new GameObject("Seasons");
                go.transform.SetParent(EnvironmentConditions.Instance.transform);
                Season.transform.SetParent(go.transform);
            }

            SeasonsManager.FillWeatherEffects(ref WeatherManagerNew.Instance.RainEffects);
            SeasonsManager.FillWeatherEffects(ref WeatherManagerNew.Instance.SnowEffects);
            SeasonsManager.FillWeatherEffects(ref WeatherManagerNew.Instance.SeasonEffects);
        }
    }
}
