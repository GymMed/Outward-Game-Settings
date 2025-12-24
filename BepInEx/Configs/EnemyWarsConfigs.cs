using BepInEx;
using BepInEx.Configuration;
using OutwardGameSettings.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.BepInEx.Configs
{
    public static class EnemyWarsConfigs
    {
        public static ConfigEntry<bool> EnableEnemyWars;
        public static ConfigEntry<float> ChanceToGetIntoWarZone;
        public static ConfigEntry<int> MinSizeOfTheWar;
        public static ConfigEntry<int> MaxSizeOfTheWar;

        public static void Init(BaseUnityPlugin plugin)
        {
            EnableEnemyWars = plugin.Config.Bind(
                "Enemy War",
                "EnableEnemyWars",
                true,
                "Enable random enemy war scenarios?"
            );

            var chanceEnemyWarDescription = new ConfigDescription(
                "What is the chance(%) of enemy war to happen within 7 days?",
                new AcceptableValueRange<float>(0, 100)
            );

            ChanceToGetIntoWarZone = plugin.Config.Bind(
                "Enemy War",
                "ChanceToGetIntoWarZone",
                2f,
                chanceEnemyWarDescription
            );

            MinSizeOfTheWar = plugin.Config.Bind(
                "Enemy War",
                "MinSizeOfTheWar",
                8,
                "Minimum size of the war in waves?"
            );

            MaxSizeOfTheWar = plugin.Config.Bind(
                "Enemy War",
                "MaxSizeOfTheWar",
                16,
                "Maximum size of the war in waves?"
            );

            ChanceToGetIntoWarZone.SettingChanged += (sender, args) =>
            {
                EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.War].CalculateHourlyChances(ChanceToGetIntoWarZone.Value);
            };
        }
    }
}
