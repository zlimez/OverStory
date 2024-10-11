using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Abyss.EventSystem;
using Abyss.SceneSystem;
using Abyss.Utils;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private AbyssScene sceneName;
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

    public void SceneTransit(AbyssScene targetScene)
    {
        AddConvoAndTransit(targetScene);
    }

    public void SceneTransit(string targetSceneName)
    {
        AddConvoAndTransit((AbyssScene)Enum.Parse(typeof(AbyssScene), targetSceneName));
    }

    public void TransitToLastScene()
    {
        Debug.Assert(GameManager.Instance.LastScene != AbyssScene.None);
        SceneTransit(GameManager.Instance.LastScene);
    }

    private void AddConvoAndTransit(AbyssScene targetScene = AbyssScene.None)
    {
        AbyssScene nextScene = (targetScene == AbyssScene.None) ? sceneName : targetScene;

        GameManager.Instance.LastScene = Parser.getSceneFromText(SceneManager.GetActiveScene().name);

        if (finalConversation != null)
        {
            GameManager.Instance.CutsceneConversation = finalConversation;
        }

        ChoiceManager.Instance.EndChoices();
        SceneLoader.Instance.LoadWithMaster(nextScene);
    }
}