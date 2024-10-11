using UnityEngine;
using UnityEditor;

namespace Abyss.SceneSystem
{
    public class SceneLoaderTestSuite : MonoBehaviour
    {
        public AbyssScene selectedScene;

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