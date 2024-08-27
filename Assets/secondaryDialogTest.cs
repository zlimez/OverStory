using UnityEngine;

public class secondaryDialogTest : MonoBehaviour
{
    public SecondaryConversation convo;
    // Start is called before the first frame update
    void Start()
    {
        SecondaryDialogueManager.Instance.StartAutomaticConversation(convo);
    }
}
