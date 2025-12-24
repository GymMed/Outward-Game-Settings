using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.BepInEx.Configs
{
    public static class EnemiesConfigs
    {
        public static ConfigEntry<bool> ScaleScenarioEnemiesPower;

        public static void Init(BaseUnityPlugin plugin)
        {
            ScaleScenarioEnemiesPower = plugin.Config.Bind(
                "Enemies",
                "ScaleScenarioEnemiesPower",
                true,
                "Adjust enemy gear to match your party’s wealth for fairer early battles."
            );
        }
    }
}
