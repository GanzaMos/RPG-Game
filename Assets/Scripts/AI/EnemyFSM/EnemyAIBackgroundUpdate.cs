using System;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RPG.Control
{
    public class EnemyAIBackgroundUpdate
    {
        Character _ch;
        EnemyAIStateManager _sm;
        Transform _characterTransform;
        Transform _playerTransform;

        public EnemyAIBackgroundUpdate(EnemyAIStateManager sm)
        {
            _sm = sm;
            _ch = sm.ch;
            _characterTransform = _ch.transform;
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform; 
        }

        public void Update()
        {
            if (_sm.TimeSinceBeenProvoked >= _sm.MaxProvokedTime)
            {
                _ch.data.ai.IsProvoked = false;
            }

            _sm.DistanceToPlayer = Vector3.Distance(_characterTransform.position, _playerTransform.position);
            _sm.TimeSinceBeenProvoked += Time.deltaTime;
            _sm.TimeSinceLastSawPlayer += Time.deltaTime;
        }
    }
}