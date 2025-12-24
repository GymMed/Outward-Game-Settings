using Mono.Cecil;
using OutwardGameSettings.Utility.Enemies;
using OutwardGameSettings.Utility.Enemies.Data;
using OutwardGameSettings.Utility.Enums;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Managers
{
    public class EnemyEquipmentManager
    {
        private static EnemyEquipmentManager _instance;

        private EnemyEquipmentManager()
        {
        }

        public static EnemyEquipmentManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EnemyEquipmentManager();

                return _instance;
            }
        }

        Dictionary<Weapon.WeaponType, List<WeaponData>> weapons = new();
        Dictionary<EquipmentSlot.EquipmentSlotIDs, List<ArmorData>> armors = new();
        List<EquipmentGroupData> bags = new();

        public int priceError = 25;

        public void Init()
        {
            try
            {
                Dictionary<Weapon.WeaponType, List<int>> skills = new();
                List<int> weaponlessSkills = new();

                foreach (KeyValuePair<string, Item> itemPair in ResourcesPrefabManager.ITEM_PREFABS)
                {
                    if (itemPair.Value is Skill skill)
                    {
#if DEBUG
                        if (skill.Name == null)
                            OutwardGameSettings.LogMessage($"EnemyEquipmentManager@Init found skill: {skill.ItemID}");
                        else
                            OutwardGameSettings.LogMessage($"EnemyEquipmentManager@Init found skill: {skill.ItemID} name: {skill.Name}");
#endif

                        if (skill.IsPassive || skill.IsCosmetic)
                            continue;

                        List<Weapon.WeaponType> skillsAvailableForWeapons = new List<Weapon.WeaponType>();

                        if (!(skill.GetRequiredWeaponTypes == null || skill.GetRequiredWeaponTypes.Count < 1))
                            skillsAvailableForWeapons.AddRange(skill.GetRequiredWeaponTypes);

                        if(!(skill.GetRequiredOffHandTypes == null || skill.GetRequiredOffHandTypes.Count < 1))
                            skillsAvailableForWeapons.AddRange(skill.GetRequiredOffHandTypes);

                        if(skillsAvailableForWeapons.Count < 1)
                        {
                            weaponlessSkills.Add(skill.ItemID);
                            continue;
                        }

                        foreach (Weapon.WeaponType type in skillsAvailableForWeapons)
                        {
                            if (!skills.TryGetValue(type, out List<int> foundSkills))
                            {
                                foundSkills = new List<int>();
                                skills.Add(type, foundSkills);
                            }
                            
                            if (!foundSkills.Contains(skill.ItemID))
                            {
                                foundSkills.Add(skill.ItemID);
                            }
                        }

                        continue;
                    }

                    if (!(itemPair.Value is Equipment equipment))
                        continue;

                    if (equipment is Weapon weapon)
                    {
                        if (weapons.TryGetValue(weapon.Type, out List<WeaponData> weaponData))
                            weaponData.Add(new WeaponData(weapon.ItemID, weapon.RawCurrentValue, weapon.EquipSlot, weapon.Type));
                        else
                        {
                            List<WeaponData> weaponNewData = new List<WeaponData>();
                            weaponNewData.Add(new WeaponData(weapon.ItemID, weapon.RawCurrentValue, weapon.EquipSlot, weapon.Type));
                            weapons.Add(weapon.Type, weaponNewData);
                        }
                        continue;
                    }

                    if (equipment is Armor armor)
                    {
                        if (armors.TryGetValue(armor.EquipSlot, out List<ArmorData> armorData))
                            armorData.Add(new ArmorData(armor.ItemID, armor.RawCurrentValue, armor.EquipSlot, armor.Class));
                        else
                        {
                            List<ArmorData> armorNewData = new List<ArmorData>();
                            armorNewData.Add(new ArmorData(armor.ItemID, armor.RawCurrentValue, armor.EquipSlot, armor.Class));
                            armors.Add(armor.EquipSlot, armorNewData);
                        }
                        continue;
                    }

                    if (equipment is Bag bag)
                    {
                        bags.Add(new EquipmentGroupData(bag.ItemID, bag.RawCurrentValue, bag.EquipSlot));
                        continue;
                    }
                }

                EnemySkillsManager.Instance.Init(skills, weaponlessSkills);
            }
            catch(Exception e)
            {
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@Init received error: \"{e.Message}\"");
            }
        }

        public static List<Weapon.WeaponType> GetAllWeaponTypes()
        {
            return Enum.GetValues(typeof(Weapon.WeaponType)).Cast<Weapon.WeaponType>().ToList();
        }

        public void SetWeaponOfType(EnemyTemplate template, Weapon.WeaponType type)
        {
            if (!GetRandomWeaponOfType(type, out WeaponData randomWeapon))
            {
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetWeaponOfType failed to retrieve Weapon! Total weapons: {weapons.Count}, failed type: {type}");
                return;
            }
#if DEBUG
            OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetWeaponOfType retrieved weapon: {randomWeapon.ItemId}");
#endif
            AssignWeaponToTemplate(template, randomWeapon);
        }

        public int SetWeaponOfTypeForPrice(EnemyTemplate template, Weapon.WeaponType type, int forPrice = 0)
        {
            if (!GetRandomWeaponOfTypeForPrice(type, out WeaponData randomWeapon, forPrice))
            {
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetWeaponOfTypeForPrice failed to retrieve Weapon! Total weapons: {weapons.Count}, failed type: {type}");
                return 0;
            }
#if DEBUG
            OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetWeaponOfType retrieved weapon: {randomWeapon.ItemId}");
#endif
            AssignWeaponToTemplate(template, randomWeapon);
            return randomWeapon.GoldValue;
        }

        public List<Weapon.WeaponType> SetRandomWeaponsForPrice(EnemyTemplate template, out int unusedMoney, int forPrice = 0)
        {
            try
            {
                OutwardGameSettings.LogMessage($"SetRandomWeaponsForPrice passing weapon receiver");
                Array values = Enum.GetValues(typeof(Weapon.WeaponType));
                Weapon.WeaponType type = (Weapon.WeaponType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

                OutwardGameSettings.LogMessage($"SetRandomWeaponsForPrice based weapon receiver");

                unusedMoney = SetWeaponOfTypeForPrice(template, type, forPrice);
                forPrice = unusedMoney;

                WeaponHoldingTypes holdingType = GetWeaponHoldingType(type);

                List<Weapon.WeaponType> weaponTypes = new List<Weapon.WeaponType> { type };

                switch (holdingType)
                {
                    case WeaponHoldingTypes.TwoHanded:
                        return weaponTypes;
                    case WeaponHoldingTypes.OffHanded:
                        {
                            Weapon.WeaponType mainType = GetRandomWeaponOfType(WeaponHoldingTypes.MainHanded);
                            unusedMoney = SetWeaponOfTypeForPrice(template, mainType, forPrice);
                            weaponTypes.Add(mainType);

                            return weaponTypes;
                        }
                    case WeaponHoldingTypes.MainHanded:
                    default:
                        {
                            Weapon.WeaponType offType = GetRandomWeaponOfType(WeaponHoldingTypes.OffHanded);
                            unusedMoney = SetWeaponOfTypeForPrice(template, offType, forPrice);
                            weaponTypes.Add(offType);

                            return weaponTypes;
                        }
                }
            } catch(Exception e)
            {
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomWeaponsForPrice encountered an error: {e.Message}\n{e.StackTrace}");
                unusedMoney = forPrice;
                return new List<Weapon.WeaponType>();
            }
        }

        public List<Weapon.WeaponType> SetRandomWeapons(EnemyTemplate template)
        {
            Array values = Enum.GetValues(typeof(Weapon.WeaponType));
            Weapon.WeaponType type = (Weapon.WeaponType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

            SetWeaponOfType(template, type);

            WeaponHoldingTypes holdingType = GetWeaponHoldingType(type);

            List<Weapon.WeaponType> weaponTypes = new List<Weapon.WeaponType> { type };
            OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomWeapon recieved WeaponTypes! current type: {type} holding type: {holdingType}");

            switch (holdingType)
            {
                case WeaponHoldingTypes.TwoHanded:
                    return weaponTypes;
                case WeaponHoldingTypes.OffHanded:
                    {
                        OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomWeapon assigning main hand {holdingType}");
                        Weapon.WeaponType mainType = GetRandomWeaponOfType(WeaponHoldingTypes.MainHanded);
                        OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomWeapon assigning main hand to {mainType}");
                        SetWeaponOfType(template, mainType);
                        weaponTypes.Add(mainType);

                        return weaponTypes;
                    }
                case WeaponHoldingTypes.MainHanded:
                default:
                    {
                        OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomWeapon assigning off hand {holdingType}");
                        Weapon.WeaponType offType = GetRandomWeaponOfType(WeaponHoldingTypes.OffHanded);
                        OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomWeapon assigning off hand to {offType}");
                        SetWeaponOfType(template, offType);
                        weaponTypes.Add(offType);

                        return weaponTypes;
                    }
            }
        }

        public static Weapon.WeaponType GetRandomWeaponOfType(WeaponHoldingTypes type)
        {
            var set = WeaponHoldingTypesHelpers.Types[type];
            int index = UnityEngine.Random.Range(0, set.Count);
            return set.ElementAt(index);
        }

        public WeaponHoldingTypes GetWeaponHoldingType(Weapon.WeaponType type)
        {
            switch (type)
            {
                case Weapon.WeaponType.Sword_1H:
                case Weapon.WeaponType.Axe_1H:
                case Weapon.WeaponType.Mace_1H:
                    {
                        return WeaponHoldingTypes.MainHanded;
                    }
                case Weapon.WeaponType.Dagger_OH:
                case Weapon.WeaponType.Chakram_OH:
                case Weapon.WeaponType.Pistol_OH:
                case Weapon.WeaponType.Shield:
                    {
                        return WeaponHoldingTypes.OffHanded;
                    }
                case Weapon.WeaponType.Halberd_2H:
                case Weapon.WeaponType.Sword_2H:
                case Weapon.WeaponType.Axe_2H:
                case Weapon.WeaponType.Mace_2H:
                case Weapon.WeaponType.Spear_2H:
                case Weapon.WeaponType.FistW_2H:
                case Weapon.WeaponType.Arrow:
                case Weapon.WeaponType.Bow:
                default:
                    {
                        return WeaponHoldingTypes.TwoHanded;
                    }
            }
        }

        public void AssignWeaponToTemplate(EnemyTemplate template, WeaponData weapon)
        {
            switch(weapon.WeaponType)
            {
                case Weapon.WeaponType.Chakram_OH:
                case Weapon.WeaponType.Dagger_OH:
                case Weapon.WeaponType.Shield:
                    {
                        template.Template.Shield_ID = weapon.ItemId;
                        break;
                    }
                case Weapon.WeaponType.Pistol_OH:
                    {
                        template.Template.Weapon_ID = weapon.ItemId;

                        SL_ItemQty bullets = new SL_ItemQty();
                        bullets.ItemID = 4400080;
                        bullets.Quantity = 15;
                        AddToPouch(template, bullets);
                        template.AIType = AIClassTypes.Gunner;
                        break;
                    }
                case Weapon.WeaponType.Arrow:
                    {
                        if (!GetRandomWeaponOfType(Weapon.WeaponType.Bow, out WeaponData randomBow))
                            OutwardGameSettings.LogMessage($"EnemyEquipmentManager@AssignWeaponToTemplate failed to retrieve Bow! Total weapons: {weapons.Count}");

                        template.Template.Weapon_ID = randomBow.ItemId;

                        SL_ItemQty arrows = new SL_ItemQty();

                        arrows.ItemID = weapon.ItemId;
                        arrows.Quantity = 15;
                        AddToPouch(template, arrows);

                        template.AIType = AIClassTypes.Archer;
                        break;
                    }
                case Weapon.WeaponType.Bow:
                    {
                        template.Template.Weapon_ID = weapon.ItemId;

                        SL_ItemQty arrows = new SL_ItemQty();

                        if (!GetRandomWeaponOfType(Weapon.WeaponType.Arrow, out WeaponData randomArrow))
                            OutwardGameSettings.LogMessage($"EnemyEquipmentManager@AssignWeaponToTemplate failed to retrieve Arrow! Total weapons: {weapons.Count}");

                        arrows.ItemID = randomArrow.ItemId;
                        arrows.Quantity = 30;
                        AddToPouch(template, arrows);

                        template.AIType = AIClassTypes.Archer;
                        break;
                    }
                case Weapon.WeaponType.Sword_2H:
                case Weapon.WeaponType.Sword_1H:
                case Weapon.WeaponType.Axe_2H:
                case Weapon.WeaponType.Axe_1H:
                case Weapon.WeaponType.Mace_2H:
                case Weapon.WeaponType.Mace_1H:
                case Weapon.WeaponType.FistW_2H:
                case Weapon.WeaponType.Halberd_2H:
                case Weapon.WeaponType.Spear_2H:
                default:
                    {
                        template.Template.Weapon_ID = weapon.ItemId;
                        break;
                    }
            }
        }

        public void SetRandomEquipment(EnemyTemplate template)
        {
            if (!GetRandomArmorOfType(EquipmentSlot.EquipmentSlotIDs.Helmet, out ArmorData randomHelmet))
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomEquipment failed to retrieve helmet! Total armors: {armors.Count}");

            if (!GetRandomArmorOfType(EquipmentSlot.EquipmentSlotIDs.Chest, out ArmorData randomChest))
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomEquipment failed to retrieve chest! Total armors: {armors.Count}");

            if (!GetRandomArmorOfType(EquipmentSlot.EquipmentSlotIDs.Foot, out ArmorData randomBoots))
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomEquipment failed to retrieve foot! Total armors: {armors.Count}");

            template.Template.Helmet_ID = randomHelmet.ItemId;
            template.Template.Chest_ID = randomChest.ItemId;
            template.Template.Boots_ID = randomBoots.ItemId;
        }

        public int SetRandomEquipmentForPrice(EnemyTemplate template, int price = 0)
        {
            if (!GetRandomArmorOfTypeForPrice(EquipmentSlot.EquipmentSlotIDs.Helmet, out ArmorData randomHelmet, price, GetMinPrice(price)))
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomEquipmentForPrice failed to retrieve helmet! Total armors: {armors.Count}");
            else
            {
                template.Template.Helmet_ID = randomHelmet.ItemId;
                price = Mathf.Clamp(price - randomHelmet.GoldValue, 0, price);
            }

            if (!GetRandomArmorOfTypeForPrice(EquipmentSlot.EquipmentSlotIDs.Chest, out ArmorData randomChest, price, GetMinPrice(price)))
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomEquipmentForPrice failed to retrieve chest! Total armors: {armors.Count}");
            else
            {
                template.Template.Chest_ID = randomChest.ItemId;
                price = Mathf.Clamp(price - randomChest.GoldValue, 0, price);
            }

            if (!GetRandomArmorOfTypeForPrice(EquipmentSlot.EquipmentSlotIDs.Foot, out ArmorData randomBoots, price, GetMinPrice(price)))
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomEquipmentForPrice failed to retrieve foot! Total armors: {armors.Count}");
            else
            {
                template.Template.Boots_ID = randomBoots.ItemId;
                price = Mathf.Clamp(price - randomBoots.GoldValue, 0, price);
            }

            return price;
        }

        public void SetRandomEquipmentForSamePrice(EnemyTemplate template, int price = 0)
        {
            if (!GetRandomArmorOfTypeForPrice(EquipmentSlot.EquipmentSlotIDs.Helmet, out ArmorData randomHelmet, price))
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomEquipmentForSamePrice failed to retrieve helmet! Total armors: {armors.Count}");

            if (!GetRandomArmorOfTypeForPrice(EquipmentSlot.EquipmentSlotIDs.Chest, out ArmorData randomChest, price))
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomEquipmentForSamePrice failed to retrieve chest! Total armors: {armors.Count}");

            if (!GetRandomArmorOfTypeForPrice(EquipmentSlot.EquipmentSlotIDs.Foot, out ArmorData randomBoots, price))
                OutwardGameSettings.LogMessage($"EnemyEquipmentManager@SetRandomEquipmentForSamePrice failed to retrieve foot! Total armors: {armors.Count}");

            template.Template.Helmet_ID = randomHelmet.ItemId;
            template.Template.Chest_ID = randomChest.ItemId;
            template.Template.Boots_ID = randomBoots.ItemId;
        }

        public void SetRandomBag(EnemyTemplate template)
        {
            template.Template.Backpack_ID = GetRandomBag().ItemId;
        }

        public int SetRandomBagForPrice(EnemyTemplate template, int forPrice = 0)
        {
            EquipmentGroupData bagData = GetRandomBagForPrice(forPrice);
            template.Template.Backpack_ID = bagData.ItemId;
            int unusedMoney = Mathf.Clamp(forPrice - bagData.GoldValue, 0, forPrice);

            return unusedMoney;
        }

        public EquipmentGroupData GetRandomBagForPrice(int forPrice = 0)
        {
            List<EquipmentGroupData> affordableBags = new List<EquipmentGroupData>();
            int allowedPrice = forPrice + priceError;

            foreach(EquipmentGroupData bag in bags)
            {
                if(bag.GoldValue < allowedPrice)
                {
                    affordableBags.Add(bag);
                }
            }

            if(affordableBags.Count < 1)
            {
                return GetRandomBag();
            }

            int randomBagNumber = UnityEngine.Random.Range(0, affordableBags.Count);

            return affordableBags[randomBagNumber];
        }

        public EquipmentGroupData GetRandomBag()
        {
            if(bags.Count < 1)
            {
                return new EquipmentGroupData();
            }

            int randomBagNumber = UnityEngine.Random.Range(0, bags.Count);

            return bags[randomBagNumber];
        }

        public bool GetListOfWeapons(Weapon.WeaponType weaponType, out List<WeaponData> foundWeapons)
        {
            if (!weapons.TryGetValue(weaponType, out List<WeaponData> weaponData))
            {
                foundWeapons = new List<WeaponData>();
                return false;
            }

            foundWeapons = weaponData;
            return true;
        }

        public bool GetRandomWeaponOfType(Weapon.WeaponType weaponType, out WeaponData weapon)
        {
            if (!weapons.TryGetValue(weaponType, out List<WeaponData> weaponData))
            {
                weapon = new WeaponData();
                return false;
            }

            if(weaponData.Count < 1)
            {
                weapon = new WeaponData();
                return false;
            }

            int randomWeaponNumber = UnityEngine.Random.Range(0, weaponData.Count);

            weapon = weaponData[randomWeaponNumber];
            return true;
        }

        public bool GetCheapestRandomWeaponOfType(Weapon.WeaponType weaponType, out WeaponData weapon)
        {
            if (!weapons.TryGetValue(weaponType, out List<WeaponData> weaponData))
            {
                weapon = new WeaponData();
                return false;
            }

            int minPrice = weaponData.Min(w => w.GoldValue);
            var cheapWeapons = weaponData
                .Where(w => w.GoldValue <= minPrice + priceError)
                .ToList();

            if(cheapWeapons.Count < 1)
            {
                weapon = new WeaponData();
                return false;
            }

            int randomWeapon = UnityEngine.Random.Range(0, cheapWeapons.Count);

            weapon = cheapWeapons[randomWeapon];
            return true;
        }

        public bool GetRandomWeaponOfTypeForPrice(Weapon.WeaponType weaponType, out WeaponData weapon, int price = 0, int minPrice = -1)
        {
            if (!weapons.TryGetValue(weaponType, out List<WeaponData> weaponData))
            {
                weapon = new WeaponData();
                return false;
            }

            List<WeaponData> fitWeapon = new List<WeaponData>();
            int maxPrice = price + priceError;

            if(minPrice == -1)
                minPrice = price - priceError;

            foreach(WeaponData currentWeapon in weaponData)
            {
                if(currentWeapon.GoldValue < maxPrice && currentWeapon.GoldValue > minPrice)
                    fitWeapon.Add(currentWeapon);
            }

            if(fitWeapon.Count < 1)
            {
                if (!GetCheapestRandomWeaponOfType(weaponType, out weapon))
                {
                    weapon = new WeaponData();
                    return false;
                }
                else
                {
                    return true;
                }
            }

            int randomWeapon = UnityEngine.Random.Range(0, fitWeapon.Count);

            weapon = fitWeapon[randomWeapon];
            return true;
        }

        public bool GetRandomArmorOfType(EquipmentSlot.EquipmentSlotIDs equipmentType, out ArmorData armor)
        {
            if (!armors.TryGetValue(equipmentType, out List<ArmorData> armorData))
            {
                armor = new ArmorData();
                return false;
            }

            if(armorData.Count < 1)
            {
                armor = new ArmorData();
                return false;
            }

            int randomArmorNumber = UnityEngine.Random.Range(0, armorData.Count);

            armor = armorData[randomArmorNumber];
            return true;
        }

        public bool GetRandomArmorOfTypeForPrice(EquipmentSlot.EquipmentSlotIDs equipmentType, out ArmorData armor, int price = 0, int minPrice = -1)
        {
            if (!armors.TryGetValue(equipmentType, out List<ArmorData> armorData))
            {
                armor = new ArmorData();
                return false;
            }

            if(armorData.Count < 1)
            {
                armor = new ArmorData();
                return false;
            }

            List<ArmorData> fitArmor = new List<ArmorData>();
            int maxPrice = price + priceError;

            if(minPrice == -1)
                minPrice = price - priceError;

            foreach(ArmorData currentArmor in armorData)
            {
                if(currentArmor.GoldValue < maxPrice && currentArmor.GoldValue > minPrice)
                    fitArmor.Add(currentArmor);
            }

            if(armorData.Count < 1)
            {
                armor = new ArmorData();
                return false;
            }

            int randomArmor = UnityEngine.Random.Range(0, fitArmor.Count);

            armor = fitArmor[randomArmor];
            return true;
        }

        public int GetMinPrice(int price)
        {
            return price - priceError;
        }

        public static void AddToPouch(EnemyTemplate template, SL_ItemQty item)
        {
            if (template.Template.Pouch_Items == null)
            {
                template.Template.Pouch_Items = new()
                {
                    item
                };
            }
            else
            {
                template.Template.Pouch_Items.Add(item);
            }
        }
    }
}
