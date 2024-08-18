using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tuples;
using Chronellium.EventSystem;

namespace DeepBreath.ReplaySystem {
    public class AnimRecorder : MonoBehaviour, Recorder<Pair<string, string>>
    {
        [SerializeField] private GameEvent transitionEvent;
        [SerializeField] private string animParamName;

        // Queue of anim events
        // NOTE: Assumes less than one anim transition per fixedUpdate run
        public Queue<bool> animTransitions = new Queue<bool>();
        
        void OnEnable() {
            EventManager.StartListening(StaticEvent.Common_GrabStateChanged, AddTransitionRecord);
        }

        void OnDisable() {
            EventManager.StopListening(StaticEvent.Common_GrabStateChanged, AddTransitionRecord);
        }


        private void AddTransitionRecord(object input) {
            bool isPositive = (bool)input;
            animTransitions.Enqueue(isPositive);
        }

        public ActionReplayRecord<Pair<string, string>> ProduceRecord() {
            if (animTransitions.Count == 0) return new AnimReplayRecord(animParamName, AnimReplayRecord.NoChange);
            return new AnimReplayRecord(animParamName, animTransitions.Dequeue().ToString());
        }
    }
}
