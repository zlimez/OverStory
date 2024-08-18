using UnityEngine;
using Chronellium.EventSystem;

/// <summary>
/// Represents a line of dialogue spoken by a character.
/// </summary>
[System.Serializable]
public class DialogueLine
{
    [SerializeField] private bool isLeft;
    [SerializeField] private Speaker speaker;
    [SerializeField] private string name;
    [SerializeField] private bool isCentered;
    [SerializeField] private float textSpeed = 1f;
    [SerializeField] private AudioClip audio;
    [SerializeField, TextArea(3, 5)] private string dialogue;
    [SerializeField] private GameEvent onLineStart = GameEvent.NoEvent;

    /// <summary>
    /// Determines whether the character speaking the line is on the left or right side of the screen.
    /// </summary>
    public bool IsLeft => isLeft;

    /// <summary>
    /// The speaker of the dialogue line.
    /// </summary>
    public Speaker Speaker => speaker;

    /// <summary>
    /// The temporary name of the character speaking the line.
    /// </summary>
    public string Name => name;

    /// <summary>
    /// Determines whether the text should be centered on the screen.
    /// </summary>
    public bool IsCentered => isCentered;

    /// <summary>
    /// The speed at which the text should be displayed.
    /// </summary>
    public float TextSpeed => textSpeed;

    /// <summary>
    /// The audio of the line.
    /// </summary>
    public AudioClip Audio => audio;

    /// <summary>
    /// The actual text of the dialogue line.
    /// </summary>
    public string Dialogue => dialogue;

    /// <summary>
    /// The event to be invoked when the dialogue line starts.
    /// </summary>
    public GameEvent OnLineStart => onLineStart;

    /// <summary>
    /// Determines whether the dialogue line has a start event.
    /// </summary>
    /// <returns><c>true</c> if the dialogue line has a start event; otherwise, <c>false</c>.</returns>
    public bool HasLineStartEvent()
    {
        return onLineStart != GameEvent.NoEvent;
    }
}