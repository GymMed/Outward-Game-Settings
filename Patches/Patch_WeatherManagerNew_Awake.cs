using HarmonyLib;
using OutwardGameSettings.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Patches
{
    [HarmonyPatch(typeof(WeatherManagerNew), nameof(WeatherManagerNew.Awake))]
    public class Patch_WeatherManagerNew_Awake
    {
        static void Postfix(WeatherManagerNew __instance)
        {
            if (__instance.SnowEffects == null)
                __instance.SnowEffects = new WeatherFollowCharacter[0];
                
            if(__instance.SnowEffects.Length < 1)
                SeasonsManager.FillSnowEffectsFromRain();
        }
    }
}
