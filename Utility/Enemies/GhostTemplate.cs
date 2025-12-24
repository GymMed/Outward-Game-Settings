using OutwardGameSettings.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies
{
    public class GhostTemplate : EnemyTemplate
    {
        public void SpawGhost()
        {
            int buffGrog = 8200702;
            SkillsIds.Add(buffGrog);

            SpawnOrReset();
        }

        public override void ApplyStats()
        {
            Template.Health = 175;
            Template.HealthRegen = 5;
        }

        public override void ApplyEquipment(int forPriceAmount = 0)
        {
            if(Template.Weapon_ID == null)
            {
                int randomWeapon = UnityEngine.Random.Range(0, 2);
                
                if(randomWeapon == 0)
                    Template.Weapon_ID = 2000042;
                else
                    Template.Weapon_ID = 2010002;
            }

            if(Template.Helmet_ID == null)
            {
                Template.Helmet_ID = 3200041;
            }

            if(Template.Chest_ID == null)
            {
                Template.Chest_ID = 3200040;
            }

            if(Template.Boots_ID == null)
            {
                Template.Boots_ID = 3200042;
            }
        }
    }
}
