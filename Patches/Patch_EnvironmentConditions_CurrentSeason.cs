using HarmonyLib;
using OutwardGameSettings.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Patches
{
    [HarmonyPatch(typeof(EnvironmentConditions), "get_CurrentSeason")]
    public class Patch_EnvironmentConditions_CurrentSeason
    {
        static bool Prefix(EnvironmentConditions __instance, ref Season __result)
        {
            var seasons = __instance.Seasons;
            int id = __instance.m_currentSeasonID;

            if (seasons == null || id < 0 || id >= seasons.Count)
            {
                OutwardGameSettings.LogMessage($"Tried to get season! setting {id}");

                SeasonsManager.Instance.TempSeasonIndex = __instance.m_currentSeasonID;
                SeasonsManager.Instance.HasAddedTempSeasonIndex = false;
                int availableSeasonId = GetNotNullSeasonID();

                if(availableSeasonId < 0)
                {
                    availableSeasonId = 0;
                    EnvironmentConditions.Instance.SeasonsEnabled = false;
                }
                __instance.m_currentSeasonID = availableSeasonId;
                __result = seasons[0];
                return false; // skip original getter completely
            }

            var season = seasons[id];
            if (season == null)
            {
                __result = seasons[0];
                return false;
            }

            __result = season;
            return false; // we fully replace the getter
        }

        private static int GetNotNullSeasonID()
        {
            int totalSeasons = EnvironmentConditions.Instance.Seasons.Count();

            for(int currentSeason = 0; currentSeason < totalSeasons; currentSeason++)
            {
                if(EnvironmentConditions.Instance.Seasons[currentSeason] != null)
                {
                    return currentSeason;
                }
            }

            return -1;
        }
    }
}
