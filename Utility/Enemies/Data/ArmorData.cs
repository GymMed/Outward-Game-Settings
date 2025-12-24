using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.Data
{
    public struct ArmorData
    {
        public int ItemId;
        public int GoldValue;
        public EquipmentSlot.EquipmentSlotIDs EquipmentSlotId;
        public Armor.ArmorClass ArmorClass;

        public ArmorData(
            int itemId, 
            int goldValue, 
            EquipmentSlot.EquipmentSlotIDs equipmentSlotId, 
            Armor.ArmorClass armorClass = Armor.ArmorClass.Light
        )
        {
            ItemId = itemId;
            GoldValue = goldValue;
            EquipmentSlotId = equipmentSlotId;
            ArmorClass = armorClass;
        }
    }
}
