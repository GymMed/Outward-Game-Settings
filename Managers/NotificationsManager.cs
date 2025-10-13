using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Managers
{
    public class NotificationsManager
    {
        private static NotificationsManager _instance;

        private NotificationsManager()
        {
        }

        public static NotificationsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NotificationsManager();

                return _instance;
            }
        }

        public void BroadcastGlobalSideNotification(string message)
        {
            Character character = null;

            foreach (PlayerSystem ps in Global.Lobby.PlayersInLobby)
            {
                character = ps.ControlledCharacter;

                if(character?.CharacterUI != null)
                    character.CharacterUI.ShowInfoNotification(message);
            }
        }

        public void BroadcastGlobalTopNotification(string message)
        {
            Character character = null;

            foreach (PlayerSystem ps in Global.Lobby.PlayersInLobby)
            {
                character = ps.ControlledCharacter;

                if(character?.CharacterUI?.NotificationPanel != null)
                    character.CharacterUI.NotificationPanel.ShowNotification(message);
            }
        }
    }
}
