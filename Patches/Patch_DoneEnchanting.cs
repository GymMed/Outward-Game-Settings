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

namespace OutwardGameSettings.Patches
{
    /*
    [HarmonyPatch(typeof(EnchantmentTable), "DoneEnchanting")]
    public class Patch_DoneEnchanting
    {
        static bool Prefix(EnchantmentTable __instance)
        {
#if DEBUG
            SL.Log($"{OutwardGameSettings.prefix} Patch_DoneEnchanting called!");
#endif
            // Only master client decides the outcome
            if (PhotonNetwork.isMasterClient)
            {
                int upgradeChance = ConfigsHelper.GetPercentageValueFromConfig(OutwardGameSettings.EnchantingSuccessChance.Value);
                int randomValue = new System.Random().Next(1, 101);
                bool success = randomValue <= upgradeChance;

                // Send result to all clients using ItemManager’s PhotonView
                var itemManager = ItemManager.Instance;
                if (itemManager != null && itemManager.photonView != null)
                {
                    itemManager.photonView.RPC(
                        "RPC_EnchantingResult",
                        PhotonTargets.All,
                        __instance.UID,
                        success
                    );
                }
                else
                {
                    SL.Log($"{OutwardGameSettings.prefix} ItemManager or its PhotonView is null.");
                }

                // Skip original DoneEnchanting (result handled via RPC)
                return false;
            }

            // Non-masters skip; they'll get the RPC
            return false;
        }
    }*/

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

                    if (__instance.PendingEnchantment?.Name != null && (__instance.m_containedItems?.Values?[0] as Equipment) != null)
                    {
                        Equipment equipment = __instance.m_containedItems.Values[0] as Equipment;

                        NotificationsManager.Instance.BroadcastGlobalSideNotification($"{__instance.PendingEnchantment.Name} on {equipment.DisplayName} failed!");
                    }
                    else
                        NotificationsManager.Instance.BroadcastGlobalSideNotification($"Enchanting failed!");

                    if(OutwardGameSettings.PlayAudioOnEnchantingDone.Value)
                        Global.AudioManager.PlaySoundAtPosition(GlobalAudioManager.Sounds.SFX_BLOCK_Sword_2H, __instance.transform, 0f, 1f, 1f, 1f, 1f);

                    return false;
                }

            }

            if (__instance.PendingEnchantment?.Name != null && (__instance.m_containedItems?.Values?[0] as Equipment) != null)
            {
                Equipment equipment = __instance.m_containedItems.Values[0] as Equipment;

                NotificationsManager.Instance.BroadcastGlobalSideNotification($"{__instance.PendingEnchantment.Name} on {equipment.DisplayName} succeeded!");
            }
            else
                NotificationsManager.Instance.BroadcastGlobalSideNotification($"Enchanting succeeded!");

            if(OutwardGameSettings.PlayAudioOnEnchantingDone.Value)
                Global.AudioManager.PlaySoundAtPosition(GlobalAudioManager.Sounds.SFX_SKILL_GongStrike_Preparation, __instance.transform, 0f, 1f, 1f, 1f, 1f);

            return true;
        }
    }
}
