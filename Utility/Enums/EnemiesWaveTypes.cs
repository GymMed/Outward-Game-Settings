using OutwardGameSettings.Utility.Enemies;
using OutwardGameSettings.Utility.Enemies.Waves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enums
{
    public enum EnemiesWaveTypes
    {
        Bandits,
        Troglodytes,
        Skeletons,
        Ghosts,
#if DEBUG
        Dummies,
        ArcherBandits,
        ArcherSkeletons,
        GunnerBandits,
        GunnerSkeletons
#endif
    }

    public static class EnemiesWaveTypesHelper
    {
        public static readonly Dictionary<EnemiesWaveTypes, EnemiesWave> EnemiesWaves = new()
        {
            { EnemiesWaveTypes.Bandits, new BanditsWave() },
            { EnemiesWaveTypes.Troglodytes, new TroglodytesWave() },
            { EnemiesWaveTypes.Skeletons, new SkeletonsWave() },
            { EnemiesWaveTypes.Ghosts, new GhostsWave() },
#if DEBUG
            { EnemiesWaveTypes.Dummies, new DummiesWave() },
            { EnemiesWaveTypes.ArcherBandits, CreateSpecializedWave(EnemiesWaveTypes.Bandits, Weapon.WeaponType.Bow) },
            { EnemiesWaveTypes.ArcherSkeletons, CreateSpecializedWave(EnemiesWaveTypes.Skeletons, Weapon.WeaponType.Bow) },
            { EnemiesWaveTypes.GunnerBandits, CreateSpecializedWave(EnemiesWaveTypes.Bandits, Weapon.WeaponType.Pistol_OH) },
            { EnemiesWaveTypes.GunnerSkeletons, CreateSpecializedWave(EnemiesWaveTypes.Skeletons, Weapon.WeaponType.Pistol_OH) }
#endif
        };

        public static EnemiesWave CreateSpecializedWave(EnemiesWaveTypes waveType, Weapon.WeaponType weaponType)
        {
            AnyWeaponWave wave = null;

            switch(waveType)
            {
                case EnemiesWaveTypes.Bandits:
                    {
                        wave = new BanditsWave();
                        break;
                    }
                case EnemiesWaveTypes.Skeletons:
                    {
                        wave = new SkeletonsWave();
                        break;
                    }
                case EnemiesWaveTypes.Troglodytes:
                default:
                    {
                        return new DummiesWave();
                    }                   
            }

            wave.WeaponTypes.Add(weaponType);

            return wave;
        }
    }
}
