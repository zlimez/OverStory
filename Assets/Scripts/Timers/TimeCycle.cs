using System.Collections;
using Abyss.EventSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace Abyss.TimeManagers
{
    public class TimeCycle : MonoBehaviour
    {
        public static readonly float CYCLE_LENGTH = 24;
        [SerializeField] float speedMod;
        [SerializeField][Tooltip("The interval of in game time passage that an event will be emitted to broadcast the current time (should evenly divide 24)")] float broadcastInterval = 1;
        [SerializeField] float timeOfCycle, totalTime;
        float _nextBcast;
        IEnumerator _cycle;

        void OnValidate() => Assert.IsTrue(CYCLE_LENGTH % broadcastInterval == 0, "Broadcast interval should evenly divide 24");

        void OnEnable()
        {
            if (GameManager.Instance == null)
                EventManager.StartListening(SystemEvents.SystemsReady, LoadStartCycle);
            else LoadStartCycle();
            EventManager.StartListening(SystemEvents.SceneTransitStart, Save);
        }

        void OnDisable()
        {
            EventManager.StopListening(SystemEvents.SystemsReady, LoadStartCycle);
            EventManager.StopListening(SystemEvents.SceneTransitStart, Save);
        }

        void LoadStartCycle(object input = null)
        {
            StopCycle();
            timeOfCycle = GameManager.Instance.TimePersistence.TimeOfCycle;
            totalTime = GameManager.Instance.TimePersistence.TtTime;
            if (totalTime == 0) EventManager.InvokeEvent(SystemEvents.TimeBcastEvent, (0f, 0f));
            _cycle = Cycle();
            StartCoroutine(_cycle);
            EventManager.StopListening(SystemEvents.SystemsReady, LoadStartCycle);
        }

        void Save(object input = null)
        {
            GameManager.Instance.TimePersistence.TimeOfCycle = timeOfCycle;
            GameManager.Instance.TimePersistence.TtTime = totalTime;
        }

        IEnumerator Cycle()
        {
            while (true)
            {
                timeOfCycle += Time.deltaTime * speedMod;
                totalTime += Time.deltaTime * speedMod;
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

        public void Forward(float fwdTime)
        {
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
