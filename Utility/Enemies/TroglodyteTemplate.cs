using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Enemies.AI;
using OutwardGameSettings.Utility.Enemies.Data;
using OutwardGameSettings.Utility.Enums;
using SideLoader;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies
{
    public class TroglodyteTemplate : EnemyTemplate
    {
        public int chanceForRandomWeapon = 20;

        public void SpawnRandomTrog()
        {
            int randomTrogDice = UnityEngine.Random.Range(0, Enum.GetValues(typeof(TroglodyteTypes)).Length);

            SpawnTrogOfType((TroglodyteTypes)randomTrogDice);
        }

        public int SpawnRandomTrogForPrice(int forPrice = 0)
        {
            int randomTrogDice = UnityEngine.Random.Range(0, Enum.GetValues(typeof(TroglodyteTypes)).Length);

            return SpawnTrogOfType((TroglodyteTypes)randomTrogDice, forPrice);
        }

        public void GiveTrogRandomWeapon(TroglodyteTypeData typeData)
        {
            int useDefaultDice = UnityEngine.Random.Range(0, 100);

            if (useDefaultDice < 100 - chanceForRandomWeapon)
            {
                Template.Weapon_ID = typeData.WeaponId;
            }
            else
            {
                int weaponType = UnityEngine.Random.Range(0, 2);

                if (weaponType == 0)
                {
                    EnemyEquipmentManager.Instance.GetRandomWeaponOfType(Weapon.WeaponType.Halberd_2H, out WeaponData randomHalberd);
                    Template.Weapon_ID = randomHalberd.ItemId;
                }
                else
                {
                    EnemyEquipmentManager.Instance.GetRandomWeaponOfType(Weapon.WeaponType.Spear_2H, out WeaponData randomSpear);
                    Template.Weapon_ID = randomSpear.ItemId;
                }
            }
        }

        public int GiveTrogRandomWeaponForPrice(TroglodyteTypeData typeData, int forPrice = 0)
        {
            try {
            int useDefaultDice = UnityEngine.Random.Range(0, 100);

            if (typeData.WeaponId != null && useDefaultDice < 100 - chanceForRandomWeapon)
            {
                Template.Weapon_ID = typeData.WeaponId;
                Item weaponPrefab = ResourcesPrefabManager.Instance.GetItemPrefab(typeData.WeaponId.Value);

                if (weaponPrefab == null)
                    return forPrice;

                return weaponPrefab.RawCurrentValue;
            }

            int weaponType = UnityEngine.Random.Range(0, 2);

            if (weaponType == 0)
            {
                if (!EnemyEquipmentManager.Instance.GetRandomWeaponOfTypeForPrice(Weapon.WeaponType.Halberd_2H, out WeaponData randomHalberd, forPrice))
                {
                    EnemyEquipmentManager.Instance.GetCheapestRandomWeaponOfType(Weapon.WeaponType.Halberd_2H, out WeaponData randomCheapestHalberd);
                    Template.Weapon_ID = randomCheapestHalberd.ItemId;
                }
                else
                    Template.Weapon_ID = randomHalberd.ItemId;
            }
            else
            {
                if(!EnemyEquipmentManager.Instance.GetRandomWeaponOfTypeForPrice(Weapon.WeaponType.Spear_2H, out WeaponData randomSpear, forPrice))
                {
                    EnemyEquipmentManager.Instance.GetCheapestRandomWeaponOfType(Weapon.WeaponType.Spear_2H, out WeaponData randomCheapestSpear);
                    Template.Weapon_ID = randomCheapestSpear.ItemId;
                }
                else
                    Template.Weapon_ID = randomSpear.ItemId;
            }

            if (Template.Weapon_ID == null)
                return forPrice;

            Item weapon = ResourcesPrefabManager.Instance.GetItemPrefab(Template.Weapon_ID.Value);
            return weapon.RawCurrentValue;
            }
            catch(Exception e)
            {
                OutwardGameSettings.LogMessage($"TroglodyteTemplate@GiveTrogRandomWeaponForPrice: {e.Message}");
                return forPrice;
            }
        }

        public int SpawnTrogOfType(TroglodyteTypes type, int forPrice = 0)
        {
            int unusedMoney = forPrice;

            try
            {
                if(!TroglodyteTypesHelper.types.TryGetValue(type, out TroglodyteTypeData typeData))
                {
                    OutwardGameSettings.LogMessage($"TroglodyteTemplate@SpawnTrogOfType [price] provided incorrect trog type.");
                }

                Template.Health = typeData.Health;
                Template.Chest_ID = typeData.ArmorId;

                if (Template.Weapon_ID == null)
                {
                    unusedMoney = GiveTrogRandomWeaponForPrice(typeData, forPrice);
                }

                if(type == TroglodyteTypes.Grenadier)
                    AddGrenadierAI();

                AssignSkills(type);

                SpawnOrReset();
            }
            catch(Exception e)
            {
                OutwardGameSettings.LogMessage($"TroglodyteTemplate@SpawnTrogOfType: {e.Message} \n{e.StackTrace}");
            }

            return unusedMoney;
        }

        public void SpawnTrogOfType(TroglodyteTypes type)
        {
            try
            {
                if(!TroglodyteTypesHelper.types.TryGetValue(type, out TroglodyteTypeData typeData))
                {
                    OutwardGameSettings.LogMessage($"TroglodyteTemplate@SpawnTrogOfType provided incorrect trog type.");
                }

                Template.Health = typeData.Health;
                Template.Chest_ID = typeData.ArmorId;

                if (Template.Weapon_ID == null)
                {
                    GiveTrogRandomWeapon(typeData);
                }

                if(type == TroglodyteTypes.Grenadier)
                    AddGrenadierAI();

                AssignSkills(type);

                SpawnOrReset();

            }
            catch(Exception e)
            {
                OutwardGameSettings.LogMessage($"TroglodyteTemplate@SpawnTrogOfType: {e.Message}");
            }
        }

        public void AddGrenadierAI()
        {
            if(AddCombatAI)
            {
                SL_CharacterAIMelee_RegenaratingInventoryItems ai = new SL_CharacterAIMelee_RegenaratingInventoryItems();
                ai.RegenerationQuantity = 2;
                ai.ItemIdToRegenerate = 6600070; //oil

                Template.AI = ai;

                Template.AI.CanBlock = this.CanBlock;
                Template.AI.CanDodge = this.CanDodge;
            }

            AddCombatAI = false;
        }

        public void AssignSkills(TroglodyteTypes type)
        {
            switch(type)
            {
                case TroglodyteTypes.Mana:
                case TroglodyteTypes.Archmage:
                    {
                        // buff
                        int buffGrog = 8200702;
                        // debuff
                        int buffGurg = 8200701;
                        SkillsIds.Add(buffGrog);
                        SkillsIds.Add(buffGurg);

                        SkillsIds.Add(8300064);
                        SkillsIds.Add(8300063);
                        SkillsIds.Add(8300062);
                        SkillsIds.Add(8300061);
                        break;
                    }
                case TroglodyteTypes.Grenadier:
                    {
                        int bombThrowGarg = 8200700;

                        SkillsIds.Add(bombThrowGarg);
                        SkillsIds.Add(8300068);
                        SkillsIds.Add(8300060);
                        break;
                    }
                default:
                    break;
            }
        }

        public override void ApplyStats()
        {
            Template.HealthRegen = 5;
        }

        public override void ApplyCharacterVisuals()
        {
            if (this.VisualData != null)
            {
                Template.CharacterVisualsData = VisualData;
                return;
            }

            VisualData = new SL_Character.VisualData();
            VisualData.SkinIndex = -999;

            Template.CharacterVisualsData = VisualData;
        }
    }
}
