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
    [HarmonyPatch(typeof(EnvironmentConditions), nameof(EnvironmentConditions.StartSeasonTime))]
    public class Patch_EnvironmentConditions_StartSeasonTime
    {
        static void Prefix(EnvironmentConditions __instance)
        {
            SeasonsManager.Instance.HasAddedSeasonEffects = false;
        }
    }
}
