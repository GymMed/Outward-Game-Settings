using OutwardModsCommunicator.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Events
{
    public static class EventBusRegister
    {
        public static void RegisterEvents()
        {
            // Register all events for publishing/subscribing, when other mods can discover them
            EventBus.RegisterEvent(OutwardGameSettings.GUID, EventBusPublisher.EnchantmentMenuTryEnchant, "Event is firied before enchanting and before checking if prerequisites a valid", ("menu", typeof(EnchantmentMenu), "The enchantment menu instance that invoked the TryEnchant method."));
            EventBus.RegisterEvent(OutwardGameSettings.GUID, EventBusPublisher.EnchantmentTableDoneEnchantingFail, "Event is firied after enchantment failiure.", ("table", typeof(EnchantmentTable)));
            EventBus.RegisterEvent(OutwardGameSettings.GUID, EventBusPublisher.EnchantmentTableDoneEnchantingSuccess, "Event is firied after enchantment success.", ("table", typeof(EnchantmentTable)));

            EventBus.RegisterEvent(OutwardGameSettings.GUID, EventBusPublisher.CharacterOnReceiveHitLearnedSkill, "Event is fired after killing enemy and learning skill.", ("skillId", typeof(int), "Provides int skill id."));

            EventBus.RegisterEvent(OutwardGameSettings.GUID, EventBusPublisher.EnemyWaveStarted, "Event is fired after all ambush enemies has been spawned.");
            EventBus.RegisterEvent(OutwardGameSettings.GUID, EventBusPublisher.EnemyWarStarted, "Event is fired after all war enemies has been spawned.");
            EventBus.RegisterEvent(OutwardGameSettings.GUID, EventBusPublisher.EnemyWandererSpawned, "Event is fired after wanderer has spawned.");
        }
    }
}
