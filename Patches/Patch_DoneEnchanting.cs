using System;
using System.Collections;
using System.Reflection;
using Photon;
using UnityEngine;
using HarmonyLib;
using OutwardGameSettings.Utility.Helpers;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using SideLoader;
using OutwardGameSettings.Managers;
using OutwardGameSettings.Events;

namespace OutwardGameSettings.Patches
{
    [HarmonyPatch(typeof(EnchantmentTable), "DoneEnchanting")]
    public class Patch_DoneEnchanting
    {
        static bool Prefix(EnchantmentTable __instance)
        {
#if DEBUG
            SL.Log($"{OutwardGameSettings.prefix} Patch_DoneEnchanting called!");
#endif
            int upgradeChance = ConfigsHelper.GetPercentageValueFromConfig(OutwardGameSettings.EnchantingSuccessChance.Value);

            if (upgradeChance != 100)
            {
                System.Random rand = new System.Random();
                int randomValue = rand.Next(1, 101);

                if (randomValue > upgradeChance)
                {
                    if (__instance.PendingEnchantment?.Name != null && (__instance.m_containedItems?.Values?[0] as Equipment) != null)
                    {
                        Equipment equipment = __instance.m_containedItems.Values[0] as Equipment;

                        NotificationsManager.Instance.BroadcastGlobalSideNotification($"Enchanting {equipment.DisplayName} with {__instance.PendingEnchantment.Name} failed!");
                    }
                    else
                        NotificationsManager.Instance.BroadcastGlobalSideNotification($"Enchanting failed!");


                    __instance.ActivateFX(false);
                    if(!PhotonNetwork.isNonMasterClientInRoom)
                    {
                        __instance.LockPillars(false);
                    }
                    __instance.ConsumeIncenses();
                    __instance.m_pendingEnchantment = null;
                    __instance.m_remainingEnchantTime = -999f;

                    var eventField = typeof(EnchantmentTable).GetField(
                        "OnDoneEnchanting",
                        BindingFlags.Instance | BindingFlags.NonPublic
                    );

                    if (eventField != null)
                    {
                        var eventDelegate = eventField.GetValue(__instance) as EventHandler;
                        if (eventDelegate != null)
                        {
                            eventDelegate.Invoke(__instance, EventArgs.Empty);
                        }
                    }

                    if(OutwardGameSettings.PlayAudioOnEnchantingDone.Value)
                        Global.AudioManager.PlaySoundAtPosition(GlobalAudioManager.Sounds.SFX_BLOCK_Sword_2H, __instance.transform, 0f, 1f, 1f, 1f, 1f);

                    EventBusPublisher.SendFailEnchanting(__instance);
                    return false;
                }

            }

            if (__instance.PendingEnchantment?.Name != null && (__instance.m_containedItems?.Values?[0] as Equipment) != null)
            {
                Equipment equipment = __instance.m_containedItems.Values[0] as Equipment;

                NotificationsManager.Instance.BroadcastGlobalSideNotification($"Enchanting {equipment.DisplayName} with {__instance.PendingEnchantment.Name} succeeded!");
            }
            else
                NotificationsManager.Instance.BroadcastGlobalSideNotification($"Enchanting succeeded!");

            if(OutwardGameSettings.PlayAudioOnEnchantingDone.Value)
                Global.AudioManager.PlaySoundAtPosition(GlobalAudioManager.Sounds.SFX_SKILL_GongStrike_Preparation, __instance.transform, 0f, 1f, 1f, 1f, 1f);

            EventBusPublisher.SendSuccessEnchanting(__instance);
            return true;
        }
    }
}
