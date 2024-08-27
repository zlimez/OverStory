using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitSceneOnEnable : MonoBehaviour
{
    public string ExitScene;
    void OnEnable()
    {
        SceneManager.LoadScene(ExitScene, LoadSceneMode.Single);
    }
}
