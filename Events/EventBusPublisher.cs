using OutwardModsCommunicator.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Events
{
    public static class EventBusPublisher
    {
        // if this keeps growing, I recommend creating enum a little bit more verbose
        public const string EnchantmentMenuTryEnchant = "EnchantmentMenu@TryEnchant";
        public const string EnchantmentTableDoneEnchantingFail = "EnchantmentTable@DoneEnchanting_Fail";
        public const string EnchantmentTableDoneEnchantingSuccess = "EnchantmentTable@DoneEnchanting_Success";

        public static void SendTryEnchant(EnchantmentMenu enchantmentMenu)
        {
            var payload = new EventPayload
            {
                ["menu"] = enchantmentMenu,
            };

            EventBus.Publish(OutwardGameSettings.GUID, EnchantmentMenuTryEnchant, payload);
        }

        public static void SendFailEnchanting(EnchantmentTable enchantmentTable)
        {
            var payload = new EventPayload
            {
                ["table"] = enchantmentTable,
            };

            EventBus.Publish(OutwardGameSettings.GUID, EnchantmentTableDoneEnchantingFail, payload);
        }

        public static void SendSuccessEnchanting(EnchantmentTable enchantmentTable)
        {
            var payload = new EventPayload
            {
                ["table"] = enchantmentTable,
            };

            EventBus.Publish(OutwardGameSettings.GUID, EnchantmentTableDoneEnchantingSuccess, payload);
        }
    }
}
