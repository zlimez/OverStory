using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronellium.SceneSystem;

public class SceneTransit : MonoBehaviour
{
    public ChronelliumScene nextScene;
    private void OnTriggerEnter(Collider collision)
    {
        if (IsPlayer(collision.gameObject))
        {
            SceneLoader.Instance.PrepLoadWithMaster(nextScene);
        }

    }

    private bool IsPlayer(GameObject otherObject)
    {
        return otherObject.CompareTag("Player");
    }
}
