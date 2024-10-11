using Abyss.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Abyss.SceneSystem
{
    public class DropdownScenes : MonoBehaviour
    {
        private TMP_Dropdown dropdown;

        private void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            dropdown.ClearOptions();
            string output = "";
            if (SceneManager.sceneCountInBuildSettings > 0)
            {
                for (int n = 0; n < SceneManager.sceneCountInBuildSettings; ++n)
                {
                    Scene scene = SceneManager.GetSceneByBuildIndex(n);
                    // Debug.Log(scene.name);
                    string pathToScene = SceneUtility.GetScenePathByBuildIndex(n);
                    string sceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);
                    output += sceneName;
                    output += scene.isLoaded ? " (Loaded, " : " (Not Loaded, ";
                    output += scene.isDirty ? "Dirty, " : "Clean, ";
                    output += scene.buildIndex >= 0 ? " in build)\n" : " NOT in build)\n";

                    var option = new TMP_Dropdown.OptionData(sceneName);
                    dropdown.options.Add(option);
                }
                dropdown.RefreshShownValue();
            }
            else
            {
                output = "No open Scenes.";
            }
            Debug.Log(output);
        }

        public void PlayGame()
        {
            SceneLoader.Instance.PrepLoadWithMaster(Parser.GetSceneFromText(dropdown.options[dropdown.value].text));
        }
    }
}