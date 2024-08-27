using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abyss.EventSystem;

// Hide GameControl Hint button when in cutscene
public class CutsceneGameControl : MonoBehaviour
{
    void OnEnable() 
    {
        if (GameControlUi.Instance != null)
        {
            GameControlUi.Instance.HideButton();
            return;
        }
        StartCoroutine(WaitForInstanceInitialisation());
    }

    void OnDisable() 
    {
        GameControlUi.Instance.ShowButton();
    }

    IEnumerator WaitForInstanceInitialisation() 
    {
        while (GameControlUi.Instance == null)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        GameControlUi.Instance.HideButton();
    }
}
