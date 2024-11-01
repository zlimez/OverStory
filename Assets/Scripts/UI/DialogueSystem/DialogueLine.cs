using UnityEngine;
using Abyss.EventSystem;

[System.Serializable]
public class DialogueLine
{
    [SerializeField] bool isLeft;
    [SerializeField] Speaker speaker;
    [SerializeField] string name;
    [SerializeField] bool isCentered;
    [SerializeField] AudioClip audio;
    [SerializeField, TextArea(3, 5)] string dialogue;
    [SerializeField] GameEvent onLineStart = GameEvent.NoEvent;


    public bool IsLeft => isLeft;
    public Speaker Speaker => speaker;
    public string Name => name;
    public bool IsCentered => isCentered;
    public AudioClip Audio => audio;
    public string Dialogue => dialogue;
    public GameEvent OnLineStart => onLineStart;
    public bool HasLineStartEvent => onLineStart != GameEvent.NoEvent;
}