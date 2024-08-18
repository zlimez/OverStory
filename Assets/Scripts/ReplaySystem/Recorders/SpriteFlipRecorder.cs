using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tuples;
using Chronellium.EventSystem;

namespace DeepBreath.ReplaySystem {
    public class SpriteFlipRecorder : MonoBehaviour, Recorder<string>
    {
        [SerializeField] private GameEvent flipCauseEvent;

        // Queue of anim events
        // NOTE: Assumes less than one anim transition per fixedUpdate run
        public Queue<bool> flips = new Queue<bool>();
        
        void OnEnable() {
            EventManager.StartListening(StaticEvent.Common_PlayerChangeDirection, AddFlipRecord);
        }

        void OnDisable() {
            EventManager.StopListening(StaticEvent.Common_PlayerChangeDirection, AddFlipRecord);
        }


        private void AddFlipRecord(object input) {
            bool isFlip = (bool)input;
            flips.Enqueue(isFlip);
        }

        public ActionReplayRecord<string> ProduceRecord() {
            if (flips.Count == 0) return new SpriteFlipReplayRecord(SpriteFlipReplayRecord.NoChange);
            return new SpriteFlipReplayRecord(flips.Dequeue().ToString());
        }
    }
}
