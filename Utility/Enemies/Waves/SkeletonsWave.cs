using OutwardGameSettings.Managers;
using ParadoxNotion.Internal;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Enemies.Waves
{
    public class SkeletonsWave : AnyWeaponWave
    {
        public SkeletonsWave() : base()
        {
            AmbushSounds = GetAmbushSoundsList();
            NotificationMessage = "Skeletons are ambushing!";
            EnemyNamePrefix = "Ambush Skeleton";
        }

        public override EnemyTemplate AddEnemy(string name, Vector3 location)
        {
            if(WeaponTypes != null && WeaponTypes.Count > 0)
                return AddEnemy(name, location, WeaponTypes);

            var dummy = GetSkeletonTemplate(name, location);

            dummy.SpawnRandomSkeleton();

            return dummy;
        }

        public override EnemyTemplate AddEnemy(string name, Vector3 position, out int unusedMoney, int equipmentPrice = 0)
        {
            unusedMoney = equipmentPrice;

            if(WeaponTypes != null && WeaponTypes.Count > 0)
                return AddEnemy(name, position, WeaponTypes, out unusedMoney, equipmentPrice);

            var dummy = GetSkeletonTemplate(name, position);

            unusedMoney = dummy.SpawnRandomSkeletonForPrice(equipmentPrice);

            return dummy;
        }

        public SkeletonTemplate AddEnemy(string name, Vector3 location, List<Weapon.WeaponType> weaponTypes, out int unusedMoney, int equipmentPrice = 0)
        {
            var dummy = GetSkeletonTemplate(name, location);

            unusedMoney = dummy.SpawnRandomSkeletonForPrice(weaponTypes, equipmentPrice);

            return dummy;
        }

        public EnemyTemplate AddEnemy(string name, Vector3 location, List<Weapon.WeaponType> weaponTypes)
        {
            var dummy = GetSkeletonTemplate(name, location);

            dummy.SpawnRandomSkeleton(weaponTypes);

            return dummy;
        }

        public SkeletonTemplate GetSkeletonTemplate(string name, Vector3 location)
        {
            return new SkeletonTemplate
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
                    Faction = Character.Factions.Mercs,
                }
            };
        }

        public List<GlobalAudioManager.Sounds> GetAmbushSoundsList()
        {
            List<GlobalAudioManager.Sounds> sounds = new List<GlobalAudioManager.Sounds>();
            sounds.Add(GlobalAudioManager.Sounds.CS_AncientDweller_Mermaid_Sing_01);
            sounds.Add(GlobalAudioManager.Sounds.CS_AncientDweller_Mermaid_Sing_02);
            sounds.Add(GlobalAudioManager.Sounds.CS_AncientDweller_Mermaid_Sing_03);
            sounds.Add(GlobalAudioManager.Sounds.CS_AncientDweller_Mermaid_Sing_04);

            return sounds;
        }
    }
}
