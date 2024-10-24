using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using DataStructures;

namespace Abyss.TimeManagers
{
    /// <summary>
    /// A Timer class that provides functionality for starting, stopping, resetting, and modifying a timer in a Unity game.
    /// </summary>
    public class CountdownTimer : MonoBehaviour
    {
        public float TimeLeft;
        public float TotalDuration = 60f;

        [System.NonSerialized] public UnityEvent<float> OnTimerChange = new();
        [System.NonSerialized] public UnityEvent OnTimerStart = new();
        [System.NonSerialized] public UnityEvent OnTimerExpire = new();
        private readonly PriorityQueue<TimeSensitiveAction> scheduledActions = new();

        private IEnumerator _timerCoroutine;

        void Awake() => TimeLeft = TotalDuration;

        public void ScheduleAction(float time, Action action)
        {
            // Not enough time left already
            if (time > TimeLeft) return;
            Debug.Log("Action scheduled at " + time);
            scheduledActions.Enqueue(new TimeSensitiveAction(time, action));
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void StartTimer()
        {
            TimeLeft = TotalDuration;
            OnTimerStart.Invoke();
            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _timerCoroutine = TimerCoroutine();
            StartCoroutine(_timerCoroutine);
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void StopTimer()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
            OnTimerChange.RemoveAllListeners();
        }

        /// <summary>
        /// Resets the timer to its initial duration.
        /// </summary>
        public void ResetTimer()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
            }
            TimeLeft = TotalDuration;
            OnTimerChange.Invoke(TimeLeft);
        }

        /// <summary>
        /// Adds time to the timer.
        /// </summary>
        /// <param name="timeToAdd">Amount of time in seconds to add to the timer.</param>
        public void AddTime(int timeToAdd)
        {
            TimeLeft += timeToAdd;
            if (TimeLeft > TotalDuration)
                TimeLeft = TotalDuration;
            OnTimerChange.Invoke(TimeLeft);
        }

        /// <summary>
        /// Coroutine that handles the timer countdown.
        /// </summary>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        private IEnumerator TimerCoroutine()
        {
            while (TimeLeft > 0)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                TimeLeft -= Time.deltaTime;
                OnTimerChange.Invoke(TimeLeft);
                while (!scheduledActions.IsEmpty && scheduledActions.Peek().ScheduledTime > TimeLeft)
                    scheduledActions.Dequeue().Execute();

                if (TimeLeft <= 0)
                    OnTimerExpire.Invoke();
            }
        }
    }

}