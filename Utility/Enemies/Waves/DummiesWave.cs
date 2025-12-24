using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Enemies.Waves
{
    internal class DummiesWave : EnemiesWave
    {
        public DummiesWave() : base()
        {
            AmbushSounds = GetAmbushSoundsList();
            NotificationMessage = "Dummies are ambushing!";
            EnemyNamePrefix = "Ambush Dummy";
        }

        public override EnemyTemplate AddEnemy(string name, Vector3 location, out int unusedMoney, int equipmentPrice = 0)
        {
            unusedMoney = equipmentPrice;
            var dummy = AddEnemy(name, location);

            return dummy;
        }

        public override EnemyTemplate AddEnemy(string name, Vector3 location)
        {
            var dummy = GetDummyTemplate(name, location);

            dummy.SpawnOrReset();

            return dummy;
        }

        public EnemyTemplate GetDummyTemplate(string name, Vector3 location)
        {

            return new EnemyTemplate
            {
                Name = name,
                AddCombatAI = true,
                SpawnPosition = location,
                Template = new SL_Character()
                {
                    Name = name,
                    UID = UID.Generate().Value,
                    Faction = Character.Factions.Bandits,
                    Weapon_ID = 2000010,
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
