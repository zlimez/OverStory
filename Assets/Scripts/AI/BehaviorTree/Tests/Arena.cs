using Abyss.EventSystem;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public readonly static string EventPostfix = " Arena Entered/Exited";

    public string arenaName;
    public bool PlayerIn = false;
    public GameObject Player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player = other.gameObject;
            PlayerIn = true;
            EventManager.InvokeEvent(new GameEvent(arenaName + EventPostfix), true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player = null;
            PlayerIn = false;
            EventManager.InvokeEvent(new GameEvent(arenaName + EventPostfix), false);
        }
    }

    public string EEEvent => arenaName + EventPostfix;
}
