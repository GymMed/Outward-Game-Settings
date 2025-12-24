using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.BepInEx.Configs
{
    public static class SeasonsConfigs
    {
        public static ConfigEntry<bool> AddWinterToEnmerkarForest;
        public static ConfigEntry<bool> AddWinterToCaldera;
        public static ConfigEntry<bool> AddWinterToHallowedMarsh;
        public static ConfigEntry<bool> AddWinterToAbrassar;

        public static ConfigEntry<bool> AddFoggySpiritsToEnmerkarForest;
        public static ConfigEntry<bool> AddFoggySpiritsToCaldera;
        public static ConfigEntry<bool> AddFoggySpiritsToHallowedMarsh;
        public static ConfigEntry<bool> AddFoggySpiritsToAbrassar;
        public static ConfigEntry<bool> AddFoggySpiritsToAntiquePlateau;
        public static ConfigEntry<bool> AddFoggySpiritsToChersonese;

        public static ConfigEntry<bool> AddGreatWarToEnmerkarForest;
        public static ConfigEntry<bool> AddGreatWarToCaldera;
        public static ConfigEntry<bool> AddGreatWarToHallowedMarsh;
        public static ConfigEntry<bool> AddGreatWarToAbrassar;
        public static ConfigEntry<bool> AddGreatWarToAntiquePlateau;
        public static ConfigEntry<bool> AddGreatWarToChersonese;

        public static void Init(BaseUnityPlugin plugin)
        {
            WinterConfigsInit(plugin);
            FoggySpiritsConfigsInit(plugin);
            GreatWarConfigsInit(plugin);
        }

        public static void GreatWarConfigsInit(BaseUnityPlugin plugin)
        {
            AddGreatWarToAbrassar = plugin.Config.Bind(
                "Additional Seasons",
                "AddGreatWarToAbrassar",
                true,
                "Will add great war season to Abrassar desert?"
            );

            AddGreatWarToEnmerkarForest = plugin.Config.Bind(
                "Additional Seasons",
                "AddGreatWarToEnmerkarForest",
                true,
                "Will add great war season to Enmerkar Forest?"
            );

            AddGreatWarToHallowedMarsh = plugin.Config.Bind(
                "Additional Seasons",
                "AddGreatWarToHallowedMarsh",
                true,
                "Will add great war season to Hallowed Marsh?"
            );

            AddGreatWarToCaldera = plugin.Config.Bind(
                "Additional Seasons",
                "AddGreatWarToCaldera",
                true,
                "Will add great war season to Caldera?"
            );

            AddGreatWarToAntiquePlateau = plugin.Config.Bind(
                "Additional Seasons",
                "AddGreatWarToAntiquePlateau",
                true,
                "Will add great war season to Antique Plateau?"
            );

            AddGreatWarToChersonese = plugin.Config.Bind(
                "Additional Seasons",
                "AddGreatWarToChersonese",
                true,
                "Will add great war season to Chersonese?"
            );
        }

        public static void FoggySpiritsConfigsInit(BaseUnityPlugin plugin)
        {
            AddFoggySpiritsToAbrassar = plugin.Config.Bind(
                "Additional Seasons",
                "AddFoggySpiritsToAbrassar",
                true,
                "Will add foggy spirits season to Abrassar desert?"
            );

            AddFoggySpiritsToEnmerkarForest = plugin.Config.Bind(
                "Additional Seasons",
                "AddFoggySpiritsToEnmerkarForest",
                true,
                "Will add foggy spirits season to Enmerkar Forest?"
            );

            AddFoggySpiritsToHallowedMarsh = plugin.Config.Bind(
                "Additional Seasons",
                "AddFoggySpiritsToHallowedMarsh",
                true,
                "Will add foggy spirits season to Hallowed Marsh?"
            );

            AddFoggySpiritsToCaldera = plugin.Config.Bind(
                "Additional Seasons",
                "AddFoggySpiritsToCaldera",
                true,
                "Will add foggy spirits season to Caldera?"
            );

            AddFoggySpiritsToAntiquePlateau = plugin.Config.Bind(
                "Additional Seasons",
                "AddFoggySpiritsToAntiquePlateau",
                true,
                "Will add foggy spirits season to Antique Plateau?"
            );

            AddFoggySpiritsToChersonese = plugin.Config.Bind(
                "Additional Seasons",
                "AddFoggySpiritsToChersonese",
                true,
                "Will add foggy spirits season to Chersonese?"
            );
        }

        public static void WinterConfigsInit(BaseUnityPlugin plugin)
        {
            AddWinterToAbrassar = plugin.Config.Bind(
                "Additional Seasons",
                "AddWinterToAbrassar",
                true,
                "Will add winter to Abrassar desert?"
            );

            AddWinterToEnmerkarForest = plugin.Config.Bind(
                "Additional Seasons",
                "AddWinterToEnmerkarForest",
                true,
                "Will add winter to Enmerkar Forest?"
            );

            AddWinterToHallowedMarsh = plugin.Config.Bind(
                "Additional Seasons",
                "AddWinterToHallowedMarsh",
                true,
                "Will add winter to Hallowed Marsh?"
            );

            AddWinterToCaldera = plugin.Config.Bind(
                "Additional Seasons",
                "AddWinterToCaldera",
                true,
                "Will add winter to Caldera?"
            );
        }
    }
}
