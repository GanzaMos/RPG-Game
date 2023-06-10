using System;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Control
{
    public class EnemyAIStateManager : MonoBehaviour
    {
        #region SerializeFields
        [Header("Chasing settings")]
        [SerializeField] float visionRadius = 5f;
        public float VisionRadius => visionRadius;
        
        [SerializeField] float maxSuspicionTime = 3f;
        public float MaxSuspicionTime => maxSuspicionTime;
        
        
        [Space(5)]
        [Header("Provoke settings")]
        [SerializeField] float maxProvokedTime = 5f;
        public float MaxProvokedTime => maxProvokedTime;
        
        [SerializeField] float neighboursProvokingRadius = 10f;
        public float NeighboursProvokingRadius => neighboursProvokingRadius;
        
        
        [Space(5)]
        [Header("Patrol settings")]
        [SerializeField] PatrolPath patrolPath;
        public PatrolPath PatrolPath => patrolPath;
        
        [SerializeField] float waypointTolerance = 1f;
        public float WaypointTolerance => waypointTolerance;
        
        [SerializeField] float maxDwellingTimeAtWaypoint = 3f;
        public float MaxDwellingTimeAtWaypoint => maxDwellingTimeAtWaypoint;
        
        
        [Space(5)]
        [Header("Search settings")]
        [SerializeField] float searchRadius = 5f;
        public float SearchRadius
        {
            get => searchRadius;
            set => searchRadius = value;
        }

        [SerializeField] int pointsToSearch = 3;
        public int PointsToSearch
        {
            get => pointsToSearch;
            set => pointsToSearch = value;
        }

        [SerializeField] float timeToSearch = 3;
        public float TimeToSearch
        {
            get => timeToSearch;
            set => timeToSearch = value;
        }


        [Space(5)]
        [Header("General settings")]
        [Range(0,2)] [SerializeField] float calmSpeedFraction = 0.5f;
        public float CalmSpeedFraction => calmSpeedFraction;
        
        #endregion //SerializeFields
        
        
        #region Constructor, Local variables, Cashed
        
        //Cashed
        Transform _playerTransform;
        public Character ch;
        EnemyBaseState _currentState;
        
        //Local variables
        float _distanceToPlayer;
        public float DistanceToPlayer
        {
            get => _distanceToPlayer;
            set => _distanceToPlayer = value;
        }

        Vector3 _guardStartPosition;
        public Vector3 GuardStartPosition => _guardStartPosition;
        
        float _timeSinceLastSawPlayer = Mathf.Infinity;
        public float TimeSinceLastSawPlayer
        {
            get => _timeSinceLastSawPlayer1;
            set => _timeSinceLastSawPlayer1 = value;
        }

        float _timeSinceBeenProvoked = Mathf.Infinity;
        public float TimeSinceBeenProvoked
        {
            get => _timeSinceBeenProvoked1;
            set => _timeSinceBeenProvoked1 = value;
        }

        int _currentWaypointIndex = 0;
        public int CurrentWaypointIndex
        {
            get => _currentWaypointIndex1;
            set => _currentWaypointIndex1 = value;
        }

        //states initializing
        public EnemyStateAttacking EnemyStateAttacking;
        public EnemyStateChasing EnemyStateChasing;
        public EnemyStateIdle EnemyStateIdle;
        public EnemyStatePatrolling EnemyStatePatrolling;
        public EnemyStateSearching EnemyStateSearching;
        
        //locals
        EnemyAIBackgroundUpdate _backgroundUpdate;
        int _pointsToSearch;
        float _timeToSearch;
        float _timeSinceLastSawPlayer1;
        float _timeSinceBeenProvoked1;
        int _currentWaypointIndex1;

        public EnemyAIStateManager()
        {
            EnemyStateAttacking = new EnemyStateAttacking(this);
            EnemyStateChasing = new EnemyStateChasing(this);
            EnemyStateIdle = new EnemyStateIdle(this);
            EnemyStatePatrolling = new EnemyStatePatrolling(this);
            EnemyStateSearching = new EnemyStateSearching(this);
        }
        
        #endregion //Constructor, Local variables, Cashed
        
        
        void Awake()
        {
            ch = GetComponentInParent<Character>() ?? InstError<Character>();
            _guardStartPosition = transform.position;
            EnemyStateAttacking.InitializeState();
            EnemyStateChasing.InitializeState();
            EnemyStateIdle.InitializeState();
            EnemyStatePatrolling.InitializeState();
            EnemyStateSearching.InitializeState();

            _backgroundUpdate = new EnemyAIBackgroundUpdate(this);
        }
        
        T InstError<T>()
        {
            string className = typeof(T).Name;
            throw new Exception($"Missing {className} component for {name} in {ch?.gameObject.name}, ID {GetInstanceID()}");
        }

        void Start()
        {
            _currentState = GetStartState();
            _currentState.EnterState();
        }
        
        void Update()
        {
            _backgroundUpdate.Update();
            if (ch.data.health.IsDead) return;
            _currentState.UpdateState();
        }

        void LateUpdate()
        {
            if (ch.data.health.IsDead) return;
            _currentState.LateUpdateState();
        }

        public void SwitchState(EnemyBaseState state)
        {
            if (state == _currentState) return;
            _currentState.ExitState();
            _currentState = state;
            _currentState.EnterState();
        }

        
        //Supporting methods
        
        public EnemyBaseState GetStartState()
        {
            if (PatrolPath)
                return EnemyStatePatrolling;
            else
                return EnemyStateIdle;
        }

        
        
        public void ProvokeEnemy(bool provokeEnemy, bool provokeEnemyNeighbours = true)
        {
            ch.data.ai.IsProvoked = provokeEnemy;
            _timeSinceBeenProvoked = 0;
            if (provokeEnemyNeighbours)
            {
                ProvokeNearbyEnemies();
            }
        }
        
        void ProvokeNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, neighboursProvokingRadius, transform.forward, 0f);
            
            foreach (var hit in hits)
            {
                var enemy = hit.transform.GetComponent<EnemyAIStateManager>();
                if (!enemy) continue;
                enemy.ProvokeEnemy(true, false);
            }
        }
    }
}