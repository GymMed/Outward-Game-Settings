using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Patches
{
    [HarmonyPatch(typeof(WeatherManagerNew), nameof(WeatherManagerNew.StopEffects))]
    public class Patch_WeatherManagerNew_StopEffects
    {
        static bool Prefix(WeatherManagerNew __instance, WeatherFollowCharacter[] _effects)
        {
            if(_effects == null || _effects.Length < 1)
                return false;
            return true;
        }
    }
}
