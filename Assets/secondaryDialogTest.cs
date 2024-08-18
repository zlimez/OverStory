using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class secondaryDialogTest : MonoBehaviour
{
    public SecondaryConversation convo;
    // Start is called before the first frame update
    void Start()
    {
        SecondaryDialogueManager.Instance.StartAutomaticConversation(convo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
