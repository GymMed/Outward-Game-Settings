using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.AI.Effects
{
    public class AIESpawnBullets : AIEffect
    {
        public override void PerformEffect(CharacterAI _owner, object[] _params)
        {
            base.PerformEffect(_owner, _params);
            if (PhotonNetwork.isMasterClient)
            {
                if (_owner.Character?.Inventory?.Pouch == null)
                    return;

                List<Item> inventoryItems = _owner.Character.Inventory.Pouch.GetContainedItems();
                int bulletsId = 4400080;
                
                if(inventoryItems.Count > 0)
                {
                    foreach (Item inventoryItem in inventoryItems)
                    {
                        if (inventoryItem.ItemID != bulletsId)
                            continue;
#if DEBUG
                        OutwardGameSettings.LogMessage($"AIESpawnBullets@PerformEffect Bullets added to inventory!");
#endif
                        return;
                    }
                }

                Item item = ResourcesPrefabManager.Instance.GenerateItem(bulletsId.ToString());

                if (item != null)
                    _owner.Character.Inventory.GenerateItem(item, 5, false);
            }
        }
    }
}
