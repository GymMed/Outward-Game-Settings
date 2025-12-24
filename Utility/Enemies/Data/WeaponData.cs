using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.Data
{
    public struct WeaponData
    {
        public int ItemId;
        public int GoldValue;
        public EquipmentSlot.EquipmentSlotIDs EquipmentSlotId;
        public Weapon.WeaponType WeaponType;

        public WeaponData(
            int itemId = 2130080, 
            int goldValue = 6, 
            EquipmentSlot.EquipmentSlotIDs equipmentSlotId = EquipmentSlot.EquipmentSlotIDs.Hands, 
            Weapon.WeaponType weaponType = Weapon.WeaponType.Spear_2H
        )
        {
            ItemId = itemId;
            GoldValue = goldValue;
            EquipmentSlotId = equipmentSlotId;
            WeaponType = weaponType;
        }
    }
}
