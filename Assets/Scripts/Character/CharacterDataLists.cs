using System;
using System.Collections.Generic;
using AI;
using AI.Sensors;

namespace RPG.Attributes
{
    public class CharacterDataLists
    {
        //Sensors
        public List<DetectableTarget> EdgeList = new List<DetectableTarget>();
        public Action<DetectableTarget> OnAddToEdgeList;
        public Action<DetectableTarget> OnRemoveFromEdgeList;
        
        public List<DetectableTarget> InHearingRadius = new List<DetectableTarget>(); 
        public Action<DetectableTarget> OnAddToHearingRadius;
        public Action<DetectableTarget> OnRemoveFromHearingRadius;
        
        public List<DetectableTarget> InVisionRadius = new List<DetectableTarget>(); 
        public Action<DetectableTarget> OnAddToInVisionRadius;
        public Action<DetectableTarget> OnRemoveFromInVisionRadius;
        
        public List<DetectableTarget> InVisionField = new List<DetectableTarget>();
        public Action<DetectableTarget> OnAddToInVisionField;
        public Action<DetectableTarget> OnRemoveFromInVisionField;

        public List<DetectableTarget> InSenseRadius = new List<DetectableTarget>();
        public Action<DetectableTarget> OnAddToInSenseRadius;
        public Action<DetectableTarget> OnRemoveFromInSenseRadius;
        
        public List<DetectableTarget> InSenseField = new List<DetectableTarget>();
        public Action<DetectableTarget> OnAddToInSenseField;
        public Action<DetectableTarget> OnRemoveFromInSenseField;
        
        public List<DetectableTarget> ThreatsInVisionRadius = new List<DetectableTarget>(); 
        public Action<DetectableTarget> OnAddToThreatsInVisionRadius;
        public Action<DetectableTarget> OnRemoveFromThreatsInVisionRadius;
        
        //Awareness
        public Dictionary<DetectableTarget, TrackedTarget> AwarenessTargets = new Dictionary<DetectableTarget, TrackedTarget>();
        public Action<DetectableTarget> OnAddToAwarenessTargets;
        public Action<DetectableTarget> OnRemoveFromAwarenessTargets;
       
        public Action<DetectableTarget> OnIncreaseToLevel1Awareness;
        public Action<DetectableTarget> OnIncreaseToLevel2Awareness;
        public Action<DetectableTarget> OnIncreaseToLevel3Awareness;
        
        public Action<DetectableTarget> OnDeclineToLevel2Awareness;
        public Action<DetectableTarget> OnDeclineToLevel1Awareness;
        public Action<DetectableTarget> OnDeclineToLevel0Awareness;
        
        public Action<DetectableTarget> OnDeclineToZeroAwareness;

        //Memory
        [Serializable]
        public class SeenTargets : SerializableDictionary<DetectableTarget, MemoryDetails> { }
        public SeenTargets seenTargets;
        public Action<DetectableTarget, MemoryDetails> OnUpdateMemoryCharacterClass;
        public Action<DetectableTarget, MemoryDetails> OnUpdateMemoryNeedToCheckFromStart;
        public Action<DetectableTarget, MemoryDetails> OnUpdateMemoryIsDead;
        public Action<DetectableTarget, MemoryDetails> OnUpdateMemoryIsProvoked;
    }
}