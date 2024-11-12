using Abyss.EventSystem;
using UnityEngine;
using System.Collections.Generic;
using Tuples;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class SpeechForPlayer : Interactable
    {
        [SerializeField] List<RefPair<string, float>> speech;

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
                foreach( var s in speech) EventManager.InvokeEvent(PlayEvents.PlayerSpeak, s);
        }

    }
}
