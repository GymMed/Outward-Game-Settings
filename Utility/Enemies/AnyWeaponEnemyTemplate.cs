using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies
{
    public class AnyWeaponEnemyTemplate : EnemyTemplate
    {
        public List<Weapon.WeaponType> weaponTypes = new List<Weapon.WeaponType>();

        public bool TryToAssignWeaponTypesWithRandomWeapons(List<Weapon.WeaponType>types, out int unusedMoney, int forPrice = 0)
        {
            unusedMoney = 0;
            if (HasFoundWeaponsAndAssignedTypes())
            {
                OutwardGameSettings.LogMessage($"AnyWeaponEnemyTemplate@TryToAssignWeaponTypesWithRandomWeapons using provided Weapon_ID/Shield_ID instead of Weapon.WeaponTypes");
                return false;
            }

            if (types == null || types.Count == 0 || types.Count > 2)
            {
                OutwardGameSettings.LogMessage($"AnyWeaponEnemyTemplate@TryToAssignWeaponTypesWithRandomWeapons tried to insert incorrect number of Weapon.WeaponTypes");
                return false;
            }

            forPrice = EnemyEquipmentManager.Instance.SetWeaponOfTypeForPrice(this, types[0], forPrice);
            unusedMoney = forPrice;
            weaponTypes.Add(types[0]);

            if(types.Count > 1)
            {
                WeaponHoldingTypes firstHand = WeaponHoldingTypesHelpers.GetHoldingType(types[0]);
                WeaponHoldingTypes secondHand = WeaponHoldingTypesHelpers.GetHoldingType(types[1]);

                if (firstHand == WeaponHoldingTypes.TwoHanded ||
                    secondHand == WeaponHoldingTypes.TwoHanded ||
                    firstHand == secondHand )
                {
                    OutwardGameSettings.LogMessage($"AnyWeaponEnemyTemplate@TryToAssignWeaponTypesWithRandomWeapons assigned one hand: {types[0]} but other type didn't fit the requirements: {types[1]}");
                    return false;
                }

                forPrice = EnemyEquipmentManager.Instance.SetWeaponOfTypeForPrice(this, types[1], forPrice);
                unusedMoney = forPrice;
                weaponTypes.Add(types[1]);
            }

            return true;
        }


        public bool TryToAssignWeaponTypesWithRandomWeapons(List<Weapon.WeaponType>types)
        {
            if (HasFoundWeaponsAndAssignedTypes())
            {
                OutwardGameSettings.LogMessage($"AnyWeaponEnemyTemplate@TryToAssignWeaponTypesWithRandomWeapons using provided Weapon_ID/Shield_ID instead of Weapon.WeaponTypes");
                return false;
            }

            if (types == null || types.Count == 0 || types.Count > 2)
            {
                OutwardGameSettings.LogMessage($"AnyWeaponEnemyTemplate@TryToAssignWeaponTypesWithRandomWeapons tried to insert incorrect number of Weapon.WeaponTypes");
                return false;
            }

            EnemyEquipmentManager.Instance.SetWeaponOfType(this, types[0]);
            weaponTypes.Add(types[0]);

            if(types.Count > 1)
            {
                WeaponHoldingTypes firstHand = WeaponHoldingTypesHelpers.GetHoldingType(types[0]);
                WeaponHoldingTypes secondHand = WeaponHoldingTypesHelpers.GetHoldingType(types[1]);

                if (firstHand == WeaponHoldingTypes.TwoHanded ||
                    secondHand == WeaponHoldingTypes.TwoHanded ||
                    firstHand == secondHand )
                {
                    OutwardGameSettings.LogMessage($"AnyWeaponEnemyTemplate@TryToAssignWeaponTypesWithRandomWeapons assigned one hand: {types[0]} but other type didn't fit the requirements: {types[1]}");
                    return false;
                }

                EnemyEquipmentManager.Instance.SetWeaponOfType(this, types[1]);
                weaponTypes.Add(types[1]);
            }

            return true;
        }

        public bool HasFoundWeaponsAndAssignedTypes()
        {
            if (Template == null)
            {
                OutwardGameSettings.LogMessage($"AnyWeaponEnemyTemplate@HasFoundWeaponsAndAssignedTypes SL_Character Template is null!");
                return false;
            }

            if (Template.Weapon_ID != null)
            {
                Weapon weapon = (Weapon)ResourcesPrefabManager.Instance.GetItemPrefab(Template.Weapon_ID.Value);

                if (weapon != null && !weaponTypes.Contains(weapon.Type))
                    weaponTypes.Add(weapon.Type);
            }

            if (Template.Shield_ID != null)
            {
                Weapon offHand = (Weapon)ResourcesPrefabManager.Instance.GetItemPrefab(Template.Shield_ID.Value);

                if (offHand != null && !weaponTypes.Contains(offHand.Type))
                    weaponTypes.Add(offHand.Type);
            }

            if (weaponTypes.Count < 1)
                return false;

            return true;
        }

        public bool HasNotFoundWeaponsAndAssignedRandom()
            => TryAssignRandomWeapons(anyWeaponEnemyTemplate => EnemyEquipmentManager.Instance.SetRandomWeapons(anyWeaponEnemyTemplate));

        public bool HasNotFoundWeaponsAndAssignedRandomForPrice(out int unusedMoney, int forPrice = 0)
        {
            int moneyAmountLeft = 0;

            bool success = TryAssignRandomWeapons(anyWeaponEnemyTemplate => EnemyEquipmentManager.Instance.SetRandomWeaponsForPrice(anyWeaponEnemyTemplate, out moneyAmountLeft, forPrice));
            unusedMoney = moneyAmountLeft;

            return success;
        }

        private bool TryAssignRandomWeapons(Func<AnyWeaponEnemyTemplate, List<Weapon.WeaponType>> assignWeaponsFunc)
        {
            if (Template.Weapon_ID == null && Template.Shield_ID == null)
            {
                weaponTypes = assignWeaponsFunc(this);
                return true;
            }

            if (!HasFoundWeaponsAndAssignedTypes())
            {
                weaponTypes = assignWeaponsFunc(this);
                return true;
            }

            return false;
        }
    }
}
