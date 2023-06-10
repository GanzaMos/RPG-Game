using System;
using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    [System.Serializable]
    public class CharacterDataStruct
    {
        public void SetCharacterReference(Character ch)
        {
            vision.ch = ch;
            sense.ch = ch;
            hearing.ch = ch;
            awareness.ch = ch;
            health.ch = ch;
            exp.ch = ch;
            ai.ch = ch;
        }

        //===================================================================================
        //Vision sensor stats
        //===================================================================================

        public VisionStruct vision;
        [System.Serializable]
        public class VisionStruct
        {
            public Character ch;
            
            [SerializeField] float horizontalAngle;
            public float HorizontalAngle
            {
                get => horizontalAngle;
                set
                {
                    horizontalAngle = value;
                    HorizontalAngleCos = Mathf.Cos(value/2 * Mathf.Deg2Rad);
                    ch.Events.OnVisionDataChange?.Invoke(this);
                }
            }
            [SerializeField] public float HorizontalAngleCos { get; private set; }
            
            [SerializeField] float verticalAngle;
            public float VerticalAngle
            {
                get => verticalAngle;
                set
                {
                    verticalAngle = value;
                    VerticalAngleCos = Mathf.Cos((value/2) * Mathf.Deg2Rad);
                    ch.Events.OnVisionDataChange?.Invoke(this);
                }
            }
            [SerializeField] public float VerticalAngleCos { get; private set; }
            
            [SerializeField] public float BottomVertex { get; set; }
            [SerializeField] public float TopVertex { get; set; }
            
            [SerializeField] float radius;
            public float Radius
            {
                get => radius;
                set
                {
                    radius = value;
                    RadiusSqr = value * value;
                    EdgeRadius = value * 3;
                    EdgeRadiusSqr = EdgeRadius * EdgeRadius; 
                    ch.Events.OnVisionDataChange?.Invoke(this);
                }
            }
            [SerializeField]
            public float RadiusSqr { get; private set; }
            public float EdgeRadius { get; private set; }
            public float EdgeRadiusSqr { get; private set; }
        }
        
        
        //===================================================================================
        //Hearing sensor stats
        //===================================================================================
        
        public HearingStruct hearing;
        [System.Serializable]
        public class HearingStruct
        {
            public Character ch;
            
            [SerializeField] float radius;
            public float Radius
            {
                get => radius;
                set
                {
                    radius = value;
                    RadiusSqr = value * value;
                }
            }
            [SerializeField] public float RadiusSqr { get; private set; }
        }
        
        //===================================================================================
        //Sense sensor stats
        //===================================================================================
        
        public SenseStruct sense;
        [System.Serializable]
        public class SenseStruct
        {
            public Character ch;
            
            [SerializeField] float radius;
            public float Radius
            {
                get => radius;
                set
                {
                    radius = value;
                    RadiusSqr = value * value;
                }
            }
            [SerializeField] public float RadiusSqr { get; private set; }
        }

        //===================================================================================
        //Awareness sensor stats
        //===================================================================================

        public AwarenessStruct awareness;
        [System.Serializable]
        public class AwarenessStruct
        {
            public Character ch;
            
            [SerializeField] public float DecreaseRate { get; set; }
            [SerializeField] public float DecreaseDelay { get; set; }
            
            [SerializeField] float ratePerSecond;
            public float RatePerSecond
            {
                get => ratePerSecond;
                set
                {
                    if (ratePerSecond != value)
                        ch.Events.OnAwarenessRatePerSecondChange?.Invoke(value);
                    ratePerSecond = value;
                }
            }

            [SerializeField] public float CrawlFactor { get; set; }
            [SerializeField] public float HearingFactor { get; set; }
            [SerializeField] public float SenseFactor { get; set; }
            [SerializeField] public float DeadBodyFactor { get; set; }
            [SerializeField] public float StartPositionTolerance { get; set; }
            [SerializeField] public bool ThreatInNear { get; set; }
        }
        
        //===================================================================================
        //Health stats
        //===================================================================================
        
        public HealthStruct health;
        [System.Serializable]
        public class HealthStruct
        {
            public Character ch;
            
            [SerializeField] public float MaxHealth { get; set; }
            [SerializeField] public float CurrentHealth { get; set; }
            [SerializeField] public float LevelUpPercentageRestore { get; set; }

            [SerializeField] bool isDead;
            public bool IsDead
            {
                get => isDead;
                set
                {
                    if (isDead != value)
                        ch.Events.OnIsDeadChange?.Invoke(value);
                    isDead = value;
                }
            }
        }
        
        
        //===================================================================================
        //Experience stats
        //===================================================================================

        public ExperienceStruct exp;
        [Serializable]
        public class ExperienceStruct
        {
            public Character ch;
            
            [SerializeField] public float CurrentExperiencePoints { get; set; }
            [SerializeField] public int CurrentLevel { get; set; }
            [SerializeField] public CharacterClass CharacterClass { get; set; }

        }
        
        
        //===================================================================================
        //AI behavior stats
        //===================================================================================
        
        public AIBehaviorStruct ai;
        [System.Serializable]
        public class AIBehaviorStruct
        {
            public Character ch;
            
            [SerializeField] public float MaxSuspicionTime { get; set; }
            [SerializeField] public float MaxProvokedTime { get; set; }
            [SerializeField] public float NeighboursProvokingRadius { get; set; }
            [SerializeField] public float WaypointTolerance { get; set; }
            [SerializeField] public float MaxDwellingTimeAtWaypoint { get; set; }
            [SerializeField] public float CalmSpeedFraction { get; set; }
            
            [SerializeField] bool isProvoked;
            public bool IsProvoked
            {
                get => isProvoked;
                set
                {
                    if (isProvoked != value)
                        ch.Events.OnIsProvokedChange?.Invoke(value);
                    isProvoked = value;
                }
            }
        }

    }
}