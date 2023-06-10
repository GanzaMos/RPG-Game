using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    [CreateAssetMenu(fileName = "StatStartPreset", menuName = "Stats/New Stat Start Preset", order = 1)]
    public class StartStatPresetSO : ScriptableObject
    {
        //===================================================================================
        //Vision sensor stats
        //===================================================================================

        #region Vision sensor stats

        [Space(5f)] [Header("Vision sensor stats")]
        
        [Tooltip("How spread is this enemy vision angle in horizontal plane, in degrees")]
        [Range(0, 360)] [SerializeField] 
        float visionHorizontalAngle = 140;

        [Tooltip("How spread is this enemy vision angle in vertical plane, in degrees")]
        [Range(0, 360)] [SerializeField] 
        float visionVerticalAngle = 30;

        [Tooltip("Bottom height point where enemies vision starts, in meters from enemies object origin point, in meters")]
        [SerializeField] 
        float visionBottomVertex = -0.9f;

        [Tooltip("Top height point where object vision starts, in meters from enemies object origin point, in meters")]
        [SerializeField] 
        float visionTopVertex = 0.9f;

        [Tooltip("How far this enemy can see objects in 'Stay' position, in meters")] 
        [SerializeField] 
        float visionRadius = 10f;

        #endregion

        //===================================================================================
        //Hearing sensor stats
        //===================================================================================

        #region Hearing sensor stats

        [Space(5f)] [Header("Hearing sensor stats")] 
        
        [Tooltip("How far can enemy hear, in meters")] 
        [SerializeField] 
        float hearingRadius = 7f;

        #endregion

        //===================================================================================
        //Sense sensor stats
        //===================================================================================

        #region Sense sensor stats

        [Space(5f)] [Header("Sense sensor stats")]
        
        [Tooltip("How far can enemy sense in 'Stay' position without hearing or seeing it, in meters")]
        [SerializeField]
        float senseRadius = 3f;

        #endregion


        //===================================================================================
        //Awareness stats
        //===================================================================================

        #region Awareness stats

        [Space(5f)] 
        [Header("Sense sensor stats")]
        
        [Tooltip("How fast this enemy decrease his awareness when don't see/hear/sense it, in point/second")]
        [SerializeField] 
        float awarenessDecreaseRate;

        [Tooltip("How much time this enemy need not see/hear/sense a suspicious target to decrease Awareness, in seconds")]
        [SerializeField] 
        float awarenessDecreaseDelay;

        [Tooltip("Awareness increase rate if object is staying and fully visible, points per second")]
        [SerializeField] 
        float awarenessRatePerSecond;

        [Tooltip("Multiply awareness coefficient for crawling. Usually less than 1. Vision coefficient = 1")]
        [SerializeField] 
        float crawlFactor;
        
        [Tooltip("Multiply awareness coefficient for hearing. Usually less than 1. Vision coefficient = 1")]
        [SerializeField] 
        float hearingFactor;

        [Tooltip("Multiply awareness coefficient for sense. Usually less than 1. Vision coefficient = 1")]
        [SerializeField] 
        float senseFactor;
        
        [Tooltip("Multiply awareness coefficient for dead body. Usually more than 1. Vision coefficient = 1")]
        [SerializeField] 
        float deadBodyFactor;

        [Tooltip("How far from its start location should be idling enemy to trigger awareness increasing, in meters")]
        [SerializeField]
        float startPositionTolerance;
        
        #endregion

        
        //===================================================================================
        //Experience, level, class
        //===================================================================================

        #region Experience, level, class

        [Space(5f)] 
        [Header("Experience, level, class stats")]
        
        [Tooltip("Character start level")]
        [SerializeField] 
        int currentLevel = 1;
        
        [Tooltip("Character class, needs for Progression SO table")]
        [SerializeField]
        CharacterClass characterClass;

        #endregion
        
        //===================================================================================
        //AI behavior
        //===================================================================================

        #region AI behavior

        [Space(5f)]
        [Header("AI behavior")]

        [Tooltip("How close can enemy reach the Waypoint before stop, in meters")]
        [SerializeField]
        float waypointTolerance = 1f;

        [Tooltip("How much time this enemy need will wait nearby every Waypoint, in seconds")]
        [SerializeField]
        float maxDwellingTimeAtWaypoint = 3f;

        [Tooltip("Enemy walk modifier that reduce his max Speed in calm (Awareness level = 0-1) state")]
        [SerializeField]
        float calmSpeedFraction = 0.5f;
        
        [Tooltip("How much time this enemy need to leave the target (if he missed it) and go back to his duties, in seconds")]
        [SerializeField]
        float maxSuspicionTime = 3f;

        [Tooltip("Time that enemy needs to restart Provoke status if he missed the target, in seconds")]
        [SerializeField]
        float maxProvokedTime = 5f;

        [Tooltip("How far can 'shouts' this enemy in case of alarm to provoke his neighbours, in meters")]
        [SerializeField]
        float neighboursProvokingRadius = 10f;

        #endregion


        //setup start structure

        public void SetupCharacterStruct(Character ch)
        {
            //setup vision sensor start stats
            ch.data.vision.HorizontalAngle = visionHorizontalAngle;
            ch.data.vision.VerticalAngle = visionVerticalAngle;
            ch.data.vision.BottomVertex = visionBottomVertex;
            ch.data.vision.TopVertex = visionTopVertex;
            ch.data.vision.Radius = visionRadius;

            //setup hearing sensor start stats
            ch.data.hearing.Radius = hearingRadius;

            //setup sense sensor start stats
            ch.data.sense.Radius = senseRadius;

            //setup awareness stats
            ch.data.awareness.DecreaseRate = awarenessDecreaseRate;
            ch.data.awareness.DecreaseDelay = awarenessDecreaseDelay;
            ch.data.awareness.RatePerSecond = awarenessRatePerSecond;
            ch.data.awareness.CrawlFactor = crawlFactor;
            ch.data.awareness.HearingFactor = hearingFactor;
            ch.data.awareness.SenseFactor = senseFactor;
            ch.data.awareness.DeadBodyFactor = deadBodyFactor;
            ch.data.awareness.StartPositionTolerance = startPositionTolerance;
            ch.data.awareness.ThreatInNear = false;


            //setup experience
            ch.data.exp.CharacterClass = characterClass;
            ch.data.exp.CurrentLevel = currentLevel;
            ch.data.exp.CurrentExperiencePoints = 0; //todo this is not quiet correct, in current version level and experience are connected
            
            //setup AI stats
            ch.data.ai.WaypointTolerance = waypointTolerance;
            ch.data.ai.MaxDwellingTimeAtWaypoint = maxDwellingTimeAtWaypoint;
            ch.data.ai.CalmSpeedFraction = calmSpeedFraction;
            ch.data.ai.MaxProvokedTime = maxProvokedTime;
            ch.data.ai.MaxSuspicionTime = maxSuspicionTime;
            ch.data.ai.NeighboursProvokingRadius = neighboursProvokingRadius;
        }
    }
}