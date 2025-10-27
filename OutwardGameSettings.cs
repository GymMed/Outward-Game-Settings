using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using OutwardModsCommunicator.EventBus;
using OutwardGameSettings.Utility.Helpers;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using OutwardGameSettings.Events;

// RENAME 'OutwardGameSettings' TO SOMETHING ELSE
namespace OutwardGameSettings
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class OutwardGameSettings : BaseUnityPlugin
    {
        // Choose a GUID for your project. Change "myname" and "mymod".
        public const string GUID = "gymmed.outward_game_settings";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "Outward Game Settings";
        // Increment the VERSION when you release a new version of your mod.
        public const string VERSION = "1.0.1";

        public static string prefix = "[GymMed-Game-Settings]";

        internal static ManualLogSource Log;

        // If you need settings, define them like so:
        public static ConfigEntry<bool> RequireRecipeToAllowEnchant;
        public static ConfigEntry<bool> UseRecipeOnEnchanting;
        public static ConfigEntry<int> EnchantingSuccessChance;
        public static ConfigEntry<bool> PlayAudioOnEnchantingDone;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"Hello world from {NAME} {VERSION}!");

            // Any config settings you define should be set up like this:
            //ExampleConfig = Config.Bind("ExampleCategory", "ExampleSetting", false, "This is an example setting.");
            RequireRecipeToAllowEnchant = Config.Bind(
                "Enchanting Modifications",
                "RequireRecipeToAllowEnchant",
                true,
                "Allow enchanting only if enchantment is on character?"
            );

            UseRecipeOnEnchanting = Config.Bind(
                "Enchanting Modifications",
                "UseRecipeOnEnchanting",
                true,
                "Remove recipe after using it on enchanting?"
            );

            var enchantDescription = new ConfigDescription(
                "What is success chance(%) of enchanting?",
                new AcceptableValueRange<int>(0, 100)
            );

            EnchantingSuccessChance = Config.Bind(
                "Enchanting Modifications",
                "EnchantingSuccessChance",
                50,
                enchantDescription
            );

            PlayAudioOnEnchantingDone = Config.Bind(
                "Enchanting Modifications",
                "PlayAudioOnEnchantingDone",
                true,
                "Play additional audio on enchanting failed/success?"
            );

            // Register all events for publishing/subscribing, when other mods can discover them
            EventBus.RegisterEvent(GUID, EventBusPublisher.EnchantmentMenuTryEnchant, ("menu", typeof(EnchantmentMenu), "The enchantment menu instance that invoked the TryEnchant method."));
            EventBus.RegisterEvent(GUID, EventBusPublisher.EnchantmentTableDoneEnchantingFail, ("table", typeof(EnchantmentTable)));
            EventBus.RegisterEvent(GUID, EventBusPublisher.EnchantmentTableDoneEnchantingSuccess, ("table", typeof(EnchantmentTable)));

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            new Harmony(GUID).PatchAll();
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {
        }

        public static void LogMessage(string message)
        {
            Log.LogMessage($"{OutwardGameSettings.prefix} {message}");
        }

        [HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
        public class ResourcesPrefabManager_Load
        {
            static void Postfix(ResourcesPrefabManager __instance)
            {
                #if DEBUG
                SL.Log($"{OutwardGameSettings.prefix} ResourcesPrefabManager@Load called!");
                #endif

                EnchantmentsHelper.FixFilterRecipe();
                EventBusDataPresenter.LogRegisteredEvents();
            }
        }

        [HarmonyPatch(typeof(EnchantmentMenu), "TryEnchant")]
        public class Patch_TryEnchant
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

                if(RequireRecipeToAllowEnchant.Value)
                {
                    if (!EnchantmentsHelper.IsEnchantmentInList(enchantmentID, enchantmentItems))
                    {
                        __instance.m_characterUI.ShowInfoNotification("You need to have enchantment!");
                        return false;
                    }
                }

                if(UseRecipeOnEnchanting.Value)
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
}
