using System.Collections;
using UnityEngine;

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
