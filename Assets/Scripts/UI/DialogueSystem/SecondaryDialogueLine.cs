using UnityEngine;
using Chronellium.EventSystem;

/// <summary>
/// Represents a line of dialogue spoken by a character.
/// </summary>
[System.Serializable]
public class SecondaryDialogueLine
{
    // Event are not invokable from SecondaryDialogue as if 
    // events are wrapped in SecondaryDialog, they will be cut off 
    // during scene transition which may cause some crucial events 
    // not invoked.

    [SerializeField] private Speaker speaker;
    [SerializeField] private AudioClip audio;
    [SerializeField, TextArea(3, 5)] private string dialogue;

    /// <summary>
    /// The speaker of the dialogue line.
    /// </summary>
    public Speaker Speaker => speaker;

    /// <summary>
    /// The audio of the line.
    /// </summary>
    public AudioClip Audio => audio;

    /// <summary>
    /// The actual text of the dialogue line.
    /// </summary>
    public string Dialogue => dialogue;
}