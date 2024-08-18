using UnityEngine;
using Chronellium.EventSystem;
using UnityEngine.SceneManagement;
using DeepBreath.Environment;
using Pathfinding;
using DigitalRuby.Tween;

public class Puzzle1 : MonoBehaviour
{
    private void Start()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPuzzle();
        }
        EventManager.StartListening(StaticEvent.Core_ResetPuzzle, ResetPuzzle);
    }

    private void ResetPuzzle(object input = null)
    {

        StateManager.realm = Realm.realWorld;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
