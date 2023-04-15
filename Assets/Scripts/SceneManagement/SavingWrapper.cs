using System;
using System.Collections;
using RPG.Saving;
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
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
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
        
        public void Delete()
        {
                GetComponent<SavingSystem>().Delete(DefaultSave);
                print("Deleted!");
        }
    }
}