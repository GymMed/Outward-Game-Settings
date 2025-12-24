using HarmonyLib;
using MapMagic;
using OutwardGameSettings.Utility.Enemies;
using SideLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Managers
{
    public class EnemySkillsManager
    {
        private static EnemySkillsManager _instance;

        private EnemySkillsManager()
        {
        }

        public static EnemySkillsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EnemySkillsManager();

                return _instance;
            }
        }

        Dictionary<Weapon.WeaponType, List<int>> skills = new();
        List<int> weaponlessSkills = new();

        public void Init(Dictionary<Weapon.WeaponType, List<int>> skills, List<int> weaponlessSkills)
        {
            this.skills = skills;
            this.weaponlessSkills = weaponlessSkills;
        }

        public void SetRandomSkill(EnemyTemplate template, Weapon.WeaponType type)
        {
            // 50/50
            int weaponlessDice = UnityEngine.Random.Range(0, 2);

            if (weaponlessDice == 1)
            {
                SetSkill(template, GetRandomWeaponlessSkillId());
                return;
            }

            if (!GetRandomSkillOfWeaponType(type, out int skillId))
                OutwardGameSettings.LogMessage($"EnemySkillsManager@SetRandomSkill failed to retrieve Skill! Total skills: {skills.Count}, failed weapon type: {type}");

            SetSkill(template, skillId);
        }

        public void SetSkill(EnemyTemplate template, int skillId)
        {
            if (!template.CharacterExists)
            {
                OutwardGameSettings.LogMessage($"EnemySkillsManager@SetSkill character null!");
                return;
            }

            SetSkill(template.Character, skillId);
        }

        public void SetSkill(Character character, int skillId)
        {
            if (character?.Inventory?.SkillKnowledge == null)
            {
                OutwardGameSettings.LogMessage($"EnemySkillsManager@SetSkill you can only attach skills when Character is spawned. Make sure Character.Invetory.SkillKnowledge exist!");
#if DEBUG
                OutwardGameSettings.LogMessage($"EnemySkillsManager@SetSkill: {character == null} {character.Inventory == null} {character.Inventory?.SkillKnowledge == null}!");
#endif
                return;
            }

            //Skill skill = (Skill)ResourcesPrefabManager.Instance.GenerateItem(skillId.ToString());
            //Skill skill = (Skill)ResourcesPrefabManager.Instance.GetItemPrefab(skillId);
			Skill skill = ItemManager.Instance.GenerateItemNetwork(skillId) as Skill;

            if (skill == null)
            {
                OutwardGameSettings.LogMessage($"EnemySkillsManager@SetSkill tried to attach missing skill with id: {skillId}");
                return;
            }

			skill.transform.SetParent(character.Inventory.SkillKnowledge.transform);
			skill.ForceUpdateParentChange();
            character.Inventory.SkillKnowledge.AddItem(skill);
            AttachSkillToAI(character, skill);
            //template.Character.Inventory.TryUnlockSkill(skill);

            //template.Character.Inventory.SkillKnowledge.AddItem(skill);
        }

        public void AttachSkillToAI(Character character, Skill skill)
        {
            try
            {
                if (character == null)
                    return;

                this.RemoveSkillPrerequisites(skill);

                CharacterAI characterAI = character.GetComponentInChildren<CharacterAI>();

                if (characterAI == null)
                {
                    OutwardGameSettings.LogMessage($"EnemySkillsManager@AttachSkillToAI missing CharacterAI! Cannot attach skill.");
                    return;
                }

                AISCombat combatState = characterAI.AIStatesRoot.GetComponentInChildren<AISCombat>(true);
                combatState.gameObject.SetActive(true);

                if (combatState == null)
                {
                    OutwardGameSettings.LogMessage($"EnemySkillsManager@AttachSkillToAI missing AISCombatMelee! Cannot attach skill. {characterAI.AIStatesRoot?.name}");
                    return;
                }

#if DEBUG
                OutwardGameSettings.LogMessage($"EnemySkillsManager@AttachSkillToAI retrieved needed gameObjects!");
#endif

                // use if he can
                AICSkillAvailable skillUseCondition = new GameObject($"Condition_{skill.Name}").AddComponent<AICSkillAvailable>();
                skillUseCondition.SkillToCheck = skill;
                //skillUseCondition.m_characterAI = characterAI;
                skillUseCondition.transform.SetParent(combatState.transform);
                skillUseCondition.SubCondition = false;
                skillUseCondition.ChildConditions = Array.Empty<AICondition>();

                //skillUseCondition.m_subConditionsValid = true;
                //skillUseCondition.m_timeOfLastUpdate = 0.0f;

                //AICTargetInRange skillTargetInRangeCondition = skillUseCondition.transform.gameObject.AddComponent<AICTargetInRange>();
                //skillTargetInRangeCondition.m_characterAI = characterAI;
                //skillTargetInRangeCondition.transform.SetParent(combatMeele.transform);

                //skillTargetInRangeCondition.SubCondition = true;
                //skillUseCondition.ChildConditions = new AICondition[] { skillTargetInRangeCondition };

                //skillUseCondition.m_conditions = new AICondition[] {skillTargetInRangeCondition};

                AIEUseSkill effectUseSkill = new GameObject($"ValidEffects").AddComponent<AIEUseSkill>();
                effectUseSkill.transform.SetParent(skillUseCondition.transform);
                effectUseSkill.SkillToUse = skill;

                //skillTargetInRangeCondition.m_validEffects = new AIEffect[] { effectUseSkill };
                //skillUseCondition.m_validEffects = new AIEffect[] { effectUseSkill };

                skillUseCondition.GroupValidEffectTrans = effectUseSkill.transform;
                //combatMeele.m_conditions = new AICondition[] { skillTargetInRangeCondition, skillTargetInRangeCondition };
                combatState.Init(characterAI);
            }
            catch(Exception e)
            {
                OutwardGameSettings.LogMessage($"EnemySkillsManager@AttachSkillToAI we encountered an error: \"{e.Message}\"");
            }
        }

        public bool GetRandomSkills(Weapon.WeaponType type, out List<int> addSkillsIds, int randomSkillLearnCount = -1)
        {
            addSkillsIds = new List<int>();

            if (!skills.TryGetValue(type, out List<int> skillIds))
            {
                return false;
            }

            int maxSkillsDice = skillIds.Count > 10 ? 10 : skillIds.Count;

            if(randomSkillLearnCount < 0)
                randomSkillLearnCount = UnityEngine.Random.Range(0, maxSkillsDice);
            else if(randomSkillLearnCount > skillIds.Count)
                randomSkillLearnCount = skillIds.Count - 1;

            int weaponlessSkillsCount = UnityEngine.Random.Range(0, randomSkillLearnCount);

            if (weaponlessSkillsCount != randomSkillLearnCount)
            {
                if(GetRandomSkillsOfWeaponType(type, out List<int> addWeaponSkillsIds, randomSkillLearnCount - weaponlessSkillsCount))
                {
                    addSkillsIds.AddRange(addWeaponSkillsIds);
                }
            }

            if(GetRandomWeaponlessSkills(out List<int> addWeaponlessSkillsIds, weaponlessSkillsCount))
            {
                addSkillsIds.AddRange(addWeaponlessSkillsIds);
            }

            return true;
        }

        public bool GetRandomWeaponlessSkills(out List<int> addSkillsIds, int randomSkillLearnCount = -1)
        {
            int maxSkillsDice = weaponlessSkills.Count > 10 ? 10 : weaponlessSkills.Count;

            if(randomSkillLearnCount < 0)
                randomSkillLearnCount = UnityEngine.Random.Range(0, maxSkillsDice);
            else if(randomSkillLearnCount > weaponlessSkills.Count)
                randomSkillLearnCount = weaponlessSkills.Count - 1;

            List<int> availableSkills = new List<int>(weaponlessSkills);
            addSkillsIds = new List<int>();

            for(int currentSkill = 0; currentSkill < randomSkillLearnCount; currentSkill++)
            {
                int randomSkill = UnityEngine.Random.Range(0, availableSkills.Count);
                addSkillsIds.Add(availableSkills[randomSkill]);
                availableSkills.RemoveAt(randomSkill);
            }
            return true;

        }

        public bool GetRandomSkillsOfWeaponType(Weapon.WeaponType weaponType, out List<int> addSkillsIds, int randomSkillLearnCount = -1)
        {
            if (!skills.TryGetValue(weaponType, out List<int> skillIds))
            {
                addSkillsIds = new List<int>();
                return false;
            }

            int maxSkillsDice = skillIds.Count > 10 ? 10 : skillIds.Count;

            if(randomSkillLearnCount < 0)
                randomSkillLearnCount = UnityEngine.Random.Range(0, maxSkillsDice);
            else if(randomSkillLearnCount > skillIds.Count)
                randomSkillLearnCount = skillIds.Count - 1;

            List<int> availableSkills = new List<int>(skillIds);
            addSkillsIds = new List<int>();

            for(int currentSkill = 0; currentSkill < randomSkillLearnCount; currentSkill++)
            {
                int randomSkill = UnityEngine.Random.Range(0, availableSkills.Count);
                addSkillsIds.Add(availableSkills[randomSkill]);
                availableSkills.RemoveAt(randomSkill);
            }
            return true;
        }

        public void RemoveSkillPrerequisites(Skill skill)
        {
            skill.ManaCost = 0;
            skill.Cooldown = 8;
            skill.m_additionalConditions = new Skill.ActivationCondition[] { };
        }

        public int GetRandomWeaponlessSkillId()
        {
            return UnityEngine.Random.Range(0, weaponlessSkills.Count);
        }

        public bool GetRandomSkillOfWeaponType(Weapon.WeaponType weaponType, out int skillId)
        {
            if (!skills.TryGetValue(weaponType, out List<int> skillIds))
            {
                skillId = -1;
                return false;
            }

            int skillIndex = UnityEngine.Random.Range(0, skillIds.Count);
            skillId = skillIds[skillIndex];
            return true;
        }

        public bool GetUnownedRandomSkillOfWeaponType(Character character, Weapon.WeaponType weaponType, out int skillId)
        {
            skillId = -1;

            if (!skills.TryGetValue(weaponType, out List<int> skillIds))
                return false;

            if (!GetUnownedSkillFromList(character, skillIds, out skillId))
                return false;

            return true;
        }

        public bool GetUnownedRandomWeaponlessSkill(Character character, out int skillId)
        {
            if (!GetUnownedSkillFromList(character, weaponlessSkills, out skillId))
                return false;

            return true;
        }

        public bool GetUnownedRandomWeaponSkill(Character character, out int skillId)
        {
            skillId = -1;
            Weapon.WeaponType[] weaponTypes = Enum.GetValues(typeof(Weapon.WeaponType))
                .Cast<Weapon.WeaponType>()                       // cast to the correct type
                .OrderBy(x => UnityEngine.Random.value)          // randomize order
                .ToArray();      

            foreach (Weapon.WeaponType weaponType in weaponTypes)
            {
                if(GetUnownedRandomSkillOfWeaponType(character, weaponType, out skillId))
                    return true;
            }

            return false;
        }

        public bool GetUnownedSkillFromList(Character character, List<int> providedAvailableSkillsIds, out int skillId)
        {
            skillId = -1;

            List<int> availableSkillsIds = new List<int>(providedAvailableSkillsIds);
            List<Item> learnedSkills = character.Inventory.SkillKnowledge.m_learnedItems;

            availableSkillsIds.RemoveAll(skillId => learnedSkills.Any(item => item.ItemID == skillId));

            if (availableSkillsIds.Count == 0)
                return false;

            int skillIndex = UnityEngine.Random.Range(0, availableSkillsIds.Count);
            skillId = availableSkillsIds[skillIndex];

            return true;
        }
    }
}
