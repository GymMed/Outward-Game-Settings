using OutwardGameSettings.BepInEx.Configs;
using OutwardGameSettings.Managers;
using OutwardGameSettings.Serializable;
using OutwardGameSettings.Utility.Timer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enums
{
    public enum EnemyHourlyEvents
    {
        War,
        Ambush,
        Wanderer
    }

    public static class EnemyHourlyEventsHelper
    {
        public static Dictionary<EnemyHourlyEvents, InGameEventController> Events = new()
        {
            // in week 0.595238f
            { EnemyHourlyEvents.War, new(EnemyWarsConfigs.ChanceToGetIntoWarZone.Value, 168) },
            // in 3 days 1.3888f
            { EnemyHourlyEvents.Ambush, new(EnemyAmbushesConfigs.ChanceToGetAmbushed.Value, 72) },
            // in 2 days 2.0833f
            { EnemyHourlyEvents.Wanderer, new(EnemySpawnsConfigs.ChanceToMeetWanderer.Value, 48) },
        };

        public static EventSaveData CreateCurrentCharacterSaveData()
        {
            return CreateSaveData(EnemyWaveManager.Instance.LastDay, EnemyWaveManager.Instance.LastHour, Events.Values.ToArray());
        }

        public static EventSaveData CreateSaveData(int lastDay, int lastHour, InGameEventController[] controllers)
        {
            var data = new EventSaveData()
            {
                LastDay = lastDay,
                LastHour = lastHour,
            };

            foreach(var controller in controllers)
            {
                data.Accumulators.Add(controller.Accumulator);
            }

            return data;
        }

        public static void ApplySaveData(EventSaveData data)
        {
            if (data == null) return;

            EnemyWaveManager.Instance.LastDay = data.LastDay;
            EnemyWaveManager.Instance.LastHour = data.LastHour;

            int totalEventsRegistered = Enum.GetValues(typeof(EnemyHourlyEvents)).Length;

            int totalEvents = data.Accumulators.Count > totalEventsRegistered ? totalEventsRegistered : data.Accumulators.Count;

            for (int currentEvent = 0; currentEvent < totalEvents; currentEvent++)
            {
                EnemyHourlyEventsHelper.Events[(EnemyHourlyEvents)currentEvent].Accumulator = data.Accumulators[currentEvent];
            }
        }

    }
}
