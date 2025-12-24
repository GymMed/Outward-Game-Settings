using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.Data
{
    public struct TroglodyteTypeData
    {
        public int Health;
        public int? WeaponId;
        public int? ArmorId;

        public TroglodyteTypeData(int health = 100, int? weaponId = null, int? armorId = null)
        {
            Health = health;
            WeaponId = weaponId;
            ArmorId = armorId;
        }
    }
}
