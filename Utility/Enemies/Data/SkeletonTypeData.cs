using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.Data
{
    public struct SkeletonTypeData
    {
        public int Health;
        public int HelmetId;
        public int ChestId;
        public int BootsId;

        public SkeletonTypeData(int health = 100, int helmetId = 3200031, int chestId = 3200030, int bootsId = 3200032)
        {
            Health = health;
            HelmetId = helmetId;
            ChestId = chestId;
            BootsId = bootsId;
        }
    }
}
