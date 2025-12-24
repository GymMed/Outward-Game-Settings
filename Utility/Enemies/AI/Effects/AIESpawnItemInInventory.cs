using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.AI.Effects
{
    public class AIESpawnItemInInventory : AIEffect
    {
        public override void PerformEffect(CharacterAI _owner, object[] _params)
        {
            base.PerformEffect(_owner, _params);
            if (PhotonNetwork.isMasterClient && (!this.RequireLocomotion || (_owner.Character.InLocomotion && _owner.Character.NextIsLocomotion)))
            {
                Item item = (Item)ResourcesPrefabManager.Instance.GenerateItem(ItemId.ToString());

                if(item != null)
                    _owner.Character.Inventory.GenerateItem(item, QuantityToSpawn, false);
            }
        }

        public int QuantityToSpawn = 1;

        public int ItemId;

        public bool RequireLocomotion = true;
    }
}
