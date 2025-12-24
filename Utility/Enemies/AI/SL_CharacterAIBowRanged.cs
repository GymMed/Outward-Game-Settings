using OutwardGameSettings.Utility.Enemies.AI.Effects;
using OutwardGameSettings.Utility.Enums;
using OutwardGameSettings.Utility.Helpers;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AI;

namespace OutwardGameSettings.Utility.Enemies.AI
{
	public class SL_CharacterAIBowRanged : SL_CharacterAI
	{
        [XmlIgnore]
        [NonSerialized]
		public AISCombatRanged AISCombatRanged = null;

        [XmlIgnore]
        [NonSerialized]
		public CharacterAI CharacterAI = null;

		protected override void ApplyToCharacter(Character character)
		{
			AIRoot airoot = new GameObject("BasicRanged_AIRoot").AddComponent<AIRoot>();
			//airoot.m_charAI.m_aiActiveOnQuestEvent = new QuestEventReference();

			airoot.gameObject.SetActive(false);
			airoot.transform.parent = character.transform;
			AISWander aiswander = new GameObject("1_Wander").AddComponent<AISWander>();
			aiswander.transform.parent = airoot.transform;
			AISSuspicious aissuspicious = new GameObject("2_Suspicious").AddComponent<AISSuspicious>();
			aissuspicious.transform.parent = airoot.transform;
			AISSuspicious aissuspicious2 = new GameObject("3_Alert").AddComponent<AISSuspicious>();
			aissuspicious2.transform.parent = airoot.transform;
			AISCombatRanged = new GameObject("4_CombatRanged").AddComponent<AISCombatRanged>();
			AISCombatRanged.transform.parent = airoot.transform;
			aiswander.ContagionRange = this.AIContagionRange;
			aiswander.ForceNotCombat = this.ForceNonCombat;
			aiswander.SpeedModif = this.Wander_Speed;
			aiswander.WanderFar = this.CanWanderFar;
			aiswander.AutoFollowPlayer = this.Wander_FollowPlayer;
			bool flag = this.Wander_PatrolWaypoints != null && this.Wander_Type == AISWander.WanderType.Patrol;
			if (flag)
			{
				GameObject gameObject = new GameObject(string.Format("Waypoints_{0}", character.UID));
				aiswander.WaypointsParent = gameObject.transform;
				for (int i = 0; i < this.Wander_PatrolWaypoints.Length; i++)
				{
					GameObject gameObject2 = new GameObject("Waypoint " + i.ToString() + 1.ToString());
					Waypoint waypoint = gameObject2.AddComponent<Waypoint>();
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.transform.position = this.Wander_PatrolWaypoints[i].WorldPosition;
					waypoint.RandomRadius = this.Wander_PatrolWaypoints[i].RandomRadius;
					waypoint.WaitTime = this.Wander_PatrolWaypoints[i].WaitTime;
				}
			}
			AICEnemyDetection aicenemyDetection = new GameObject("Detection").AddComponent<AICEnemyDetection>();
			aicenemyDetection.transform.parent = aiswander.transform;
			AIESwitchState aieswitchState = new GameObject("DetectEffects").AddComponent<AIESwitchState>();
			aieswitchState.ToState = aissuspicious;
			aieswitchState.transform.parent = aicenemyDetection.transform;
			aicenemyDetection.DetectEffectsTrans = aieswitchState.transform;
			aissuspicious.SpeedModif = this.Suspicious_Speed;
			aissuspicious.SuspiciousDuration = this.Suspicious_Duration;
			aissuspicious.Range = this.Suspicious_Range;
			aissuspicious.WanderFar = this.CanWanderFar;
			aissuspicious.TurnModif = this.Suspicious_TurnModif;
			AIESwitchState aieswitchState2 = new GameObject("EndSuspiciousEffects").AddComponent<AIESwitchState>();
			aieswitchState2.ToState = aiswander;
			AIESheathe aiesheathe = aieswitchState2.gameObject.AddComponent<AIESheathe>();
			aiesheathe.Sheathed = true;
			aieswitchState2.transform.parent = aissuspicious.transform;
			aissuspicious.EndSuspiciousEffectsTrans = aieswitchState2.transform;
			AICEnemyDetection aicenemyDetection2 = new GameObject("Detection").AddComponent<AICEnemyDetection>();
			aicenemyDetection2.transform.parent = aissuspicious.transform;
			AIESwitchState aieswitchState3 = new GameObject("DetectEffects").AddComponent<AIESwitchState>();
			aieswitchState3.ToState = AISCombatRanged;
			aieswitchState3.transform.parent = aicenemyDetection2.transform;
			aicenemyDetection2.DetectEffectsTrans = aieswitchState3.transform;
			AIESwitchState aieswitchState4 = new GameObject("SuspiciousEffects").AddComponent<AIESwitchState>();
			aieswitchState4.ToState = aissuspicious2;
			aieswitchState4.transform.parent = aicenemyDetection2.transform;
			aicenemyDetection2.SuspiciousEffectsTrans = aieswitchState4.transform;
			aissuspicious2.SpeedModif = this.Suspicious_Speed;
			aissuspicious2.SuspiciousDuration = this.Suspicious_Duration;
			aissuspicious2.Range = this.Suspicious_Range;
			aissuspicious2.WanderFar = this.CanWanderFar;
			aissuspicious2.TurnModif = this.Suspicious_TurnModif;
			AIESwitchState aieswitchState5 = new GameObject("EndSuspiciousEffects").AddComponent<AIESwitchState>();
			aieswitchState5.ToState = aissuspicious;
			AIESheathe aiesheathe2 = aieswitchState5.gameObject.AddComponent<AIESheathe>();
			aiesheathe2.Sheathed = true;
			aieswitchState5.transform.parent = aissuspicious2.transform;
			aissuspicious2.EndSuspiciousEffectsTrans = aieswitchState5.transform;
			AICEnemyDetection aicenemyDetection3 = new GameObject("Detection").AddComponent<AICEnemyDetection>();
			aicenemyDetection3.transform.parent = aissuspicious2.transform;
			AIESwitchState aieswitchState6 = new GameObject("DetectEffects").AddComponent<AIESwitchState>();
			aieswitchState6.ToState = AISCombatRanged;
			aieswitchState6.transform.parent = aicenemyDetection3.transform;
			aicenemyDetection3.DetectEffectsTrans = aieswitchState6.transform;
			AISCombatRanged.ChargeTime = this.Combat_ChargeTime;
			//aiscombatRanged.TargetVulnerableChargeTimeMult = this.Combat_TargetVulnerableChargeModifier;
			//aiscombatRanged.ChargeAttackRangeMult = this.Combat_ChargeAttackRangeMulti;
			//aiscombatRanged.ChargeAttackTimeToAttack = this.Combat_ChargeTimeToAttack;
			//aiscombatRanged.ChargeStartRangeMult = this.Combat_ChargeStartRangeMult;
			//aiscombatRanged.AttackPatterns = this.Combat_AttackPatterns;
			AISCombatRanged.SpeedModifs = this.Combat_SpeedModifiers;
			AISCombatRanged.ChanceToAttack = this.Combat_ChanceToAttack;
			AISCombatRanged.KnowsUnblockable = this.Combat_KnowsUnblockable;
			AISCombatRanged.DodgeCooldown = this.Combat_DodgeCooldown;
			AISCombatRanged.CanBlock = this.CanBlock;
			AISCombatRanged.CanDodge = this.CanDodge;
			AISCombatRanged.ShootChargeWeapon = true;
			AICEnemyDetection aicenemyDetection4 = new GameObject("Detection").AddComponent<AICEnemyDetection>();
			aicenemyDetection4.transform.parent = AISCombatRanged.transform;
			AIESwitchState aieswitchState7 = new GameObject("EndCombatEffects").AddComponent<AIESwitchState>();
			aieswitchState7.ToState = aiswander;
			aieswitchState7.transform.parent = AISCombatRanged.transform;
			character.gameObject.AddComponent<NavMeshAgent>();
			character.gameObject.AddComponent<AISquadMember>();
			character.gameObject.AddComponent<EditorCharacterAILoadAI>();
			NavMeshObstacle component = character.GetComponent<NavMeshObstacle>();
			bool flag2 = component != null;
			if (flag2)
			{
				UnityEngine.Object.Destroy(component);
			}
			CharacterAI = character.gameObject.AddComponent<CharacterAI>();
			CharacterAI.m_character = character;
			CharacterAI.AIStatesPrefab = airoot;

			CharacterHelpers.FixCharacterAINullOnQuestEvent(CharacterAI);
            AddQuiverRenewal(AISCombatRanged, CharacterAI);

			CharacterAI.GetAIStates();
		}

		public void AddQuiverRenewal(AISCombat combatState, CharacterAI characterAI)
		{
            AICQuiverEmpty quiverEmptyCondition = new GameObject($"Condition_Quiver_Empty").AddComponent<AICQuiverEmpty>();
            quiverEmptyCondition.DetectionUpdateTime = 1;
            //quiverEmptyCondition.m_characterAI = characterAI;
            quiverEmptyCondition.transform.SetParent(combatState.transform);
            quiverEmptyCondition.SubCondition = false;

            //quiverEmptyCondition.m_subConditionsValid = true;

            AIESpawnRandomQuiver effectSpawnArrows = new GameObject($"ValidEffects").AddComponent<AIESpawnRandomQuiver>();
            effectSpawnArrows.transform.SetParent(quiverEmptyCondition.transform);

            quiverEmptyCondition.GroupValidEffectTrans = quiverEmptyCondition.transform;
            //combatState.Init(characterAI);
		}

		public float AIContagionRange = 20f;

		public Vector2 Range = new Vector2(8, 30);

		public float Wander_Speed = 1.1f;

		public bool Wander_FollowPlayer;

		public AISWander.WanderType Wander_Type = AISWander.WanderType.Wander;

		public SL_Waypoint[] Wander_PatrolWaypoints;

		public float Suspicious_Speed = 1.75f;

		public float Suspicious_Duration = 5f;

		public float Suspicious_Range = 30f;

		public float Suspicious_TurnModif = 0.9f;

		public Vector2 Combat_ChargeTime = new Vector2(0.4f, 1.2f);

		public float Combat_TargetVulnerableChargeModifier = 0.5f;

		public float Combat_ChargeAttackRangeMulti = 1f;

		public float Combat_ChargeTimeToAttack = 0.4f;

		public Vector2 Combat_ChargeStartRangeMult = new Vector2(0.8f, 3f);

		public float[] Combat_SpeedModifiers = new float[]
		{
			1.1f,
			1.3f,
			1.8f
		};

		public float Combat_ChanceToAttack = 75f;

		public bool Combat_KnowsUnblockable = true;

		public float Combat_DodgeCooldown = 3f;

		public AttackPattern[] Combat_AttackPatterns = new AttackPattern[]
		{
			new AttackPattern
			{
				ID = 0,
				Chance = 20f,
				Range = new Vector2(0.9f, 2.5f),
				Attacks = new AttackPattern.AtkTypes[1]
			},
			new AttackPattern
			{
				ID = 1,
				Chance = 15f,
				Range = new Vector2(0f, 2.9f),
				Attacks = new AttackPattern.AtkTypes[2]
			},
			new AttackPattern
			{
				ID = 2,
				Chance = 30f,
				Range = new Vector2(0f, 1.5f),
				Attacks = new AttackPattern.AtkTypes[]
				{
					AttackPattern.AtkTypes.Special
				}
			},
			new AttackPattern
			{
				ID = 3,
				Chance = 30f,
				Range = new Vector2(0f, 1.5f),
				Attacks = new AttackPattern.AtkTypes[]
				{
					AttackPattern.AtkTypes.Normal,
					AttackPattern.AtkTypes.Special
				}
			},
			new AttackPattern
			{
				ID = 4,
				Chance = 30f,
				Range = new Vector2(0f, 1.3f),
				Attacks = new AttackPattern.AtkTypes[]
				{
					AttackPattern.AtkTypes.Normal,
					AttackPattern.AtkTypes.Normal,
					AttackPattern.AtkTypes.Special
				}
			}
		};
	}
}
