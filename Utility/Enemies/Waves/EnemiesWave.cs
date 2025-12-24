using Epic.OnlineServices.RTCAudio;
using OutwardGameSettings.BepInEx.Configs;
using OutwardGameSettings.Events;
using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Helpers;
using OutwardGameSettings.Utility.Helpers.Generic;
using SideLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Enemies.Waves
{
    public class EnemiesWave
    {
        private List<EnemyTemplate> enemies = new();

        private int minEnemies = 1;
        private int maxEnemies = 5;

        private int minRadiusSpawn = 20;
        private int maxRadiusSpawn = 40;

        private string notificationMessage = "Enemies are ambushing!";
        private string enemyNamePrefix = "Ambush Enemy";

        List<GlobalAudioManager.Sounds> ambushSounds = new List<GlobalAudioManager.Sounds>();

        private List<Vector3> cachedPositions = new();

        public EnemiesWave()
        {
        }

        public EnemiesWave(
            List<GlobalAudioManager.Sounds> ambushPossibleSounds,
            string notificationMessage = "Enemies are ambushing!", 
            string enemyNamePrefix = "Ambush Enemy", 
            int minEnemies = 1, 
            int maxEnemies = 5, 
            int minRadius = 20, 
            int maxRadius = 40
        )
        {
            AmbushSounds = ambushPossibleSounds;
            EnemyNamePrefix = enemyNamePrefix;
            NotificationMessage = notificationMessage;
            MinEnemies = minEnemies;
            MaxEnemies = maxEnemies;
            MinRadiusSpawn = minRadius; 
            MaxRadiusSpawn = maxRadius;
        }

        public int MinEnemies { get => minEnemies; set => minEnemies = value; }
        public int MaxEnemies { get => maxEnemies; set => maxEnemies = value; }
        public int MinRadiusSpawn { get => minRadiusSpawn; set => minRadiusSpawn = value; }
        public int MaxRadiusSpawn { get => maxRadiusSpawn; set => maxRadiusSpawn = value; }
        public string NotificationMessage { get => notificationMessage; set => notificationMessage = value; }
        public string EnemyNamePrefix { get => enemyNamePrefix; set => enemyNamePrefix = value; }
        public List<EnemyTemplate> Enemies { get => enemies; set => enemies = value; }
        public List<GlobalAudioManager.Sounds> AmbushSounds { get => ambushSounds; set => ambushSounds = value; }

        public virtual IEnumerator StartWave(bool shouldNotify = true, bool isWar = false)
        {
            Character masterCharacter = CharacterManager.Instance.GetFirstLocalCharacter();

            if (masterCharacter == null)
                yield break;

            int totalEnemies = UnityEngine.Random.Range(MinEnemies, MaxEnemies);

            yield return EnemyWaveManager.Instance.StartCoroutine(PrepareSpawnPositions(totalEnemies));

            if(EnemiesConfigs.ScaleScenarioEnemiesPower.Value)
                yield return EnemyWaveManager.Instance.StartCoroutine(SpawnEnemies(EnemyNamePrefix, CharacterHelpers.GetLobbiesSilverWorth()));
            else
                yield return EnemyWaveManager.Instance.StartCoroutine(SpawnEnemies(EnemyNamePrefix));

            if(shouldNotify)
                NotificationsManager.Instance.BroadcastGlobalTopNotification(NotificationMessage);

            int randomAudio = UnityEngine.Random.Range(0, AmbushSounds.Count);

            Global.AudioManager.PlaySoundAtPosition(AmbushSounds[randomAudio], masterCharacter.transform.position);

            if(!isWar)
                EventBusPublisher.SendEnemyWaveStarted();
        }

        IEnumerator PrepareSpawnPositions(int totalPositions)
        {
            cachedPositions.Clear();

            Character masterCharacter = CharacterManager.Instance.GetFirstLocalCharacter();

            if (masterCharacter == null)
                yield break;

            Vector3 position;

            for (int i = 0; i < totalPositions; i++)
            {
                position = LocationHelpers.GetRandomNavMeshPosition(
                    masterCharacter.transform.position,
                    MinRadiusSpawn,
                    MaxRadiusSpawn,
                    60
                );

                if (position != Vector3.zero)
                {
                    cachedPositions.Add(position);
                }

                // yield every few iterations to avoid frame freeze
                if (i % 3 == 0)
                    yield return null;
            }
        }

        IEnumerator SpawnEnemies(string enemyName)
        {
            foreach (var position in cachedPositions)
            {
                AddEnemy(enemyName, position);
                yield return new WaitForSeconds(0.1f); // delay between spawns
            }
        }

        IEnumerator SpawnEnemies(string enemyName, int equipmentPrice = 0)
        {
            int totalMoney = equipmentPrice;

            foreach (var position in cachedPositions.ToList())
            {
                AddEnemy(enemyName, position, out int unusedMoney, totalMoney);
                totalMoney -= Mathf.Clamp(unusedMoney, 0, totalMoney);
                yield return new WaitForSeconds(0.1f); // delay between spawns
            }
        }

        public EnemyTemplate AddEnemyAroundMaster(string name)
        {
            Character masterCharacter = CharacterManager.Instance.GetFirstLocalCharacter();

            if (masterCharacter == null)
                return new EnemyTemplate();

            Vector3 randomLocation = LocationHelpers.GetRandomNavMeshPosition(masterCharacter.transform.position, MinRadiusSpawn, MaxRadiusSpawn);

            if (randomLocation == Vector3.zero)
                randomLocation = masterCharacter.transform.position;

            EnemyTemplate enemyTemplate = AddEnemy(name, randomLocation);

            return enemyTemplate;
        }

        public virtual EnemyTemplate AddEnemy(string name, Vector3 position, out int unusedMoney, int equipmentPrice = 0)
        {
            unusedMoney = 0;
            return new EnemyTemplate();
        }

        public virtual EnemyTemplate AddEnemy(string name, Vector3 position)
        {
            return new EnemyTemplate();
        }

        public void CleanDeadBodies()
        {
            int totalAliveEnemies = 0;
            int totalDeadEnemies = 0;

            foreach(EnemyTemplate enemy in enemies)
            {
                if (enemy.Character.Alive)
                {
                    totalAliveEnemies++;
                    continue;
                }

                DestroyEnemy(enemy);
                totalDeadEnemies++;
            }

            OutwardGameSettings.LogMessage($"EnemiesWave@CleanDeadBodies Total Alive Enemies: {totalAliveEnemies} Total Dead Enemies: {totalDeadEnemies}");
        }

        public void DestroyEnemy(EnemyTemplate dummy)
        {
            if(Enemies.Contains(dummy))
                Enemies.Remove(dummy);

            if (dummy.CharacterExists)
                dummy.DestroyCharacter();
        }
    }
}
