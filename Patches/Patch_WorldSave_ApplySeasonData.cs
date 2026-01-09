using HarmonyLib;
using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Enums;
using OutwardGameSettings.Utility.Seasons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Patches
{
    [HarmonyPatch(typeof(WorldSave), nameof(WorldSave.ApplySeasonData))]
    public class Patch_WorldSave_ApplySeasonData
    {
        static void Prefix(WorldSave __instance)
        {
            if (EnvironmentConditions.Instance.Seasons.Count <= 0 || EnvironmentConditions.Instance.Seasons[0] == null)
                return;

            if (!SeasonsManager.HasInsertableSeason())
                return;

#if DEBUG
            OutwardGameSettings.LogMessage($"season Data {EnvironmentConditions.Instance.SeasonsEnabled} {EnvironmentConditions.Instance.Seasons.Count} {EnvironmentConditions.Instance.SeasonAreaID}");
#endif

            if(!EnvironmentConditions.Instance.SeasonsEnabled)
            {
                EnvironmentConditions.Instance.SeasonsEnabled = true;
            }

            if(!SeasonsManager.Instance.HasAddedSeasonEffects)
            {
                SeasonsManager.Instance.AddHourEffectsToSeason();
            }

            if (SeasonsManager.Instance.HasAddedSeasons)
                return;

            SeasonsManager.TryAddSeasons();
            SeasonsManager.Instance.HasAddedSeasons = true;
        }
    }
}
