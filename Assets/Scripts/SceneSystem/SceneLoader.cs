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
        [SerializeField] GameObject transitCam;
        public Settings.Scene LastScene { get; private set; } = Settings.Scene.None;
        public bool InTransit { get; private set; } = false;
        AsyncOperation loadingAsyncOperation;
        Action<object> currLoadWithMaster;
        readonly HashSet<Settings.Scene> loadedScenes = new();
        public Settings.Scene ActiveScene { get; private set; }

        public bool HasScene(Settings.Scene scene) => loadedScenes.Contains(scene);

        protected override void Awake()
        {
            base.Awake();

            Debug.Log(SceneManager.GetActiveScene().name);
            ActiveScene = Parser.GetSceneFromText(SceneManager.GetActiveScene().name);
            loadedScenes.Add(ActiveScene);
            if (ActiveScene != Settings.Scene.Master) loadedScenes.Add(Settings.Scene.Master);
        }

        public bool PrepLoadWithMaster(Settings.Scene newScene, bool rmMasterAftTransit = false, Settings.Scene[] discardedScenes = null)
        {
            if (currLoadWithMaster != null)
            {
                Debug.LogWarning("Last Scene have not completed loading");
                return false;
            }
            EventManager.InvokeEvent(SystemEvents.SceneTransitPrep);
            currLoadWithMaster = (object input) =>
            {
                if (ActiveScene != Settings.Scene.Master) LastScene = ActiveScene;
                UnloadScenes(discardedScenes);
                StartCoroutine(LoadSceneAsync(newScene, rmMasterAftTransit));
            };

            EventManager.Subscribe(UIEvents.BlackIn, currLoadWithMaster);
            return true;
        }

        private void UnloadScenes(Settings.Scene[] discardedScenes)
        {
            if (discardedScenes == null)
                UnloadScene(LastScene);
            else
            {
                foreach (Settings.Scene scene in discardedScenes)
                {
                    if (scene == Settings.Scene.Master)
                        Debug.LogWarning("Unloading master scene risks disabling core functionalities hence ignored");
                    else UnloadScene(scene);
                }
            }
        }

        private IEnumerator LoadSceneAsync(Settings.Scene scene, bool rmMasterAftTransit, bool isAdditive = true, bool byPrep = true)
        {
            InTransit = true;
            EventManager.InvokeEvent(SystemEvents.SceneTransitStart, scene);
            ActiveScene = scene;

            if (byPrep)
            {
                EventManager.Unsubscribe(UIEvents.BlackIn, currLoadWithMaster);
                currLoadWithMaster = null;
            }

            if (isAdditive)
            {
                transitCam.SetActive(true);
                loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                loadedScenes.Add(scene);
            }
            else loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

            while (!loadingAsyncOperation.isDone)
                yield return null;

            InTransit = false;
            transitCam.SetActive(false);

            ActiveScene = scene;
            EventManager.InvokeEvent(SystemEvents.SceneTransitDone);
            EventManager.InvokeQueueEvents();
            if (rmMasterAftTransit) UnloadScene(Settings.Scene.Master);
            EventManager.InvokeEvent(SystemEvents.ChangeCameraBG);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        }

        public bool LoadWithMaster(Settings.Scene newScene, Settings.Scene[] discardedScenes = null)
        {
            if (ActiveScene != Settings.Scene.Master) LastScene = ActiveScene;
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

        private void UnloadScene(Settings.Scene scene)
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