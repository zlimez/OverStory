using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Speaker", order = 0)]
public class Speaker : ScriptableObject
{
    [SerializeField] private string speakerName;
    [SerializeField] private Sprite sprite;

    public string SpeakerName => speakerName;
    public Sprite Sprite => sprite;
}