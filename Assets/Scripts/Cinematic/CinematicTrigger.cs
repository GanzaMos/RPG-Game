using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace RPG.Cinematic
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool _alreadyTriggered = false;
        void OnTriggerEnter(Collider other)
        {
            if(!_alreadyTriggered && other.gameObject.tag == "Player")
            {
                _alreadyTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }
    }
}

