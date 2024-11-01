using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public abstract class CutsceneChoiceHelperBase : MonoBehaviour
{
    public PlayableDirector timeline;
    public string choiceText1, choiceText2, choiceText3;
    private Choice choice1, choice2, choice3;
    private bool finished = false;

    void Awake()
    {
        InitialiseChoice();
    }

    private void InitialiseChoice()
    {
        choice1 = new Choice(choiceText1);
        choice2 = new Choice(choiceText2);
        choice3 = new Choice(choiceText3);
    }

    public void BeginChoices()
    {
        timeline.playableGraph.GetRootPlayable(0).SetSpeed(0);
        ChoiceManager.Instance.StartChoice(choice1, choice2, choice3);
        StartCoroutine(WaitUntilChoiceStart());
        finished = true;
    }

    public abstract void SelectOne(object o = null);
    public abstract void SelectTwo(object o = null);
    public abstract void SelectThree(object o = null);

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
            yield return null;
    }
}
