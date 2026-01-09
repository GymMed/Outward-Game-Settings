using HarmonyLib;
using OutwardGameSettings.Managers;
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
            if (EnvironmentConditions.Instance.Seasons.Count <= 0 || EnvironmentConditions.Instance.Seasons[0] == null)
                return;

            if (!SeasonsManager.HasInsertableSeason())
                return;

            if(!SeasonsManager.Instance.HasAddedSeasonEffects)
            {
                SeasonsManager.Instance.AddHourEffectsToSeason();
            }
        }
    }
}
