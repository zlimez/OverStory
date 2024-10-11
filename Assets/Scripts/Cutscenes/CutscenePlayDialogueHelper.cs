using Abyss.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePlayDialogueHelper : MonoBehaviour
{
    // Description: This class helps with playing a dialogue after transitioning to a new scene. 
    // Use case: When a cutscene is ending and there is one last conversation to be played after transitioning to a new scene.
    // Usage: Place this script in an empty object in the new scene.

    // Start is called before the first frame update
    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("GameManager not initialized yet");
            EventManager.StartListening(SystemEventCollection.SystemsReady, StartConvoIfPresent);
        }
        else
        {
            StartConvoIfPresent();
        }
    }

    private void StartConvoIfPresent(object input = null)
    {
        Debug.Assert(GameManager.Instance != null);

        // Needed for cutscenes - to start a convo after transitioning to new scene
        if (GameManager.Instance.CutsceneConversation != null)
        {
            DialogueManager.Instance.StartConversation(GameManager.Instance.CutsceneConversation);
            GameManager.Instance.CutsceneConversation = null;
        }
        // may not look right?
        else if (GameManager.Instance.CutsceneSecondaryConversation != null)
        {
            StartCoroutine(StartSecondaryConversation(GameManager.Instance.CutsceneSecondaryConversation));
        }
    }

    IEnumerator StartSecondaryConversation(SecondaryConversation convo)
    {
        yield return new WaitForSeconds(.3f);
        SecondaryDialogueManager.Instance.StartAutomaticConversation(GameManager.Instance.CutsceneSecondaryConversation);
        GameManager.Instance.CutsceneSecondaryConversation = null;
    }
}
