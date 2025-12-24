using HarmonyLib;
using OutwardGameSettings.BepInEx.Configs;
using OutwardGameSettings.Events;
using OutwardGameSettings.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.OnReceiveHit))]
    public class Patch_Character_OnReceiveHit
    {
        static void Prefix(Character __instance, Weapon _weapon, float _damage, DamageList _damageList, Vector3 _hitDir, Vector3 _hitPoint, float _angle, float _angleDir, Character _dealerChar, float _knockBack)
        {
            if (!SkillsExpertiseConfigs.CanLearnSkillsOnKill.Value)
                return;

            // problem to solve if effect(bleed/poison) kills enemy and player changes weapon
            if (_dealerChar == null || _dealerChar.CurrentWeapon == null)
                return;

            //if (_dealerChar.Faction != Character.Factions.Player)
            //    return;
            if (_dealerChar.IsAI)
                return;

            OutwardGameSettings.LogMessage($"Patch_Character_OnReceiveHit instance health: {__instance.Health} {__instance.Alive} {__instance.IsDead}");
            if (_damage < __instance.Health || !__instance.Alive)
                return;
            OutwardGameSettings.LogMessage($"Patch_Character_OnReceiveHit instance passed");

            bool isPlayerInLobby = false;

            foreach(var playerSystem in Global.Lobby.PlayersInLobby)
            {
                if(_dealerChar.UID.Value == playerSystem.ControlledCharacter.UID.Value)
                {
                    isPlayerInLobby = true;
                    break;
                }
            }

            if (!isPlayerInLobby)
                return;

            OutwardGameSettings.LogMessage($"Patch_Character_OnReceiveHit player is in lobby");
            int learnSkillDice = UnityEngine.Random.Range(1, 101);

            if (SkillsExpertiseConfigs.ChanceToLearnSkillOnKill.Value < learnSkillDice)
                return;
            OutwardGameSettings.LogMessage($"Patch_Character_OnReceiveHit dice passed");

            Weapon.WeaponType weaponType = _weapon.Type;

            if(weaponType == Weapon.WeaponType.Arrow)
            {
                weaponType = _dealerChar.CurrentWeapon.Type;

                if(weaponType != Weapon.WeaponType.Bow)
                {
                    weaponType = Weapon.WeaponType.Pistol_OH;
                }
            }

            int skillId = -1;

            if (!EnemySkillsManager.Instance.GetUnownedRandomSkillOfWeaponType(_dealerChar, weaponType, out skillId))
            {
                if(!SkillsExpertiseConfigs.IfAllWeaponSkillsKnownLearnRandom.Value)
                    return;

                if (!EnemySkillsManager.Instance.GetUnownedRandomWeaponSkill(_dealerChar, out skillId))
                    if (!EnemySkillsManager.Instance.GetUnownedRandomWeaponlessSkill(_dealerChar, out skillId))
                    {
                        OutwardGameSettings.LogMessage($"Patch_Character_OnReceiveHit character has all the skills!");
                        return;
                    }
            }

            _dealerChar.Inventory.ReceiveSkillReward(skillId);
            EventBusPublisher.SendLearnedSkillOnKill(skillId);
#if DEBUG
            OutwardGameSettings.LogMessage($"Patch_Character_OnReceiveHit gave skill: {skillId}");
#endif
        }
    }
}
