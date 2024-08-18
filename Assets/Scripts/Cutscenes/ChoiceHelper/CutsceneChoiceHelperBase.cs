using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneChoiceHelperBase : MonoBehaviour
{
    public PlayableDirector timeline;
    public string choiceText1, choiceText2, choiceText3;
    private Choice choice1, choice2, choice3;
    private bool choice1Activated, choice2Activated, choice3Activated = true;
    private bool finished = false;

    void Awake()
    {
        InitialiseChoice();
    }

    private void InitialiseChoice()
    {
        choice1 = new Choice(choiceText1, Choice1, choice1Activated);
        choice2 = new Choice(choiceText2, Choice2, choice2Activated);
        choice3 = new Choice(choiceText3, Choice3, choice3Activated);
    }

    public void BeginChoices()
    {
        timeline.playableGraph.GetRootPlayable(0).SetSpeed(0);
        ChoiceManager.Instance.StartChoice(choice1, choice2, choice3);
        StartCoroutine(WaitUntilChoiceStart());
        finished = true;
    }

    // Invoked Events
    // Change according to what is required
    public void Choice1(object o = null)
    {
    }

    public void Choice2(object o = null)
    {
        //choice3Activated = true;
    }

    public void Choice3(object o = null)
    {
        // DialogueManager.Instance.StartConversation(convo4);
    }

    private void Update()
    {
        if (finished && !ChoiceManager.Instance.InChoice && !DialogueManager.Instance.InDialogue)
        {
            timeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
            finished = false;
        }
    }

    IEnumerator WaitUntilChoiceStart()
    {
        while (!ChoiceManager.Instance.InChoice)
        {
            yield return null;
        }
    }
}
