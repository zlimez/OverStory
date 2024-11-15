using Abyss.EventSystem;
using UnityEngine;
using System.Collections.Generic;
using Tuples;
using UnityEditor;
using System.Collections;

namespace Abyss.Interactables
{
    // TODO: Add persistence or refresh items logic
    public class SpeechForOthers : CondInteractable
    {
        [SerializeField] List<Pair<string, float>> speech;
        [SerializeField] float intervalTime = 1;
        [SerializeField] SpeechManager speechManager;
        [SerializeField] bool isLoop;

        void OnEnable()
        {
            foreach (var s in speech) intervalTime += s.Tail;
            base.CheckIsMet();
            if (_isMet && isLoop) InvokeRepeating("Speak", 0f, intervalTime);
        }

        void OnDisable()
        {
            if (_isMet && isLoop) CancelInvoke("Speak");
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            base.CheckIsMet();
            if (_isMet && !isLoop && collider.CompareTag("Player")) Speak();
        }

        private void Speak()
        {
            foreach (var s in speech) speechManager.EnqueueDialogue(s.Head, s.Tail);
        }

    }
}
