using System;
using System.Collections;
using RPG.Saving;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        
        const string DefaultSave = "save";

        IEnumerator Start()
        {
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediately();
            yield return GetComponent<SavingSystem>().LoadLastScene(DefaultSave);
            yield return fader.FadeIn();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                Load();
            }
        }

        public void Save()
        {
                GetComponent<SavingSystem>().Save(DefaultSave);
                print("Saved!");
        }

        public void Load()
        {
                GetComponent<SavingSystem>().Load(DefaultSave);
                print("Loaded!");
        }
    }
}