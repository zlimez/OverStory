using Abyss.EventSystem;
using Abyss.Settings;
using UnityEngine;

public class Aggro : MonoBehaviour
{
    public readonly static string EventPostfix = " Aggro Entered/Exited";

    public bool PlayerIn { get; private set; }
    public GameObject Player { get; private set; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tag.Player))
        {
            Player = other.gameObject;
            PlayerIn = true;
            EventManager.InvokeEvent(new GameEvent(EEEvent), true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tag.Player))
        {
            Player = null;
            PlayerIn = false;
            EventManager.InvokeEvent(new GameEvent(EEEvent), false);
        }
    }

    public string EEEvent => gameObject.name + EventPostfix;
}
