using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.Data
{
    public struct EquipmentGroupData
    {
        public int ItemId;
        public int GoldValue;
        public EquipmentSlot.EquipmentSlotIDs EquipmentSlotId;

        public EquipmentGroupData(
            int itemId, 
            int goldValue, 
            EquipmentSlot.EquipmentSlotIDs equipmentSlotId
        )
        {
            ItemId = itemId;
            GoldValue = goldValue;
            EquipmentSlotId = equipmentSlotId;
        }
    }
}
