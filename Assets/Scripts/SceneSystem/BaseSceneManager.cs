using Abyss.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Abyss.SceneSystem
{
    public class BaseSceneManager : MonoBehaviour
    {
        void Awake()
        {
            // Load the master scene if it is not already loaded
            if (SceneLoader.Instance == null && Parser.GetSceneFromText(SceneManager.GetActiveScene().name) != Settings.Scene.Master)
                SceneManager.LoadSceneAsync(Settings.Scene.Master.ToString(), LoadSceneMode.Additive);
        }
    }
}