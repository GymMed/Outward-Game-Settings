using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using OutwardGameSettings.Utility.Helpers;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using OutwardGameSettings.Events;
using OutwardGameSettings.Managers;
using OutwardGameSettings.BepInEx.Configs;
using OutwardGameSettings.Utility.Enums;
using OutwardGameSettings.Managers.Testing;
using UnityEngine;

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
        public const string VERSION = "1.1.1";

        public static string prefix = "[GymMed-Game-Settings]";

#if DEBUG
        public const string Start_War = "Start War!";
        public const string Spawn_Wave = "Spawn Wave!";
        public const string Clean_Dead_Bodies = "Clean Dead Bodies";
#endif

        internal static ManualLogSource Log;

        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"Hello world from {NAME} {VERSION}!");

            // Any config settings you define should be set up like this:
            //ExampleConfig = Config.Bind("ExampleCategory", "ExampleSetting", false, "This is an example setting.");
            EnchantmentRecipesConfigs.Init(this);
            SkillsExpertiseConfigs.Init(this);

            EnemiesConfigs.Init(this);
            EnemyAmbushesConfigs.Init(this);
            EnemyWarsConfigs.Init(this);
            EnemySpawnsConfigs.Init(this);
            SeasonsConfigs.Init(this);

            SeasonsManager.Instance.Init();

            EventBusRegister.RegisterEvents();

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            new Harmony(GUID).PatchAll();

#if DEBUG
            DynamicDebugger.Init();

            CustomKeybindings.AddAction(Start_War, KeybindingsCategory.CustomKeybindings, ControlType.Both);
            CustomKeybindings.AddAction(Spawn_Wave, KeybindingsCategory.CustomKeybindings, ControlType.Both);

            foreach(string keyBinding in TestingKeyBindings.EnemiesKeyBindings)
            {
                CustomKeybindings.AddAction(keyBinding, KeybindingsCategory.CustomKeybindings, ControlType.Both);
            }

            CustomKeybindings.AddAction(Clean_Dead_Bodies, KeybindingsCategory.CustomKeybindings, ControlType.Both);

            OutwardGameSettings.LogMessage("Setting full stack trace logging...");
            Application.logMessageReceived += (condition, stack, type) =>
            {
                if (type == LogType.Exception)
                {
                    OutwardGameSettings.LogMessage($"[GLOBAL EXCEPTION] {condition}\n{stack}");
                }
            };
#endif
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {
#if DEBUG
            if (CustomKeybindings.GetKeyDown(Start_War))
            {
                EnemyWaveManager.Instance.StartWarOfRandomSize();
            }

            if (CustomKeybindings.GetKeyDown(Spawn_Wave))
            {
                EnemyWaveManager.Instance.SpawnRandomWave(EnemyAmbushesConfigs.NotifyOnAmbush.Value);
            }

            for (int currentKey = 0; currentKey < TestingKeyBindings.EnemiesKeyBindings.Length; currentKey++)
            {
                if (CustomKeybindings.GetKeyDown(TestingKeyBindings.EnemiesKeyBindings[currentKey]))
                {
                    EnemyWaveManager.Instance.StartWave(currentKey, EnemyAmbushesConfigs.NotifyOnAmbush.Value);
                }
            }

            if (CustomKeybindings.GetKeyDown(Clean_Dead_Bodies))
            {
                EnemyWaveManager.Instance.CleanDeadBodies();
            }
#endif
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
                try
                {
                    EnchantmentsHelper.FixFilterRecipe();
                    EnemyEquipmentManager.Instance.Init();
                    EnemyWaveManager.Instance.Init();
                }
                catch(Exception ex)
                {
                    LogMessage($"ResourcesPrefabManager@Load error: \"{ex.Message}\"");
                }
            }
        }
    }
}
