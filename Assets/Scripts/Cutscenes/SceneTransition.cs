using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Chronellium.EventSystem;
using Chronellium.SceneSystem;
using Chronellium.Utils;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private ChronelliumScene sceneName;
    [SerializeField] private Conversation finalConversation;

    private GameEvent sceneStart = new GameEvent("Scene start");

    private void Start()
    {
        EventManager.InvokeEvent(sceneStart);
    }

    public void TransitToNextScene()
    {
        AddConvoAndTransit();
    }

    public void SceneTransit(ChronelliumScene targetScene)
    {
        AddConvoAndTransit(targetScene);
    }

    public void SceneTransit(string targetSceneName)
    {
        AddConvoAndTransit((ChronelliumScene)Enum.Parse(typeof(ChronelliumScene), targetSceneName));
    }

    public void TransitToLastScene()
    {
        Debug.Assert(GameManager.Instance.LastScene != ChronelliumScene.None);
        SceneTransit(GameManager.Instance.LastScene);
    }

    private void AddConvoAndTransit(ChronelliumScene targetScene = ChronelliumScene.None)
    {
        ChronelliumScene nextScene = (targetScene == ChronelliumScene.None) ? sceneName : targetScene;

        GameManager.Instance.LastScene = Parser.getSceneFromText(SceneManager.GetActiveScene().name);

        if (finalConversation != null)
        {
            GameManager.Instance.CutsceneConversation = finalConversation;
        }

        ChoiceManager.Instance.EndChoices();
        SceneLoader.Instance.LoadWithMaster(nextScene);
    }
}