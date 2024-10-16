using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conversation", order = 1)]
public class Conversation : ScriptableObject
{
    // [SerializeField] private bool endWithChoice = false;
    // [SerializeField] bool isBlackBackground = false;
    [SerializeField] Speaker leftSpeaker = null;
    [SerializeField] Speaker rightSpeaker = null;
    [SerializeField] DialogueLine[] allLines = null;

    // public bool EndWithChoice => endWithChoice;
    // public bool IsBlackBackground => isBlackBackground;
    public Speaker LeftSpeaker => leftSpeaker;
    public Speaker RightSpeaker => rightSpeaker;
    public DialogueLine[] AllLines => allLines;
}