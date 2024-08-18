using Chronellium.EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeepBreath.ReplaySystem {
    public abstract class ActionReplayPair<U, T, V> : MonoBehaviour where U : MonoBehaviour, Recorder<V> where T : MonoBehaviour, Replayer<V>
    {
        public U spiritualComponent;
        public T realComponent;
        private bool isInReplayMode = true;
        private Queue<ActionReplayRecord<V>> actionReplayRecords = new Queue<ActionReplayRecord<V>>();

        void OnEnable() {
            EventManager.StartListening(StaticEvent.Core_SwitchToRealWorld, Replay);
            EventManager.StartListening(StaticEvent.Core_SwitchToOtherWorld, Record);
        }

        void OnDisable() {
            EventManager.StopListening(StaticEvent.Core_SwitchToRealWorld, Replay);
            EventManager.StopListening(StaticEvent.Core_SwitchToOtherWorld, Record);
        }

        private void Record(object input = null)
        {
            realComponent.gameObject.SetActive(false);
            spiritualComponent.gameObject.SetActive(true);
            isInReplayMode = false;
        }

        private void Replay(object input = null)
        {
            realComponent.gameObject.SetActive(true);
            spiritualComponent.gameObject.SetActive(false);
            isInReplayMode = true;
        }

        private void FixedUpdate()
        {
            if (isInReplayMode == false)
            {
                actionReplayRecords.Enqueue(spiritualComponent.ProduceRecord());
            }
            else if (actionReplayRecords.Count > 0)
            {
                realComponent.Consume(actionReplayRecords.Dequeue());
                if (actionReplayRecords.Count == 0) EventManager.InvokeEvent(new GameEvent(DynamicEvent.ReplayCompleteEventPrefix + realComponent.name));
            }
        }

        public void ResetReplayRecords() {
            actionReplayRecords.Clear();
        }
    }
}
