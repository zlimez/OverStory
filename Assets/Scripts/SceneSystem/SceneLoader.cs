using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Abyss.EventSystem;
using Abyss.Utils;
using System;

namespace Abyss.SceneSystem
{
    // Must be placed under "Master" scene
    public class SceneLoader : StaticInstance<SceneLoader>
    {
        // [SerializeField] Camera sceneTransitCamera;
        public AbyssScene LastScene { get; private set; }
        public bool InTransit { get; private set; } = false;
        private AsyncOperation loadingAsyncOperation;
        private Action<object> currLoadWithMaster;
        private readonly HashSet<AbyssScene> loadedScenes = new();
        public AbyssScene ActiveScene { get; private set; }

        public bool HasScene(AbyssScene scene) => loadedScenes.Contains(scene);

        protected override void Awake()
        {
            base.Awake();

            Debug.Log(SceneManager.GetActiveScene().name);
            ActiveScene = Parser.GetSceneFromText(SceneManager.GetActiveScene().name);
            loadedScenes.Add(ActiveScene);
            if (ActiveScene != AbyssScene.Master) loadedScenes.Add(AbyssScene.Master);
        }

        public bool PrepLoadWithMaster(AbyssScene newScene, bool removeMasterAftTransit = false, AbyssScene[] discardedScenes = null)
        {
            if (currLoadWithMaster != null)
            {
                Debug.LogWarning("Last Scene have not completed loading");
                return false;
            }
            EventManager.InvokeEvent(SystemEvents.SceneTransitPrep);
            currLoadWithMaster = (object input) =>
            {
                if (ActiveScene != AbyssScene.Master) LastScene = ActiveScene;
                UnloadScenes(discardedScenes);
                StartCoroutine(LoadSceneAsync(newScene, removeMasterAftTransit));
            };

            EventManager.StartListening(UIEvents.BlackIn, currLoadWithMaster);
            return true;
        }

        private void UnloadScenes(AbyssScene[] discardedScenes)
        {
            if (discardedScenes == null)
                UnloadScene(LastScene);
            else
            {
                foreach (AbyssScene scene in discardedScenes)
                {
                    if (scene == AbyssScene.Master)
                        Debug.LogWarning("Unloading master scene risks disabling core functionalities hence ignored");
                    else UnloadScene(scene);
                }
            }
        }

        private IEnumerator LoadSceneAsync(AbyssScene scene, bool removeMasterAftTransit, bool isAdditive = true, bool byPrep = true)
        {
            InTransit = true;
            EventManager.InvokeEvent(SystemEvents.SceneTransitStart);
            ActiveScene = scene;

            if (byPrep)
            {
                EventManager.StopListening(UIEvents.BlackIn, currLoadWithMaster);
                currLoadWithMaster = null;
            }

            if (isAdditive)
            {
                // sceneTransitCamera.enabled = true;
                loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                loadedScenes.Add(scene);
            }
            else loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

            while (!loadingAsyncOperation.isDone)
                yield return null;

            InTransit = false;
            // sceneTransitCamera.enabled = false;

            ActiveScene = scene;
            EventManager.InvokeEvent(SystemEvents.SceneTransitDone);
            EventManager.InvokeQueueEvents();
            if (removeMasterAftTransit) UnloadScene(AbyssScene.Master);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        }

        public bool LoadWithMaster(AbyssScene newScene, AbyssScene[] discardedScenes = null)
        {
            if (ActiveScene != AbyssScene.Master) LastScene = ActiveScene;
            UnloadScenes(discardedScenes);
            StartCoroutine(LoadSceneAsync(newScene, false, true, false));
            return true;
        }

        public float GetLoadingProgress()
        {
            if (loadingAsyncOperation != null)
                return loadingAsyncOperation.progress;
            else return 1f;
        }

        private void UnloadScene(AbyssScene scene)
        {
            Debug.Log("Unloading " + scene);
            if (loadedScenes.Contains(scene))
            {
                SceneManager.UnloadSceneAsync(scene.ToString());
                loadedScenes.Remove(scene);
            }
        }
    }
}