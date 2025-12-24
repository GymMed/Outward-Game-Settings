using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enums
{
    public enum WeaponHoldingTypes
    {
        MainHanded,
        OffHanded,
        TwoHanded,
    }

    public static class WeaponHoldingTypesHelpers
    {
        public static readonly HashSet<Weapon.WeaponType> MainHanded = new()
        {
            Weapon.WeaponType.Sword_1H,
            Weapon.WeaponType.Axe_1H,
            Weapon.WeaponType.Mace_1H
        };

        public static readonly HashSet<Weapon.WeaponType> OffHanded = new()
        {
            Weapon.WeaponType.Dagger_OH,
            Weapon.WeaponType.Chakram_OH,
            Weapon.WeaponType.Pistol_OH,
            Weapon.WeaponType.Shield
        };

        public static readonly HashSet<Weapon.WeaponType> TwoHanded = new()
        {
            Weapon.WeaponType.Halberd_2H,
            Weapon.WeaponType.Sword_2H,
            Weapon.WeaponType.Axe_2H,
            Weapon.WeaponType.Mace_2H,
            Weapon.WeaponType.Spear_2H,
            Weapon.WeaponType.FistW_2H,
            Weapon.WeaponType.Arrow,
            Weapon.WeaponType.Bow,
        };

        public static readonly Dictionary<WeaponHoldingTypes, HashSet<Weapon.WeaponType>> Types = new()
        {
            { WeaponHoldingTypes.MainHanded, MainHanded },
            { WeaponHoldingTypes.OffHanded, OffHanded },
            { WeaponHoldingTypes.TwoHanded, TwoHanded },
        };

        public static WeaponHoldingTypes GetHoldingType(Weapon.WeaponType type)
        {
            foreach (var kvp in Types)
            {
                if (kvp.Value.Contains(type))
                    return kvp.Key;
            }

            return WeaponHoldingTypes.TwoHanded; // fallback default
        }
    }
}
