using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Enemies.Visuals;
using OutwardGameSettings.Utility.Enums;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies
{
    public class BanditTemplate : AnyWeaponEnemyTemplate
    {
        public int backPackChance = 20;

        public void SpawnBandit(List<Weapon.WeaponType> types, bool hasSkills = true)
        {
            TryToAssignWeaponTypesWithRandomWeapons(types);
            SpawnWithRandomEquipmentAndSkills(hasSkills);
        }

        public int SpawnBandit(List<Weapon.WeaponType> types, bool hasSkills = true, int forPrice = 0)
        {
            TryToAssignWeaponTypesWithRandomWeapons(types, out int unusedMoney, forPrice);
            return SpawnWithRandomEquipmentAndSkillsForPrice(hasSkills, unusedMoney);
        }

        public void SpawnBandit(bool hasSkills = true)
        {
            HasNotFoundWeaponsAndAssignedRandom();
            SpawnWithRandomEquipmentAndSkills(hasSkills);
        }

        public int SpawnBandit(bool hasSkills = true, int forPrice = 0)
        {
            HasNotFoundWeaponsAndAssignedRandomForPrice(out int unusedMoney, forPrice);
            return SpawnWithRandomEquipmentAndSkillsForPrice(hasSkills, unusedMoney);
        }

        public int SpawnWithRandomEquipmentAndSkillsForPrice(bool hasSkills = true, int forPrice = 0)
        {
            try
            {
                int unusedMoney = EnemyEquipmentManager.Instance.SetRandomEquipmentForPrice(this, forPrice);

                if (hasSkills)
                {
                    foreach (Weapon.WeaponType weaponType in weaponTypes)
                    {
                        if (!EnemySkillsManager.Instance.GetRandomSkills(weaponType, out List<int> skillsIds))
                            continue;

                        SkillsIds.UnionWith(skillsIds);
                    }
                }

                SpawnOrReset();
                return unusedMoney;
            }
            catch (Exception e)
            {
                OutwardGameSettings.LogMessage($"BanditTemplate@SpawnBandit: {e.Message}");
                return forPrice;
            }
        }

        public void SpawnWithRandomEquipmentAndSkills(bool hasSkills = true)
        {
            try
            {
                EnemyEquipmentManager.Instance.SetRandomEquipment(this);

                if (hasSkills)
                {
                    foreach (Weapon.WeaponType weaponType in weaponTypes)
                    {
                        if (!EnemySkillsManager.Instance.GetRandomSkills(weaponType, out List<int> skillsIds))
                            continue;

                        SkillsIds.UnionWith(skillsIds);
                    }
                }

                SpawnOrReset();
            }
            catch (Exception e)
            {
                OutwardGameSettings.LogMessage($"BanditTemplate@SpawnBandit: {e.Message}");
            }
        }

        public override void ApplyStats()
        {
            if(Health == null)
            {
                int health = UnityEngine.Random.Range(100, 600);
                Template.Health = health;
                Health = health;
            }
            base.ApplyStats();
        }


        public override void ApplyCharacterVisuals()
        {
            if (this.VisualData != null)
            {
                Template.CharacterVisualsData = VisualData;
                return;
            }

            VisualData = new SL_Character.VisualData();

            int randomRace = UnityEngine.Random.Range(0, RacesHelper.races.Count);

            VisualData.SkinIndex = randomRace;

            int randomGender = UnityEngine.Random.Range(0, 2);

            RacesHelper.races.TryGetValue((Races)randomRace, out RaceData race);

            VisualData.Gender = (Character.Gender)randomGender;
            GenderData raceGenderData = race.GetGender((Character.Gender)randomGender);

            int randomHairStyle = UnityEngine.Random.Range(0, raceGenderData.TotalHairStyles);
            VisualData.HairStyleIndex = randomHairStyle;

            int randomHairColor = UnityEngine.Random.Range(0, raceGenderData.TotalHairColors);
            VisualData.HairColorIndex = randomHairColor;

            int randomHeadVariation = UnityEngine.Random.Range(0, raceGenderData.TotalHeadVariations);
            VisualData.HeadVariationIndex = randomHeadVariation;

            Template.CharacterVisualsData = VisualData;
        }

        public override void ApplyEquipment(int forPriceAmount = 0)
        {
            if(Template.Helmet_ID == null &&
                Template.Chest_ID == null &&
                Template.Boots_ID == null
                )
            {
                EnemyEquipmentManager.Instance.SetRandomEquipmentForPrice(this, forPriceAmount);
            }

            if(Template.Backpack_ID == null)
            {
                int randomBackPackChance = UnityEngine.Random.Range(0, 101);

                if(randomBackPackChance < backPackChance)
                {
                    EnemyEquipmentManager.Instance.SetRandomBag(this);
                    return;
                }
            }
        }
    }
}
