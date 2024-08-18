using UnityEngine;
using UnityEditor;

namespace Chronellium.SceneSystem
{
    public class SceneLoaderTestSuite : MonoBehaviour
    {
        public ChronelliumScene selectedScene;

        public void LoadSelectedScene()
        {
            SceneLoader.Instance.PrepLoadWithMaster(selectedScene.ToString());
        }

        public void ReloadCurrentScene()
        {
            SceneLoader.Instance.ReloadCurrentSceneWithMaster();
        }
    }
}