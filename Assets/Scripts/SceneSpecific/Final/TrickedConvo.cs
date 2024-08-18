using UnityEngine;

public class TrickedConvo : MonoBehaviour
{
    private bool hasTriggered;
    public Conversation convo;
    public Conversation clownConvo;
    public Conversation clownConvo2;
    public Animation clownWalkAway;
    public GameObject clown;
    public GameObject playerStopper;
    public GameObject family;

    // Tag based so each entity can represent a group
    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        DialogueManager.Instance.StartConversation(convo, onFinish);
        hasTriggered = true;
    }

    void onFinish()
    {
        Debug.Log("warningConvoFinish");
        clown.SetActive(true);
        Invoke("SwitchClownDir", .8f);
        Invoke("StartNextConvo", 1f);
    }

    public void StartNextConvo()
    {
        Debug.Log("StartingNextConvo");
        DialogueManager.Instance.StartConversation(clownConvo, onClownFinish);
    }

    void SwitchClownDir()
    {
        clown.GetComponent<SpriteRenderer>().flipX = true;

    }
    void onClownFinish()
    {
        Debug.Log("warningConvoFinish");
        family.SetActive(false);
        Invoke("StartNextConvo2", 1f);
    }
    public void StartNextConvo2()
    {
        Debug.Log("StartingNextConvo");
        DialogueManager.Instance.StartConversation(clownConvo2, onClownFinish2);
    }
    void onClownFinish2()
    {
        Debug.Log("finalClownDialog");
        clown.GetComponent<Animator>().SetBool("isMoving", true);
        clownWalkAway.Play();
        playerStopper.SetActive(false);

    }
}