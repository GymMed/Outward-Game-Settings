using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Patches
{
    [HarmonyPatch(typeof(AttackSkill), "OwnerHasAllRequiredItems")]
    public class Patch_AttackSkill_OwnerHasAllRequiredItems
    {
        private static FieldInfo m_ownerCharacterField;
        private static FieldInfo m_weaponReqField;
        private static FieldInfo m_offhandReqField;
        
        static Patch_AttackSkill_OwnerHasAllRequiredItems()
        {
            var skillType = typeof(Skill);
            m_ownerCharacterField = skillType.GetField("m_ownerCharacter", BindingFlags.NonPublic | BindingFlags.Instance);
            
            var attackSkillType = typeof(AttackSkill);
            m_weaponReqField = attackSkillType.GetField("m_weaponReq", BindingFlags.NonPublic | BindingFlags.Instance);
            m_offhandReqField = attackSkillType.GetField("m_offhandReq", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        
        [HarmonyPrefix]
        static bool Prefix(AttackSkill __instance, bool _TryingToActivate, ref bool __result)
        {
            Character ownerCharacter = m_ownerCharacterField?.GetValue(__instance) as Character;
            
            // Only apply custom logic for AI characters
            if (ownerCharacter == null || !ownerCharacter.IsAI)
            {
                return true; // Run original for players
            }
            
            // Custom logic for AI (no UI notifications)
            try
            {
                bool hasWeapon = true;
                bool hasAmmo = true;
                
                m_weaponReqField?.SetValue(__instance, null);
                m_offhandReqField?.SetValue(__instance, null);
                
                // Check required weapon types
                if (__instance.RequiredWeaponTypes != null && __instance.RequiredWeaponTypes.Count > 0)
                {
                    if (ownerCharacter.CurrentWeapon == null || 
                        !__instance.RequiredWeaponTypes.Contains(ownerCharacter.CurrentWeapon.Type))
                    {
                        hasWeapon = false;
                    }
                    else
                    {
                        m_weaponReqField?.SetValue(__instance, ownerCharacter.CurrentWeapon);
                    }
                }
                
                // Check required offhand types
                if (__instance.RequiredOffHandTypes != null && __instance.RequiredOffHandTypes.Count > 0)
                {
                    if (ownerCharacter.LeftHandWeapon == null || 
                        !__instance.RequiredOffHandTypes.Contains(ownerCharacter.LeftHandWeapon.Type))
                    {
                        hasWeapon = false;
                    }
                    else
                    {
                        m_offhandReqField?.SetValue(__instance, ownerCharacter.LeftHandWeapon);
                    }
                }
                
                // Check ammunition
                if (__instance.AmmunitionTypes != null && __instance.AmmunitionTypes.Count > 0)
                {
                    hasAmmo = false;
                    for (int i = 0; i < __instance.AmmunitionTypes.Count; i++)
                    {
                        if (ownerCharacter.Inventory.HasAmmunitionTypeEquipped(__instance.AmmunitionTypes[i]))
                        {
                            if (__instance.AmmunitionAmount == -1 || 
                                ownerCharacter.Inventory.GetEquippedAmmunition().RemainingAmount >= __instance.AmmunitionAmount)
                            {
                                hasAmmo = true;
                                break;
                            }
                            break;
                        }
                    }
                }
                
                // NO UI NOTIFICATION FOR AI - just return false
                if (!hasWeapon || !hasAmmo)
                {
                    __result = false;
                    return false;
                }
                
                // Check imbue requirement
                if (hasWeapon && __instance.RequireImbue && 
                    (ownerCharacter.CurrentWeapon == null || !ownerCharacter.CurrentWeapon.Imbued))
                {
                    __result = false;
                    return false;
                }
                
                // Check required tags
                if (hasWeapon && __instance.RequiredTags != null && __instance.RequiredTags.Length != 0)
                {
                    bool hasTag = false;
                    
                    if (ownerCharacter.Inventory.SkillKnowledge.IsItemLearned(8205170) && 
                        (__instance.ItemID == 8100210 || __instance.ItemID == 8100220 || 
                         __instance.ItemID == 8100230 || __instance.ItemID == 8100240))
                    {
                        hasTag = true;
                    }
                    
                    for (int j = 0; j < __instance.RequiredTags.Length; j++)
                    {
                        if ((ownerCharacter.CurrentWeapon != null && 
                             ownerCharacter.CurrentWeapon.HasTag(__instance.RequiredTags[j].Tag)) || 
                            (ownerCharacter.LeftHandWeapon != null && 
                             ownerCharacter.LeftHandWeapon.HasTag(__instance.RequiredTags[j].Tag)) || 
                            (ownerCharacter.LeftHandEquipment != null && 
                             ownerCharacter.LeftHandEquipment.HasTag(__instance.RequiredTags[j].Tag)))
                        {
                            hasTag = true;
                            break;
                        }
                    }
                    
                    // NO UI NOTIFICATION FOR AI - just return false
                    if (!hasTag)
                    {
                        __result = false;
                        return false;
                    }
                }
                
                // Call base method logic
                __result = CheckBaseRequiredItems(__instance, _TryingToActivate, ownerCharacter);
                return false;
            }
            catch (Exception ex)
            {
                OutwardGameSettings.LogMessage($"Patch_AttackSkill_OwnerHasAllRequiredItem [AttackSkill AI Patch] Error: {ex}");
                return true; // Fallback to original
            }
        }
        
        private static bool CheckBaseRequiredItems(Skill skill, bool tryingToActivate, Character ownerCharacter)
        {
            if (skill.RequiredItems == null || skill.RequiredItems.Length == 0)
            {
                return true;
            }
            
            var updateMethod = typeof(Skill).GetMethod("UpdateLowestActivationCount", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            updateMethod?.Invoke(skill, null);
            
            if (tryingToActivate)
            {
                for (int i = 0; i < skill.RequiredItems.Length; i++)
                {
                    if (skill.RequiredItems[i] != null && 
                        skill.RequiredItems[i].Item != null && 
                        !ownerCharacter.Inventory.OwnsItem(skill.RequiredItems[i].Item.ItemID, 
                                                            skill.RequiredItems[i].Quantity))
                    {
                        // NO UI NOTIFICATION FOR AI
                        return false;
                    }
                }
                return true;
            }
            
            var lowestCountField = typeof(Skill).GetField("m_lowestActivationCount", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            int lowestCount = (int)(lowestCountField?.GetValue(skill) ?? 0);
            return lowestCount > 0;
        }
    }
}
