using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Abyss.SceneSystem
{
    public class BaseSceneManager : MonoBehaviour
    {
        protected virtual void Awake()
        {
            // Load the base scene if base scene is not already loaded in the background
            if (SceneLoader.Instance == null || SceneManager.GetActiveScene().name != AbyssScene.Master.ToString())
            {
                SceneManager.LoadSceneAsync(AbyssScene.Master.ToString(), LoadSceneMode.Additive);
            }
        }
    }
}