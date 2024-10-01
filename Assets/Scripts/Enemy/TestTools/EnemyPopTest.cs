using Abyss.Utils;
using UnityEngine.UI;

namespace Abyss.Environment.Enemy.Test
{
    public class EnemyPopTest : PersistentSingleton<EnemyPopTest>
    {
        public Button nextGenButton;
        public Button nextSceneButton;

        void Start()
        {
            nextGenButton.onClick.AddListener(EnemyPopManager.Instance.NextGeneration);
            nextSceneButton.onClick.AddListener(NextScene);
        }

        public void NextScene()
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (activeScene == "GATest")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GATestNext");
            }
            else if (activeScene == "GATestNext")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GATest");
            }
            else UnityEngine.Debug.LogWarning("Script should only used for GATest scenes");

        }
    }
}
