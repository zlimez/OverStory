using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public bool InTransition { get; private set; }
        private Action onLoaderCallback;
        private AsyncOperation loadingAsyncOperation;
        private UnityAction<object> currLoaderWithMaster;

        private HashSet<AbyssScene> loadedScenes = new HashSet<AbyssScene>();
        public AbyssScene ActiveScene { get; private set; }

        /// <summary>
        /// Initializes the SceneLoader.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Debug.Log(SceneManager.GetActiveScene().name);
            ActiveScene = Parser.getSceneFromText(SceneManager.GetActiveScene().name);
            loadedScenes.Add(ActiveScene);
            loadedScenes.Add(AbyssScene.Master);
            InTransition = false;
        }

        /// <summary>
        /// Adds a new entry scene.
        /// </summary>
        public void AddEntryScene(AbyssScene sceneName)
        {
            loadedScenes.Add(sceneName);
        }
        /// <summary>
        /// Records the player's position in the current scene.
        /// </summary>
        private void RecordPlayerPosition()
        {
            //GameObject player = GameObject.FindWithTag("Player");
            //if (player != null)
            //{
            //GameManager.Instance.LastPosition = player.transform.position;
            //GameManager.Instance.LastPositionFlipX = player.GetComponent<PlayerCharacterController>().PlayerSprite.GetComponent<SpriteRenderer>().flipX;
            //}
        }

        /// <summary>
        /// Records the last active scene.
        /// </summary>
        private void RecordLastScene()
        {
            AbyssScene lastScene = ActiveScene;
            if (lastScene != AbyssScene.Master)
            {
                GameManager.Instance.LastScene = lastScene;
            }
        }

        /// <summary>
        /// Prepares the loading of a new scene with Master.
        /// </summary>
        public void PrepLoadWithMaster(AbyssScene newScene, bool removeMasterAftTransit = false, AbyssScene[] discardedScenes = null)
        {
            InTransition = true;
            GameManager.Instance.PauseGame();
            EventManager.InvokeEvent(SystemEventCollection.TransitionWithMaster);

            if (currLoaderWithMaster != null)
            {
                Debug.LogWarning("Last Scene have not completed loading");
                return;
            }
            currLoaderWithMaster = (object input) =>
            {
                RecordPlayerPosition();
                RecordLastScene();

                UnloadScenes(discardedScenes);

                StartCoroutine(LoadSceneAsync(newScene, removeMasterAftTransit));
            };

            EventManager.StartListening(UIEventCollection.CurtainDrawn, currLoaderWithMaster);
        }

        /// <summary>
        /// Prepares the loading of a new scene with Master.
        /// </summary>
        public void PrepLoadWithMaster(string newScene, bool removeMasterAftTransit = false, string[] discardedScenes = null)
        {
            AbyssScene parsedScene = Parser.getSceneFromText(newScene);

            PrepLoadWithMaster(
                parsedScene,
                removeMasterAftTransit,
                discardedScenes?.Select(s => Parser.getSceneFromText(s)).ToArray()
            );
        }

        /// <summary>
        /// Reloads the current scene with Master.
        /// </summary>
        public void ReloadCurrentSceneWithMaster()
        {
            PrepLoadWithMaster(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Unloads the specified scenes.
        /// </summary>
        private void UnloadScenes(AbyssScene[] discardedScenes)
        {
            if (discardedScenes == null)
            {
                foreach (AbyssScene scene in loadedScenes)
                {
                    UnloadScene(scene);
                }
                loadedScenes.Clear();
            }
            else
            {
                foreach (AbyssScene scene in discardedScenes)
                {
                    if (scene == AbyssScene.Master)
                    {
                        Debug.LogWarning("Unloading master scene risks disabling core functionalities hence ignored");
                    }
                    else
                    {
                        UnloadScene(scene);
                        loadedScenes.Remove(scene);
                    }
                }
            }
        }

        /// <summary>
        /// Loads a new scene asynchronously with the Master scene.
        /// </summary>
        private IEnumerator LoadSceneAsync(AbyssScene scene, bool removeMasterAftTransit, bool isAdditive = true, bool isQueued = true)
        {
            ActiveScene = scene;

            if (isQueued)
            {
                EventManager.StopListening(UIEventCollection.CurtainDrawn, currLoaderWithMaster);
                currLoaderWithMaster = null;
            }

            if (isAdditive)
            {
                sceneTransitCamera.enabled = true;
                loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
                loadedScenes.Add(scene);
            }
            else
            {
                loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
                EventManager.InvokeEvent(SystemEventCollection.Transition);
            }

            while (!loadingAsyncOperation.isDone)
            {
                yield return null;
            }
            InTransition = false;
            sceneTransitCamera.enabled = false;

            if (isQueued)
            {
                GameManager.Instance.ResumeGame();
                yield return new WaitForSecondsRealtime(0.5f);
            }

            ActiveScene = scene;
            EventManager.InvokeEvent(SystemEventCollection.TransitionWithMasterCompleted);
            EventManager.InvokeQueueEvents();
            if (removeMasterAftTransit) UnloadMaster();

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        }

        /// <summary>
        /// Loads a new scene with the Master scene.
        /// </summary>
        public void LoadWithMaster(AbyssScene newScene, AbyssScene[] discardedScenes = null)
        {
            RecordPlayerPosition();
            RecordLastScene();

            UnloadScenes(discardedScenes);

            StartCoroutine(LoadSceneAsync(newScene, false, true, false));
        }

        /// <summary>
        /// Returns the loading progress as a float between 0 and 1.
        /// </summary>
        public float GetLoadingProgress()
        {
            if (loadingAsyncOperation != null)
            {
                return loadingAsyncOperation.progress;
            }
            else
            {
                return 1f;
            }
        }

        /// <summary>
        /// Invokes the loader callback.
        /// </summary>
        public void LoaderCallback()
        {
            if (onLoaderCallback != null)
            {
                onLoaderCallback();
                onLoaderCallback = null;
            }
        }

        /// <summary>
        /// Unloads the Master scene.
        /// </summary>
        private void UnloadMaster()
        {
            UnloadScene(AbyssScene.Master);
        }

        /// <summary>
        /// Unloads the specified scene.
        /// </summary>
        private void UnloadScene(AbyssScene scene)
        {
            if (loadedScenes.Contains(scene))
            {
                Debug.Log("Unloading " + scene);
                SceneManager.UnloadSceneAsync(scene.ToString());
            }
        }
    }
}