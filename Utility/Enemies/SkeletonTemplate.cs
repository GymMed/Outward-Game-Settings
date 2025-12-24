using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Enemies.Data;
using OutwardGameSettings.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies
{
    public class SkeletonTemplate : AnyWeaponEnemyTemplate
    {
        public int backPackChance = 20;

        public void SpawnRandomSkeleton()
        {
            int skeletonType = UnityEngine.Random.Range(0, Enum.GetValues(typeof(SkeletonTypes)).Length);

            SpawnSkeletonOfType((SkeletonTypes)skeletonType);
        }

        public int SpawnRandomSkeletonForPrice(int equipmentPrice = 0)
        {
            int skeletonType = UnityEngine.Random.Range(0, Enum.GetValues(typeof(SkeletonTypes)).Length);

            return SpawnSkeletonOfType((SkeletonTypes)skeletonType, equipmentPrice);
        }

        public void SpawnRandomSkeleton(List<Weapon.WeaponType> types)
        {
            int skeletonType = UnityEngine.Random.Range(0, Enum.GetValues(typeof(SkeletonTypes)).Length);

            SpawnSkeletonOfTypeWithWeaponTypes((SkeletonTypes)skeletonType, types);
        }

        public int SpawnRandomSkeletonForPrice(List<Weapon.WeaponType> types, int equipmentPrice = 0)
        {
            int skeletonType = UnityEngine.Random.Range(0, Enum.GetValues(typeof(SkeletonTypes)).Length);

            return SpawnSkeletonOfTypeWithWeaponTypesForPrice((SkeletonTypes)skeletonType, types, equipmentPrice);
        }

        public void SpawnSkeletonOfTypeWithWeaponTypes(SkeletonTypes type, List<Weapon.WeaponType> types)
        {
            TryToAssignWeaponTypesWithRandomWeapons(types);
            SpawnSkeletonOfTypeWeaponless(type);
        }

        public int SpawnSkeletonOfTypeWithWeaponTypesForPrice(SkeletonTypes type, List<Weapon.WeaponType> types, int withMoney = 0)
        {
            TryToAssignWeaponTypesWithRandomWeapons(types, out int unusedMoney, withMoney);
            SpawnSkeletonOfTypeWeaponless(type);
            return unusedMoney;
        }

        public void SpawnSkeletonOfTypeWeaponless(SkeletonTypes type, bool hasSkills = true)
        {
            try
            {
                SkeletonTypesHelper.types.TryGetValue(type, out SkeletonTypeData typeData);

                Template.Health = typeData.Health;
                Template.Helmet_ID = typeData.HelmetId;
                Template.Chest_ID = typeData.ChestId;
                Template.Boots_ID = typeData.BootsId;

                if (hasSkills)
                {
                    foreach (Weapon.WeaponType weaponType in weaponTypes)
                    {
                        int randomSkillsLearnCount = UnityEngine.Random.Range(1, 3);
                        if (!EnemySkillsManager.Instance.GetRandomSkillsOfWeaponType(weaponType, out List<int> skillsIds, randomSkillsLearnCount))
                            continue;

                        SkillsIds.UnionWith(skillsIds);
                    }
                }

                SpawnOrReset();
            }
            catch(Exception e)
            {
                OutwardGameSettings.LogMessage($"SkeletonTemplate@SpawnSkeletonOfType: {e.Message}");
            }
        }

        public int SpawnSkeletonOfType(SkeletonTypes type, int equipmentPrice = 0)
        {
            HasNotFoundWeaponsAndAssignedRandomForPrice(out int unusedMoney, equipmentPrice);
            SpawnSkeletonOfTypeWeaponless(type);
            return unusedMoney;
        }

        public void SpawnSkeletonOfType(SkeletonTypes type)
        {
            HasNotFoundWeaponsAndAssignedRandom();
            SpawnSkeletonOfTypeWeaponless(type);
        }

        public override void ApplyStats()
        {
            Template.HealthRegen = 5;
        }

        public override void ApplyEquipment(int forPriceAmount = 0)
        {
            if(Template.Helmet_ID == null)
            {
                Template.Helmet_ID = 3200031;
            }

            if(Template.Chest_ID == null)
            {
                Template.Chest_ID = 3200030;
            }

            if(Template.Boots_ID == null)
            {
                Template.Boots_ID = 3200032;
            }

            if(Template.Backpack_ID == null)
            {
                int randomBackPackChance = UnityEngine.Random.Range(0, 101);

                if (randomBackPackChance < backPackChance)
                {
                    EnemyEquipmentManager.Instance.SetRandomBagForPrice(this, forPriceAmount);
                    return;
                }
            }
        }
    }
}
