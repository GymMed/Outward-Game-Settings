using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.Waves
{
    public class AnyWeaponWave: EnemiesWave
    {
        public List<Weapon.WeaponType> WeaponTypes = new();
    }
}
