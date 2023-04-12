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
        void Start()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
            _player = GameObject.FindWithTag("Player");
        }

        void DisableControl(PlayableDirector pd)
        {
            _player.GetComponent<ActionScheduler>().CancelCurrentAction();
            _player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector pd)
        {
            _player.GetComponent<PlayerController>().enabled = true;
        }
    }
}