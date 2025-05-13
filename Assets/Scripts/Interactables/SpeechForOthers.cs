using UnityEngine;
using System.Collections.Generic;
using Tuples;
using Abyss.EventSystem;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class SpeechForOthers : MonoBehaviour
    {
        [SerializeField] List<Pair<string, float>> speech;
        [SerializeField][Tooltip("Pertains to looping type dialog determines the interval between each rep")] float intervalTime = 1;
        [SerializeField] SpeechManager speechManager;
        [SerializeField] bool isLoop;
        [SerializeField] EventCondChecker eventCond;
        bool _isRepeating = false;

        void OnEnable()
        {
            foreach (var s in speech) intervalTime += s.Tail;
            if (isLoop)
            {
                if (EventLedger.Instance != null && eventCond.IsMet())
                {
                    InvokeRepeating(nameof(Speak), 0f, intervalTime);
                    _isRepeating = true;
                }
                else EventManager.StartListening(SystemEvents.LedgerReady, RepWrapper);
            }
        }

        void RepWrapper(object input = null)
        {
            InvokeRepeating(nameof(Speak), 0f, intervalTime);
            _isRepeating = true;
            EventManager.StopListening(SystemEvents.LedgerReady, RepWrapper);
        }

        void OnDisable()
        {
            if (_isRepeating) CancelInvoke(nameof(Speak));
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (!isLoop && collider.CompareTag(Settings.Tag.Player) && eventCond.IsMet()) Speak();
        }

        private void Speak()
        {
            foreach (var s in speech) speechManager.EnqueueDialogue(s.Head, s.Tail);
        }
    }
}
