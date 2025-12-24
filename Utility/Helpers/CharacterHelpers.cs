using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Helpers
{
    public static class CharacterHelpers
    {
        public static int GetLobbiesSilverWorth()
        {
            Character character = null;
            int totalSilverWorth = 0;

            foreach (PlayerSystem ps in Global.Lobby.PlayersInLobby)
            {
                character = ps.ControlledCharacter;

                if(character?.Inventory?.TotalValue != null)
                    totalSilverWorth += character.Inventory.TotalValue;
            }

            return totalSilverWorth;
        }

        public static void FixCharacterAINullOnQuestEvent(CharacterAI charAI)
        {
            var type = typeof(CharacterAI);
            var field = type.GetField("m_aiActiveOnQuestEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                var currentValue = field.GetValue(charAI);
                if (currentValue == null)
                {
                    // Assign a new QuestEventReference instance
                    var newQER = Activator.CreateInstance(field.FieldType);
                    field.SetValue(charAI, newQER);
                }
            }
        }
    }
}
