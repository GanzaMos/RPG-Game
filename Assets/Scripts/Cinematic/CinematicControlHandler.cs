using System;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

namespace RPG.Cinematic
{
    public class CinematicControlHandler : MonoBehaviour
    {
        GameObject _player;
        PlayerController _playerController;
        ActionScheduler _actionScheduler;
        PlayableDirector _playableDirector;

        void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _playerController = _player.GetComponent<PlayerController>() ?? InstError<PlayerController>();
            _actionScheduler = _player.GetComponent<ActionScheduler>() ?? InstError<ActionScheduler>();
            _playableDirector = GetComponent<PlayableDirector>() ?? InstError<PlayableDirector>();
        }

        T InstError<T>()
        {
            string className = typeof(T).Name;
            throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
        }
        
        void Start()
        {
            _playableDirector.played += DisableControl;
            _playableDirector.stopped += EnableControl;
        }

        void DisableControl(PlayableDirector pd)
        {
            _actionScheduler.CancelCurrentAction();
            _playerController.enabled = false;
        }

        void EnableControl(PlayableDirector pd)
        {
            _playerController.enabled = true;
        }
    }
}