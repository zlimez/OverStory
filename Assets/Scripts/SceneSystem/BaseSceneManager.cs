using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chronellium.SceneSystem
{
    public class BaseSceneManager : MonoBehaviour
    {
        protected virtual void Awake()
        {
            // Load the base scene if base scene is not already loaded in the background
            if (SceneLoader.Instance == null || SceneManager.GetActiveScene().name != ChronelliumScene.Master.ToString())
            {
                SceneManager.LoadSceneAsync(ChronelliumScene.Master.ToString(), LoadSceneMode.Additive);
            }
        }
    }
}