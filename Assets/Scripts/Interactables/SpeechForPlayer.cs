using Abyss.EventSystem;
using UnityEngine;
using System.Collections.Generic;
using Tuples;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class SpeechForPlayer : MonoBehaviour
    {
        [SerializeField] EventCondChecker eventCond;
        [SerializeField] List<RefPair<string, float>> speech;

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player") && eventCond.IsMet())
                foreach (var s in speech) EventManager.InvokeEvent(PlayEvents.PlayerSpeak, s);
        }
    }
}
