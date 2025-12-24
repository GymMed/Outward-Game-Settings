using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Enemies.Waves
{
    public class TroglodytesWave : EnemiesWave
    {
        public TroglodytesWave() : base()
        {
            AmbushSounds = GetAmbushSoundsList();
            NotificationMessage = "Troglodyte are ambushing!";
            EnemyNamePrefix = "Ambush Troglodyte";
        }

        public override EnemyTemplate AddEnemy(string name, Vector3 location, out int unusedMoney, int equipmentPrice = 0)
        {
            var dummy = GetTroglodyteTemplate(name, location);

            unusedMoney = dummy.SpawnRandomTrogForPrice(equipmentPrice);

            return dummy;
        }

        public override EnemyTemplate AddEnemy(string name, Vector3 location)
        {
            var dummy = GetTroglodyteTemplate(name, location);

            dummy.SpawnRandomTrog();

            return dummy;
        }

        public TroglodyteTemplate GetTroglodyteTemplate(string name, Vector3 location)
        {
            return new TroglodyteTemplate
            {
                Name = name,
                AddCombatAI = true,
                CanBlock = true,
                SpawnPosition = location,
                Template = new SL_Character()
                {
                    Name = name,
                    UID = UID.Generate().Value,
                    Faction = Character.Factions.Tuanosaurs,
                }
            };
        }

        public List<GlobalAudioManager.Sounds> GetAmbushSoundsList()
        {
            List<GlobalAudioManager.Sounds> sounds = new List<GlobalAudioManager.Sounds>();
            sounds.Add(GlobalAudioManager.Sounds.CS_Troglodyte_Taunt_Grunt1);
            sounds.Add(GlobalAudioManager.Sounds.CS_Troglodyte_Taunt_Grunt2);
            sounds.Add(GlobalAudioManager.Sounds.CS_Troglodyte_Taunt_Grunt3);
            sounds.Add(GlobalAudioManager.Sounds.CS_Troglodyte_Taunt_Grunt4);

            return sounds;
        }
    }
}
