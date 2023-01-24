using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRShooterKit.Multiplayer
{
    public class RemoveDontDestroyOnLoad : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }
    }
}

