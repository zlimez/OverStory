using Abyss.Interactables;
using UnityEngine;

public class LabConsole : CondInteractable
{
    [SerializeField] Sprite OnSprite;
    [SerializeField] Conversation labConsoleConvo;
    SpriteRenderer _spriteRenderer;

    void Awake() => _spriteRenderer = GetComponent<SpriteRenderer>();

    public override void Interact()
    {
        _spriteRenderer.sprite = OnSprite;
        DialogueManager.Instance.HardStartConvo(labConsoleConvo);
        base.Interact();
    }
}
