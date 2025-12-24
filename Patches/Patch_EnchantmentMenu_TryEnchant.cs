using HarmonyLib;
using OutwardGameSettings.BepInEx.Configs;
using OutwardGameSettings.Events;
using OutwardGameSettings.Utility.Helpers;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Patches
{
    [HarmonyPatch(typeof(EnchantmentMenu), "TryEnchant")]
    public class Patch_EnchantmentMenu_TryEnchant
    {
        static bool Prefix(EnchantmentMenu __instance)
        {
#if DEBUG
            SL.Log($"{OutwardGameSettings.prefix} Patch_TryEnchant called!");
#endif
            EventBusPublisher.SendTryEnchant(__instance);

            // If I am sure that errors will occure I let them pass to original method to get caught and print default messages
            if (!__instance.m_refItemInChest)
            {
                return true;
            }
            int enchantmentID = __instance.GetEnchantmentID();

            if (enchantmentID == -1 || __instance.m_refItemInChest.IsEnchanted)
            {
                return true;
            }
            Enchantment enchantment = ResourcesPrefabManager.Instance.GetEnchantmentPrefab(enchantmentID);

            if (enchantment == null)
                return true; // Continue the original method

            if (__instance.m_refEnchantmentStation.ContainedItem == null)
                return true;

            List<EnchantmentRecipeItem> enchantmentItems = EnchantmentsHelper.GetAvailableEnchantmentRecipeItemsInInventory(__instance.m_refEnchantmentStation.ContainedItem, __instance.LocalCharacter.Inventory);

            if(EnchantmentRecipesConfigs.RequireRecipeToAllowEnchant.Value)
            {
                if (!EnchantmentsHelper.IsEnchantmentInList(enchantmentID, enchantmentItems))
                {
                    __instance.m_characterUI.ShowInfoNotification("You need to have enchantment!");
                    return false;
                }
            }

            if(EnchantmentRecipesConfigs.UseRecipeOnEnchanting.Value)
            {

                EnchantmentRecipeItem foundItem = EnchantmentsHelper.GetEnchantmentInTheList(enchantmentID, enchantmentItems);

                if (foundItem)
                {
                    __instance.m_characterUI.ShowInfoNotification($"{foundItem.Name} has been used!");
                    ItemManager.Instance.DestroyItem(foundItem);
                }
            }
            return true; // Continue the original method
        }
    }
}
