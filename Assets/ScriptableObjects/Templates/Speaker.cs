using UnityEngine;

/// <summary>
/// Represents a speaker for in-game dialogues or events.
/// </summary>
[CreateAssetMenu(menuName = "Dialogue/Speaker", order = 0)]
public class Speaker : ScriptableObject
{
    [SerializeField] private string speakerName;
    [SerializeField] private Sprite speakerSprite;

    /// <summary>
    /// Gets the name of the speaker.
    /// </summary>
    public string SpeakerName => speakerName;

    /// <summary>
    /// Gets the sprite of the speaker.
    /// </summary>
    public Sprite SpeakerSprite => speakerSprite;
}