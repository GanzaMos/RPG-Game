using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AI.Sensors;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace AI.Action_System
{
    public class ActionController : MonoBehaviour
    {
        public Character ch;
        public EnemyAIStateManager stateManager;
        
        public List<BaseAction> actionSequence = new List<BaseAction>();

        Dictionary<Type, Queue<BaseAction>> _actionQueues;

        //Locals
        BaseAction _tempAction;
        
        void Awake()
        {
            ch = GetComponent<Character>();
            stateManager = ch.GetComponentInChildren<EnemyAIStateManager>();

            SetDictionaryWithPools();
        }

        
        //We create a Dictionary
        //Every key = derived from BaseAction class
        //Every Value = Queue of Actions for this class
        //We don't create and destroy Actions, we reuse them from Queues
        void SetDictionaryWithPools()
        {
            _actionQueues = new Dictionary<Type, Queue<BaseAction>>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type baseType = typeof(BaseAction);
            IEnumerable<Type> derivedTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
            
            foreach (Type derivedType in derivedTypes)
                _actionQueues.Add(derivedType, new Queue<BaseAction>());
        }
        
        
        public void AddActionToSequence<T>(DetectableTarget reasonForAction) where T : BaseAction
        {
            var actionType = typeof(T);

            //Any spare instance of Action in Queue? No? Create one
            if (_actionQueues[actionType].Count == 0)
                AddNewActionInstanceToQueue<T>(reasonForAction);
            
            //Add it to Action Sequence
            //Remove from the Queue
            _tempAction = _actionQueues[actionType].Dequeue();
            _tempAction.ReasonForAction = reasonForAction;
            actionSequence.Add(_tempAction);
        }

        
        void AddNewActionInstanceToQueue<T>(DetectableTarget reasonForAction) where T : BaseAction
        {
            T action = (T)Activator.CreateInstance(typeof(T), this);
            action.ReasonForAction = reasonForAction;
            _actionQueues[typeof(T)].Enqueue(action);
        }
        
        
        public void RemoveActionFromSequence(BaseAction action)
        {
            //Remove this Action from Sequence
            actionSequence.Remove(action);
            
            //Clear it, so we will be able to reuse it when we'll need it
            action.ExitAction();
            
            //Add this empty Action to it's Queue
            _actionQueues[typeof(BaseAction)].Enqueue(action);
        }
        
        
        void UpdateAction<T>(T updatedAction, DetectableTarget reasonForAction) where T : BaseAction
        {
        }

        void ReorderActions()
        {
            
        }
        
        void Update()
        {
            if (actionSequence.Count == 0) return;
           
            UpdateActiveActionTimer();
            UpdateStartTimers();
        }

        
        //update timers in all Actions in the pool
        void UpdateStartTimers()
        {
            foreach (var action in actionSequence)
            {
                action.UpdateStartTimer(Time.deltaTime);
            }
        }
        
        
        //update timer only in currently active Action
        void UpdateActiveActionTimer()
        {
            BaseAction minPriorityAction = actionSequence[0];

            if (actionSequence.Count > 1)
            {
                for (int i = 1; i < actionSequence.Count; i++)
                    if (actionSequence[i].ActionPriority < minPriorityAction.ActionPriority)
                        minPriorityAction = actionSequence[i];
            }

            minPriorityAction.UpdateActiveTimer(Time.deltaTime);
        }
    }
}