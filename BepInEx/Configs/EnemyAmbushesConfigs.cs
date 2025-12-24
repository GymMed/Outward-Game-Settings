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
    public static class EnemyAmbushesConfigs
    {
        public static ConfigEntry<bool> EnableEnemyAmbushes;
        public static ConfigEntry<int> ChanceToGetAmbushed;
        public static ConfigEntry<bool> NotifyOnAmbush;

        public static void Init(BaseUnityPlugin plugin)
        {
            EnableEnemyAmbushes = plugin.Config.Bind(
                "Enemy Ambush",
                "EnableEnemyAmbushes",
                true,
                "Enable random enemy ambush scenarios?"
            );

            var chanceEnemyAmbushDescription = new ConfigDescription(
                "What is the chance(%) of enemy ambush to happen within 3 days?",
                new AcceptableValueRange<int>(0, 100)
            );

            ChanceToGetAmbushed = plugin.Config.Bind(
                "Enemy Ambush",
                "ChanceToGetAmbushed",
                3,
                chanceEnemyAmbushDescription
            );

            NotifyOnAmbush = plugin.Config.Bind(
                "Enemy Ambush",
                "NotifyOnAmbush",
                true,
                "Notify on enemy ambush?"
            );

            ChanceToGetAmbushed.SettingChanged += (sender, args) =>
            {
                EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.Ambush].CalculateHourlyChances(ChanceToGetAmbushed.Value);
            };
        }
    }
}
