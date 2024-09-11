using UnityEngine;

/// <summary>
/// Represents a conversation consisting of multiple dialogue lines.
/// </summary>
[CreateAssetMenu(menuName = "Dialogue/SecondaryConversation", order = 2)]
public class SecondaryConversation : ScriptableObject
{

    [Header("Dialogue Lines")]
    [SerializeField] private SecondaryDialogueLine[] allLines = null;

    /// <summary>
    /// Gets the event prefix for identifying conversation events.
    /// </summary>
    public static string EventPrefix => "Hector Finished Convo: ";

    /// <summary>
    /// Gets the dialogue lines of the conversation.
    /// </summary>
    public SecondaryDialogueLine[] AllLines => allLines;
}