﻿using System;
using RPG.Saving;
using System.Collections;
using RPG.Control;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] PortalIdentifier portalIdentifier;

        enum PortalIdentifier
        {
            A, B, C, D, E
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(SceneTransition());
            }
        }

        IEnumerator SceneTransition()
        {
            DontDestroyOnLoad(this);

            Fader fader = FindObjectOfType<Fader>();
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
            yield return fader.FadeOut();

            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            if (savingWrapper != null) print("found savingWrapper");
            
            savingWrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
            
            savingWrapper.Load();

            Transform anotherPortalSpawnPoint = GetAnotherPortalSpawnPoint();
            UpdatePlayerLocation(anotherPortalSpawnPoint);
            
            savingWrapper.Save();
            
            yield return new WaitForSeconds(0.5f); 
            fader.FadeIn();
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
            
            Destroy(gameObject);
        }
        
        Transform GetAnotherPortalSpawnPoint()
        {
            Portal[] anotherPortals = FindObjectsOfType<Portal>();
            
            foreach (var anotherPortal in anotherPortals)
            {
                if (anotherPortal == this) continue;
                if (anotherPortal.portalIdentifier == portalIdentifier)
                {
                    return anotherPortal.spawnPoint;
                }
            }
            
            print("Can't find another Portal");
            return null;
        }
        
        void UpdatePlayerLocation(Transform anotherPortalSpawnPoint)
        {
            Transform player = GameObject.FindWithTag("Player").transform;
            player.GetComponent<NavMeshAgent>().Warp(anotherPortalSpawnPoint.position);
            player.rotation = anotherPortalSpawnPoint.rotation;
        }
    }
}