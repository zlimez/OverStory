using UnityEngine;
using Abyss.EventSystem;

[System.Serializable]
public class DialogueLine
{
    [SerializeField] private bool isLeft;
    [SerializeField] private Speaker speaker;
    [SerializeField] private string name;
    [SerializeField] private bool isCentered;
    [SerializeField] private AudioClip audio;
    [SerializeField, TextArea(3, 5)] private string dialogue;
    [SerializeField] private GameEvent onLineStart = GameEvent.NoEvent;


    public bool IsLeft => isLeft;
    public Speaker Speaker => speaker;
    public string Name => name;
    public bool IsCentered => isCentered;
    public AudioClip Audio => audio;
    public string Dialogue => dialogue;
    public GameEvent OnLineStart => onLineStart;
    public bool HasLineStartEvent => onLineStart != GameEvent.NoEvent;
}