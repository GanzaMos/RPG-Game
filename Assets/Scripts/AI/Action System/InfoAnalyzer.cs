using System;
using System.Collections.Generic;
using System.Text;
using AI.Sensors;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace AI.Action_System
{
    public class InfoAnalyzer : MonoBehaviour
    {
        Character _ch;
        ActionController _actionController;

        //Lists
        List<DetectableTarget> _inSenseField;
        List<DetectableTarget> _inVisionField;
        CharacterDataLists.SeenTargets _seenTargets;
        Dictionary<DetectableTarget, TrackedTarget> _awarenessTargets;

        float _timer;
        float _timerPeriod = 0.1f;

        void Awake()
        {
            _ch = GetComponentInParent<Character>();
            _actionController = GetComponent<ActionController>();
            _timer = _timerPeriod;

            SetLists();
        }

        void SetLists()
        {
            _awarenessTargets = _ch.DataLists.AwarenessTargets;
            _inSenseField = _ch.DataLists.InSenseField;
            _inVisionField = _ch.DataLists.InVisionField;
            _seenTargets = _ch.DataLists.seenTargets;
        }

        void OnEnable()
        {
            _ch.DataLists.OnIncreaseToLevel1Awareness += AwarenessUpToLevel1;
            _ch.DataLists.OnIncreaseToLevel2Awareness += AwarenessUpToLevel2;
            _ch.DataLists.OnIncreaseToLevel3Awareness += AwarenessUpToLevel3;
            
            _ch.DataLists.OnDeclineToLevel2Awareness += AwarenessDownToLevel2;
            _ch.DataLists.OnDeclineToLevel1Awareness += AwarenessDownToLevel1;
            _ch.DataLists.OnDeclineToLevel0Awareness += AwarenessDownToLevel0;
        }

        void OnDisable()
        {
            _ch.DataLists.OnIncreaseToLevel1Awareness -= AwarenessUpToLevel1;
            _ch.DataLists.OnIncreaseToLevel2Awareness -= AwarenessUpToLevel2;
            _ch.DataLists.OnIncreaseToLevel3Awareness -= AwarenessUpToLevel3;
            
            _ch.DataLists.OnDeclineToLevel2Awareness -= AwarenessDownToLevel2;
            _ch.DataLists.OnDeclineToLevel1Awareness -= AwarenessDownToLevel1;
            _ch.DataLists.OnDeclineToLevel0Awareness -= AwarenessDownToLevel0;
        }

        void Update()
        {
            if (!CanUpdate()) return;
            _timer = 0;
            
        }

        bool CanUpdate()
        {
            _timer += Time.deltaTime;
            if (_ch.data.awareness.ThreatInNear || _timer < _timerPeriod)
                return true;
            return false;
        }
        

        void AwarenessUpToLevel1(DetectableTarget target)
        {
            _actionController.AddActionToSequence<ActionLookAt>(target);

            //PLAYER
            if (_inVisionField.Contains(target))
            {
                //Chase him(Player Character)
                //Call other enemies(Run mode)
                //OR
                //Run for help(any enemy)
                //OR
                //Run to edge of the map
            }
            else
            {
                //Look at RawPosition
            }
        }

        bool GuardIsAbsent(DetectableTarget target)
        {
            return false;
        }

        void AwarenessUpToLevel2(DetectableTarget target)
        {
            //ABSENT GUARD
            //IF captain
            //Go to guard position, look around twice
            //Search area nearby

            //PROVOKED ENEMY
            //Run and search for location where is going provoking enemy
            
            //DEAD BODY
            //Same as awareness 1
            
            //PLAYER
            if (_inVisionField.Contains(target))
            {
                //Same as awareness 1
            }
            else
            {
                //Walk to Check It(raw position)
                //Call other enemies(walk mode)
            }
        }
        
        void AwarenessUpToLevel3(DetectableTarget target)
        {
            //ABSENT GUARD
            //Same as awareness 2
            
            //PROVOKED ENEMY
            //same as awareness 2
            
            //DEAD BODY
            //Same as awareness 2
            
            if (_inVisionField.Contains(target))
            {
                //Same as awareness 2
            }
            else
            {
                //Run to Check it (raw position)
                //Call other enemies (run mode)
            }
        }
        
        void AwarenessDownToLevel2(DetectableTarget target)
        {
        }
        
        void AwarenessDownToLevel1(DetectableTarget target)
        {
        }
        
        void AwarenessDownToLevel0(DetectableTarget target)
        {
        }
        

        bool CheckAwareness(DetectableTarget target)
        {
            if (_awarenessTargets.ContainsKey(target))
            {
                if (_awarenessTargets[target].Awareness >= 2) 
                    return true;
                return false;
            }
            return false;
        }

        void CheckDeadBody()
        {
           
        }

        void CheckForMissingEnemy()
        {
            throw new NotImplementedException();
        }

        
    }
}