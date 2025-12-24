using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.BepInEx.Configs
{
    public static class EnchantmentRecipesConfigs
    {
        public static ConfigEntry<bool> RequireRecipeToAllowEnchant;
        public static ConfigEntry<bool> UseRecipeOnEnchanting;
        public static ConfigEntry<int> EnchantingSuccessChance;
        public static ConfigEntry<bool> PlayAudioOnEnchantingDone;

        public static void Init(BaseUnityPlugin plugin)
        {
            RequireRecipeToAllowEnchant = plugin.Config.Bind(
                "Enchanting Modifications",
                "RequireRecipeToAllowEnchant",
                true,
                "Allow enchanting only if enchantment is on character?"
            );

            UseRecipeOnEnchanting = plugin.Config.Bind(
                "Enchanting Modifications",
                "UseRecipeOnEnchanting",
                true,
                "Remove recipe after using it on enchanting?"
            );

            var enchantDescription = new ConfigDescription(
                "What is success chance(%) of enchanting?",
                new AcceptableValueRange<int>(0, 100)
            );

            EnchantingSuccessChance = plugin.Config.Bind(
                "Enchanting Modifications",
                "EnchantingSuccessChance",
                50,
                enchantDescription
            );

            PlayAudioOnEnchantingDone = plugin.Config.Bind(
                "Enchanting Modifications",
                "PlayAudioOnEnchantingDone",
                true,
                "Play additional audio on enchanting failed/success?"
            );
        }
    }
}
