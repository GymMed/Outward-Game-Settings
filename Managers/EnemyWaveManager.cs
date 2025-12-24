using OutwardGameSettings.BepInEx.Configs;
using OutwardGameSettings.Utility.Enemies;
using OutwardGameSettings.Utility.Enemies.Waves;
using OutwardGameSettings.Utility.Helpers.Generic;
using OutwardGameSettings.Utility.Helpers;
using SideLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using OutwardGameSettings.Events;
using OutwardGameSettings.Utility.Timer.Controllers;
using OutwardGameSettings.Utility.Enums;

namespace OutwardGameSettings.Managers
{
    public class EnemyWaveManager : MonoBehaviour
    {
        private static EnemyWaveManager _instance;

        public static EnemyWaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var obj = new GameObject("GymMed_EnemyWaveManager");
                    _instance = obj.AddComponent<EnemyWaveManager>();
                    DontDestroyOnLoad(obj);
                }

                return _instance;
            }
        }

        private List<EnemiesWave> enemiesWaves = new();
        private int lastHour = 0;
        private int lastDay = 0;

        public List<EnemiesWave> EnemiesWaves { get => enemiesWaves; set => enemiesWaves = value; }
        public int LastHour { get => lastHour; set => lastHour = value; }
        public int LastDay { get => lastDay; set => lastDay = value; }

        private void Awake()
        {
            // Prevent duplicate managers across scene loads
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Init()
        {
            //lastDay = EnvironmentConditions.Instance.m_baseDay;
            //lastHour = (int)EnvironmentConditions.Instance.TimeOfDay;

            foreach(KeyValuePair<EnemiesWaveTypes, EnemiesWave> wave in EnemiesWaveTypesHelper.EnemiesWaves)
            {
                EnemiesWaves.Add(wave.Value);
            }

            SL.OnSceneLoaded += () =>
            {
                TOD_Time time = TOD_Sky.Instance.TODTime;

                if (time == null)
                {
                    OutwardGameSettings.LogMessage($"EnemyWaveManager@Init TODTime is missing! Cannot set enemy spawn timers.");
                    return;
                }

                time.OnHour += () =>
                {
                    CheckForEventChance();
                };
            };
        }

        public void CheckForEventChance()
        {
            IncreaseHour();
#if DEBUG
            OutwardGameSettings.LogMessage($"EnemyWaveManager@CheckForEventChance checking events {lastDay} day {lastHour} hour!");
#endif
            CheckHourPassAndSimulateEvents();

            // only ambush by bandits even though enough space for war
            if(AreaManager.Instance.GetIsCurrentAreaTownOrCity())
            {
                EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.War].SimulateCheck();
                EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.Wanderer].SimulateCheck();

                if (!EnemyAmbushesConfigs.EnableEnemyAmbushes.Value)
                    return;

                if (EnvironmentConditions.Instance.TimeOfDay > 20f || EnvironmentConditions.Instance.TimeOfDay < 4f)
                {
                    EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.Ambush].SimulateCheck();
                    return;
                }

                if (!EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.Ambush].PassedHourlyCheck())
                    return;

                SpawnRandomBanditWave();
                return;
            }

            // wars are restricted only to open worlds
            if (AreasHelper.IsCurrentAreaInOpenWorld())
            {
                RollDiceForEvent(EnemyHourlyEvents.Ambush);
                RollDiceForEvent(EnemyHourlyEvents.Wanderer);
                RollDiceForEvent(EnemyHourlyEvents.War);

                return;
            }

            RollDiceForEvent(EnemyHourlyEvents.Ambush);
            RollDiceForEvent(EnemyHourlyEvents.Wanderer);
        }

        public void RollDiceForEvent(EnemyHourlyEvents passedEvent)
        {
            switch(passedEvent)
            {
                case EnemyHourlyEvents.War:
                    {
                        if (EnemyWarsConfigs.EnableEnemyWars.Value && 
                            EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.War].PassedHourlyCheck())
                            StartWarOfRandomSize();
                        break;
                    }
                case EnemyHourlyEvents.Ambush:
                    {
                        if(EnemyAmbushesConfigs.EnableEnemyAmbushes.Value && 
                            EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.Ambush].PassedHourlyCheck())
                            SpawnRandomWave(EnemyAmbushesConfigs.NotifyOnAmbush.Value);
                        break;
                    }
                case EnemyHourlyEvents.Wanderer:
                default:
                    {
                        if (EnemySpawnsConfigs.EnableRandomEnemySpawns.Value && 
                            EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.Wanderer].PassedHourlyCheck())
                        {
                            SpawnRandomWanderer();
                        }
                        break;
                    }
            }
        }

        public void SpawnRandomWanderer()
        {
            if (EnemiesWaves.Count < 1)
                return;

            int randomWaveIndex = UnityEngine.Random.Range(0, EnemiesWaves.Count);
            SpawnWanderer(randomWaveIndex);
        }

        public void SpawnRandomBanditWave(bool notifyOnAmbush = false)
        {
            List<BanditsWave> banditWaves = new();

            foreach(EnemiesWave wave in EnemiesWaves)
            {
                if(wave is BanditsWave banditWave)
                {
                    banditWaves.Add(banditWave);
                }
            }

            if (banditWaves.Count == 0)
            {
                OutwardGameSettings.LogMessage($"EnemyWaveManager@SpawnRandomBanditWave couldn't find any bandit waves in manager!");
                return;
            }

            int randomBanditWaveIndex = UnityEngine.Random.Range(0, banditWaves.Count);
            StartCoroutine(banditWaves[randomBanditWaveIndex].StartWave(notifyOnAmbush));
        }

        public void SpawnRandomWave(bool notifyOnAmbush = true)
        {
            OutwardGameSettings.LogMessage($"EnemyWaveManager@SpawnRandomWave got called! {EnemiesWaves.Count}.");
            try
            {
                Character masterCharacter = CharacterManager.Instance.GetFirstLocalCharacter();// Global.Lobby.PlayersInLobby[0].ControlledCharacter;

                if (masterCharacter == null)
                    return;

                if(EnemiesWaves.Count < 1)
                {
                    OutwardGameSettings.LogMessage($"EnemyWaveManager@SpawnRandomWave missing enemy waves. Total enemy waves: {EnemiesWaves.Count}.");
                    return;
                }

                int randomAmbushType = UnityEngine.Random.Range(0, EnemiesWaves.Count);

                StartCoroutine(EnemiesWaves[randomAmbushType].StartWave(notifyOnAmbush));
            }
            catch(Exception ex)
            {
                OutwardGameSettings.LogMessage($"EnemyWaveManager@SpawnRandomWave encountered and error: \"{ex.Message}\"");
            }
        }

        public void StartWarOfRandomSize()
        {
            int size = UnityEngine.Random.Range(
                EnemyWarsConfigs.MinSizeOfTheWar.Value, 
                EnemyWarsConfigs.MaxSizeOfTheWar.Value
            );

            NotificationsManager.Instance.BroadcastGlobalTopNotification("War between factions has started!");
            StartCoroutine(StartWar(size));
        }

        public IEnumerator StartWar(int wavesAmount)
        {
            for (int currentWave = 0; currentWave < wavesAmount; currentWave++)
            {
                SpawnRandomWave(false);
                yield return new WaitForSeconds(0.3f);
            }

            EventBusPublisher.SendEnemyWarStarted();
        }

        public void StartWave(int wave, bool notifyOnAmbush = true)
        {
            if (wave < EnemiesWaves.Count)
                StartCoroutine(EnemiesWaves[wave].StartWave(notifyOnAmbush));
        }

        public void SpawnWanderer(int waveIndex)
        {
            if(waveIndex > EnemiesWaves.Count - 1)
            {
                return;
            }

            EnemiesWaves[waveIndex].AddEnemyAroundMaster("Wanderer");
            EventBusPublisher.SendEnemyWandererSpawned();
        }

        public void CleanDeadBodies()
        {
            foreach(EnemiesWave wave in EnemiesWaves)
            {
                wave.CleanDeadBodies();
            }
        }

        public void IncreaseHour()
        {
            if(lastHour == 23)
            {
                lastDay++;
                lastHour = 0;
                return;
            }

            lastHour++;
        }

        public void CheckHourPassAndSimulateEvents()
        {
            if (TOD_Sky.Instance?.Cycle?.DateTime == null)
            {
                OutwardGameSettings.LogMessage($"EnemyWaveManager@CheckHourPassAndSimulateEvents Date time is null. Can't increase events chance!");
                return;
            }

            DateTime gameTime = TOD_Sky.Instance.Cycle.DateTime;

            OutwardGameSettings.LogMessage($"EnemyWaveManager@CheckHourPassAndSimulateEvents Checking hours passed! {gameTime}");

            int simulateAmount = 0;
            // used instead of gameTime day because developers never used it
            int gameDay = Mathf.FloorToInt(EnvironmentConditions.GameTimeF / 24f) + 1;

            if(lastDay < gameDay)
            {
                int daysPassed = gameDay - lastDay;
                simulateAmount = daysPassed * 24;
            }

            int gameHours = gameTime.Hour;// (int)EnvironmentConditions.Instance.TimeOfDay;

            if(lastHour != gameHours)
            {
                if(lastHour < gameHours)
                {
                    simulateAmount = gameHours - lastHour;
                }
                else
                {
                    int leftHoursToFullDay = 24 - lastHour;
                    simulateAmount += leftHoursToFullDay + gameHours;
                }
            }

            for(int currentSimulatedHour = 0; currentSimulatedHour < simulateAmount; currentSimulatedHour++)
            {
                EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.War].SimulateCheck();
                EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.Ambush].SimulateCheck();
                EnemyHourlyEventsHelper.Events[EnemyHourlyEvents.Wanderer].SimulateCheck();
            }

            lastHour = gameHours;
            lastDay = gameDay;
        }
    }
}
