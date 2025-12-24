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
    [HarmonyPatch(typeof(EnvironmentConditions), nameof(EnvironmentConditions.UpdateSeason))]
    public class Patch_EnvironmentConditions_UpdateSeason
    {
        static void Prefix(EnvironmentConditions __instance)
        {
            if (!EnvironmentConditions.Instance.SeasonsEnabled || __instance.Seasons.Count <= 0 || __instance.Seasons[0] == null)
            {
                if (!SeasonsManager.HasInsertableSeason())
                    return;

#if DEBUG
                OutwardGameSettings.LogMessage($"season Data {EnvironmentConditions.Instance.SeasonsEnabled} {EnvironmentConditions.Instance.Seasons.Count} {EnvironmentConditions.Instance.SeasonAreaID}");
#endif
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
