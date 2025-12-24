using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Enemies.Waves
{
    public class GhostsWave : EnemiesWave
    {
        public GhostsWave() : base()
        {
            AmbushSounds = GetAmbushSoundsList();
            NotificationMessage = "Ghosts are ambushing!";
            EnemyNamePrefix = "Ambush Ghost";
        }

        public override EnemyTemplate AddEnemy(string name, Vector3 location, out int unusedMoney, int equipmentPrice = 0)
        {
            unusedMoney = equipmentPrice;
            return AddEnemy(name, location);
        }

        public override EnemyTemplate AddEnemy(string name, Vector3 location)
        {
            var dummy = GetGhostTemplate(name, location);

            dummy.SpawGhost();

            return dummy;
        }

        public GhostTemplate GetGhostTemplate(string name, Vector3 location)
        {
            return new GhostTemplate 
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
                    Faction = Character.Factions.CorruptionSpirit,
                }
            };
        }

        public List<GlobalAudioManager.Sounds> GetAmbushSoundsList()
        {
            List<GlobalAudioManager.Sounds> sounds = new List<GlobalAudioManager.Sounds>();
            sounds.Add(GlobalAudioManager.Sounds.CS_Kruger_Roar);
            sounds.Add(GlobalAudioManager.Sounds.CS_Kruger_RoarLong);
            sounds.Add(GlobalAudioManager.Sounds.CS_Kruger_Death);

            return sounds;
        }
    }
}
