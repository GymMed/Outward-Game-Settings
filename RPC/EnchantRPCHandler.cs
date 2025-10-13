using OutwardGameSettings.Managers;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Photon;

namespace OutwardGameSettings.RPC
{
    /*
    public class EnchantRPCHandler : Photon.MonoBehaviour
    {
        [PunRPC]
        public static void RPC_EnchantingResult(string uid, bool success, PhotonMessageInfo info)
        {
            var itemManager = ItemManager.Instance;
            var item = itemManager.GetItem(uid);

            if (!(item is EnchantmentTable table))
            {
                SL.Log($"{OutwardGameSettings.prefix} Could not find EnchantmentTable with UID={uid}");
                return;
            }

            if (!success)
            {
                // Broadcast failure notification
                NotificationsManager.Instance.BroadcastGlobalSideNotification($"{table.PendingEnchantment.Name} on {table.ContainedItem.Name} failed!");
            }

            if (success)
            {
                // Call original DoneEnchanting via reflection
                var method = typeof(EnchantmentTable).GetMethod(
                    "DoneEnchanting",
                    BindingFlags.Instance | BindingFlags.NonPublic
                );
                method?.Invoke(table, null);
            }
            else
            {
                // Failure — partial cleanup, same as original but without enchant
                table.ActivateFX(false);

                if (!PhotonNetwork.isNonMasterClientInRoom)
                {
                    table.LockPillars(false);
                }

                table.ConsumeIncenses();

                var pendingField = typeof(EnchantmentTable).GetField("m_pendingEnchantment",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                var timeField = typeof(EnchantmentTable).GetField("m_remainingEnchantTime",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                pendingField?.SetValue(table, null);
                timeField?.SetValue(table, -999f);

                var eventField = typeof(EnchantmentTable).GetField("OnDoneEnchanting",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                (eventField?.GetValue(table) as EventHandler)?.Invoke(table, EventArgs.Empty);
            }
        }

    }
    */
}
