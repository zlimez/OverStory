using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Abyss.EventSystem;
using Abyss.Utils;

namespace Abyss.SceneSystem
{
    /// <summary>
    /// Manages the loading and unloading of scenes.
    /// </summary>
    public class SceneLoader : StaticInstance<SceneLoader>
    {
        [SerializeField] private Camera sceneTransitCamera;
        public AbyssScene LastScene { get; private set; }
        public bool InTransit { get; private set; } = false;
        private AsyncOperation loadingAsyncOperation;
        private UnityAction<object> currLoadWithMaster;
        private readonly HashSet<AbyssScene> loadedScenes = new();
        public AbyssScene ActiveScene { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Debug.Log(SceneManager.GetActiveScene().name);
            ActiveScene = Parser.GetSceneFromText(SceneManager.GetActiveScene().name);
            loadedScenes.Add(ActiveScene);
            if (ActiveScene != AbyssScene.Master) loadedScenes.Add(AbyssScene.Master);
        }

        public void PrepLoadWithMaster(AbyssScene newScene, bool removeMasterAftTransit = false, AbyssScene[] discardedScenes = null)
        {
            if (currLoadWithMaster != null)
            {
                Debug.LogWarning("Last Scene have not completed loading");
                return;
            }
            EventManager.InvokeEvent(SystemEventCollection.SceneTransitPrep);
            currLoadWithMaster = (object input) =>
            {
                if (ActiveScene != AbyssScene.Master) LastScene = ActiveScene;
                UnloadScenes(discardedScenes);
                StartCoroutine(LoadSceneAsync(newScene, removeMasterAftTransit));
            };

            EventManager.StartListening(UIEventCollection.BlackIn, currLoadWithMaster);
        }

        private void UnloadScenes(AbyssScene[] discardedScenes)
        {
            if (discardedScenes == null)
            {
                foreach (AbyssScene scene in loadedScenes)
                    UnloadScene(scene);
                loadedScenes.Clear();
            }
            else
            {
                foreach (AbyssScene scene in discardedScenes)
                {
                    if (scene == AbyssScene.Master)
                        Debug.LogWarning("Unloading master scene risks disabling core functionalities hence ignored");
                    else
                    {
                        UnloadScene(scene);
                        loadedScenes.Remove(scene);
                    }
                }
            }
        }

        private IEnumerator LoadSceneAsync(AbyssScene scene, bool removeMasterAftTransit, bool isAdditive = true, bool byPrep = true)
        {
            InTransit = true;
            EventManager.InvokeEvent(SystemEventCollection.SceneTransitStart);
            ActiveScene = scene;

            if (byPrep)
            {
                EventManager.StopListening(UIEventCollection.BlackIn, currLoadWithMaster);
                currLoadWithMaster = null;
            }

            if (isAdditive)
            {
                sceneTransitCamera.enabled = true;
                loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                loadedScenes.Add(scene);
            }
            else loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

            while (!loadingAsyncOperation.isDone)
                yield return null;

            InTransit = false;
            sceneTransitCamera.enabled = false;

            ActiveScene = scene;
            EventManager.InvokeEvent(SystemEventCollection.SceneTransitDone);
            EventManager.InvokeQueueEvents();
            if (removeMasterAftTransit) UnloadScene(AbyssScene.Master);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        }

        public void LoadWithMaster(AbyssScene newScene, AbyssScene[] discardedScenes = null)
        {
            if (ActiveScene != AbyssScene.Master) LastScene = ActiveScene;
            UnloadScenes(discardedScenes);
            StartCoroutine(LoadSceneAsync(newScene, false, true, false));
        }

        public float GetLoadingProgress()
        {
            if (loadingAsyncOperation != null)
                return loadingAsyncOperation.progress;
            else return 1f;
        }

        private void UnloadScene(AbyssScene scene)
        {
            if (loadedScenes.Contains(scene))
                SceneManager.UnloadSceneAsync(scene.ToString());
        }
    }
}