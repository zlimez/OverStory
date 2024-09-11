using UnityEngine;

/// <summary>
/// Represents a conversation consisting of multiple dialogue lines.
/// </summary>
[CreateAssetMenu(menuName = "Dialogue/Conversation", order = 1)]
public class Conversation : ScriptableObject
{
    [Header("Settings")]
    [SerializeField] private bool endWithChoice = false;
    [SerializeField] private bool isBlackBackground = false;
    [SerializeField] private float textSpeed = 1f;

    [Header("Speakers")]
    [SerializeField] private Speaker startingLeftSpeaker = null;
    [SerializeField] private Speaker startingRightSpeaker = null;

    [Header("Dialogue Lines")]
    [SerializeField] private DialogueLine[] allLines = null;

    /// <summary>
    /// Gets the event prefix for identifying conversation events.
    /// </summary>
    public static string EventPrefix => "Hector Finished Convo: ";

    /// <summary>
    /// Gets a value indicating whether the conversation ends with a choice.
    /// </summary>
    public bool EndWithChoice => endWithChoice;

    /// <summary>
    /// Gets a value indicating whether the conversation has a black background.
    /// </summary>
    public bool IsBlackBackground => isBlackBackground;

    /// <summary>
    /// Gets the starting left speaker of the conversation.
    /// </summary>
    public Speaker StartingLeftSpeaker => startingLeftSpeaker;

    /// <summary>
    /// Gets the starting right speaker of the conversation.
    /// </summary>
    public Speaker StartingRightSpeaker => startingRightSpeaker;

    /// <summary>
    /// Gets the text speed of the conversation.
    /// </summary>
    public float TextSpeed => textSpeed;

    /// <summary>
    /// Gets the dialogue lines of the conversation.
    /// </summary>
    public DialogueLine[] AllLines => allLines;
}