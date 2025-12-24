using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.BepInEx.Configs
{
    public static class SkillsExpertiseConfigs
    {
        public static ConfigEntry<bool> CanLearnSkillsOnKill;
        public static ConfigEntry<int> ChanceToLearnSkillOnKill;
        public static ConfigEntry<bool> IfAllWeaponSkillsKnownLearnRandom;

        public static void Init(BaseUnityPlugin plugin)
        {
            CanLearnSkillsOnKill = plugin.Config.Bind(
                "Character Skills",
                "CanLearnSkillsOnKill",
                true,
                "Can character learn random weapon skills after killing enemy?"
            );

            var skillLearnDescription = new ConfigDescription(
                "What is success chance(%) of learning new weapon skills on enemy kill?",
                new AcceptableValueRange<int>(0, 100)
            );

            ChanceToLearnSkillOnKill = plugin.Config.Bind(
                "Character Skills",
                "LearnSkillOnKillChance",
                2,
                skillLearnDescription
            );

            IfAllWeaponSkillsKnownLearnRandom = plugin.Config.Bind(
                "Character Skills",
                "IfAllWeaponSkillsKnownLearnRandom",
                true,
                "If all specific weapon skills learned next should learn skills of random weapon type instead?"
            );
        }
    }
}
