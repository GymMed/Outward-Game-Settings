using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Enemies.AI;
using OutwardGameSettings.Utility.Enemies.Data;
using OutwardGameSettings.Utility.Enums;
using SideLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Enemies
{
    public class EnemyTemplate
    {
        public string Name { get; set; }
        public SL_Character Template { get; set; }

        public bool CharacterExists { get => Character != null; }
        public Character Character;
        public SL_Character.VisualData VisualData = null;
        public SL_CharacterAI AI = null;

        public bool AddCombatAI;
        public bool CanDodge;
        public bool CanBlock;
        public int? Health = null;

        public Vector3 SpawnPosition;

        public HashSet<int> SkillsIds { get; set; } = new();

        public AIClassTypes AIType = AIClassTypes.Warrior;

        public void DestroyCharacter()
        {
            CustomCharacters.DestroyCharacterRPC(Character);
        }

        public void SpawnOrReset()
        {
            if (Template == null)
            {
                OutwardGameSettings.LogMessage("EnemyTemplate@SpawnOrReset null template!");
                return;
            }
            else
                Template.Unregister();

            Template.SaveType = CharSaveType.Scene;

            if (AddCombatAI)
            {
                switch(AIType)
                {
                    case AIClassTypes.Gunner:
                        {
                            Template.AI = new SL_CharacterAIGunnerRanged();
                            break;
                        }
                    case AIClassTypes.Archer:
                        {
                            Template.AI = new SL_CharacterAIBowRanged();
                            break;
                        }
                    case AIClassTypes.Warrior:
                    default:
                        {
                            Template.AI = new SL_CharacterAIMeleeFixed();
                            break;
                        }
                }

                Template.AI.CanBlock = this.CanBlock;
                Template.AI.CanDodge = this.CanDodge;
            }

            ApplyCharacterVisuals();
            ApplyRandomCharacterScale();
            ApplyEquipment();
            ApplyStats();

            Template.LootableOnDeath = true;
            Template.DropPouchContents = true;

            Template.OnSpawn += (spawningChar, name) =>
            {
                AddTypeNeeds();
                EnemyWaveManager.Instance.StartCoroutine(AddSkills());
            };

            if (CharacterExists)
            {
                CustomCharacters.DestroyCharacterRPC(Character);
                Template.Unregister();
            }

            SL.Log("EnemyTamplate@SpawnOrReset Spawning clone, AI: " + (Template.AI?.ToString() ?? "null"));

            try
            {
                Template.ApplyTemplate();
                Character = Template.Spawn(SpawnPosition, UID.Generate());
            }
            catch(Exception ex)
            {
                SL.Log($"EnemyTamplate@SpawnOrReset encountered an error: \"{ex.Message}\"");
            }
        }

        public virtual IEnumerator AddSkills()
        {
            foreach(int skill in SkillsIds)
            {
                EnemySkillsManager.Instance.SetSkill(this, skill);
                yield return new WaitForSeconds(0.1f); // delay between attaching skills
            }
        }

        public virtual void AddTypeNeeds()
        {
            switch(AIType)
            {
                case AIClassTypes.Archer:
                {
                    if (!EnemyEquipmentManager.Instance.GetRandomWeaponOfType(Weapon.WeaponType.Arrow, out WeaponData randomArrow))
                    {
                        OutwardGameSettings.LogMessage($"EnemyTemplate@AddTypeNeeds failed to retrieve Arrow!");
                        return;
                    }

                    Item arrows = (Item)ResourcesPrefabManager.Instance.GenerateItem(randomArrow.ItemId.ToString());

                    if (arrows == null)
                        return;

                    Character.Inventory.GenerateItem(arrows, 5, true);
                    break;
                }
                case AIClassTypes.Gunner:
                {
                    int bulletsId = 4400080;
                    Item bullets = (Item)ResourcesPrefabManager.Instance.GenerateItem(bulletsId.ToString());

                    if (bullets == null)
                        return;

                    Character.Inventory.GenerateItem(bullets, 5, true);

                    int fireReloadSkill = 8200600;
                    SkillsIds.Add(fireReloadSkill);
                    break;
                }
                default:
                    break;
            }
        }

        public virtual void ApplyStats()
        {
            if(Health != null)
                Template.Health = Health;
            else
                Template.Health = 100;

            Template.HealthRegen = 5;
        }

        public virtual void ApplyCharacterVisuals()
        {
            if (VisualData != null)
            {
                Template.CharacterVisualsData = VisualData;
                return;
            }
        }

        public virtual void ApplyRandomCharacterScale()
        {
            float randomScale = UnityEngine.Random.Range(0.6f, 1.4f);

            Template.Scale = new Vector3(randomScale, randomScale, randomScale);
        }

        public virtual void ApplyEquipment(int forPriceAmount = 0)
        {
            return;
        }

        [Obsolete]
        public void Reset(Vector3 pos, bool newspawn)
        {
            SpawnOrReset();
        }
    }
}
