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
    public static class EnemySpawnsConfigs
    {
        public static ConfigEntry<bool> EnableRandomEnemySpawns;
        public static ConfigEntry<float> ChanceToMeetWanderer;

        public static void Init(BaseUnityPlugin plugin)
        {
            EnableRandomEnemySpawns = plugin.Config.Bind(
                "Enemy Random Spawn",
                "EnableRandomEnemySpawns",
                true,
                "Enable random enemy wanderer spawn scenarios?"
            );

            var chanceEnemySpawnDescription = new ConfigDescription(
                "What is the chance(%) of enemy spawning nearby in 24 hours?",
                new AcceptableValueRange<float>(0, 100)
            );

            ChanceToMeetWanderer = plugin.Config.Bind(
                "Enemy Random Spawn",
                "ChanceToMeetWanderer",
                3f,
                chanceEnemySpawnDescription
            );

            ChanceToMeetWanderer.SettingChanged += (sender, args) =>
            {
                EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.War].CalculateHourlyChances(ChanceToMeetWanderer.Value);
            };
        }
    }
}
