using Abyss.SceneSystem;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    public AbyssScene nextScene;
    void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: Change to PrepLoadWithMaster
        if (other.CompareTag("Player"))
            SceneLoader.Instance.PrepLoadWithMaster(nextScene);
    }
}
