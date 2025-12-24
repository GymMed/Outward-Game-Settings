using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Enemies.Waves
{
    public class BanditsWave : AnyWeaponWave
    {
        public BanditsWave() : base()
        {
            AmbushSounds = GetAmbushSoundsList();
            NotificationMessage = "Bandits are ambushing!";
            EnemyNamePrefix = "Ambush Bandit";
        }

        public override EnemyTemplate AddEnemy(string name, Vector3 location)
        {
            if (WeaponTypes != null && WeaponTypes.Count > 0)
            {
                return AddEnemy(name, location, WeaponTypes);
            }

            var dummy = GetBanditTemplate(name, location);

            dummy.SpawnBandit(true);

            return dummy;
        }

        public override EnemyTemplate AddEnemy(string name, Vector3 position, out int unusedMoney, int equipmentPrice = 0)
        {
            unusedMoney = equipmentPrice;

            if (WeaponTypes != null && WeaponTypes.Count > 0)
            {
                return AddEnemy(name, position, WeaponTypes, out unusedMoney, equipmentPrice);
            }

            var dummy = GetBanditTemplate(name, position);

            unusedMoney = dummy.SpawnBandit(true, equipmentPrice);

            return dummy;
        }

        public BanditTemplate AddEnemy(string name, Vector3 location, List<Weapon.WeaponType> weaponTypes, out int unusedMoney, int equipmentPrice = 0)
        {
            var dummy = GetBanditTemplate(name, location);

            unusedMoney = dummy.SpawnBandit(weaponTypes, true, equipmentPrice);

            return dummy;
        }

        public BanditTemplate AddEnemy(string name, Vector3 location, List<Weapon.WeaponType> weaponTypes)
        {
            var dummy = GetBanditTemplate(name, location);

            dummy.SpawnBandit(weaponTypes, true);

            return dummy;
        }

        public BanditTemplate GetBanditTemplate(string name, Vector3 location)
        {
            return new BanditTemplate
            {
                Name = name,
                AddCombatAI = true,
                CanDodge = true,
                CanBlock = true,
                SpawnPosition = location,
                Template = new SL_Character()
                {
                    Name = name,
                    UID = UID.Generate().Value,
                    Faction = Character.Factions.Bandits,
                    //Weapon_ID = 2000010,
                    //Shield_ID = 2000010,
                    //Helmet_ID = 2000010,
                    //Chest_ID = 2000010,
                    //Boots_ID = 3000043,
                    //Backpack_ID = 2000010,
                }
            };
        }

        public List<GlobalAudioManager.Sounds> GetAmbushSoundsList()
        {
            List<GlobalAudioManager.Sounds> sounds = new List<GlobalAudioManager.Sounds>();
            sounds.Add(GlobalAudioManager.Sounds.CS_Human_Yelling1);
            sounds.Add(GlobalAudioManager.Sounds.CS_Human_Yelling2);
            sounds.Add(GlobalAudioManager.Sounds.CS_Human_Yelling3);
            sounds.Add(GlobalAudioManager.Sounds.CS_Human_Yelling4);
            sounds.Add(GlobalAudioManager.Sounds.CS_Human_Yelling5);
            sounds.Add(GlobalAudioManager.Sounds.CS_Human_Yelling6);
            sounds.Add(GlobalAudioManager.Sounds.CS_Human_Yelling7);

            return sounds;
        }
    }
}
