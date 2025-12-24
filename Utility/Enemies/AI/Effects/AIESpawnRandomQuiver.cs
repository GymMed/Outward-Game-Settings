using OutwardGameSettings.Managers;
using OutwardGameSettings.Utility.Enemies.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.AI.Effects
{
    public class AIESpawnRandomQuiver : AIEffect
    {
        public override void PerformEffect(CharacterAI _owner, object[] _params)
        {
            base.PerformEffect(_owner, _params);
            if (PhotonNetwork.isMasterClient)
            {
                if (_owner.Character?.Inventory?.Pouch == null)
                    return;

                List<Item> inventoryItems = _owner.Character.Inventory.Pouch.GetContainedItems();
                
                if(inventoryItems.Count > 0)
                {
                    foreach (Item inventoryItem in inventoryItems)
                    {
                        if(inventoryItem is Weapon weapon && weapon.Type == Weapon.WeaponType.Arrow)
                        {
#if DEBUG
                            OutwardGameSettings.LogMessage($"AIESpawnRandomQuiver@PerformEffect Arrow Equipped!");
#endif
                            _owner.Character.Inventory.EquipItem(weapon, true);
                            return;
                        }
                    }
                }

                if (!EnemyEquipmentManager.Instance.GetRandomWeaponOfType(Weapon.WeaponType.Arrow, out WeaponData randomArrow))
                {
                    OutwardGameSettings.LogMessage($"AIESpawnRandomQuiver@PerformEffect failed to retrieve Arrow!");
                    return;
                }

                Item item = (Item)ResourcesPrefabManager.Instance.GenerateItem(randomArrow.ItemId.ToString());

                if (item != null)
                    _owner.Character.Inventory.GenerateItem(item, 5, true);
            }
        }
    }
}
