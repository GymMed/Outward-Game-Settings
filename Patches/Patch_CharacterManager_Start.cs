using HarmonyLib;
using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Patches
{
    [HarmonyPatch(typeof(CharacterManager), nameof(CharacterManager.Start))]
    public class Patch_CharacterManager_Start
    {
        static void Postfix()
        {
            DataSerializer.Instance.LoadCurrentCharacterEvents();
        }
    }
}
