using HarmonyLib;
using OutwardGameSettings.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Patches
{
    [HarmonyPatch(typeof(WorldSave), nameof(WorldSave.PrepareSave))]
    public class Patch_WorldSave_PrepareSave
    {
        static void Postfix()
        {
            DataSerializer.Instance.SaveCurrentCharacterEvents();
        }
    }
}
