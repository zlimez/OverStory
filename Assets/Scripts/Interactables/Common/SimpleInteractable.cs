using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: set activation clauses
public class SimpleInteractable : Interactable
{
    public List<Conversation> commentConvos;

    public override void Interact()
    {
        DialogueManager.Instance.StartConversation(commentConvos[Random.Range(0, commentConvos.Count)]);
    }

}