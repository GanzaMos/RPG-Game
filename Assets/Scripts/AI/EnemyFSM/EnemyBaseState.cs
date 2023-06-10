using System;
using RPG.Attributes;
using RPG.Combat;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;

namespace RPG.Control
{
    public abstract class EnemyBaseState
    {
        protected EnemyAIStateManager sm;
        protected Fighter Fighter;
        protected Mover Mover;
        protected Animator Animator;
        protected Transform ThisTransform;
        protected Transform Player;
        protected Character Ch;
        
        public EnemyBaseState(EnemyAIStateManager stateManager)
        {
            sm = stateManager;
        }

        public virtual void InitializeState()
        {
            Mover = sm.gameObject.GetComponent<Mover>() ?? InstError<Mover>();
            Fighter = sm.gameObject.GetComponent<Fighter>() ?? InstError<Fighter>();
            Animator = sm.gameObject.GetComponent<Animator>() ?? InstError<Animator>();
            Ch = sm.gameObject.GetComponent<Character>() ?? InstError<Character>();
            Player = GameObject.FindWithTag("Player").transform;
        }
        
        protected T InstError<T>()
        {
            string className = typeof(T).Name;
            throw new Exception($"Missing {className} component for EnemyBaseState, ID {sm.GetInstanceID()}");
        }
        
        public virtual void EnterState()
        {
        }

        public virtual void UpdateState()
        {
        }

        public virtual void LateUpdateState()
        {
        }
        
        public virtual void ExitState()
        {
        }
    }
}