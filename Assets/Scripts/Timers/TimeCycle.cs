using System;
using System.Collections;
using Abyss.EventSystem;
using Abyss.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace Abyss.TimeManagers
{
    public class TimeCycle : StaticInstance<TimeCycle>
    {
        public static readonly float CYCLE_LENGTH = 24;
        public float SpeedMod;
        [SerializeField][Tooltip("The interval of in game time passage that an event will be emitted to broadcast the current time (should evenly divide 24)")] float broadcastInterval = 1;
        [SerializeField] float startTime = -1;
        [SerializeField] float timeOfCycle, totalTime;
        public float TotalTime => totalTime;
        float _nextBcast;
        IEnumerator _cycle;

        void OnValidate() => Assert.IsTrue(CYCLE_LENGTH % broadcastInterval == 0, "Broadcast interval should evenly divide 24");

        void OnEnable()
        {
            if (GameManager.Instance == null)
                EventManager.StartListening(SystemEvents.SystemsReady, LoadStartCycle);
            else LoadStartCycle();
            EventManager.StartListening(SystemEvents.SceneTransitStart, Save);
            EventManager.StartListening(PlayEvents.Rested, Forward);
        }

        void OnDisable()
        {
            EventManager.StopListening(SystemEvents.SystemsReady, LoadStartCycle);
            EventManager.StopListening(SystemEvents.SceneTransitStart, Save);
            EventManager.StopListening(PlayEvents.Rested, Forward);
        }

        void LoadStartCycle(object input = null)
        {
            StopCycle();
            // can be overriden by instance setting startTime in case want a scene to start at a specific time
            timeOfCycle = startTime == -1 ? GameManager.Instance.TimePersistence.TimeOfCycle : startTime;
            totalTime = GameManager.Instance.TimePersistence.TotalTime;
            if (totalTime == 0)
            {
                float initBcast = Mathf.Floor(timeOfCycle / broadcastInterval) * broadcastInterval;
                EventManager.InvokeEvent(SystemEvents.TimeBcastEvent, (initBcast, 0f));
                _nextBcast = initBcast + broadcastInterval;
            }
            _cycle = Cycle();
            StartCoroutine(_cycle);
            EventManager.StopListening(SystemEvents.SystemsReady, LoadStartCycle);
        }

        void Save(object input = null)
        {
            GameManager.Instance.TimePersistence.TimeOfCycle = timeOfCycle;
            GameManager.Instance.TimePersistence.TotalTime = totalTime;
        }

        IEnumerator Cycle()
        {
            while (true)
            {
                timeOfCycle += Time.deltaTime * SpeedMod;
                totalTime += Time.deltaTime * SpeedMod;
                if (timeOfCycle >= CYCLE_LENGTH)
                {
                    timeOfCycle = 0;
                    _nextBcast = broadcastInterval;
                    EventManager.InvokeEvent(SystemEvents.TimeBcastEvent, (0f, totalTime));
                }
                else if (timeOfCycle >= _nextBcast)
                {
                    EventManager.InvokeEvent(SystemEvents.TimeBcastEvent, (_nextBcast, totalTime));
                    _nextBcast += broadcastInterval;
                }
                yield return null;
            }
        }

        void Forward(object input)
        {
            float fwdTime = (float)input;
            timeOfCycle = (timeOfCycle + fwdTime) % CYCLE_LENGTH;
            totalTime += fwdTime;
            _nextBcast = timeOfCycle % broadcastInterval == 0 ? timeOfCycle : timeOfCycle - timeOfCycle % broadcastInterval;
        }

        public void StopCycle()
        {
            if (_cycle != null)
            {
                StopCoroutine(_cycle);
                _cycle = null;
            }
        }
    }
}
