using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Abyss.SceneSystem
{
    [CustomEditor(typeof(SceneLoaderTestSuite))]
    public class SceneLoaderTestSuiteEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SceneLoaderTestSuite testSuite = (SceneLoaderTestSuite)target;

            if (GUILayout.Button("Load Selected Scene"))
            {
                testSuite.LoadSelectedScene();
            }

            if (GUILayout.Button("Reload Current Scene"))
            {
                testSuite.ReloadCurrentScene();
            }
        }
    }
}
#endif