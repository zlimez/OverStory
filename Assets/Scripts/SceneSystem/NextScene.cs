using Abyss.SceneSystem;
using UnityEngine;
using Abyss.Settings;

public class NextScene : MonoBehaviour
{
    public Scene nextScene;
    [SerializeField] bool loadWithMaster = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: Change to PrepLoadWithMaster
        if (other.CompareTag(Tag.Player))
            SceneLoader.Instance.PrepLoadWithMaster(nextScene, !loadWithMaster);
    }
}
