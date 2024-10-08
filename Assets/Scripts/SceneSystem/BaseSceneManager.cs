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
            // Load the master scene if it is not already loaded
            if (SceneLoader.Instance == null || SceneManager.GetActiveScene().name != AbyssScene.Master.ToString())
                SceneManager.LoadSceneAsync(AbyssScene.Master.ToString(), LoadSceneMode.Additive);
        }
    }
}