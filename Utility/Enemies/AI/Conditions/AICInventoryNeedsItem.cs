using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Enemies.AI.Conditions
{
    public class AICInventoryNeedsItem : AICondition
    {
        private new void Update()
        {
            base.Update();
            if (Time.time - this.m_timeOfLastUpdate > this.DetectionUpdateTime)
            {
                this.m_timeOfLastUpdate = Time.time;
                List<Item> ownsItems = this.m_characterAI.Character.Inventory.GetOwnedItems(SearchingItemId);

                int totalAmount = 0;
                foreach (Item currentItem in ownsItems)
                {
                    totalAmount += currentItem.RemainingAmount;
                }

                if (ownsItems == null || ownsItems.Count > 0 || totalAmount < NeededItemAmount)
                {
                    if (!this.Reverse)
                    {
                        this.ConditionValidEvent();
                        return;
                    }
                    this.ConditionInvalidEvent();
                    return;
                }
                else
                {
                    if (!this.Reverse)
                    {
                        this.ConditionInvalidEvent();
                        return;
                    }
                    this.ConditionValidEvent();
                }
            }
        }

        public int SearchingItemId;

        public int NeededItemAmount = 1;

        public bool Reverse;

        public float DetectionUpdateTime = 0.5f;

        private float m_timeOfLastUpdate;
    }
}
